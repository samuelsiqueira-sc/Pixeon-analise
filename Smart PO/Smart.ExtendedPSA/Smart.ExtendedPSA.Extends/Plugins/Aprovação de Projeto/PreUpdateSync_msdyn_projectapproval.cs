using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// </summary>
    public class PreUpdateSync_msdyn_projectapproval : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe
        /// </summary>       
        public PreUpdateSync_msdyn_projectapproval() : base(typeof(PreUpdateSync_msdyn_projectapproval)) { }

        /// <summary>
        /// Execução do plugin
        /// </summary>
        /// <param name="localcontext">Local Context</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            #region Licença
            // Chamada da Action de Licenças
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localcontext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localcontext.OrganizationService.Execute(ActionRequest);
            #endregion

            #region ResX

            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localcontext.LoadResxMessages(resxFileName);
            #endregion

            // variáveis utilizadas
            msdyn_projectapproval target = localcontext.GetTarget<msdyn_projectapproval>();
            msdyn_projectapproval preImg = localcontext.GetPreImage<msdyn_projectapproval>();            
                    
        }

    }
}
