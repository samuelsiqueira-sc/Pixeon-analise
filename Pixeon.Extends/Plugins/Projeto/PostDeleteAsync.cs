using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Projeto
{
    public class PostDeleteAsync : PluginBase
    {
        public PostDeleteAsync() : base(typeof(PostDeleteAsync)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_project preImage = localcontext.GetPreImage<msdyn_project>();
            ProjetoBusiness business = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            DeleteOwnerTeam(preImage, business);
        }

        public void DeleteOwnerTeam(msdyn_project preImage, ProjetoBusiness business)
        {
            if (preImage.OwningTeam != null)
            {
                business.DeleteOwnerTeam(preImage);
            }
        }
    }
}
