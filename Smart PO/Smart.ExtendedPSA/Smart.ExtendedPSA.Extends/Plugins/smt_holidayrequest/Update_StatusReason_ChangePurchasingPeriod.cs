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
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_holidayrequest
{
    /// <summary>
    /// Classe principal
    /// </summary>
    public class Update_StatusReason_ChangePurchasingPeriod : PluginBase
    {

        // TODO: Criar um método e tirar a regra do ExecuteCrmPlugin.... (Ex:Plugin de tipo de horas)
        // TODO: CORRIGIR NOME DA CLASSE E SEPARAR O UPDATE DO CREATE....
        private List<Resx> messages;
        /// <summary>
        /// Base
        /// </summary>
        public Update_StatusReason_ChangePurchasingPeriod() : base(typeof(Update_StatusReason_ChangePurchasingPeriod)) { }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="localContext">Contecto de execução</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }

            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);

            // Variáveis utilizadas
            BusinessVacationRequest business = new BusinessVacationRequest(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            var target = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_holiday_request>();
            List<smt_purchasing_period> purchasingPeriod = new List<smt_purchasing_period>();

            business.ValidateLicense();
            if (localContext.PluginExecutionContext.MessageName.ToUpper() == "UPDATE")
            {
                var vacationRequestPreImage = localContext.GetPreImage<CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_holiday_request>();

                if (target.StatusCode != null)
                {
                    switch (target.StatusCodeEnum.Value)
                    {
                        // TODO: COMPARAR OPTIONSET UTILIZANDO ENUM...
                        case smt_holiday_request_StatusCode.Aprovado:
                            business.UpdateUsedDaysInApproval(vacationRequestPreImage, messages);
                            break;

                        // TODO: COMPARAR OPTIONSET UTILIZANDO ENUM...
                        case smt_holiday_request_StatusCode.Cancelado:
                            business.UpdateUsedDaysInCancel(vacationRequestPreImage, messages);
                            break;

                        // TODO: COMPARAR OPTIONSET UTILIZANDO ENUM...
                        case smt_holiday_request_StatusCode.Enviado:
                            business.VerifyIfIsAntecipation(vacationRequestPreImage, messages, target);
                            break;
                    }
                }
            }
           
        }

    }
}