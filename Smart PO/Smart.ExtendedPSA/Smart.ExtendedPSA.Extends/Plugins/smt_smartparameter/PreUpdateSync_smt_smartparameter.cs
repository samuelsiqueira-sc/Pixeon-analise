using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;

namespace Smart.ExtendedPSA.Plugins.Plugins.SmartParameter
{
    /// <summary>
    /// Pre Update Sync Smart Paramter
    /// </summary>
    public class PreUpdateSync_smt_smartparameter : PluginBase
    {
        private List<Resx> messages;
        /// <summary>
        /// Constructor
        /// </summary>
        public PreUpdateSync_smt_smartparameter() : base(typeof(PreUpdateSync_smt_smartparameter))
        {
        }

        /// <summary>
        /// Método responsável por chamar os métodos que checam se o registro sendo lançado é repitido
        /// </summary>
        /// <param name="localContext">Variável que vai capturar o contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            // Chamada da Action de validação de licenças
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);

            // Chamada da Action de validação de licenças

            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);

            SmartParameterBusiness smartParameterBusiness = new SmartParameterBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);
            var smartParameterContext = localContext.GetTarget<smt_smartparameter>();
            var smartParameterPreImage = localContext.GetPreImage<smt_smartparameter>("PreImage");

            if (localContext.PluginExecutionContext.PrimaryEntityName.ToLower() == smt_smartparameter.EntityLogicalName)
            {
                localContext.Trace("Iniciando plug-in");
                
                UpdateParameter(smartParameterContext, smartParameterPreImage, localContext, messages);
            }
        }
        /// <summary>
        /// Método que vai checar se o parametro criado ja existe ou não
        /// </summary>
        /// <param name="smartParameterContext">Entidade Parametros</param>
        /// <param name="smartParameterPreImage">PreImage</param>
        /// <param name="localContext">Contexto do Plugin</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        private void UpdateParameter(smt_smartparameter smartParameterContext, smt_smartparameter smartParameterPreImage, LocalPluginContext localContext, List<Resx> messages)
        {
            SmartParameterBusiness smartParameterBusiness = new SmartParameterBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);
            ParameterName_IsUnique_InUpdate(smartParameterContext, smartParameterPreImage, smartParameterBusiness);
        }
        /// <summary>
        /// Verifica se o nome do parâmetro é único para o context dele. Usado no momento na atualização do Parâmetro.
        /// </summary>
        /// <param name="smartParameterContext">Context</param>
        /// <param name="smartParameterPreImage">PreImage</param>
        /// <param name="smartParameterBusiness">Business do plugin</param>
        public void ParameterName_IsUnique_InUpdate(smt_smartparameter smartParameterContext, smt_smartparameter smartParameterPreImage, SmartParameterBusiness smartParameterBusiness)
        {
            if (smartParameterContext != null && smartParameterPreImage != null)
            {
                if (smartParameterContext.Contains("smt_name") && smartParameterContext.Contains("smt_organizationalunit"))
                {
                    smartParameterBusiness.ParameterName_IsUnique_Creation(smartParameterContext, messages);
                }
                else if (smartParameterContext.Contains("smt_name"))
                {
                    smartParameterBusiness.ParameterName_IsUnique_Creation(
                        new smt_smartparameter()
                        {
                            smt_name = smartParameterContext.smt_name,
                            smt_organizationalunit = smartParameterPreImage.smt_organizationalunit
                        },
                        messages
                    );
                }
                else
                {
                    smartParameterBusiness.ParameterName_IsUnique_Creation(
                        new smt_smartparameter()
                        {
                            smt_name = smartParameterPreImage.smt_name,
                            smt_organizationalunit = smartParameterContext.smt_organizationalunit
                        },
                        messages
                    );
                }
            }
        }
    }
}
