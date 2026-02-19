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
    public class PostUpdateSync_BookableResourceBooking : PluginBase
    {
        public PostUpdateSync_BookableResourceBooking() : base(typeof(PostUpdateSync_BookableResourceBooking)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_resourcerequirementBusiness business = new msdyn_resourcerequirementBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            BookableResourceBooking target = localcontext.GetTarget<BookableResourceBooking>();
            BookableResourceBooking preImage = localcontext.GetPreImage<BookableResourceBooking>();

            ValidateUpdate(target, preImage, business, localcontext);
        }


        protected void ValidateUpdate(BookableResourceBooking target, BookableResourceBooking preImage, msdyn_resourcerequirementBusiness business, LocalPluginContext localContext)
        {
            if (target.BookingStatus != null)
                business.ValidateUpdateCancel(localContext.PluginExecutionContext.UserId, preImage, target.BookingStatus.Id, target.Id);

        }
    }
}
