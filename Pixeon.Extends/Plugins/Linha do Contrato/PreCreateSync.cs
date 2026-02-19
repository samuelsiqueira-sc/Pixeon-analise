using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Linha_do_Contrato
{
    /// <summary>
    /// Doc. 
    /// </summary>
   public class PreCreateSync : PluginBase
    {
        /// <summary>
        /// Doc. 
        /// </summary>
        public PreCreateSync() : base(typeof(PreCreateSync)) { }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="localContext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            // Validação da Licença. 

            // Declaração das variaveis utilizadas.
            salesorderdetailBusiness business = new salesorderdetailBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            SalesOrderDetail target = localContext.GetTarget<SalesOrderDetail>();

            // Invocação dos métodos. 
           // UpdateRevenuesQuote(target, business);
        }

        /// <summary>
        /// Função que irá realizar as atualizações na cotação.
        /// </summary>
        /// <param name="target">Target da Linha de Produto da Cotação</param>
        /// <param name="business">Business.</param>
        protected void UpdateRevenuesQuote(SalesOrderDetail target, salesorderdetailBusiness business)
        {
            if (target.ProductTypeCode.Value == 1)
            {
                Money totalRevenue = business.TotalRevenuesRecurrence(target);
                business.UpdateSalesOrderRecurrence(target.SalesOrderId.Id, totalRevenue);
            }
            else if (target.ProductTypeCode.Value == 5)
            {
                Money totalEventual = business.TotalRevenuesEventual(target);
                business.UpdateSalesOrderEventual(target.SalesOrderId.Id, totalEventual);
            }
        }
    }
}
