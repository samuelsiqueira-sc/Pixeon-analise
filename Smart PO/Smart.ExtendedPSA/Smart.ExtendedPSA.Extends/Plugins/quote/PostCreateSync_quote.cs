using CRM.Smart.ExtendedPSA.Extends.Earlybound;
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

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// a
    /// </summary>
    public class PostCreateSync_quote : PluginBase
    {
        /// <summary>
        /// Classe construtora
        /// </summary>
        public PostCreateSync_quote() : base(typeof(PostCreateSync_quote)) { }

        /// <summary>
        /// Método utilizado para receber o contexto da execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto do Plugin</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region Licença 

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

            #endregion

            #region RESX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            Quote quote = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.Quote>();
            QuoteValidate(localContext, messages, quote);
        }

        private void QuoteValidate(LocalPluginContext localContext, List<Resx> messages, Quote quote)
        {
            smt_type_hourBusiness quoteBusiness = new smt_type_hourBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            quoteBusiness.ValidaCotacao(quote, messages);
        }
    }
}
