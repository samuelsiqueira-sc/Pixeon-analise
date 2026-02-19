using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.CustomActions.Smt_holiday_request
{
    /// <summary>
    /// a
    /// </summary>
    public class AC_CancelApprovalHoliday : PluginBase
    {
        /// <summary>
        /// a
        /// </summary>
        public AC_CancelApprovalHoliday() : base(typeof(AC_CancelApprovalHoliday))
        {

        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="localContext">a</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            var business = new ProjectBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            Entity entity = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];
            int value = ((OptionSetValue)entity.Attributes["statuscode"]).Value;

            if (value == 100000001)
                business.CancelHolidayEntry(entity);
        }
    }

}
