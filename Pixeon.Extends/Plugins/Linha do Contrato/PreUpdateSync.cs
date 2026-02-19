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
    public class PreUpdateSync : PluginBase
    {
        /// <summary>
        /// Doc. 
        /// </summary>
        public PreUpdateSync() : base(typeof(PreCreateSync)) { }

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
            SalesOrderDetail preImage = localContext.GetPreImage<SalesOrderDetail>();

            // Invocação dos métodos. 
           // UpdateRevenuesQuote(target, preImage, business);

        }

        /// <summary>
        /// Função que irá realizar as atualizações na cotação.
        /// </summary>
        /// <param name="target">Target da Linha de Produto da Cotação</param>
        /// <param name="image">Pre Image da entidade salesorderdetail.</param>
        /// <param name="business">Business.</param>
        protected void UpdateRevenuesQuote(SalesOrderDetail target, SalesOrderDetail image, salesorderdetailBusiness business)
        {
            if (target.ProductTypeCode.Value == 1)
            {
                Money totalRevenue = business.TotalRevenuesRecurrenceUpdating(target, image);
                business.UpdateSalesOrderRecurrence(image.SalesOrderId.Id, totalRevenue);
            }
            else if (target.ProductTypeCode.Value == 5)
            {
                Money totalEventual = business.TotalRevenuesEventualUpdating(target, image);
                business.UpdateSalesOrderEventual(image.SalesOrderId.Id, totalEventual);
            }
        }
    }
}
