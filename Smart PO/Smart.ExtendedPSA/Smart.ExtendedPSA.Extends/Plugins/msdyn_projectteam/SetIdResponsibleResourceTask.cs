using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;

namespace Smart.ExtendedPSA.Extends.Plugins.msdyn_projectteam
{
    /// <summary>
    /// PG_SetId
    /// </summary>
    public class SetIdResponsibleResourceTask : PluginBase
    {

        /// <summary>
        /// PG_SetId
        /// </summary>
        public SetIdResponsibleResourceTask() : base(typeof(SetIdResponsibleResourceTask)) { }

        /// <summary>
        /// ExecuteCrmPlugin
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {

            var context = localContext.
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            // Pegar valor do campo e-mail smt_str_email_resource
            // Variaveis utilizadas 
            var bookableResource = new BookableResource();


            using (var orgContext = new OrganizationServiceContext(service))

                bookableResource = (from resource in orgconte
                                    where
                                        select);





        }




    }
}
