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
    /// doc.
    /// </summary>
    public class PostUpdateSync : PluginBase
    {
        /// <summary>
        /// Execução do plugin
        /// </summary>
        /// <param name="localcontext">doc.</param>
        public PostUpdateSync() : base(typeof(PostUpdateSync)) { }
        /// <summary>
        /// Execução do plugin
        /// </summary>
        /// <param name="localcontext">doc.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Declaração de variáveis. 
            msdyn_project entity = localcontext.GetTarget<msdyn_project>();
            msdyn_project entityPreImage = localcontext.GetPreImage<msdyn_project>();
            ProjetoBusiness business = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos
            RecurrenceRevenueProject(entity, entityPreImage, business);
        }
        /// <summary>
        /// ´Método que realiza o reconhecimento de recetia recorrente
        /// </summary>
        /// <param name="project">Projeto</param>
        /// <param name="preImage">PreImage de Projeto</param>
        /// <param name="business">Business de projeto</param>
        public void RecurrenceRevenueProject(msdyn_project project, msdyn_project preImage, ProjetoBusiness business)
        {
            if (project.StatusCode.Value == 192350002)
            {
                if (preImage.msdyn_salesorderid != null)
                {
                    var salesOrder = business.GetSalesOrder(preImage);

                    if (salesOrder != null)
                    {
                        if (salesOrder.smt_pl_type_rr != null && salesOrder.smt_pl_type_rr.Value == 180580001 && salesOrder.smt_dt_access == null)
                        {
                            business.GetProductsSalesOrder(salesOrder);
                        }
                    }
                }
            }
        }
    }
}
