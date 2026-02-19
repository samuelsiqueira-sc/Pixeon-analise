using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CRM.Pixeon.Extends.Business;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk;

namespace Pixeon.Extends.Plugins.Tarefa_do_Projeto
{
    public class PreCreateSync_projecttask : PluginBase
    {
        public PreCreateSync_projecttask() : base(typeof(PreCreateSync_projecttask)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_projecttask target = localcontext.GetTarget<msdyn_projecttask>();
            projecttaskBusiness business = new projecttaskBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            AssociateProjectContract(target, business);
        }

        public void AssociateProjectContract(msdyn_projecttask projecttask, projecttaskBusiness business)
        {
            if (projecttask.msdyn_project != null)
            {
                business.AssociateSalesOrder(projecttask);
            }
        }
    }
}
