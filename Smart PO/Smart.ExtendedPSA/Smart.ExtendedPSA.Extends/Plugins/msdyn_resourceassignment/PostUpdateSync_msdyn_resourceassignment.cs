using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Plugins
{
     /// <summary>
     /// hgjh
     /// </summary>
    public class PostUpdateSync_msdyn_resourceassignment : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='Plugin'/> class.
        /// </summary>
        public PostUpdateSync_msdyn_resourceassignment():base(typeof(PostUpdateSync_msdyn_resourceassignment)) { }

        /// <summary>
        /// Plugin execution
        /// </summary>
        /// <param name="localContext">Plugin execution context</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region Validação da Licença Smart PO
            /*WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);*/
            #endregion

            msdyn_resourceassignment entity = localContext.GetTarget<msdyn_resourceassignment>();
            var preImg = localContext.GetPreImage<msdyn_resourceassignment>();
            var business = new msdyn_resourceassignmentBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            var tarefaBusiness = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
           
            business.AtualizaEsforçoPresenteTarefasPais(entity,preImg, tarefaBusiness);
        }
    }
}
