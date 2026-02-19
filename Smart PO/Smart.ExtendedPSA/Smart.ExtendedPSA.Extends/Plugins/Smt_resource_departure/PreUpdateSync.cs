using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Business;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;

namespace Smart.ExtendedPSA.Extends.Plugins.Smt_resource_departure
{
    /// <summary>
    /// Plugin de Pre Update do Afastamento do Recurso.
    /// </summary>
    public class PreUpdateSync : PluginBase
    {
        /// <summary>
        /// Base.
        /// </summary>
        public PreUpdateSync() : base(typeof(PreUpdateSync)) { }

        /// <summary>
        /// Método para chamada de variáveis não locais.
        /// </summary>
        /// <param name="pluginContext"> Contexto local - localContext .</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext pluginContext)
        {
            smt_resource_departure_eb targetDeparture = pluginContext.GetTarget<smt_resource_departure_eb>();
            smt_resource_departure_eb preimage = pluginContext.GetMergePreImage<smt_resource_departure_eb>();
            ResourceDepartureBusiness business = new ResourceDepartureBusiness(pluginContext.OrganizationService, pluginContext.OrganizationServiceAdmin, pluginContext.TracingService);
            SetDepartureName(targetDeparture, preimage, business);
        }

        /// <summary>
        /// Método que executa a atualização do nome do afastamento do recurso.
        /// </summary>
        /// <param name="targetDeparture">Target da entidade atual.</param>
        /// <param name="preimage">PreImage do formulário de afastamento.</param>
        /// <param name="business">Business de afastamento do recurso com busca de recurso relacionado.</param>
        public void SetDepartureName(smt_resource_departure_eb targetDeparture, smt_resource_departure_eb preimage, ResourceDepartureBusiness business)
        {
            if (preimage.smt_dt_start != null)
            {
                Guid resourceId = preimage.smt_lp_resource.Id;
                BookableResource resource = business.SearchResource(resourceId);
                targetDeparture.smt_name = $"{resource.Name.ToString()} | {targetDeparture.smt_dt_start.Value.ToShortDateString()}";
            }
        }
    }
}
