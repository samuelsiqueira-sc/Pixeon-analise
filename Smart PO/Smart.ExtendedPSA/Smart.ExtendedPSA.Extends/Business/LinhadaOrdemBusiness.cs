using CRM.Smart.ExtendedPSA.Extends.Business;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Documentar.
    /// </summary>
    public class LinhadaOrdemBusiness : BaseBusiness
    {
        /// <summary>
        /// Método construtor da Business.
        /// </summary>
        /// <param name="service"> service InitialUser</param>
        /// <param name="serviceAdmin"> service Admin</param>
        /// <param name="tracingService"> tracing </param>
        /// <param name="messages">Mensagens resx.</param>
        public LinhadaOrdemBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Realiza a busca um parâmetro de acordo com o seu nome. 
        /// </summary>
        /// <param name="parameterName">Nome do pârametr.</param>
        /// <param name="BuId">Nome do pârametr.</param>
        /// <returns>Retorna as informações do parâmetro.</returns>
        public smt_smartparameter GetSmartParameter(string parameterName, Guid BuId)
        {
            try
            {
                using (CrmServiceContext crmContext = new CrmServiceContext(Service))
                {
                    smt_smartparameter parameterOne = crmContext.smt_smartparameterSet.Where(parameter => parameter.smt_name == parameterName && parameter.smt_organizationalunit.Id == BuId).Select(parameter =>
                    new smt_smartparameter { smt_value = parameter.smt_value }).FirstOrDefault();

                    if (parameterOne == null)
                    {
                        parameterOne = crmContext.smt_smartparameterSet.Where(parameter => parameter.smt_name == parameterName && parameter.smt_organizationalunit == null).Select(parameter =>
                        new smt_smartparameter { smt_value = parameter.smt_value }).FirstOrDefault();
                    }
                    return parameterOne;
                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                throw invalidPluginExecutionException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return null;
        }

        /// <summary>
        ///  Busca por um projeto. 
        /// </summary>
        /// <param name="projectGuid">ID do projeto.</param>
        /// <returns>Retorna um projeto que satisfaça as condições de busca.</returns>
        public msdyn_project GetProject(Guid? projectGuid)
        {
            try
            {
                using (CrmServiceContext crmContext = new CrmServiceContext(Service))
                {
                    return crmContext.msdyn_projectSet.Where(project => project.Id == projectGuid).Select(project => new msdyn_project
                    {
                        smt_margemcontrato = project.smt_margemcontrato,
                        smt_valorcontratado = project.smt_valorcontratado,
                        Id = project.Id,
                        msdyn_ContractOrganizationalUnitId = project.msdyn_ContractOrganizationalUnitId,
                    }).FirstOrDefault();
                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                throw invalidPluginExecutionException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return null;
        }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="salesOrderDetailProject">doc</param>
        /// <param name="value">doc</param>
        public void SetValue(msdyn_project salesOrderDetailProject, decimal value)
        {
            salesOrderDetailProject.smt_margemcontrato = value;
            Update(salesOrderDetailProject);
        }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="salesOrderDetailProject">doc</param>
        /// <param name="priceperunit">doc</param>
        public void ChangeContractedAmountNull(msdyn_project salesOrderDetailProject, decimal priceperunit)
        {
            salesOrderDetailProject.smt_valorcontratado = new Money(priceperunit);
            salesOrderDetailProject.EntityState = EntityState.Changed;
            Update(salesOrderDetailProject);
        }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="salesOrderDetailProject">doc</param>
        /// <param name="target">doc</param>   
        public void ChangeValue(msdyn_project salesOrderDetailProject, SalesOrderDetail target)
        {
            salesOrderDetailProject.smt_valorcontratado = target.PricePerUnit;
            salesOrderDetailProject.EntityState = EntityState.Changed;
            Update(salesOrderDetailProject);
        }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <param name="projeto">Doc.</param>
        /// <returns>Doc.</returns>
        public msdyn_project RetrieveProject(Guid projeto)
        {

            try
            {

                using (CrmServiceContext crmContext = new CrmServiceContext(Service))
                {
                    return crmContext.msdyn_projectSet.Where(project => project.Id == projeto).Select(project => new msdyn_project
                    {
                        Id = project.Id,

                    }).FirstOrDefault();
                }

            }
            catch (ArgumentNullException argumentNull)
            {
                throw argumentNull;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <param name="projectGuid">Doc.</param>
        /// <returns>Doc.</returns>
        public List<msdyn_projecttask> RetrieveProjectTasks(Guid projectGuid)
        {

            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.msdyn_projecttaskSet.Where(tasks => tasks.msdyn_project.Id == projectGuid &&
                    tasks.smt_statusprojetoEnum != msdyn_projecttask_smt_statusprojeto.Concluido &&
                    tasks.smt_statusprojetoEnum != msdyn_projecttask_smt_statusprojeto.AprovadoAutomaticamente &&
                    tasks.smt_lp_milestone != null).ToList();

                }
            }
            catch (ArgumentNullException argumentNull)
            {
                throw argumentNull;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <param name="taskGuid">Doc.</param>
        /// <returns>Doc.</returns>
        public smt_milestone_project RetrieveTaskMilestone(msdyn_projecttask taskGuid)
        {

            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.smt_milestone_projectSet.Where(milestone => milestone.smt_milestone_projectId == taskGuid.smt_lp_milestone.Id).Select(m => new smt_milestone_project
                    {

                        smt_dc_stage = m.smt_dc_stage,

                    }).FirstOrDefault();

                }
            }
            catch (ArgumentNullException argumentNull)
            {
                throw argumentNull;
            }
            catch (Exception ex)
            {
                throw ex;
            } 

        }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="projecttask">doc</param>
        public void UpdateAsAdmin(msdyn_projecttask projecttask)
        {
            try
            {
                ServiceAdmin.Update(projecttask);
            }
            catch (Exception ex)
            {
                CreateLog($"Erro ao atualizar a receita estimada da tarefa do projeto {ex}", projecttask.Id, default(Guid));
            }
        }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="error">doc</param>
        /// <param name="projecttask">doc</param>
        /// /// <param name="user">doc</param>
        public void CreateLog(string error, Guid projecttask, Guid user)
        {
            smt_log log = new smt_log()
            {
                smt_name = Messages.GetMessageById(error),
                smt_st_entityname = "Tarefa do projeto (msdyn_projecttask)",
                smt_pl_eventtypeEnum = smt_log_smt_pl_eventtype.Erro,
                smt_dt_eventdate = DateTime.Now,
                smt_lp_executinguser = Service.Retrieve(SystemUser.EntityLogicalName, user, GetColumnSet("systemuserid")).ToEntityReference(),
                smt_st_recordname = Messages.GetMessageById(error),
                smt_st_recordid = projecttask.ToString(),
                smt_tx_message = Messages.GetMessageById(error),
                smt_st_eventorigin = "Plugin AssociateResourceWithProjectTask",
            };
            Service.Create(log);
        }

    }

}
