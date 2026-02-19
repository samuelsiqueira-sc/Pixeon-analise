using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Earlybound;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using Microsoft.Xrm.Sdk;

namespace Pixeon.Extends.Plugins.Despesas
{
    public class PostUpdateAsync_msdyn_expense : PluginBase
    {
        public PostUpdateAsync_msdyn_expense() : base(typeof(PostUpdateAsync_msdyn_expense)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_expense expense = localcontext.GetTarget<msdyn_expense>();
            msdyn_expense PreImage = localcontext.GetPreImage<msdyn_expense>();
            msdyn_expenseBusiness business = new msdyn_expenseBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            //// Invocação dos métodos. 
            if (expense.msdyn_ExpenseStatus != null && expense.msdyn_ExpenseStatus.Value == 192350001)
            {
                SetBillingType(PreImage, business);
            }
        }

        public void SetBillingType(msdyn_expense preImage, msdyn_expenseBusiness business)
        {
            msdyn_projectapproval approval = business.RetrieveProjectApproval(preImage);

            if (approval != null)
            {
                business.UpdateProjectApproval(approval, preImage);
            }
        }

    }
}
