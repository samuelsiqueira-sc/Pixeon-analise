using CRM.Smart.ExtendedPSA.Extends.Business;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Client;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// a
    /// </summary>
    public class ConfirmDialogBusiness : BaseBusiness
    {
        /// <summary>
        /// a        /// </summary>
        /// <param name="service">a</param>
        /// <param name="serviceAdmin">a</param>
        /// <param name="tracingService">a</param>
        /// <param name="messages">a</param>
        public ConfirmDialogBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="entity">a</param>
        public void UpdateHolidayRequest(Entity entity)
        {
            // smt_holiday_requestid
            if (entity.Contains("smt_tx_register_id"))
            {
                Guid holidayRequestId = new Guid((string)entity.Attributes["smt_tx_register_id"]);
                string justification = (string)entity.Attributes["smt_st_justification"];

                Entity holidayRequestEntity = ReturnHolidayRequest(holidayRequestId);

                if (holidayRequestEntity != null)
                {
                    try
                    {
                        Entity uptHolidayResquest = new Entity("smt_holiday_request");
                        uptHolidayResquest.Id = holidayRequestEntity.Id;
                        uptHolidayResquest["smt_st_justification"] = justification;
                        Service.Update(uptHolidayResquest);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException("Erro ao atualizar Solicitação de Férias.");
                    }
                }
            }
        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="holidayId">a</param>
        /// <returns>a</returns>
        public Entity ReturnHolidayRequest(Guid holidayId)
        {
            ColumnSet columnSet = new ColumnSet("smt_holiday_requestid");
            return Service.Retrieve("smt_holiday_request", holidayId, columnSet);

        }
    }
}
