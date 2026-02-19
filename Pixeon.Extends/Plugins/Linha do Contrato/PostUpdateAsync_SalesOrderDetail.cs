using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins.Linha_do_Contrato
{
    public class PostUpdateAsync_SalesOrderDetail : PluginBase
    {
        public PostUpdateAsync_SalesOrderDetail() : base(typeof(PostUpdateAsync_SalesOrderDetail)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            SalesOrderDetail target = localcontext.GetTarget<SalesOrderDetail>();
            SalesOrderDetail preImage = localcontext.GetPreImage<SalesOrderDetail>();
            salesorderdetailBusiness business = new salesorderdetailBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            UpdateProjectTasks(target, preImage, business);
        }

        public void UpdateProjectTasks(SalesOrderDetail target, SalesOrderDetail preImage, salesorderdetailBusiness business)
        {
            if (target.PricePerUnit != null || target.msdyn_Project != null)
                business.UpdateEstimatedRevenueFromProjectTasks(target, preImage);
        }
    }
}
