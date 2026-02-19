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
    public class salesorderdetailBusiness : BaseBusiness
    {

        /// <summary>
        /// Método contrutor para receber alguns parametros precisos para a execução dos métodos da classe.
        /// </summary>
        /// <param name="service">Variavel de Serviço</param>
        /// <param name="serviceAdmin"> ServiceADM</param>
        /// <param name="tracingService">Atributo do tipo IorganizationService, utilizado para executar métodos do SDK do Dynamics CRM</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        public salesorderdetailBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <returns>Doc.</returns>
        /// <param name="salesOrderDetail">Linha da ordem.</param>
        public Money TotalRevenuesRecurrence(SalesOrderDetail salesOrderDetail)
        {
            List<Money> result = null;

            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    result = context.SalesOrderDetailSet.Where(orderDetail => orderDetail.ProductTypeCode.Value == 1
                    && orderDetail.SalesOrderId == salesOrderDetail.SalesOrderId
                    && orderDetail.ExtendedAmount != null
                    ).Select(amount => amount.ExtendedAmount)
                    .ToList();

                    decimal totalRecurrent = salesOrderDetail.PricePerUnit.Value != null ? salesOrderDetail.PricePerUnit.Value : 0;
                    totalRecurrent += salesOrderDetail.Tax != null ? salesOrderDetail.Tax.Value + result.Sum(total => total.Value) : result.Sum(total => total.Value);

                    return new Money(totalRecurrent);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <returns>Doc.</returns>
        /// <param name="salesOrderDetail">Linha da ordem.</param>
        public Money TotalRevenuesEventual(SalesOrderDetail salesOrderDetail)
        {
            List<Money> result = null;

            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    result = context.SalesOrderDetailSet.Where(orderDetail => orderDetail.ProductTypeCode.Value == 5
                    && orderDetail.SalesOrderId == salesOrderDetail.SalesOrderId
                    && orderDetail.ExtendedAmount != null
                    ).Select(amount => amount.ExtendedAmount)
                    .ToList();

                    decimal totalEventual = salesOrderDetail.PricePerUnit.Value != null ? salesOrderDetail.PricePerUnit.Value : 0;
                    totalEventual += salesOrderDetail.Tax != null ? salesOrderDetail.Tax.Value + result.Sum(total => total.Value) : result.Sum(total => total.Value);

                    return new Money(totalEventual);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <returns>Doc.</returns>
        /// <param name="salesOrderDetail">Linha da ordem.</param>
        /// <param name="image">Pre image.</param>
        public Money TotalRevenuesRecurrenceUpdating(SalesOrderDetail salesOrderDetail, SalesOrderDetail image)
        {
            List<Money> result = null;

            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    result = context.SalesOrderDetailSet.Where(orderDetail => orderDetail.ProductTypeCode.Value == 1
                    && orderDetail.SalesOrderId == image.SalesOrderId
                    && orderDetail.ExtendedAmount != null
                    ).Select(amount => amount.ExtendedAmount)
                    .ToList();

                    var totalRecurrence = salesOrderDetail.PricePerUnit != null ? salesOrderDetail.PricePerUnit.Value : image.PricePerUnit != null ? image.PricePerUnit.Value : 0;
                    totalRecurrence += salesOrderDetail.Tax != null ? salesOrderDetail.Tax.Value + result.Sum(a => a.Value) : image.Tax != null ? image.Tax.Value + result.Sum(a => a.Value) : result.Sum(a => a.Value);

                    return new Money(totalRecurrence);
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
        /// <returns>Doc.</returns>
        /// <param name="salesOrderDetail">Linha da ordem.</param>
        /// <param name="image">Pre image.</param>
        public Money TotalRevenuesEventualUpdating(SalesOrderDetail salesOrderDetail, SalesOrderDetail image)
        {
            List<Money> result = null;

            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    result = context.SalesOrderDetailSet.Where(orderDetail => orderDetail.ProductTypeCode.Value == 5
                    && orderDetail.SalesOrderId == image.SalesOrderId
                    && orderDetail.ExtendedAmount != null
                    ).Select(amount => amount.ExtendedAmount)
                    .ToList();

                    var totalRecurrence = salesOrderDetail.PricePerUnit != null ? salesOrderDetail.PricePerUnit.Value : image.PricePerUnit != null ? image.PricePerUnit.Value : 0;
                    totalRecurrence += salesOrderDetail.Tax != null ? salesOrderDetail.Tax.Value + result.Sum(a => a.Value) : image.Tax != null ? image.Tax.Value + result.Sum(a => a.Value) : result.Sum(a => a.Value);

                    return new Money(totalRecurrence);
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
        /// <param name="salesOrderGuid">GUID da Ordem.</param>
        /// <param name="recurrenceTotal">Total do Recorrent.</param>
        public void UpdateSalesOrderRecurrence(Guid salesOrderGuid, Money recurrenceTotal)
        {
            SalesOrder order = new SalesOrder
            {
                Id = salesOrderGuid,
                smt_mn_total_recurrence = recurrenceTotal,
                EntityState = EntityState.Changed,
            };
            Update(order);
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="salesOrderGuid">GUID da Ordem.</param>
        /// <param name="eventualTotal">Total do Eventual.</param>
        public void UpdateSalesOrderEventual(Guid salesOrderGuid, Money eventualTotal)
        {
            SalesOrder order = new SalesOrder
            {
                Id = salesOrderGuid,
                smt_mn_value_eventual_final = eventualTotal,
                EntityState = EntityState.Changed,
            };
            Update(order);
        }

        public void UpdateEstimatedRevenueFromProjectTasks(SalesOrderDetail target, SalesOrderDetail preImage)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                Money price = target.PricePerUnit != null ? new Money (target.PricePerUnit.Value) : new Money (preImage.PricePerUnit.Value);
                var project2 = preImage.msdyn_Project != null ? preImage.msdyn_Project : target.msdyn_Project;
                // decimal discount = (price.Value * preImage.smt_dc_imposto.Value) / 100;
               // Money liquidRevenue = new Money(price.Value - discount);
                // Se a linha for eventual
                if (preImage.ProductTypeCode.Value == 5 && project2 != null)
                {
                    List<msdyn_projecttask> tasks = GetEventualProjecttasks(project2);
                    
                    UpdateEventualRevenue(tasks /*liquidRevenue*/, price);
                }
                else if (preImage.ProductTypeCode.Value == 1)
                {
                    SalesOrder salesOrder = context.CreateQuery<SalesOrder>().Where(s => s.Id == preImage.SalesOrderId.Id).FirstOrDefault();
                    msdyn_project project = context.CreateQuery<msdyn_project>().Where(p => p.msdyn_salesorderid.Id == salesOrder.Id).FirstOrDefault();
                    UpdateRecurrenceRevenue(project.ToEntityReference(), /*liquidRevenue*/ price, salesOrder);
                }
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


        public msdyn_projecttask GetRecurrenceProjecttasks(EntityReference project, bool login)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var projecttasks = (from task in context.CreateQuery<msdyn_projecttask>()
                                    join marco in context.milestone_projectSet on task.smt_lp_milestone.Id equals marco.Id
                                    where task.msdyn_project == project
                                    && task.smt_statusprojeto != new OptionSetValue(100000002) && task.smt_lp_milestone != null
                                    && marco.smt_pl_type.Value == 0 && marco.smt_bl_login_password == login
                                    select task).FirstOrDefault();

                return projecttasks;
            }
        }

        public void UpdateEventualRevenue(List<msdyn_projecttask> projecttasks/*, Money liquidRevenue*/, Money revenue)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                foreach (msdyn_projecttask task in projecttasks)
                {
                    smt_milestone_project marco = context.CreateQuery<smt_milestone_project>().Where(m => m.Id == task.smt_lp_milestone.Id).FirstOrDefault();
                    msdyn_projecttask taskUpdate = new msdyn_projecttask { Id = task.Id };
                    taskUpdate.smt_mn_estimated_revenue = new Money((revenue.Value * marco.smt_dc_stage.Value) / 100);
                   // taskUpdate.smt_mn_estimated_liquidrevenue = new Money((liquidRevenue.Value * marco.smt_dc_stage.Value) / 100);
                    taskUpdate.smt__dt_estimated = task.msdyn_scheduledend;
                    ServiceAdmin.Update(taskUpdate);
                }
            }
        }

        public void UpdateRecurrenceRevenue(EntityReference project, /*Money liquidRevenue,*/ Money revenue, SalesOrder salesOrder)
        {
            // Se o contrato for carência ou padrão
            if (salesOrder.smt_pl_type_rr.Value == 180580001 || salesOrder.smt_pl_type_rr.Value == 180580002) // Se o contrato for Carência ou Padrão
            {
                msdyn_projecttask task = GetRecurrenceProjecttasks(project, true);
                msdyn_projecttask taskUpdate = new msdyn_projecttask { Id = task.Id };
               // taskUpdate.smt_mn_estimated_liquidrevenue = liquidRevenue;
                taskUpdate.smt_mn_estimated_revenue = revenue;
                ServiceAdmin.Update(taskUpdate);
            }
            else if (salesOrder.smt_pl_type_rr.Value == 180580000 || salesOrder.smt_pl_type_rr.Value == 100000001) // Se o contrato for go/live ou Intalação
            {
                msdyn_projecttask task = GetRecurrenceProjecttasks(project, false);
                msdyn_projecttask taskUpdate = new msdyn_projecttask { Id = task.Id };
               // taskUpdate.smt_mn_estimated_liquidrevenue = liquidRevenue;
                taskUpdate.smt_mn_estimated_revenue = revenue;
                ServiceAdmin.Update(taskUpdate);
            }
        }
    }
}
