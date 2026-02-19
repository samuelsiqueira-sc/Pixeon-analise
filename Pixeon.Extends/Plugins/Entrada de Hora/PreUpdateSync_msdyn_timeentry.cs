using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Earlybound;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins.Entrada_de_Hora
{
    public class PreUpdateSync_msdyn_timeentry : PluginBase
    {
        public PreUpdateSync_msdyn_timeentry() : base(typeof(PreUpdateSync_msdyn_timeentry)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_timeentry target = localcontext.GetTarget<msdyn_timeentry>();
            TimeEntryBusiness business = new TimeEntryBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            ParentProjectTask(target, business);
        }

        public void ParentProjectTask(msdyn_timeentry target, TimeEntryBusiness business)
        {
            if (target.msdyn_projectTask != null)
            {
                business.IfProjectTaskIsParent(target);
            }
        }
    }
}
