using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmEarlyBound;
using CRM.Pixeon.Extends.Business;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using Microsoft.Xrm.Sdk.Query;
using CRM.Pixeon.Extends.Earlybound;

namespace Pixeon.Extends.Business
{
    /// <summary>
    /// class
    /// </summary>
    public class projecttaskBusiness : BaseBusiness
    {
        /// <summary>
        /// Método construtor da business
        /// </summary>
        /// <param name="service"> service</param>
        /// <param name="serviceAdmin"> service Admin </param>
        /// <param name="tracingService"> tracing Service </param>
        /// <param name="messages"> mensagens resx </param>
        public projecttaskBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="logicalname"> .</param>
        /// <param name="idRecord"> .</param>
        /// <param name="columns"> .</param>
        /// <returns> rr</returns>
        public Entity Retrieve(String logicalname, Guid idRecord, ColumnSet columns)
        {
            Entity entity = new Entity();
            try
            {
                entity = ServiceAdmin.Retrieve(logicalname, idRecord, columns);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Não existe um registro de {logicalname} com o id {idRecord} + {ex.Message}");
            }
            return entity;
        }

        /// <summary>
        /// Método de validação de reconhecimento de receita
        /// </summary>
        /// <param name="mergeTask"> merge da tarefa do projeto </param>
        public void ValidateRecurrentRevenue(msdyn_projecttask mergeTask)
        {
            if (mergeTask.msdyn_project != null)
            {
                msdyn_project project = Retrieve(msdyn_project.EntityLogicalName, mergeTask.msdyn_project.Id, new ColumnSet(true)).ToEntity<msdyn_project>();

                OptionSetValue padrao = new OptionSetValue(180580002);
                OptionSetValue instalacao = new OptionSetValue(180580000);
                if (project != null && project.msdyn_salesorderid != null)
                {
                    SalesOrder contract = Retrieve(SalesOrder.EntityLogicalName, project.msdyn_salesorderid.Id, new ColumnSet(true)).ToEntity<SalesOrder>();

                    if (contract.smt_pl_type_rr != null && contract.smt_pl_type_rr.Value == padrao.Value)
                    {
                        RevenueDefault(mergeTask, contract);
                    }
                    else if (contract.smt_pl_type_rr != null && contract.smt_pl_type_rr.Value == instalacao.Value)
                    {
                        GoLiveRevenue(contract, mergeTask);
                    }
                }
            }
        }

        /// <summary>
        /// Método para receita recorrente Go/Live Instalação
        /// </summary>
        /// <param name="contract"> contrato do projeto</param>
        /// <param name="mergeTask"> Merge da tarefa do projeto </param>
        public void GoLiveRevenue(SalesOrder contract, msdyn_projecttask mergeTask)
        {
            if (mergeTask.smt_statusprojeto.Value == 100000002)
            {
                using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
                {
                    List<msdyn_projecttask> tasks = (from task in serviceContext.CreateQuery<msdyn_projecttask>()
                                                     where task.msdyn_project.Id == mergeTask.msdyn_project.Id
                                                     && task.Id != mergeTask.Id
                                                     select task).ToList<msdyn_projecttask>();

                    if (tasks != null && tasks.Count > 0)
                    {
                        List<msdyn_projecttask> openTasks = tasks.Where(a => (a.smt_statusprojeto != null && a.smt_statusprojeto.Value != 100000002) || a.smt_statusprojeto == null).ToList<msdyn_projecttask>();

                        if (openTasks == null || openTasks.Count > 0)
                            GetSalesOrderDetails(contract, "golive");
                    }
                    else
                        GetSalesOrderDetails(contract, "golive");

                }
            }
        }

        /// <summary>
        /// Buscar Tarefa Raíz.
        /// </summary>
        /// <param name="idpai"> id da primeira tarefa pai</param>
        /// <returns> Id da Tarefa Raiz </returns>
        public Guid GetParentTask(Guid idpai)
        {
            Guid idParentTask = default(Guid);
            msdyn_projecttask projectTask = new msdyn_projecttask();

            using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
            {
                // Busca tarefas do projeto até encontrar uma que seja raiz
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
        /// Método para receita recorrente padrão
        /// </summary>
        /// <param name="mergeTask"> Merge da tarefa do projeto</param>
        /// <param name="contract"> contrato do projeto </param>
        public void RevenueDefault(msdyn_projecttask mergeTask, SalesOrder contract)
        {
            if (mergeTask.smt_statusprojeto.Value == 100000002 && mergeTask.smt_bl_recurring_release == true) // Se status da tarefa estiver sendo alterado para concluído
            {
                GetSalesOrderDetails(contract, "padrao");
            }
        }

        /// <summary>
        /// Busca as linhas de contrato do projeto baseadas em produto.
        /// </summary>
        /// <param name="contract">contrato do projeto </param>
        /// <param name="typerr"> tipo de rr</param>
        public void GetSalesOrderDetails(SalesOrder contract, string typerr)
        {
            using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
            {
                List<SalesOrderDetail> linhas = (from linha in serviceContext.CreateQuery<SalesOrderDetail>()
                                                 where linha.SalesOrderId.Id == contract.Id
                                                 && linha.ProductTypeCode == new OptionSetValue(1)
                                                 select linha).ToList<SalesOrderDetail>();

                // Se houverem linhas de contrato baseadas em produto, chama o método para atualizar as mesmas
                if (linhas != null && linhas.Count > 0)
                    UpdateSalesOrderDetails(linhas, contract, typerr);
            }

        }

        /// <summary>
        /// Atualiza as linhas de contrato do projeto baseadas em produto
        /// </summary>
        /// <param name="salesOrderDetails"> Lista de linhas de contrato </param>
        /// <param name="contract"> Contrato do projeto </param>
        /// <param name="typerr"> tipo de rr </param>
        public void UpdateSalesOrderDetails(List<SalesOrderDetail> salesOrderDetails, SalesOrder contract, string typerr)
        {
            foreach (SalesOrderDetail linha in salesOrderDetails)
            {
                SalesOrderDetail detail = new SalesOrderDetail();
                detail.Id = linha.Id;
                detail.msdyn_BillingStatus = new OptionSetValue(192350004);
                ServiceAdmin.Update(detail);
            }

            SalesOrder contrato = new SalesOrder();
            contrato.Id = contract.Id;
            if (typerr == "padrao")
                contrato.smt_dt_access = DateTime.Now;
            else if (typerr == "golive")
                contrato.smt_dt_end = DateTime.Now;

            ServiceAdmin.Update(contrato);
        }

        public void AssociateSalesOrder(msdyn_projecttask projecttask)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                msdyn_project project = context.CreateQuery<msdyn_project>().Where(p => p.Id == projecttask.msdyn_project.Id).FirstOrDefault();

                projecttask.smt_lp_salesorderid = project.msdyn_salesorderid;
                projecttask.smt_lp_customer = project.msdyn_customer;
            }
        }
    }
}
