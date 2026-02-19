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

namespace Pixeon.Extends.Plugins
{
    /// <summary>
    /// Chamada
    /// </summary>
    public class PreUpdateSync_QuoteDetail : PluginBase
    {
        /// <summary>
        /// Chaamda
        /// </summary>
        public PreUpdateSync_QuoteDetail() : base(typeof(PreUpdateSync_QuoteDetail))
        { }
        /// <summary>
        /// Chamada
        /// </summary>
        /// <param name="localcontext">Contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            QuoteDetail target = localcontext.GetTarget<QuoteDetail>();
            QuoteDetail preImage = localcontext.GetPreImage<QuoteDetail>();

          // UpdateRevenuesQuote(target, preImage, localcontext);
        }
        /// <summary>
        /// Função que irá realizar as atualizações na cotação.
        /// </summary>
        /// <param name="target">Target da Linha de Produto da Cotação</param>
        /// <param name="preImage">PreImage da Linha de Produto</param>
        /// <param name="localcontext">Contexto</param>
        protected void UpdateRevenuesQuote(QuoteDetail target, QuoteDetail preImage, LocalPluginContext localcontext)
        {
            QuoteBusiness business = new QuoteBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if (preImage.ProductTypeCode.Value == 1)
            {
                var totalRevenue = business.TotalRevenuesRecurrenceUpdating(target, preImage);

                Quote quote = new Quote();
                quote.Id = preImage.QuoteId.Id;
                quote.smt_mn_recurrence_total = totalRevenue;
                localcontext.OrganizationServiceAdmin.Update(quote);

            }
            else if (preImage.ProductTypeCode.Value == 5)
            {
                var totalEventual = business.TotalRevenuesEventualUpdating(target, preImage);

                Quote quote = new Quote();
                quote.QuoteId = preImage.QuoteId.Id;
                quote.smt_mn_final_recurrence_price = totalEventual;
                localcontext.OrganizationServiceAdmin.Update(quote);
            }
        }
    }
}
