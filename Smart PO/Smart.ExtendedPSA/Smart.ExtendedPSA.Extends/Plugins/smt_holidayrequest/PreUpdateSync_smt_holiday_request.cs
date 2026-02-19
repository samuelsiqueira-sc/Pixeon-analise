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
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_holidayrequest
{

    // TODO: Wesley Silva - Corrigido
    // TODO: Wesley Silva - Corrigido

    /// <summary>
    /// Plugin responsável por checar se as férias solicitadas tem ao menos 10 dias
    /// </summary>
    public class PreUpdateSync_smt_holiday_request : PluginBase
    {
        private List<Resx> messages;
        /// <summary>
        /// Construtor da classe
        /// </summary>
        public PreUpdateSync_smt_holiday_request() : base(typeof(PreUpdateSync_smt_holiday_request)) { }
        /// <summary>
        /// Método que irá checar se as férias solicitadas tem ao menos 10 dias
        /// </summary>
        /// <param name="localContext">Variável que vai capturar o contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region ValidateLicense

            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }
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
            BusinessVacationRequest business = new BusinessVacationRequest(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            var target = localContext.GetTarget<smt_holiday_request>();

            RenameHolidayRequest(localContext, target);

            ChangePurchasingPeriod(localContext, target, business);

            // CheckIfHasMoreThanTenDays(target, localContext, messages);
        }

        /// <summary>
        ///  Método para atualizar o nome da solicitação de férias para o nome do recurso + data de início + data de término.
        /// </summary>
        /// <param name="localContext"> Context </param>
        /// <param name="target"> Entidade vinda do contexto </param>
        private void RenameHolidayRequest(LocalPluginContext localContext, smt_holiday_request target)
        {
            var PreImage = localContext.GetPreImage<smt_holiday_request>();

            var recursoName = localContext.OrganizationServiceAdmin.Retrieve(BookableResource.EntityLogicalName, PreImage.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.Name)).ToEntity<BookableResource>();

            if (target.smt_dt_start != null && target.smt_dt_end != null)
            {
                target.smt_name = $"{recursoName.Name} - {target.smt_dt_start.Value.ToShortDateString()} - {target.smt_dt_end.Value.ToShortDateString()}";
            }
            else if (target.smt_dt_start == null && target.smt_dt_end != null)
            {
                target.smt_name = $"{recursoName.Name.ToString()} - {PreImage.smt_dt_start.Value.ToShortDateString()} - {target.smt_dt_end.Value.ToShortDateString()}";
            }
            else if (target.smt_dt_start != null && target.smt_dt_end == null)
            {
                target.smt_name = $" {recursoName.Name.ToString()} - {target.smt_dt_start.Value.ToShortDateString()} - {PreImage.smt_dt_end.Value.ToShortDateString()}";
            }

        }

        /// <summary>
        /// Método que chama os cálculos da business para atualizar saldo do período aquisitivo.
        /// </summary>
        /// <param name="localContext"> Context </param>
        /// <param name="target"> Solicitação de Férias </param>
        /// <param name="business"> business </param>
        private void ChangePurchasingPeriod(LocalPluginContext localContext, smt_holiday_request target, BusinessVacationRequest business)
        {
            var vacationRequestPreImage = localContext.GetPreImage<smt_holiday_request>();

            if (target.StatusCode != null)
            {
                switch (target.StatusCodeEnum.Value)
                {
                    // TODO: Wesley Silva - Corrigido
                    case smt_holiday_request_StatusCode.Aprovado:
                        business.UpdateUsedDaysInApproval(vacationRequestPreImage, messages);
                        break;

                    // TODO: Wesley Silva - Corrigido
                    case smt_holiday_request_StatusCode.Cancelado:
                        business.UpdateUsedDaysInCancel(vacationRequestPreImage, messages);
                        break;

                    // TODO: Wesley Silva - Corrigido
                    case smt_holiday_request_StatusCode.Enviado:
                        business.VerifyIfIsAntecipation(vacationRequestPreImage, messages, target);
                        break;
                }
            }
        }

        /// <summary>
        /// Método que ira chamar a Business e validar a regra do tipo de horas.
        /// </summary>
        /// <param name="target">Ferias</param>
        /// <param name="localContext">contexto</param>
        /// <param name="messages">RESX</param>
        private void CheckIfHasMoreThanTenDays(CRM.Smart.ExtendedPSA.Extends.Earlybound.smt_holiday_request target, LocalPluginContext localContext, List<Resx> messages)
        {
            using (var context = new CrmServiceContext(localContext.OrganizationServiceAdmin))
            {
                BusinessVacationRequest business = new BusinessVacationRequest(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

                // Não se pode declarar um uma viriavel primitiva (int) com valor "null", entretanto se usar o tipo Nullable<T> sendo T um tipo o compilador permite
                // Nullable <int> i = null; == int? i = null;
                smt_holiday_request holyRequest = localContext.GetTarget<smt_holiday_request>();
                DateTime? dtInicial = null;
                DateTime? dtFinal = null;
              
                // Checa se o campo é null antes de atribuir o valor do campo a variavel
                if (holyRequest.smt_dt_start != null)
                {
                    dtInicial = holyRequest.smt_dt_start.Value;
                }
                if (holyRequest.smt_dt_end != null)
                {
                    dtFinal = holyRequest.smt_dt_end.Value;
                }

                smt_holiday_request preImage = localContext.GetPreImage<smt_holiday_request>("PreImage");
                DateTime? preDtInicio = null;
                DateTime? preDtFim = null;
                if (preImage.smt_dt_start != null)
                {
                    preDtInicio = preImage.smt_dt_start.Value;
                }
                if (preImage.smt_dt_end != null)
                {
                    preDtFim = preImage.smt_dt_end.Value;
                }
                if (dtInicial != null && dtFinal != null)
                {
                    // Os castings (DateTime) se fazem necessários nesse caso primeiramente porque o tipo DateTime? não tem acesso aos atributos do tipo TimeSpan que contém o TotalDays
                    // e essa função retorna double por isso o casting para (int). Dessa maneira os dias são calculados independentemente da passagem dos meses.
                    int? quantDias = (int)((DateTime)dtFinal - (DateTime)dtInicial).TotalDays;

                    Guid idResource = preImage.smt_lp_resource.Id;

                    BookableResource bookableResource = localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, idResource, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit)).ToEntity<BookableResource>();

                    // TODO: Andre - Corrigido
                    var parameters = business.GetParameter(bookableResource, "MINIMUM DAYS TO ASK FOR HOLIDAYS");

                    if (parameters != null)
                    {
                        int diasParametro = Convert.ToInt32(parameters.smt_value);

                        if (quantDias < diasParametro)
                        {
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                        }
                    }
                    else
                    {
                        // TODO: Andre - Corrigido
                        var parametersGlobal = business.GetParameter(null, "MINIMUM DAYS TO ASK FOR HOLIDAYS", true);

                        if (parametersGlobal != null)
                        {
                            int diasParametro = Convert.ToInt32(parametersGlobal.smt_value);

                            if (quantDias < diasParametro)
                            {
                                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                            }
                        }
                    }
                }
                else if ((dtInicial != null && dtFinal == null) || (dtInicial != new DateTime() && dtFinal == new DateTime()))
                {
                    int? quantDias = (int)((DateTime)preDtFim - (DateTime)dtInicial).TotalDays;

                    Guid idResource = preImage.smt_lp_resource.Id;

                    BookableResource bookableResource = localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, idResource, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit)).ToEntity<BookableResource>();

                    // TODO: Andre - Corrigido
                    var parameters = business.GetParameter(bookableResource, "MINIMUM DAYS TO ASK FOR HOLIDAYS");

                    if (parameters != null)
                    {
                        int diasParametro = Convert.ToInt32(parameters.smt_value);
                        if (quantDias < diasParametro)
                        {
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                        }
                    }
                    else
                    {
                        // TODO: Andre - Corrigido
                        var parametersGlobal = business.GetParameter(null, "MINIMUM DAYS TO ASK FOR HOLIDAYS", true);

                        if (parametersGlobal != null)
                        {
                            int diasParametro = Convert.ToInt32(parametersGlobal.smt_value);

                            if (quantDias < diasParametro)
                            {
                                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                            }
                        }
                    }
                }
                else if ((dtFinal != null && dtInicial == null) || (dtFinal != new DateTime() && dtInicial == new DateTime()))
                {
                    int? quantDias = (int)((DateTime)dtFinal - (DateTime)preDtInicio).TotalDays;

                    Guid idResource = preImage.smt_lp_resource.Id;

                    BookableResource bookableResource = localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, idResource, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit)).ToEntity<BookableResource>();

                    // TODO: Andre - Corrigido
                    var parameters = business.GetParameter(bookableResource, "MINIMUM DAYS TO ASK FOR HOLIDAYS");

                    if (parameters != null)
                    {
                        int diasParametro = Convert.ToInt32(parameters.smt_value);

                        if (quantDias < diasParametro)
                        {
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                        }
                    }
                    else
                    {
                        // TODO: Andre - Corrigido
                        var parametersGlobal = business.GetParameter(null, "MINIMUM DAYS TO ASK FOR HOLIDAYS", true);                    

                        if (parametersGlobal != null)
                        {
                            int diasParametro = Convert.ToInt32(parametersGlobal.smt_value);

                            if (quantDias < diasParametro)
                            {
                                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                            }
                        }
                    }
                }

            }
        }
    }
}