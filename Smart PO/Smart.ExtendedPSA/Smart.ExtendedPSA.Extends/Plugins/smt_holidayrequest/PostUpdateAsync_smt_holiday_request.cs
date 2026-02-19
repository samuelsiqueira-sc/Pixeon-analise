using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Query;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_holidayrequest
{
    /// <summary>
    /// Classe para plugins que rodam no Post Update de solicitação de férias.
    /// </summary>
    public class PostUpdateAsync_smt_holiday_request : PluginBase
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public PostUpdateAsync_smt_holiday_request() : base(typeof(PostUpdateAsync_smt_holiday_request)) { }

        /// <summary>
        /// ExecuteCrmPlugin. Classe principal que chama os métodos.
        /// </summary>
        /// <param name="localcontext"> contexto de execução. </param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            var target = localcontext.GetTarget<smt_holiday_request>();
            var preImg = (smt_holiday_request)localcontext.GetPreImage<smt_holiday_request>("preImg");
            var business = new BusinessVacationRequest(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            ValidateLicense(localcontext);

            SendTimeEntryInVacationApproval(preImg, target, localcontext, business);

        }

        private void ValidateLicense(LocalPluginContext localcontext)
        {
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
        }

        private void SendTimeEntryInVacationApproval(smt_holiday_request preImg, smt_holiday_request target, LocalPluginContext localcontext, BusinessVacationRequest business)
        {
            BookableResource resource = localcontext.OrganizationServiceAdmin.Retrieve(BookableResource.EntityLogicalName, preImg.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.UserId)).ToEntity<BookableResource>();
            SystemUser user = localcontext.OrganizationService.Retrieve(SystemUser.EntityLogicalName, resource.UserId.Id, new ColumnSet("parentsystemuserid")).ToEntity<SystemUser>();

            // Se as férias forem do tipo normal ou coletivas o plugin segue.
            if (preImg.smt_smt_pl_typeEnum == smt_holiday_request_smt_smt_pl_type.Normal || preImg.smt_smt_pl_typeEnum == smt_holiday_request_smt_smt_pl_type.Coletivas)
            {
                // Busca usuários que não precisam lançar entrada de horas em caso de férias
                var withoutTimeEntry = business.WithoutTimeEntry(preImg.smt_lp_resource.Id);

                if (withoutTimeEntry == null)
                {

                    if (target.StatusCodeEnum == smt_holiday_request_StatusCode.Aprovado && user.ParentSystemUserId != null)
                        business.CreateHolidayEntry(/*posImg,*/ user.ParentSystemUserId.Id, localcontext.OrganizationServiceFactory, preImg);
                    else if (target.StatusCodeEnum == smt_holiday_request_StatusCode.Cancelado && user.ParentSystemUserId != null)
                        business.CancelHolidayEntry(/*posImg,*/ preImg, user.ParentSystemUserId.Id, localcontext.OrganizationServiceFactory);
                    else if ((target.StatusCodeEnum == smt_holiday_request_StatusCode.Aprovado || target.StatusCodeEnum == smt_holiday_request_StatusCode.Cancelado) && user.ParentSystemUserId == null)
                    {
                        // Tipo de Evento: Erro
                        OptionSetValue eventType = new OptionSetValue(100000000);
                        String recordid = preImg.Id.ToString();
                        String message = "Erro ao criar registro de entrada de horas. O usuário não tem um gerente. Adicione um gerente a esse usuário.";
                        String name = "Erro ao criar entrada de horas.";
                        // Criar log com as informações do porquê não criou as entradas de hora.
                        business.CreateLogs(eventType, recordid, message, name, resource.UserId.Id);
                    }
                }
            }
        }

    }
}
