using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Query;
using System.Globalization;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// classe business de Reconhecimento de Receita Mensal
    /// </summary>
    public class smt_monthly_revenue_recognitionBusiness : BaseBusiness
    {
        /// <summary>
        ///  Construtor da classe business
        /// </summary>
        /// <param name="service"> service </param>
        /// <param name="serviceAdmin"> service adm</param>
        /// <param name="tracingService"> tracing </param>
        /// <param name="messages"> mensagens resx </param>
        public smt_monthly_revenue_recognitionBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Faz o cálculo do valor total de receita reconhecida para a tarefa do projeto nesse mês.
        /// </summary>
        /// <param name="revenueRecognition"> Reconhecimento de receita </param>
        /// <param name="idParentTask"> id da Tarefa Raiz do reconhecimento de receita</param>
        /// <returns> valor reconhecido para o projeto nesse mês </returns>
        public decimal SumTotalRevenueToProjectTaskThisMonth(smt_monthly_revenue_recognition revenueRecognition, Guid idParentTask)
        {
            List<msdyn_projecttask> tasks = new List<msdyn_projecttask>();
            decimal tasksFilhas = 0;

            using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
            {
                    // Todas as tarefas filhas do projeto do reconhecimento de receita.
                    tasks = (from ts in crmContext.CreateQuery<msdyn_projecttask>()
                                                     where ts.smt_statusprojetoEnum == msdyn_projecttask_smt_statusprojeto.Concluido
                                                     && ts.smt_lp_milestone != null && ts.msdyn_project.Id == revenueRecognition.smt_lp_project.Id
                                                     && ts.Id != idParentTask && ts.smt_mn_recognized_revenue != null
                                                     select ts).ToList();

                // Data do último reconhecimento de receita criado para a mesma tarefa do projeto
                DateTime? dt = GetLastMonthlyRevenue(revenueRecognition);

                if (dt != null)
                {
                    tasksFilhas = tasks.Where(a => GetParentTask(a.msdyn_parenttask != null ? a.msdyn_parenttask.Id : a.Id) == idParentTask && a.smt_dt_recognized > dt).Sum(a => a.smt_mn_recognized_revenue.Value);
                }
                else
                {
                    tasksFilhas = tasks.Where(a => GetParentTask(a.msdyn_parenttask != null ? a.msdyn_parenttask.Id : a.Id) == idParentTask).Sum(a => a.smt_mn_recognized_revenue.Value);
                }

                return tasksFilhas;

            }
        }

        /// <summary>
        /// Método para fazer retrieve de linha.
        /// </summary>
        /// <param name="logicalname">nome lógico da entidade</param>
        /// <param name="id">id do registro </param>
        /// <param name="columnSet"> columset </param>
        /// <param name="executionContext"> contexto de execução</param>
        /// <returns> resultado do retrieve </returns>
        public Entity Retrieve(String logicalname, Guid id, ColumnSet columnSet, IPluginExecutionContext executionContext)
        {
            try
            {
                Entity result = ServiceAdmin.Retrieve(logicalname, id, columnSet);

                return result;
            }
            catch(Exception ex)
            {
                CreateLog(ex.Message, id, executionContext.UserId);
                return null;
            }

        }
        /// <summary>
        /// Atualiza ou deleta reconhecimento de receita mensal
        /// </summary>
        /// <param name="revenueRecognition"> reconhecimento target </param>
        /// <param name="monthlyRevenue"> receita arrecadada no mes</param>
        /// <param name="localContext"> localPluginContext </param>
        public void UpdateMonthlyRevenueRecognition(smt_monthly_revenue_recognition revenueRecognition, decimal monthlyRevenue, IPluginExecutionContext localContext)
        {
            // Se o valor arrecadado no mes for maior que zero, atualiza o reconhecimento de receita com a data de hoje e o valor arrecadado.
            if (monthlyRevenue != null && monthlyRevenue > 0)
            {
                smt_monthly_revenue_recognition revenue = new smt_monthly_revenue_recognition();
                revenue.smt_mn_monthly_revenue_recognized = new Money(monthlyRevenue);
                revenue.smt_dt_date = DateTime.Now;
                revenue.Id = revenueRecognition.Id;
                CultureInfo cultureInfo = new CultureInfo("pt-BR");
                msdyn_project project = Retrieve(msdyn_project.EntityLogicalName, revenueRecognition.smt_lp_project.Id, new ColumnSet("msdyn_subject"), localContext).ToEntity<msdyn_project>();
                msdyn_projecttask projecttask = Retrieve(msdyn_projecttask.EntityLogicalName, revenueRecognition.smt_lp_projecttask.Id, new ColumnSet("msdyn_subject"), localContext).ToEntity<msdyn_projecttask>();

                if (project != null && projecttask != null)
                revenue.smt_name = $"Reconhecimento de {cultureInfo.DateTimeFormat.GetMonthName(DateTime.Now.Month)} : {project.msdyn_subject} - {projecttask.msdyn_subject}";

                try
                {
                    ServiceAdmin.Update(revenue);
                }
                catch (Exception ex)
                {
                    CreateLog(ex.Message, revenueRecognition.Id, localContext.UserId);
                }
            }
            else
            {
                // Se não for maior que zero deleta o reconhecimento de receita do mes.
                try
                {
                    ServiceAdmin.Delete(smt_monthly_revenue_recognition.EntityLogicalName, revenueRecognition.Id);
                }
                catch (Exception ex)
                {
                   CreateLog(ex.Message, revenueRecognition.Id, localContext.UserId);
                }
            }
        }

        /// <summary>
        /// Buscar Tarefa Raíz.
        /// </summary>
        /// <param name="idpai"> id da tarefa raiz</param>
        /// <returns> Id da Tarefa Raiz </returns>
        public Guid GetParentTask(Guid idpai)
        {
            Guid idParentTask = default(Guid);
            msdyn_projecttask projectTask = new msdyn_projecttask();

            using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
            {
                // Busca tarefas do projeto até encontrar a raiz
                do
                {
                    projectTask = (from t in crmContext.CreateQuery<msdyn_projecttask>()
                                   where t.Id == idpai
                                   select t).FirstOrDefault();

                    if (projectTask != null && projectTask.msdyn_parenttask != null)
                    {
                        // Se existir a tarefa vai atribuir o id do pai para a variável de retorno do método
                        idpai = projectTask.msdyn_parenttask.Id;
                    }
                    else
                    {
                        idpai = projectTask.Id;
                    }
                } while (projectTask.msdyn_parenttask != null);

                return idpai;
            }
        }

        /// <summary>
        /// Busca a data do último reconhecimento de receita criado para a mesma tarefa do projeto do contexto
        /// </summary>
        /// <param name="revenueRecognition"> reconhecimento de receita </param>
        /// <returns> data do ultimo reconhecimento de receita </returns>
        public DateTime? GetLastMonthlyRevenue(smt_monthly_revenue_recognition revenueRecognition)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                smt_monthly_revenue_recognition lastRevenueRecognition = (from revenue in context.CreateQuery<smt_monthly_revenue_recognition>()
                                                                          where revenue.smt_lp_projecttask == revenueRecognition.smt_lp_projecttask
                                                                          && revenue.Id != revenueRecognition.Id
                                                                          select revenue).OrderByDescending(a => a.smt_dt_date).FirstOrDefault();
                if (lastRevenueRecognition != null)
                {
                    return lastRevenueRecognition.smt_dt_date;
                }
                else
                {
                    return null;
                }

            }
        }
        /// <summary>
        /// Criação de log de erro. 
        /// </summary>
        /// <param name="error">Nome do erro</param>
        /// <param name="revenueRecognition">ID do reconhecimento da receita</param>
        /// <param name="user">ID do usuário</param>
        public void CreateLog(string error, Guid revenueRecognition, Guid user)
        {
            smt_log log = new smt_log()
            {
                smt_name = Messages.GetMessageById(error),
                smt_st_entityname = "Reconhecimento de Receita Mensal (smt_monthly_revenue_reconigtion)",
                smt_pl_eventtypeEnum = smt_log_smt_pl_eventtype.Erro,
                smt_dt_eventdate = DateTime.Now,
                smt_lp_executinguser = Service.Retrieve(SystemUser.EntityLogicalName, user, GetColumnSet("systemuserid")).ToEntityReference(),
                smt_st_recordname = Messages.GetMessageById(error),
                smt_st_recordid = revenueRecognition.ToString(),
                smt_tx_message = error,
                smt_st_eventorigin = "Plugin PostUpdateAsync_smt_monthly_revenue_reconigtion",
            };
            Service.Create(log);
        }
    }
}
