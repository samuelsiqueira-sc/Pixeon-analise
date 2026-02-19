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
using Smart.ExtendedPSA.Extends.MultiLanguage;

namespace Smart.ExtendedPSA.Extends.Plugins.Smt_purchasing_period
{
    // TODO: Wesley Silva - Corrigido

    /// <summary>
    /// Plugins em PreOperation no create Synchronous de Período Aquisitivo
    /// </summary>
    public class PreCreateSync_smt_purchasing_period : PluginBase
    {
        private List<Resx> messages;

        // TODO: Wesley Silva - Corrigido
        /// <summary>
        /// Método Construtor
        /// </summary>
        public PreCreateSync_smt_purchasing_period() : base(typeof(PreCreateSync_smt_purchasing_period)) { } 

        /// <summary>
        /// class execute
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
       protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region ValidateLicense
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
            #endregion
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);

            // Declaração da entidade-alvo que será trabalhada. 
            var target = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_purchasing_period>();

            SetToEndDate(target);
            RenamePurchasingPeriod(localContext, target);
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

        private void RenamePurchasingPeriod(LocalPluginContext localContext, smt_purchasing_period target)
        {
            var lpResource = target.smt_lp_resource;
            var nomeRecurso = localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, lpResource.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet(BookableResource.Fields.Name)).ToEntity<BookableResource>();
            var dtInicio = target.smt_dt_start.Value;

            if (nomeRecurso != null && nomeRecurso.Name != null && dtInicio != null)
            {
                target.smt_name = nomeRecurso.Name + " - " + dtInicio.Year.ToString();
            }
            else if (nomeRecurso == null || nomeRecurso.Name == null)
            {// TODO: Wesley Silva - Corrigido
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.PCSPP01));
            }
            else
            {// TODO: Wesley Silva - Corrigido
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.PCSPP02));
            }
        }
    }
}
