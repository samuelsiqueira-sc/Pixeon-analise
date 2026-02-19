using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Business
{
    /// <summary>
    /// Doc. 
    /// </summary>
    public class TimeEntryBusiness : BaseBusiness
    {
        /// <summary>
        /// Método construtor da business
        /// </summary>
        /// <param name="service"> service</param>
        /// <param name="serviceAdmin"> service Admin </param>
        /// <param name="tracingService"> tracing Service </param>
        /// <param name="messages"> mensagens resx </param>
        public TimeEntryBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <returns>Aprovação.</returns>
        /// <param name="timeEntry">Entrada de Hora.</param>
        public msdyn_projectapproval RetrieveProjectApproval(msdyn_timeentry timeEntry)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.ProjectApprovalSet.Where(
                    approval => approval.msdyn_TimeEntry.Id == timeEntry.Id).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="projectApproval">Aprovação do projeto.</param>
        /// <param name="timeEntry">Entrada de Hora.</param>
        public void UpdateProjectApproval(msdyn_projectapproval projectApproval, msdyn_timeentry timeEntry)
        {
            int option = timeEntry.smt_pl_billingtypee.Value;

            switch (option)
            {
                // Passível de Cobrança
                case 192350001:

                    option = 192350001;
                    break;

                // Não Passível de Cobrança
                case 192350000:

                    option = 192350000;
                    break;

                // Complementar
                case 180580002:

                    option = 192350002;
                    break;

                // Não Disponível   
                case 180580003:

                    option = 192350003;
                    break;

                default: break;
            }

            msdyn_projectapproval approval = new msdyn_projectapproval
            {
                Id = projectApproval.Id,
                msdyn_BillingType = new OptionSetValue(option),
                EntityState = EntityState.Changed,
            };
            Update(approval);
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="projectApproval">Aprovação do projeto.</param>
        /// <param name="timeEntry">Entrada de Hora.</param>
        public void UpdateTimeEntry(msdyn_projectapproval projectApproval, EntityReference timeEntry)
        {
            int option = projectApproval.msdyn_BillingType.Value;

            switch (option)
            {
                // Passível de Cobrança
                case 192350001:

                    option = 192350001;
                    break;

                // Não Passível de Cobrança
                case 192350000:

                    option = 192350000;
                    break;

                // Complementar
                case 180580002:

                    option = 192350002;
                    break;

                // Não Disponível   
                case 180580003:

                    option = 192350003;
                    break;

                default: break;
            }

            msdyn_timeentry approval = new msdyn_timeentry
            {
                Id = timeEntry.Id,
                smt_pl_billingtypee = new OptionSetValue(option),
                EntityState = EntityState.Changed,
            };
            Update(approval);
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="projectApproval">Aprovação do projeto.</param>
        /// <param name="timeEntry">Entrada de Hora.</param>
        public void UpdateTimeEntry(msdyn_projectapproval projectApproval, msdyn_timeentry timeEntry)
        {
            int option = projectApproval.msdyn_BillingType.Value;

            switch (option)
            {
                // Passível de Cobrança
                case 192350001:

                    option = 192350001;
                    break;

                // Não Passível de Cobrança
                case 192350000:

                    option = 192350000;
                    break;

                // Complementar
                case 180580002:

                    option = 192350002;
                    break;

                // Não Disponível   
                case 180580003:

                    option = 192350003;
                    break;

                default: break;
            }

            msdyn_timeentry time = new msdyn_timeentry
            {
                Id = timeEntry.Id,
                smt_pl_billingtypee = new OptionSetValue(option),
                EntityState = EntityState.Changed,
            };
            Update(time);
        }

        public void IfProjectTaskIsParent(msdyn_timeentry target)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                List<msdyn_projecttask> childTasks = context.CreateQuery<msdyn_projecttask>().Where(p => p.msdyn_parenttask.Id == target.msdyn_projectTask.Id).ToList();

                if (childTasks.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Não é possível lançar horas em uma tarefa pai.");
                }
            }
        }

        public void UpdateProject(msdyn_timeentry preImage)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                msdyn_project project = context.CreateQuery<msdyn_project>().Where(p => p.Id == preImage.msdyn_project.Id).FirstOrDefault();

                decimal? billable = context.CreateQuery<msdyn_timeentry>().Where(t => t.msdyn_project != null && t.msdyn_project.Id == preImage.msdyn_project.Id && t.smt_pl_billingtypee != null && (t.smt_pl_billingtypee.Value == 192350001 || t.smt_pl_billingtypee.Value == 192350002)).ToList().Sum(t => t.msdyn_duration) / new decimal(60);
                decimal? nobillable = context.CreateQuery<msdyn_timeentry>().Where(t => t.msdyn_project != null && t.msdyn_project.Id == preImage.msdyn_project.Id && t.smt_pl_billingtypee != null && (t.smt_pl_billingtypee.Value == 192350000 || t.smt_pl_billingtypee.Value == 192350003)).ToList().Sum(t => t.msdyn_duration) / new decimal(60);
                nobillable += context.CreateQuery<msdyn_timeentry>().Where(t => t.msdyn_project != null && t.msdyn_project.Id == preImage.msdyn_project.Id && t.smt_pl_billingtypee == null).ToList().Sum(t => t.msdyn_duration) / new decimal(60);
                msdyn_actual actual = context.CreateQuery<msdyn_actual>().Where(d => d.msdyn_Project != null && d.msdyn_Project.Id == project.Id && d.msdyn_description == "Horas Legado").FirstOrDefault();

                if (actual != null)
                {
                    if (actual.smt_pl_billingtypee != null && (actual.smt_pl_billingtypee.Value == 192350001 || actual.smt_pl_billingtypee.Value == 192350002))
                    {
                        billable += actual.msdyn_Quantity;
                    }
                    else
                    {
                        nobillable += actual.msdyn_Quantity;
                    }
                }
                
                decimal? saldo = /*project.msdyn_plannedhours -*/ billable;

                msdyn_project updateProject = new msdyn_project
                {
                    Id = project.Id,
                    smt_dc_billable_hours = billable,
                    smt_dc_nonbillable_hours = nobillable,
                    smt_dc_balance_hours = saldo
                };

                ServiceAdmin.Update(updateProject);

            }
        }

        public void UpdateProjectDevolvido(msdyn_timeentry preImage)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                msdyn_project project = context.CreateQuery<msdyn_project>().Where(p => p.Id == preImage.msdyn_project.Id).FirstOrDefault();

                decimal? billable = context.CreateQuery<msdyn_timeentry>().Where(t => t.msdyn_project != null && t.msdyn_project.Id == preImage.msdyn_project.Id && t.msdyn_entryStatusEnum == msdyn_timeentrystatus.Aprovado && t.smt_pl_billingtypee != null && (t.smt_pl_billingtypee.Value == 192350001 || t.smt_pl_billingtypee.Value == 192350002)).ToList().Sum(t => t.msdyn_duration) / new decimal(60);
                decimal? nobillable = context.CreateQuery<msdyn_timeentry>().Where(t => t.msdyn_project != null && t.msdyn_project.Id == preImage.msdyn_project.Id && t.msdyn_entryStatusEnum == msdyn_timeentrystatus.Aprovado && t.smt_pl_billingtypee != null && (t.smt_pl_billingtypee.Value == 192350000 || t.smt_pl_billingtypee.Value == 192350003)).ToList().Sum(t => t.msdyn_duration) / new decimal(60);
                nobillable += context.CreateQuery<msdyn_timeentry>().Where(t => t.msdyn_project != null && t.msdyn_project.Id == preImage.msdyn_project.Id && t.smt_pl_billingtypee == null).ToList().Sum(t => t.msdyn_duration) / new decimal(60);
                msdyn_actual actual = context.CreateQuery<msdyn_actual>().Where(d => d.msdyn_Project != null && d.msdyn_Project.Id == project.Id && d.msdyn_description == "Horas Legado").FirstOrDefault();

                if (actual != null)
                {
                    if (actual.smt_pl_billingtypee != null && (actual.smt_pl_billingtypee.Value == 192350001 || actual.smt_pl_billingtypee.Value == 192350002))
                    {
                        billable += actual.msdyn_Quantity;
                    }
                    else
                    {
                        nobillable += actual.msdyn_Quantity;
                    }
                }

                decimal? saldo = /*project.msdyn_plannedhours - */billable;

                msdyn_project updateProject = new msdyn_project
                {
                    Id = project.Id,
                    smt_dc_billable_hours = billable,
                    smt_dc_nonbillable_hours = nobillable,
                    smt_dc_balance_hours = saldo
                };

                ServiceAdmin.Update(updateProject);

            }
        }
        /// <summary>
        /// Encontra o registro de uma tarefa de projeto.
        /// </summary>
        /// <param name="taskId">Id da tarefa de projeto.</param>
        /// <returns>Retorna o registro da tarefa de projeto encontrado ou null, caso contrário.</returns>
        public msdyn_projecttask FindProjectTask(Guid taskId)
        {
            using (CrmServiceContext localHost = new CrmServiceContext(Service))
            {
                var task = from t in localHost.CreateQuery<msdyn_projecttask>()
                           where t.Id == taskId
                           select t;

                return task.FirstOrDefault();
            }
        }
        /// <summary>
        /// Procura o registro de um projeto.
        /// </summary>
        /// <param name="projectId">Id do projeto</param>
        /// <returns>Retorna o registro do projetou ou null caso não haja.</returns>
        public msdyn_project FindProject(Guid projectId)
        {
            using (CrmServiceContext localHost = new CrmServiceContext(Service))
            {
                var project = from p in localHost.CreateQuery<msdyn_project>()
                              where p.Id == projectId
                              select new msdyn_project
                              {
                                  Id = p.Id,
                                  msdyn_projectmanager = p.msdyn_projectmanager,
                                  smt_lp_temporary_manager = p.smt_lp_temporary_manager
                              };

                return project.FirstOrDefault();
            }
        }
        /// <summary>
        /// Encontra o registro de um usuario de um Recurso Reservável.
        /// </summary>
        /// <param name="userId">Id do usuário do Recurso Reservpavel</param>
        /// <returns>Retorna o id do usuário do Recurso Reservável ou null caso não haja.</returns>
        public SystemUser FindUser(Guid userId)
        {
            using (CrmServiceContext localHost = new CrmServiceContext(Service))
            {
                var user = from u in localHost.CreateQuery<BookableResource>()
                           where u.Id == userId
                           select new SystemUser
                           {
                               Id = u.UserId.Id
                           };

                return user.FirstOrDefault();
            }
        }
    }
}
