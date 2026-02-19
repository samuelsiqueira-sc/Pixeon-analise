using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    // TODO: RODRIGO/CORRIGIDO - Criei uma pasta para a entidade QUOTE e aloquei aqui os plugins corretos para esta entidade que anteriormente estavam em smt_type_hours.

    // TODO: RODRIGO: COLOCAR DESCRIÇÃO
    /// <summary>
    /// Construtor padrão. 
    /// </summary>
    public class PostUpdateSync_quote : PluginBase
    {
        /// <summary>
        /// Classe contrutora
        /// </summary>
        public PostUpdateSync_quote() : base(typeof(PostUpdateSync_quote))
        {
        }


        // TODO: (RODRIGO/CORRIGIDO) COLOCAR DESCRIÇÃO
       /// <summary>
       /// Validação de licença, declaração do target e image(s) e invocação dos demais métodos. 
       /// </summary>
       /// <param name="localContext">Contexto local de execução.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext.PluginExecutionContext.InputParameters.Contains("Target") && localContext.PluginExecutionContext.InputParameters["Target"] is Entity)
            {
                #region Resx
                List<Resx> messages;
                string resxFileName = ResxExtension.webResourceName;
                messages = localContext.LoadResxMessages(resxFileName);
                #endregion

                #region Licença
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

                Quote quote = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.Quote>();
                PrimaryContractValidate(localContext, messages, quote);
            }
        }

        // TODO: (RODRIGO/CORRIGIDO) COLOCAR DESCRIÇÃO

        /// <summary>
        /// Valida o contrato principal, atualizando-o        /// </summary>
        /// <param name="context">Contexto local.</param>
        /// <param name="messages">Lista RESX.</param>
        /// <param name="quote">Entidade de cotação.</param>
        private void PrimaryContractValidate(LocalPluginContext context, List<Resx> messages, Quote quote)
        {
            smt_type_hourBusiness quoteBusiness = new smt_type_hourBusiness(context.OrganizationService, context.OrganizationServiceAdmin, context.TracingService, null);
            quoteBusiness.ValidaLinhaCotacao(quote, messages);
        }
    }

}
