using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// Plugin é executado no Pre Update da entidade TimeeEntry
    /// </summary>
    public class PreUpdateSync_msdyn_timeentry : PluginBase
    {
        /// <summary>
        /// Construtor da classe
        /// </summary>
        public PreUpdateSync_msdyn_timeentry() : base(typeof(PreUpdateSync_msdyn_timeentry)) { }

        /// <summary>
        /// Método que contém o código que vai checar se o recurso está tentando lançar mais horas que o expediente dele
        /// </summary>
        /// <param name="localContext">Variável que vai capturar o contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region Licença
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

            #region ResX
            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry>();
            CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntryImage = localContext.GetPreImage<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry>();

            // Esse metodo é exeucutado apenas no Update do campo msdyn_entrystatus
            if (timeEntry.msdyn_entryStatus != null)
            {
                CalculateHourBank(localContext, timeEntry, timeEntryImage, messages);
                // ValidateDescription(localContext, timeEntry, timeEntryImage, messages);
            }
        }
        /// <summary>
        /// Calcula a hora que será calculada no banco de hora
        /// </summary>
        /// <param name="localContext">Contexto local.</param>
        /// <param name="timeEntry">Target, entrada de horas.</param>
        /// <param name="timeEntryImage">Target, entrada de horas.</param>
        /// <param name="messages">Lista de mensagens RESX.</param>
        private void CalculateHourBank(LocalPluginContext localContext, msdyn_timeentry timeEntry, msdyn_timeentry timeEntryImage, List<Resx> messages)
        {
            if (timeEntry.msdyn_entryStatus.Value.Equals(msdyn_timeentrystatus.Approved.GetHashCode()))
            {
                // Msdyn_timeentryBusiness Business = new Msdyn_timeentryBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

                BookableResource recursoReservavel = (BookableResource)localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, timeEntryImage.msdyn_bookableresource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract));
                ColumnSet modeloContrato_CS = new ColumnSet(new String[] { "smt_smt_dc_sunday_hours_bank", "smt_dc_holidays_hours_bank" });

                smt_model_contract modeloContrato = (smt_model_contract)localContext.OrganizationService.Retrieve(smt_model_contract.EntityLogicalName, recursoReservavel.smt_lp_model_contract.Id, modeloContrato_CS);

                // Tipo de Hora = Trabalho
                if (timeEntryImage.msdyn_type.Value.Equals(msdyn_timeentrytype.Work.GetHashCode()))
                {
                    Entity TipoHoras = localContext.OrganizationService.Retrieve("smt_type_hours", timeEntryImage.smt_lp_type_hours.Id, new ColumnSet("smt_pl_type_hours"));

                    if (((OptionSetValue)TipoHoras["smt_pl_type_hours"]).Value.Equals(smt_type_hours_smt_pl_type_hours.BancodeHorasDomingos.GetHashCode()))
                    {
                        timeEntry.smt_dc_timebank = modeloContrato.smt_smt_dc_sunday_hours_bank != null && modeloContrato.smt_smt_dc_sunday_hours_bank.Value != 0 ? timeEntryImage.msdyn_duration.Value + ((decimal)timeEntryImage.msdyn_duration.Value * (decimal)(modeloContrato.smt_smt_dc_sunday_hours_bank.Value / 100)) : timeEntryImage.msdyn_duration.Value;
                    }
                    else if (((OptionSetValue)TipoHoras["smt_pl_type_hours"]).Value.Equals(smt_type_hours_smt_pl_type_hours.BancodeHorasFeriados.GetHashCode()))
                    {
                        timeEntry.smt_dc_timebank = modeloContrato.smt_dc_holidays_hours_bank != null && modeloContrato.smt_dc_holidays_hours_bank.Value != 0 ? timeEntryImage.msdyn_duration.Value + ((decimal)timeEntryImage.msdyn_duration.Value * (decimal)(modeloContrato.smt_dc_holidays_hours_bank.Value / 100)) : timeEntryImage.msdyn_duration.Value;
                    }
                    else if (((OptionSetValue)TipoHoras["smt_pl_type_hours"]).Value.Equals(smt_type_hours_smt_pl_type_hours.BancodeHorasDiaUtil.GetHashCode()))
                    {
                        timeEntry.smt_dc_timebank = timeEntryImage.msdyn_duration.Value;
                    }
                }
                // Tipo de Hora = Ausencia
                else if (timeEntryImage.msdyn_type.Value.Equals(msdyn_timeentrytype.Absence.GetHashCode()) && timeEntryImage.smt_pl_type_absence.Value.Equals(smt_type_absence.CompensacaodeHoras.GetHashCode()))
                {
                    timeEntry.smt_dc_timebank = -timeEntryImage.msdyn_duration.Value;
                }
            }
        }
        /// <summary>
        /// No momento do envio, verifica se o campo descrição está preenchido.
        /// </summary>
        /// <param name="localContext"> context </param>
        /// <param name="timeEntry"> Entrada de horas</param>
        /// <param name="timeEntryImage"> Pre Image da entrada de horas</param>
        /// <param name="messages"> mensagens resx </param>
        private void ValidateDescription(LocalPluginContext localContext, msdyn_timeentry timeEntry, msdyn_timeentry timeEntryImage, List<Resx> messages)
        {
            if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Submitted)
            {
                if (timeEntryImage.msdyn_description == null)
                {
                    throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.VALDESC01));
                }
            }
        }
    }
}
