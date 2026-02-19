using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_confirm_holiday_request
{
    /// <summary>
    /// ExecuteCrmPlugin
    /// </summary>
    /// 
    public class PreCreateConfirmHolidayRequest : PluginBase
    {
        /// <summary>
        /// ExecuteCrmPlugin
        /// </summary>
        /// 
        public PreCreateConfirmHolidayRequest() : base(typeof(PreCreateConfirmHolidayRequest)) { }

        /// <summary>
        /// ExecuteCrmPlugin
        /// </summary>
        /// <param name="localContext">localContext</param>
        /// 
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            // Chamada da Action de validação de licenças
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);

            // Chamada da Action de validação de licenças
            var confirm = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_button_dialog>();

            using (var context = new CrmServiceContext(localContext.OrganizationServiceAdmin))
            {
                if (confirm.smt_pl_confirmation_type.Value == 1) // Caso Férias coletivas
                {
                    var ids = confirm.smt_tx_register_id.Split(',');
                    foreach (var id in ids)
                    {    
                        var guid = new Guid(id);
                        
                        var resource = (from a in context.BookableResourceSet
                                         where a.Id == guid
                                         select a).FirstOrDefault();

                         var holiday = new smt_holiday_request();
                         holiday.smt_lp_resource = resource.ToEntityReference();
                         holiday.smt_dt_start = confirm.smt_DT_holiday_start_date;
                         holiday.smt_dt_end = confirm.smt_DT_holiday_end_date;
                         holiday.smt_smt_pl_type = new OptionSetValue(100000001);

                         var idHoliday = localContext.OrganizationServiceAdmin.Create(holiday);

                         smt_holiday_request updateHoliday = new smt_holiday_request();
                         updateHoliday.Id = idHoliday;
                         updateHoliday.statecode = new OptionSetValue(1);
                         updateHoliday.StatusCodeEnum = smt_holiday_request_StatusCode.Aprovado;
                         updateHoliday.EntityState = EntityState.Changed;
                         
                         localContext.OrganizationServiceAdmin.Update(updateHoliday);

                    }
                } 
                else if (confirm.smt_pl_confirmation_type.Value == 2) // Caso Cancelamento de Férias
                {
                    var ids = confirm.smt_ST_register_id.Split(',');
                    foreach (var id in ids)
                    {
                        var guid = new Guid(id);

                        smt_holiday_request updateHoliday = new smt_holiday_request();
                        updateHoliday.Id = guid;
                        updateHoliday.statecode = new OptionSetValue(1);
                        updateHoliday.StatusCodeEnum = smt_holiday_request_StatusCode.Cancelado;
                        updateHoliday.EntityState = EntityState.Changed;
                        localContext.OrganizationServiceAdmin.Update(updateHoliday);
                    }
                       
                }
            }
        }
    }
}
