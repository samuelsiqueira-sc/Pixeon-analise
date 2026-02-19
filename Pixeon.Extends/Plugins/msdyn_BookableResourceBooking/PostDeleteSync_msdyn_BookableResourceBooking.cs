using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using Pixeon.Extends.Plugins;
using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using Pixeon.Extends.Business;
using Microsoft.Xrm.Sdk;
using CrmEarlyBound;

namespace Pixeon.Extends.Plugins.msdyn_BookableResourceBooking
{
    public class PostDeleteSync_BookableResourceBooking : PluginBase
    {
        public PostDeleteSync_BookableResourceBooking() : base(typeof(PostDeleteSync_BookableResourceBooking)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_resourcerequirementBusiness business = new msdyn_resourcerequirementBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            BookableResourceBooking preImage = localcontext.GetPreImage<BookableResourceBooking>();

            ValidateUpdate(preImage, business, localcontext);
        }

        protected void ValidateUpdate( BookableResourceBooking preImage, msdyn_resourcerequirementBusiness business, LocalPluginContext localContext)
        {
            business.ValidateDelete(localContext.PluginExecutionContext.InitiatingUserId, preImage);
        }
    }
}
