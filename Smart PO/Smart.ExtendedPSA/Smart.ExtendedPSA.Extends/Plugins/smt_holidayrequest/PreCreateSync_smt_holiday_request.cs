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
    public class PreCreateSync_smt_holiday_request : PluginBase
    {
        /// <summary>
        /// Construtor da classe
        /// </summary>
        public PreCreateSync_smt_holiday_request() : base(typeof(PreCreateSync_smt_holiday_request)) { }
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

            #region ResX

            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            var target = localContext.GetTarget<smt_holiday_request>();
            RenameHolidayRequest(target, localContext);
            // CheckIfHasMoreThanTenDays(target, localContext, messages);

        }

        /// <summary>
        ///  Método para atualizar o nome da solicitação de férias para o nome do recurso + data de início + data de término.
        /// </summary>
        /// <param name="target"> Entidade vinda do contexto </param>
        /// <param name="localContext"> Context </param>
        private void RenameHolidayRequest(smt_holiday_request target, LocalPluginContext localContext)
        {
            var recursoName = localContext.OrganizationServiceAdmin.Retrieve(BookableResource.EntityLogicalName, target.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.Name)).ToEntity<BookableResource>();
            target.smt_name = $" {recursoName.Name} - {target.smt_dt_start.Value.ToShortDateString()} - {target.smt_dt_end.Value.ToShortDateString()}";

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

                if (dtInicial != null && dtFinal != null)
                {
                    int? quantDias = (int)((DateTime)dtFinal - (DateTime)dtInicial).TotalDays;

                    // Pegando id do recurso reservável.
                    Guid idResource = holyRequest.smt_lp_resource.Id;

                    // Fazendo retrieve na entidade de recursos reserváveis onde o id do recurso seja o mesmo do registro. Trazer a unidade organizacional do mesmo.
                    BookableResource idUnitOrg = localContext.OrganizationService.Retrieve(BookableResource.EntityLogicalName, idResource, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit)).ToEntity<BookableResource>();

                    // TODO: Andre - Corrigido
                    // Trazer o parâmetro que tenha a mesma unidade organizacional do recurso, e o nome do parâmetro criado.
                    var parameter = business.GetParameter(idUnitOrg, "MINIMUM DAYS TO ASK FOR HOLIDAYS");

                    // Se retornar algum parâmetro, pegar o valor desse parâmetro
                    if (parameter != null)
                    {
                        // Converter o valor que é string para int.
                        int diasParameter = Convert.ToInt32(parameter.smt_value);

                        // Se a quantidade de dias solicitado for menor do que o valor no parâmetro, retornar throw que impede a criação do registro.
                        if (quantDias < diasParameter)
                        {
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                        }
                    }
                    else
                    {
                        // TODO: Andre - Corrigido
                        // Se não houver nenhum parâmetro com aquele nome e aquele unidade organizacional, buscar um que só tenha o nome, e a Unidade Organizacional seja nula.
                        var parameterGlobal = business.GetParameter(null, "MINIMUM DAYS TO ASK FOR HOLIDAYS", true);

                        // Se retornar algum registro, fazer a mesma validação acima da quantidade de dias solicitada.
                        if (parameterGlobal != null)
                        {
                            int diasParameter = Convert.ToInt32(parameter.smt_value);

                            if (quantDias < diasParameter)
                            {
                                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD01));
                            }
                        }
                    }
                }
                else if (dtInicial == null)
                {
                    throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD02));
                }
                else
                {
                    throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.CIHMTTD03));
                }

            }
        }
    }
}
