using CRM.Smart.ExtendedPSA.Extends.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using System.Text;
using System.Threading.Tasks;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;

namespace Smart.ExtendedPSA.Extends.Plugins.Smt_resource_departure
{
    /// <summary>
    /// Plugin de Pre Create do Afastamento do Recurso.
    /// </summary>
    public class PreCreateSync : PluginBase
    {
        /// <summary>
        /// Base.
        /// </summary>
        public PreCreateSync() : base(typeof(PreCreateSync)) { }

        /// <summary>
        /// Método para chamada de variáveis não locais.
        /// </summary>
        /// <param name="localcontext">Contexto local do formulário.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            List<Resx> messages;
            string restFileName = ResxExtension.webResourceName;
            messages = localcontext.LoadResxMessages(restFileName);
            smt_resource_departure_eb target = localcontext.GetTarget<smt_resource_departure_eb>();
            ResourceDepartureBusiness business = new ResourceDepartureBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService);
            SetName(target, business, messages);
        }

        /// <summary>
        /// Método que preenche na criação o nome do registro na entidade de afastamento do recurso.
        /// </summary>
        /// <param name="target">Target da entidade atual.</param>
        /// <param name="business">Business de afastamento do recurso com busca de recurso relacionado.</param>
        /// <param name="messages">Mensagens de erro Resx.</param>
        public void SetName(smt_resource_departure_eb target, ResourceDepartureBusiness business, List<Resx> messages)
        {
            Guid resourceId = target.smt_lp_resource.Id;
            BookableResource resource = business.SearchResource(resourceId);
            business.DepartureDates(target, resource, messages);
            target.smt_name = $" {resource.Name.ToString()} | {target.smt_dt_start.Value.ToShortDateString()}";
        }
    }
}
