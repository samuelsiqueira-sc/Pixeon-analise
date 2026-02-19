using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.CustomActions
{
    /// <summary>
    /// Action generica para atualizar campo acumulado 
    /// </summary>
    public class AC_CalculateRollup : PluginBase
    {
        /// <summary>
        /// AC_CalculateRollup
        /// </summary>
        public AC_CalculateRollup() : base(typeof(AC_CalculateRollup))
        {

        }
        /// <summary>
        /// ExecuteCrmPlugin
        /// </summary>
        /// <param name="localContext">localContext</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region Licença
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);
            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);

            #endregion

            #region ResX
            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            String entityString = localContext.PluginExecutionContext.InputParameters["Entity"].ToString();
            String id = localContext.PluginExecutionContext.InputParameters["Id"].ToString();
            EntityReference entity = new EntityReference(entityString, new Guid(id));
            String fields = localContext.PluginExecutionContext.InputParameters["Fields"].ToString();

            CalculateRollup(entity, fields, localContext);
        }
        /// <summary>
        /// CalculateRollup
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="fields">fields</param>
        /// <param name="localContext">localContext</param>
        private void CalculateRollup(EntityReference entity, String fields, LocalPluginContext localContext)
        {
            var fieldsList = fields.ToString().Split(';');

            foreach (var c in fieldsList.ToList())
            {
                CalculateRollupFieldRequest request = new CalculateRollupFieldRequest
                {
                    Target = entity,
                    FieldName = c
                };
                localContext.OrganizationService.Execute(request);
            }
        }
    }
}
