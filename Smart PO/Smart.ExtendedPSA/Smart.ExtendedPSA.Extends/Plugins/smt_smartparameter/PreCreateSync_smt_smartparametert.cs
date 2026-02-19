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
    /// Pre Create Sync Smart Paramter
    /// </summary>
    public class PreCreateSync_smt_smartparametert : PluginBase
    {
        private List<Resx> messages;
        /// <summary>
        /// Constructor
        /// </summary>
        public PreCreateSync_smt_smartparametert() : base(typeof(PreCreateSync_smt_smartparametert))
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
            
            var smartParameterContext = localContext.GetTarget<smt_smartparameter>();

            if (localContext.PluginExecutionContext.PrimaryEntityName.ToLower() == smt_smartparameter.EntityLogicalName)
            {
                localContext.Trace("Iniciando plug-in");

                CreateParameter(smartParameterContext, localContext, messages);
            }
        }
        /// <summary>
        /// Método que vai checar se o parametro criado ja existe ou não
        /// </summary>
        /// <param name="smartParameterContext">Entidade Parametros</param>
        /// <param name="localContext">Contexto do Plugin</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        private void CreateParameter(smt_smartparameter smartParameterContext, LocalPluginContext localContext, List<Resx> messages)
        {
            SmartParameterBusiness smartParameterBusiness = new SmartParameterBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);
            smartParameterBusiness.ParameterName_IsUnique_Creation(smartParameterContext, messages);
        }
    }
}
