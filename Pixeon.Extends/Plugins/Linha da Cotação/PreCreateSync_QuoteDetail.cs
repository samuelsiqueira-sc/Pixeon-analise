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
    public class PreCreateSync_QuoteDetail : PluginBase
    {
        /// <summary>
        /// Chaamda
        /// </summary>
        public PreCreateSync_QuoteDetail() : base(typeof(PreCreateSync_QuoteDetail))
        { }
        /// <summary>
        /// Chamada
        /// </summary>
        /// <param name="localcontext">Contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Declaração das variaveis utilizadas. 
            QuoteDetail target = localcontext.GetTarget<QuoteDetail>();

            // Invocação dos metodos. 
            // UpdateRevenuesQuote(target, localcontext);
        }
        /// <summary>
        /// Função que irá realizar as atualizações na cotação.
        /// </summary>
        /// <param name="target">Target da Linha de Produto da Cotação</param>
        /// <param name="localcontext">Contexto</param>
        protected void UpdateRevenuesQuote(QuoteDetail target, LocalPluginContext localcontext)
        {
            QuoteBusiness business = new QuoteBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if (target.ProductTypeCode.Value == 1)
            {
               var totalRevenue = business.TotalRevenuesRecurrence(target);

                Quote quote = new Quote();
                quote.Id = target.QuoteId.Id;
                quote.smt_mn_recurrence_total = totalRevenue;
                localcontext.OrganizationServiceAdmin.Update(quote);

            }
            else if (target.ProductTypeCode.Value == 5)
            {
                var totalEventual = business.TotalRevenuesEventual(target);

                Quote quote = new Quote();
                quote.QuoteId = target.QuoteId.Id;
                quote.smt_mn_final_recurrence_price = totalEventual;
                localcontext.OrganizationServiceAdmin.Update(quote);
            }
        }
    }
}
