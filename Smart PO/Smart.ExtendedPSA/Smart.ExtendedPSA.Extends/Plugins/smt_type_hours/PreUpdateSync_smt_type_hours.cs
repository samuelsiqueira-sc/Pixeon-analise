using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Smart.ExtendedPSA.Extends.MultiLanguage;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_type_hours
{
    /// <summary>
    /// Iniciaçaõ da Classe
    /// </summary>
    public class PreUpdateSync_smt_type_hours : PluginBase
    {
        /// <summary>
        /// PreCreat_type_hour_limit
        /// </summary>
        public PreUpdateSync_smt_type_hours() : base(typeof(PreUpdateSync_smt_type_hours)) { }

        /// <summary>
        /// Chamada da classe que executa a lógica
        /// </summary>
        /// <param name="localcontext">Local Context</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            #region Licença
            // Chamada da Action de Licenças
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localcontext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localcontext.OrganizationService.Execute(ActionRequest);
            #endregion

            #region ResX

            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localcontext.LoadResxMessages(resxFileName);
            #endregion

            var target = localcontext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_type_hours>();

            CheckTypeHours(target, localcontext, messages);
        }

        /// <summary>
        /// Método que ira chamar a Business e validar a regra do tipo de horas.
        /// </summary>
        /// <param name="target">Tipo de Horas</param>
        /// <param name="localContext">Contexto do Plugin</param>
        /// <param name="messages">RESX</param>
        private void CheckTypeHours(CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_type_hours target, LocalPluginContext localContext, List<Resx> messages)
        {
            smt_type_hourBusiness business = new smt_type_hourBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

            if (target.smt_pl_type_hours != null)
            {
                if (target.smt_pl_type_hoursEnum == CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_type_hours_smt_pl_type_hours.Normal)
                {
                    var result = business.CheckTypeHours(target.smt_pl_type_hoursEnum);

                    if (result == true)
                    {
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.ExistingHour));
                    }
                }
            }
        }
    }
}
