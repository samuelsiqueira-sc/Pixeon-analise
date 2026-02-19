using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins
{
    public class PostUpdateAsync_Salesorder : PluginBase
    {
        public PostUpdateAsync_Salesorder() : base(typeof(PostUpdateAsync_Salesorder)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            SalesOrder target = localcontext.GetTarget<SalesOrder>();
            SalesOrder preImage = localcontext.GetPreImage<SalesOrder>();
            SalesOrderBusiness business = new SalesOrderBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            UpdateEventualRevenue(target, business);
            UpdateRecurrenceRevenue(target, preImage, business);
        }

        public void UpdateEventualRevenue(SalesOrder target, SalesOrderBusiness business)
        {
            if (target.smt_mn_value_eventual_final != null)
            {
                business.UpdateTasksEstimatedRevenue(target);
            }
        }

        public void UpdateRecurrenceRevenue(SalesOrder target, SalesOrder preImage, SalesOrderBusiness business)
        {
            if (target.smt_mn_recognized_rr != null && preImage.smt_bt_recognized_recurrence != true)
            {

                business.UpdateRecurrenceRevenue(target, preImage);
            }
        }
    }
}
