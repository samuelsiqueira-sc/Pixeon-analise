using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Business;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using CrmEarlyBound;
using CRM.Pixeon.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Pixeon.Extends.Business
{
    public class SalesOrderBusiness : BaseBusiness
    {
        public SalesOrderBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        public void UpdateTasksEstimatedRevenue(SalesOrder target)
        {
            UpdateEventualSalesOrderDetail(target.smt_mn_value_eventual_final, target.Id);

            // msdyn_project project = GetProject(target.Id);

            // if (project != null)
            // {
            //    List<msdyn_projecttask> tasks = GetEventualProjecttasks(project.ToEntityReference());

            // UpdateProjectTasks(tasks, target.smt_mn_value_eventual_final, target);
            // }
        }

        public void UpdateEventualSalesOrderDetail(Money eventualRevenue, Guid salesOrderid)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var detail = context.SalesOrderDetailSet.Where(a => a.SalesOrderId == new EntityReference(SalesOrder.EntityLogicalName, salesOrderid) && a.ProductTypeCode.Value == 5).FirstOrDefault();

                if (detail != null)
                {
                    SalesOrderDetail salesOrderDetail = new SalesOrderDetail
                    {
                        Id = detail.Id,
                        PricePerUnit = eventualRevenue,
                        EntityState = EntityState.Changed
                    };

                    ServiceAdmin.Update(salesOrderDetail);
                }
            }
        }

        public void UpdateRecurrenceSalesOrderDetail(Money recurrenceRevenue, Guid salesOrderid)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var detail = context.SalesOrderDetailSet.Where(a => a.SalesOrderId == new EntityReference(SalesOrder.EntityLogicalName, salesOrderid) && a.ProductTypeCode.Value == 1).FirstOrDefault();

                if (detail != null)
                {
                    SalesOrderDetail salesOrderDetail = new SalesOrderDetail
                    {
                        Id = detail.Id,
                        PricePerUnit = recurrenceRevenue,
                        EntityState = EntityState.Changed
                    };

                    ServiceAdmin.Update(salesOrderDetail);
                }
            }
        }

        public void UpdateProjectTasks(List<msdyn_projecttask> projecttasks, Money valueFinal, SalesOrder salesorder)
        {
            foreach (msdyn_projecttask projecttask in projecttasks)
            {
                smt_milestone_project milestone = GetMilestone(projecttask.smt_lp_milestone.Id);

                if (milestone.smt_pl_type.Value == 1)
                {
                    UpdateEventualTask(projecttask, milestone, valueFinal);
                }
                else if (milestone.smt_pl_type.Value == 0)
                {
                    UpdateRecurrenceTask(projecttask, salesorder, valueFinal);
                }
            }
        }

        public void UpdateEventualTask(msdyn_projecttask projecttask, smt_milestone_project milestone, Money eventualFinal)
        {
            decimal estimatedRevenue = (eventualFinal.Value * (decimal)milestone.smt_dc_stage) / 100;

            msdyn_projecttask task = new msdyn_projecttask
            {
                Id = projecttask.Id,
                smt_mn_estimated_revenue = new Money(estimatedRevenue),
                EntityState = EntityState.Changed
            };

            ServiceAdmin.Update(task);
        }

        public void UpdateRecurrenceTask(msdyn_projecttask projecttask, SalesOrder salesOrder, Money recurrenceFinal)
        {
            msdyn_projecttask taskUpdate = new msdyn_projecttask
            {
                Id = projecttask.Id,
                smt_mn_estimated_revenue = recurrenceFinal,
                EntityState = EntityState.Changed
            };

            if (salesOrder.smt_pl_type_rr != null && salesOrder.smt_pl_type_rr.Value == 180580001)
                taskUpdate.smt__dt_estimated = salesOrder.smt_dt_recurrence_charge_planned_date;
            else
                taskUpdate.smt__dt_estimated = projecttask.msdyn_scheduledend;

            ServiceAdmin.Update(taskUpdate);
        }

        public msdyn_project GetProject(Guid salesorderid)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                return context.CreateQuery<msdyn_project>().Where(a => a.msdyn_salesorderid == new EntityReference(SalesOrder.EntityLogicalName, salesorderid)).FirstOrDefault();
            }
        }

        public List<msdyn_projecttask> GetRecurrenceProjecttasks(EntityReference project)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var projecttasks = (from task in context.CreateQuery<msdyn_projecttask>()
                                    join marco in context.milestone_projectSet on task.smt_lp_milestone.Id equals marco.Id
                                    where task.msdyn_project == project
                                    && task.smt_statusprojeto != new OptionSetValue(100000002) && task.smt_lp_milestone != null
                                    && marco.smt_pl_type.Value == 0
                                    select task).ToList();

                return projecttasks;
            }
        }

        public List<msdyn_projecttask> GetEventualProjecttasks(EntityReference project)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var projecttasks = (from task in context.CreateQuery<msdyn_projecttask>()
                                    join marco in context.milestone_projectSet on task.smt_lp_milestone.Id equals marco.Id
                                    where task.msdyn_project == project
                                    && task.smt_statusprojeto != new OptionSetValue(100000002) && task.smt_lp_milestone != null
                                    && marco.smt_pl_type.Value == 1
                                    select task).ToList();

                return projecttasks;
            }
        }
        public smt_milestone_project GetMilestone(Guid id)
        {
            using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
            {
                return serviceContext.milestone_projectSet.Where(a => a.Id == id).FirstOrDefault();
            }

        }

        public EntityReference GetUOFromOwner(EntityReference owner)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                EntityReference unitOrganization = (from resource in context.BookableResourceSet
                                                    where resource.UserId == owner
                                                    select resource.msdyn_organizationalunit).FirstOrDefault();

                return unitOrganization;
            }
        }

        public smt_parameter_alocation GetUO(SalesOrder order)
        {
            smt_parameter_alocation alocation = new smt_parameter_alocation();

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                Account account = ServiceAdmin.Retrieve(Account.EntityLogicalName, order.CustomerId.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("smt_pl_customer_region")).ToEntity<Account>();

                if (order.smt_pl_classification != null && order.smt_smt_pl_family != null && order.smt_pl_focused_product != null && order.smt_pl_items != null && account.smt_pl_customer_region != null)
                {
                    var parameters = (from a in crmService.CreateQuery<smt_parameter_alocation>()
                                      where a.smt_pl_region != null && a.smt_pl_sales_classification != null && a.smt_plm_family != null
                                      && a.smt_plm_focused_product != null && a.smt_plm_items != null
                                      select a).ToList();

                    alocation = parameters.Where(a => a.smt_pl_region.Any(b => b.Value == account.smt_pl_customer_region.Value) && a.smt_pl_sales_classification.Any(b => b.Value == order.smt_pl_classification.Value)
                    && a.smt_plm_family.Any(d => d.Value == order.smt_smt_pl_family.Value) && a.smt_plm_focused_product.Any(e => e.Value == order.smt_pl_focused_product.Value)
                    && a.smt_plm_items.Any(f => f.Value == order.smt_pl_items.Value)).Select(a => a).FirstOrDefault();

                }
            }
            return alocation;
        }

        /// <summary>
        /// Busca a cotação relacionada ao contrato.
        /// </summary>
        /// <param name="quoteId">Guid da cotação.</param>
        /// <returns>Cotação contendo apenas a unidade de contratação</returns>
        public Quote GetOrganizationUnit(Guid quoteId)
        {
            
            Quote cotacao;

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
               cotacao = ServiceAdmin.Retrieve(Quote.EntityLogicalName, quoteId, new Microsoft.Xrm.Sdk.Query.ColumnSet("msdyn_contractorganizationalunitid")).ToEntity<Quote>();
                
            }
            return cotacao;
        }

        /// <summary>
        /// A finalidade do método é buscar uma Unidade Organizacional através do campo "Dominio da Equipe"
        /// </summary>
        /// <param name="domainName">Nome do dominio obtido através da cotação. </param>
        /// <returns> Unidade Organizacional </returns>
        public msdyn_organizationalunit GetUOByDomain(string domainName)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    var unitOrganizations = (from uo in context.CreateQuery<msdyn_organizationalunit>()
                                             where uo.msdyn_name != null &&
                                             uo.smt_st_domain == domainName
                                             select uo).FirstOrDefault();

                    return unitOrganizations;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateRecurrenceRevenue(SalesOrder target, SalesOrder preImage)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                // Define o valor recorrente  como ou o valor passado pelo contexto ou o Valor de RR a ser Reconhecido valor anterior
                Money totalRecurrence = target.smt_mn_recognized_rr != null ? target.smt_mn_recognized_rr : preImage.smt_mn_recognized_rr;
                // int? period = target.smt_int_charge_period != null ? target.smt_int_charge_period : preImage.smt_int_charge_period;

                // Money recurrenceFinal = new Money(totalRecurrence.Value / period.Value);

                // SalesOrderDetail recurrenceDetail = context.CreateQuery<SalesOrderDetail>().Where(s => s.SalesOrderId.Id == target.Id && s.ProductTypeCode.Value == 1).FirstOrDefault();

                // Calcula o valo líquido
                // decimal tax = (recurrenceFinal.Value * recurrenceDetail.Tax.Value) / 100;
                // recurrenceFinal.Value -= tax;

                UpdateRecurrenceSalesOrderDetail(totalRecurrence, target.Id);

                // msdyn_project project = GetProject(target.Id);

                // if (project != null)
                // {
                //    List<msdyn_projecttask> projecttasks = GetRecurrenceProjecttasks(project.ToEntityReference());

                // UpdateProjectTasks(projecttasks, recurrenceFinal, preImage);
                // }
            }
        }
        /// <summary>
        /// Obtém a Modalidade de Canal definida na cotação
        /// </summary>
        /// <param name="order">Contrato a ser alterado</param>
        public Quote getModalidadeCanal(SalesOrder order)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var quote = (from q in context.CreateQuery<Quote>()
                             where q.Id == order.QuoteId.Id
                             select new Quote { smt_op_modalidade_canal = q.smt_op_modalidade_canal }).FirstOrDefault();
                return quote;
            }
           
        }

        public void CloseSalesOrder(SalesOrder target)
        {
            var cancelRequest = new CancelSalesOrderRequest()
            {
                OrderClose = new OrderClose()
                {
                    SalesOrderId = target.ToEntityReference(),
                    Subject = "Close Sales Order " + DateTime.Now
                },
                Status = new OptionSetValue(180580000)
            };

            ServiceAdmin.Execute(cancelRequest);
        }

        /// <summary>
        /// Retorna o parâmetro de alocação
        /// </summary>
        /// <param name="sales">Cotação</param>
        /// <returns>time</returns>
        public msdyn_organizationalunit GetTeamOwnerSales(SalesOrder sales)
        {
            msdyn_organizationalunit unit = new msdyn_organizationalunit();

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                var team = (from i in crmService.CreateQuery<Team>()
                            where i.TeamId == sales.OwnerId.Id
                            select i).FirstOrDefault();

                if (team != null)
                {

                    var negocio = (from i in crmService.CreateQuery<BusinessUnit>()
                                   where i.BusinessUnitId == team.BusinessUnitId.Id
                                   select i).FirstOrDefault();
                    if (negocio != null)
                    {
                        unit = (from u in crmService.CreateQuery<msdyn_organizationalunit>()
                                where u.msdyn_name == negocio.Name
                                select u).FirstOrDefault();
                    }
                }

                return unit;
            }
        }

        /// <summary>
        /// Retorna o parâmetro de alocação
        /// </summary>
        /// <param name="sales">Cotação</param>
        /// <returns>time</returns>
        public msdyn_organizationalunit GetUserOwnerSales(SalesOrder sales)
        {
            msdyn_organizationalunit unit = new msdyn_organizationalunit();

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                var team = (from i in crmService.CreateQuery<SystemUser>()
                            where i.SystemUserId == sales.OwnerId.Id
                            select i).FirstOrDefault();

                if (team != null)
                {

                    var negocio = (from i in crmService.CreateQuery<BusinessUnit>()
                                   where i.BusinessUnitId == team.BusinessUnitId.Id
                                   select i).FirstOrDefault();
                    if (negocio != null)
                    {
                        unit = (from u in crmService.CreateQuery<msdyn_organizationalunit>()
                                where u.msdyn_name == negocio.Name
                                select u).FirstOrDefault();
                    }
                }

                return unit;
            }
        }

        public void GetArqs(SalesOrder target)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var arqs = (from a in crmService.CreateQuery<smt_arquivossalesforce>()
                            where a.smt_lp_cotacao == target.QuoteId
                            select a).ToList();

                foreach (var arq in arqs)
                {
                    smt_arquivossalesforce arquivo = new smt_arquivossalesforce();
                    arquivo.Id = arq.Id;
                    arquivo.smt_lp_project_contract = target.ToEntityReference();

                    ServiceAdmin.Update(arquivo);
                }
            }
        }

        public void GetDuplicate(SalesOrder target)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var salesOrder = (from a in crmService.CreateQuery<SalesOrder>()
                                  where a.smt_st_notificationid == target.smt_st_notificationid
                                  && a.StateCode.Value == 0
                                  select a.Id).ToList();

                if (salesOrder.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Já existe um contrato de projeto com esse número de oportunidade. Por favor, contate o administrador.");
                }

            }
        }
    }
}
