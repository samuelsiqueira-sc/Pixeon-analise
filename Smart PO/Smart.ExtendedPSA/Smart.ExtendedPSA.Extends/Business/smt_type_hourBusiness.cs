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
using CRM.Smart.ExtendedPSA.Extends.Business;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Business da classse Tipo de Horas
    /// </summary>
    public class smt_type_hourBusiness : BaseBusiness
    {

        /// <summary>
        /// Metodos da Business
        /// </summary>
        /// <param name="service">service</param>
        /// <param name="serviceAdmin">serviceAdmin</param>
        /// <param name="tracingService">tracingService</param>
        /// <param name="messages">messges</param>
        public smt_type_hourBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Método que verifica se já exite um tipo de horas deste tipo.
        /// </summary>
        /// <param name="type_enum">Tipo da Entrada de Horas</param>
        /// <returns>Verdadeiro = EXISTE ; Falso = NÃO EXISTE</returns>
        public bool CheckTypeHours(smt_type_hours_smt_pl_type_hours? type_enum)
        {
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                var type = (from types in serviceContext.smt_type_hoursSet
                            where types.smt_pl_type_hoursEnum == type_enum
                            select new smt_type_hours
                            {
                                smt_type_hoursId = types.smt_type_hoursId,
                                smt_name = types.smt_name
                            }).FirstOrDefault();

                if (type != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Valida alguns campos ao criar uma linha da cotação.
        /// </summary>
        /// <param name="quote">Objeto referente a cotação</param>
        /// <param name="listResx">Objeto para guardar a lista de mensagens</param>
        public void ValidaLinhaCotacao(Quote quote, List<Resx> listResx)
        {
            if (Service != null && quote.Id != Guid.Empty)
            {
                if (quote.Attributes.Contains("smt_lp_contract_main"))
                {
                    EntityReference erContratoPrincipal = (EntityReference)quote["smt_lp_contract_main"];
                    EntityCollection retornoLinhaCotacao = ValidaLinhaCotacao(quote.Id);

                    if (retornoLinhaCotacao != null && retornoLinhaCotacao.Entities.Count > 0)
                    {
                        foreach (var itemCotacao in retornoLinhaCotacao.Entities)
                        {
                            try
                            {
                                QuoteDetail quoteDetail = new QuoteDetail();
                                quoteDetail.Id = itemCotacao.Id;
                                quoteDetail.smt_lp_main_contract = erContratoPrincipal;
                                if (quote.Attributes["smt_lp_contract_main"] == null)
                                {
                                    quoteDetail.smt_lp_contract_line = null;
                                    quoteDetail.smt_pl_contract_line_type = null;
                                }
                                Service.Update(quoteDetail);
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidPluginExecutionException(listResx.GetMessageById(ResxExtension.QUTUPERR));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Método utilizado para retornar linhas de cotação
        /// </summary>
        /// <param name="idCotacao"> Id da cotação</param>
        /// <returns>a</returns>
        public EntityCollection ValidaLinhaCotacao(Guid idCotacao)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='quotedetail'>
                                <attribute name='productid' />
                                <attribute name='productdescription' />
                                <attribute name='priceperunit' />
                                <attribute name='quantity' />
                                <attribute name='extendedamount' />
                                <attribute name='quotedetailid' />
                                <order attribute='productid' descending='false' />
                                <filter type='and'>
                                  <condition attribute='quoteid' operator='eq' uiname='Cotação Alol 002' uitype='quote' value='{" + idCotacao + @"}' />
                                </filter>
                              </entity>
                            </fetch>";

            EntityCollection retornoCotacao = Service.RetrieveMultiple(new FetchExpression(query));

            if (retornoCotacao != null && retornoCotacao.Entities.Count > 0)
                return retornoCotacao;

            return null;
        }

        /// <summary>
        /// Método utilizado para atualizar a cotação
        /// </summary>
        /// <param name="entity">Objecto que será atualizado</param>
        public void UpdateCotacao(Entity entity)
        {
            Service.Update(entity);
        }

        /// <summary>
        /// Valida algumas alterações na cotação.
        /// </summary>
        /// <param name="entity">Objeto referente a cotação</param>
        /// <param name="resxes"> Objeto para armazenar as mensagens</param>
        public void ValidaCotacao(Entity entity, List<Resx> resxes)
        {
            if (entity.Attributes.Contains("quoteid") && entity.Attributes["quoteid"] != null)
            {
                QuoteDetail quoteDetail = new QuoteDetail();
                Guid idCotacao = entity.Id;
                ColumnSet columnSet = new ColumnSet("smt_pl_additive_contract", "smt_lp_contract_main");
                Entity cotacao = Service.Retrieve("quote", idCotacao, columnSet);
                if (cotacao == null)
                    return;

                if (!cotacao.Attributes.Contains("smt_pl_additive_contract"))
                    return;

                int aditivo = ((OptionSetValue)cotacao["smt_pl_additive_contract"]).Value;

                if (cotacao.Attributes.Contains("smt_pl_additive_contract") && aditivo == 100000000)
                {
                    EntityReference erContratoPrincipal = (EntityReference)cotacao["smt_lp_contract_main"];

                    try
                    {
                        quoteDetail.Id = entity.Id;
                        quoteDetail.smt_lp_main_contract = erContratoPrincipal;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(resxes.GetMessageById(ResxExtension.QUTUPTER));
                    }
                }
                else if (!cotacao.Attributes.Contains("smt_pl_additive_contract") && aditivo == 100000000)
                    throw new InvalidPluginExecutionException(resxes.GetMessageById(ResxExtension.QUTCRER));
            }
            else
                throw new InvalidPluginExecutionException(resxes.GetMessageById(ResxExtension.QUTCRER));
        }
    }
}
