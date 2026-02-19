using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Smart.ExtendedPSA.Extends.Plugins.Smt_purchasing_period
{
    /// <summary>
    /// PreOperation Update Synchronous em Período aquisitivo
    /// </summary>
    public class PreUpdateSync_smt_purchasing_period : PluginBase
    {
        // TODO: Wesley - Corrigido
        // TODO: Wesley - Corrigido
        // TODO: Wesley - Corrigido

        /// <summary>
        /// Construtor
        /// </summary>
        public PreUpdateSync_smt_purchasing_period() : base(typeof(PreUpdateSync_smt_purchasing_period)) { }

        /// <summary>
        /// class execute
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);

            // Declaração da entidade-alvo que será trabalhada. 
            var target = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_purchasing_period>();
            var preImage = localContext.GetPreImage<smt_purchasing_period>("preImage");

            SetToEndDate(target);
            RenamePurchasingPeriod(localContext, target, preImage);
        }

        /// <summary>
        /// Método para atualizar a data final do Período Aquisitivo.
        /// </summary>
        /// <param name="target"> período aquisitivo </param>
        private void SetToEndDate(smt_purchasing_period target)
        {
            var startDate = target.smt_dt_start.Value;
            target.smt_dt_end = startDate.AddYears(1).AddDays(-1); // Com base na data inicial, adiciona um ano e então subtrai um dia. 
        }

        /// <summary>
        /// Método para renomear período aquisitivo
        /// </summary>
        /// <param name="localContext"> Contexto </param>
        /// <param name="target"> Período Aquisitivo</param>
        /// <param name="preImage">Pre Image de Período Aquisitivo</param>
        private void RenamePurchasingPeriod(LocalPluginContext localContext, smt_purchasing_period target, smt_purchasing_period preImage)
        {
            var nomeRecurso = localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, preImage.smt_lp_resource.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(BookableResource.Fields.Name)).ToEntity<BookableResource>();

            if (target.smt_dt_start != null && nomeRecurso.Name != null)
            {
                target.smt_name = $"{nomeRecurso.Name} - {target.smt_dt_start.Value.Year.ToString()}";
            }
        }
    }
}
