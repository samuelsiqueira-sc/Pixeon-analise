using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;

namespace Smart.ExtendedPSA.Extends.Plugins.Smt_purchasing_period
{
    /// <summary>
    /// PostOperation Update Synchronous em Período aquisitivo
    /// </summary>
    public class PostUpdateSync_smt_purchasing_period : PluginBase
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public PostUpdateSync_smt_purchasing_period() : base(typeof(PostUpdateSync_smt_purchasing_period)) { }

        /// <summary>
        /// class execute
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
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

            // Declaração da entidade-alvo que será trabalhada. 
            var target = localContext.GetTarget<smt_purchasing_period>();
            var preImage = localContext.GetPreImage<smt_purchasing_period>();

            CalculateRollupFieldRequest(localContext, preImage);
        }

        /// <summary>
        /// Método para atualizar a data final do Período Aquisitivo.
        /// </summary>
        ///<param name="localContext"> Contexto </param>
        /// <param name="preImage"> período aquisitivo </param>
        private void CalculateRollupFieldRequest(LocalPluginContext localContext, smt_purchasing_period preImage)
        {
            try
            {
                /*TO-DO*/
                BusinessVacationRequest calculateBusiness = new BusinessVacationRequest(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService);

                Entity entity = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];
                EntityReference demandaReference = null;

                if (preImage.Attributes.Contains("smt_lp_resource"))
                {
                    demandaReference = preImage.smt_lp_resource;
                }

                String stringEntity = entity.LogicalName.ToString().ToLower();
                List<String> fields = calculateBusiness.GetFields(stringEntity);

                if (demandaReference != null)
                {
                    foreach (var field in fields)
                    {
                        calculateBusiness.CalculateRollup(demandaReference, field);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
