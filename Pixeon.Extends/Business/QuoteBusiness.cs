using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using Pixeon.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Business
{
    /// <summary>
    /// Chamada
    /// </summary>
    public class QuoteBusiness : BaseBusiness
    {
        /// <summary>
        /// Método contrutor para receber alguns parametros precisos para a execução dos métodos da classe.
        /// </summary>
        /// <param name="service">Variavel de Serviço</param>
        /// <param name="serviceAdmin"> ServiceADM</param>
        /// <param name="tracingService">Atributo do tipo IorganizationService, utilizado para executar métodos do SDK do Dynamics CRM</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        public QuoteBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// realiza a soma de todos os valores das linhas de produto de uma cotação em expecifico.
        /// </summary>
        /// <param name="quoteDetail">Recebe Linha do Produto como parametro</param>
        /// <returns>retorna valor total das linhas de produtos</returns>
        public Money TotalRevenuesRecurrence(QuoteDetail quoteDetail)
        {
            using (var crmContext = new CrmServiceContext(ServiceAdmin))
            {
                var total = (from q in crmContext.CreateQuery<QuoteDetail>()
                             where q.ProductTypeCode.Value == 1
                             && q.QuoteId == quoteDetail.QuoteId
                             && q.ExtendedAmount != null
                             select q.ExtendedAmount).ToList();

                var totalRecurrence = quoteDetail.PricePerUnit != null ? quoteDetail.PricePerUnit.Value : 0;
                totalRecurrence += quoteDetail.Tax != null ? quoteDetail.Tax.Value + total.Sum(a => a.Value) : total.Sum(a => a.Value);

                return new Money(totalRecurrence);
            }
        }

        /// <summary>
        /// Realiza a soma de todos os valores de linhas da cotação baseado em serviço.
        /// </summary>
        /// <param name="quoteDetail">Recebe linha da cotação como parametro</param>
        /// <returns>Retorna valor total das linhas baseadas em serviços da cotação.</returns>
        public Money TotalRevenuesEventual(QuoteDetail quoteDetail)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var total = (from q in crmService.CreateQuery<QuoteDetail>()
                             where q.ProductTypeCode.Value == 5
                             && q.QuoteId == quoteDetail.QuoteId
                             && q.ExtendedAmount != null
                             select q.ExtendedAmount).ToList();

                var totalEventual = quoteDetail.PricePerUnit != null ? quoteDetail.PricePerUnit.Value : 0;
                totalEventual += quoteDetail.Tax != null ? quoteDetail.Tax.Value + total.Sum(a => a.Value) : total.Sum(a => a.Value);

                return new Money(totalEventual);
            }
        }


        /// <summary>
        /// realiza a soma de todos os valores das linhas de produto de uma cotação em expecifico.
        /// </summary>
        /// <param name="quoteDetail">Recebe Linha do Produto como parametro</param>
        /// <param name="preImage">Recebe preimage</param>
        /// <returns>retorna valor total das linhas de produtos</returns>
        public Money TotalRevenuesRecurrenceUpdating(QuoteDetail quoteDetail, QuoteDetail preImage)
        {
            using (var crmContext = new CrmServiceContext(ServiceAdmin))
            {
                var total = (from q in crmContext.CreateQuery<QuoteDetail>()
                             where q.ProductTypeCode.Value == 1
                             && q.QuoteId == preImage.QuoteId
                             && q.QuoteDetailId != quoteDetail.Id
                             && q.ExtendedAmount != null
                             select q.ExtendedAmount).ToList();

                var totalRecurrence = quoteDetail.PricePerUnit != null ? quoteDetail.PricePerUnit.Value : preImage.PricePerUnit != null ? preImage.PricePerUnit.Value : 0;
                totalRecurrence += quoteDetail.Tax != null ? quoteDetail.Tax.Value + total.Sum(a => a.Value) : preImage.Tax != null ? preImage.Tax.Value + total.Sum(a => a.Value) : total.Sum(a => a.Value);

                return new Money(totalRecurrence);
            }
        }

        /// <summary>
        /// Realiza a soma de todos os valores de linhas da cotação baseado em serviço.
        /// </summary>
        /// <param name="quoteDetail">Recebe linha da cotação como parametro</param>
        /// <param name="preImage">Recebe preimage</param>
        /// <returns>Retorna valor total das linhas baseadas em serviços da cotação.</returns>
        public Money TotalRevenuesEventualUpdating(QuoteDetail quoteDetail, QuoteDetail preImage)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var total = (from q in crmService.CreateQuery<QuoteDetail>()
                             where q.ProductTypeCode.Value == 5
                             && q.QuoteId == preImage.QuoteId
                             && q.QuoteDetailId != quoteDetail.Id
                             && q.ExtendedAmount != null
                             select q.ExtendedAmount).ToList();

                var totalEventual = quoteDetail.PricePerUnit != null ? quoteDetail.PricePerUnit.Value : preImage.PricePerUnit != null ? preImage.PricePerUnit.Value : 0;
                totalEventual += quoteDetail.Tax != null ? quoteDetail.Tax.Value + total.Sum(a => a.Value) : preImage.Tax != null ? preImage.Tax.Value + total.Sum(a => a.Value) : total.Sum(a => a.Value);

                return new Money(totalEventual);
            }
        }

        /// <summary>
        /// Retorna todas as cotações associadas a cotação de acordo com o número da oportunidade (SF)
        /// </summary>
        /// <param name="quote">Cotação</param>
        /// <returns>lista de cotações</returns>
        public List<Quote> RetrieveQuotesByNumberOpp(Quote quote)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var quotes = (from q in crmService.CreateQuery<Quote>()
                              where q.smt_st_notificationid != null
                              && q.smt_st_notificationid == quote.smt_st_notificationid
                              && q.QuoteId != quote.QuoteId
                              select q).ToList();

                return quotes;
            }
        }

        /// <summary>
        /// Associa as cotações que possuem o mesmo número de opp (SF)
        /// </summary>
        /// <param name="quote">Cotação atual</param>
        /// <param name="quotesAssociate">Cotações antigas</param>
        public void Associate(Quote quote, List<Quote> quotesAssociate)
        {
            foreach (var q in quotesAssociate)
            {
                Relationship relationship = new Relationship("smt_quote_quote");
                relationship.PrimaryEntityRole = EntityRole.Referencing;
                EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
                EntityReference secondaryEntity = new EntityReference(Quote.EntityLogicalName, q.Id);
                relatedEntities.Add(secondaryEntity);

                Service.Associate(Quote.EntityLogicalName, quote.Id, relationship, relatedEntities);
            }
        }
        /// <summary>
        /// Retorna o parâmetro de alocação
        /// </summary>
        /// <param name="quote">Cotação</param>
        /// <returns>time</returns>
        public smt_parameter_alocation GetTeam(Quote quote)
        {
            smt_parameter_alocation alocation = new smt_parameter_alocation();

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
               Account account = ServiceAdmin.Retrieve(Account.EntityLogicalName, quote.CustomerId.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("smt_pl_customer_region")).ToEntity<Account>();

                if (quote.smt_pl_family != null && quote.smt_txt_integrador != null && quote.smt_pl_classification != null)
                {
                    // Busca os parâmetros de alocação ativos
                    var parameters = (from a in crmService.CreateQuery<smt_parameter_alocation>()
                                      where a.smt_plm_family != null 
                                         && a.smt_txt_integrador != null
                                         && a.smt_pl_sales_classification != null
                                         && a.StateCode == smt_parameter_alocationState.Active                                            
                                      select a).ToList();

                     alocation = parameters.Where(a => a.smt_plm_family.Any(d => d.Value == quote.smt_pl_family.Value)
                     && a.smt_txt_integrador.Split(';').Any(b => b.ToLower() == quote.smt_txt_integrador.ToLower())
                     && a.smt_pl_sales_classification.Any(c => c.Value == quote.smt_pl_classification.Value)).Select(a => a).FirstOrDefault();
                }
            }
            return alocation;
        }

        /// <summary>
        /// Retorna o parâmetro de alocação
        /// </summary>
        /// <param name="quote">Cotação</param>
        /// <returns>time</returns>
        public msdyn_organizationalunit GetTeamOwner(Quote quote)
        {
            msdyn_organizationalunit unit = new msdyn_organizationalunit();
            Quote cotacao = ServiceAdmin.Retrieve(Quote.EntityLogicalName, quote.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("smt_st_email")).ToEntity<Quote>();
            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                var team = (from i in crmService.CreateQuery<Team>()
                            where i.TeamId == quote.OwnerId.Id
                            select i).FirstOrDefault();

                if (team != null)
                {
                    if (team.IsDefault.Value == true && cotacao.smt_st_email == null)
                    {
                        throw new InvalidPluginExecutionException($"A equipe {team.Name} é padrão da Unidade de Negócios. Não é permitido associar uma equipe padrão a uma cotação. Por favor, selecione outra equipe.");
                    }

                    var negocio = (from i in crmService.CreateQuery<BusinessUnit>()
                                   where i.BusinessUnitId == team.BusinessUnitId.Id
                                   select i).FirstOrDefault();
                    if (negocio != null)
                    {
                        unit = (from u in crmService.CreateQuery<msdyn_organizationalunit>()
                                where u.msdyn_name == negocio.Name
                                select u).FirstOrDefault();
                    }
                }

                return unit;
            }
        }

        /// <summary>
        /// Retorna o parâmetro de alocação
        /// </summary>
        /// <param name="quote">Cotação</param>
        /// <returns>time</returns>
        public smt_parameter_alocation GetTeamDefault(Quote quote)
        {
            smt_parameter_alocation alocation = new smt_parameter_alocation();

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {

                var parameters = (from a in crmService.CreateQuery<smt_parameter_alocation>()
                                  where a.smt_bl_error_alocation == true
                                  && a.smt_lp_organizationalunit == quote.msdyn_ContractOrganizationalUnitId
                                  select a).FirstOrDefault();

            }
            return alocation;
        }

        /// <summary>
        /// Esta função ter por finalidade obter o registro da Equipe de Serviços. 
        /// </summary>
        /// <returns>Retorna a equipe de serviços.</returns>
        public Team RetrieveServiceTeam()
        {
            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                return crmService.TeamSet.Where(team => team.smt_bt_serviceteam == true).FirstOrDefault();
            }
        }

        /// <summary>
        /// Informa se a equipe proprietária da cotação é equipe de serviço.
        /// </summary>
        /// <param name="target"> preImage da cotação </param>
        /// <returns> true ou false</returns>
        public bool? OwnerTeamIsService(Quote target)
        {
            bool? isService = false;
            Team team = GetOwnerTeam(target);

            if (team != null && team.smt_bt_serviceteam != null)
                isService = team.smt_bt_serviceteam;

            return isService;
        }

        /// <summary>
        /// Busca a Unidade Organizacional da equipe de serviços
        /// </summary>
        /// <param name="mergedQuote"> Merged de Cotação </param>
        /// <returns> Unidade Organizacional </returns>
        public EntityReference GetUnitOrganizationServiceTeam(Quote mergedQuote)
        {
            EntityReference UnitOrganization = new EntityReference();

            // Busca a equipe proprietária
            Team team = GetOwnerTeam(mergedQuote);

            // Retorna a unidade organizacional da equipe de serviços
            if (team != null)
                UnitOrganization = GetOrganizationUnitFromServiceTeam(team);

            return UnitOrganization;
        }

        /// <summary>
        /// Busca a unidade organizacional da equipe de serviços com base na unidade do recurso do primeiro usuário que encontrar nela.
        /// </summary>
        /// <param name="team"> equipe </param>
        /// <returns> unidade organizacional da equipe de serviços </returns>
        public EntityReference GetOrganizationUnitFromServiceTeam(Team team)
        {
            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                Guid? idUser = (from teammember in crmService.TeamMembershipSet
                                where teammember.TeamId == team.Id
                                select teammember.SystemUserId).FirstOrDefault();

                EntityReference unitOrganization = (from resource in crmService.BookableResourceSet
                                                    where resource.UserId.Id == idUser
                                                    && resource.msdyn_organizationalunit != null
                                                    select resource.msdyn_organizationalunit).FirstOrDefault();

                return unitOrganization;
            }
        }

        /// <summary>
        /// Busca a Equipe proprietária da Cotação
        /// </summary>
        /// <param name="quote"> Cotação </param>
        /// <returns> Equipe proprietária da Cotação </returns>
        public Team GetOwnerTeam(Quote quote)
        {
            // Team team = new Team();
            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                var team = (from teams in crmService.CreateQuery<Team>()
                            where teams.Id == quote.OwnerId.Id
                            select teams).FirstOrDefault().ToEntity<Team>();

                return team;
            }
        }

        /// <summary>
        /// A finalidade do método é buscar uma equipe através do campo "Dominio da Equipe"
        /// </summary>
        /// <param name="domainName">Nome do dominio obtido através da cotação. </param>
        public Team GetTeamByDomain(string domainName)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.TeamSet.Where(team => team.smt_st_teamdomain == domainName).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// A finalidade do método é buscar uma equipe através do campo "Dominio da Equipe"
        /// </summary>
        /// <param name="domainName">Nome do dominio obtido através da cotação. </param>
        public msdyn_organizationalunit GetUOByDomain(string domainName)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    var unitOrganizations = (from uo in context.CreateQuery<msdyn_organizationalunit>()
                                             where uo.msdyn_name != null &&
                                             uo.smt_st_domain == domainName
                                             select uo).FirstOrDefault();

                    return unitOrganizations;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateOwnerUOQuote(Quote quote)
        {
            Quote quoteUpdate = quote;
            Update(quoteUpdate);
        }

        /// <summary>
        /// Este método ter por finalidade atualizar o proprietário de uma determinada cotação. 
        /// </summary>
        /// <param name="quoteId">A cotação que será atualizada.</param>
        /// <param name="teamReference">Nome do dominio obtido através da cotação. </param>
        /// <param name="organizationalunit"> unidade organizacional </param>
        public void UpdateQuote(Guid quoteId, EntityReference teamReference)
        {
            Quote quote = new Quote
            {
                Id = quoteId,
                OwnerId = teamReference,
            };
            Update(quote);
        }

        public msdyn_organizationalunit GetPixeonOrganization()
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                var pixeonUO = (from uo in context.CreateQuery<msdyn_organizationalunit>()
                                where uo.msdyn_name == "PIXEON"
                                select uo).FirstOrDefault();

                return pixeonUO;
            }
        }

        public void UpdateOwnerQuote(Quote target, Quote preImage, Quote quoteUpdate)
        {
            OptionSetValue classification = target.smt_pl_classification != null ? target.smt_pl_classification : preImage.smt_pl_classification != null ? preImage.smt_pl_classification : null;
            OptionSetValue family = target.smt_pl_family != null ? target.smt_pl_family : preImage.smt_pl_family != null ? preImage.smt_pl_family : null;
            OptionSetValue focusedProduct = target.smt_pl_focused_product != null ? target.smt_pl_focused_product : preImage.smt_pl_focused_product != null ? preImage.smt_pl_focused_product : null;
            OptionSetValue items = target.smt_pl_items != null ? target.smt_pl_items : preImage.smt_pl_items != null ? preImage.smt_pl_items : null;

            if (classification != null && family != null && focusedProduct != null && items != null)
            {
                var mergedQuote = new Quote()
                {
                    smt_pl_classification = classification,
                    smt_pl_family = family,
                    smt_pl_focused_product = focusedProduct,
                    smt_pl_items = items,
                    CustomerId = preImage.CustomerId
                };

                var alocation = GetTeam(mergedQuote);

                if (alocation != null && alocation.smt_lp_team != null)
                {
                    // quoteUpdate.msdyn_contractorganizationalunitid = alocation.smt_lp_organizationalunit;
                    quoteUpdate.OwnerId = new EntityReference("team", alocation.smt_lp_team.Id);
                    UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
                    // localContext.OrganizationServiceAdmin.Update(quoteUpdate);
                }
            }
        }

        public void UpdateUnitOrganizationQuote(Guid quoteId, EntityReference organizationalUnit)
        {
            Quote quoteUpdate = new Quote
            {
                Id = quoteId,
                msdyn_ContractOrganizationalUnitId = organizationalUnit
            };

            try
            {
                quoteUpdate.EntityState = EntityState.Changed;
                Update(quoteUpdate);
            }
            catch (Exception ex)
            {
                string name = "Erro ao atualizar unidade de contratação da cotação";
                string message = $"message: {ex.Message} + source: {ex.Source} + stackTrace: {ex.StackTrace}";
                CreateLog(name, Quote.EntityLogicalName, quoteId.ToString(), quoteId.ToString(), message);
            }
        }

        /// <summary>
        /// Cria Logs
        /// </summary>
        /// <param name="name"> nome do log</param>
        /// <param name="entityname"> nome da entidade em que ocorreu o erro</param>
        /// <param name="recordName"> nome do registro </param>
        /// <param name="recordId"> id do registro </param>
        /// <param name="message"> mensagem de erro </param>
        public void CreateLog(string name, string entityname, string recordName, string recordId, string message)
        {
            smt_log log = new smt_log()
            {
                smt_name = name,
                smt_st_entityname = entityname,
                smt_pl_eventtype = new OptionSetValue(100000000), // Erro
                smt_st_eventorigin = "PostUpdateAsync_quote",
                smt_dt_eventdate = DateTime.Now,
                smt_st_recordname = recordName,
                smt_st_recordid = recordId,
                smt_tx_message = message
            };

            ServiceAdmin.Create(log);
        }

        /// <summary>
        /// Fecha a cotação
        /// </summary>
        /// <param name="quote">Recebe a Cotação como parâmetro</param>
        public void CloseQuote(Quote quote)
        {


            CloseQuoteRequest closeQuoteRequest = new CloseQuoteRequest()
            {
                QuoteClose = new QuoteClose()
                {
                    QuoteId = quote.ToEntityReference(),
                    Subject = "Quote Close " + DateTime.Now.ToString(),
                },
                Status = new OptionSetValue(-1),
                RequestName = "CloseQuote",
            };
            Service.Execute(closeQuoteRequest);
        }

        /// <summary>
        /// Fecha a cotação
        /// </summary>
        public smt_smartparameter GetDefaultUnit()
        {
            using (var service = new CrmServiceContext(ServiceAdmin))
            {
                var parameter = (from p in service.CreateQuery<smt_smartparameter>()
                                 where p.smt_name == "DEFAULT UNIT QUOTE"
                                 select p).FirstOrDefault();

                return parameter;
            }
        }

        public List<Calendario> GetCalendarios(DateTime start, DateTime end, String calendarName)
        {
            String retorno = string.Empty;
            try
            {
                Calendar calendar = null;
                using (var orgContext = new CrmServiceContext(Service))
                {
                    do
                    {
                        calendar = (from c in orgContext.CreateQuery<Calendar>()
                                    where c.Name.Contains(calendarName)
                                    select new Calendar
                                    {
                                        Id = c.Id
                                    }).FirstOrDefault();

                        if (calendar == null)
                            calendarName = " ";
                    }
                    while (calendar == null);
                }
                var parameters = new ParameterCollection();
                parameters.Add("calendarId", calendar.Id.ToString());
                parameters.Add("startTimeLocal", start);
                parameters.Add("endTimeLocal", end);
                var ActionObterCalendario = new OrganizationRequest("smt_obtercalendario")
                {
                    Parameters = parameters
                };
                OrganizationResponse response = Service.Execute(ActionObterCalendario);
                if (response.Results.FirstOrDefault().ToString() != null)
                {
                    retorno = response.Results.FirstOrDefault().ToString();
                    retorno = retorno.Replace("retorno, [", null);
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(retorno)))
                    {
                        var deserializedCalendario = new List<Calendario>();
                        var ser = new DataContractJsonSerializer(deserializedCalendario.GetType());
                        deserializedCalendario = ser.ReadObject(ms) as List<Calendario>;
                        ms.Close();
                        return deserializedCalendario;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Erro na busca do calendário: \n\n{ex.Message} \n\n retorno: {retorno}");
            }
        }

        public DateTime TempoSLA(List<Calendario> calendarios, DateTime sla, int duration)
        {
            try
            {
                DateTime retorno = DateTime.Now;

                List<Calendario> calender = calendarios;
                calendarios = (from c in calender
                               where c.PSA_EndTime.DayOfWeek != DayOfWeek.Sunday && c.PSA_EndTime.DayOfWeek != DayOfWeek.Saturday
                               orderby c.PSA_StartTime ascending
                               select c).ToList<Calendario>();

                DateTime inicio = calendarios[0].PSA_StartTime;
                DateTime fim = calendarios[1].PSA_EndTime;

                if (sla > fim)
                {
                    int i = 0;
                    foreach (Calendario calendario in calendarios)
                    {
                        if (i == (duration + 1) * 2)
                        {
                            retorno = calendario.PSA_EndTime;
                        }
                        i++;
                    }
                }
                else
                {
                    int i = 0;
                    foreach (Calendario calendario in calendarios)
                    {
                        if (i == duration * 2)
                        {
                            retorno = calendario.PSA_EndTime;
                        }
                        i++;
                    }
                }
                return retorno;

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Erro no metodo TempoSLA: \n\n" + ex.Message);
            }
        }

        public String GetCalendarName()
        {
            using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
            {
                return serviceContext.CreateQuery<smt_smartparameter>().Where(p => p.smt_name == "Calendário Padrão").FirstOrDefault().smt_value;
            }
        }

        public void UpdateEmailDates(Quote target)
        {
            Quote quote = new Quote
            {
                Id = target.Id,
                smt_dt_email_ownermanager = target.smt_dt_email_ownermanager,
                smt_dt_email_director = target.smt_dt_email_director,
                smt_dt_email_verticalmanager = target.smt_dt_email_verticalmanager
            };

            ServiceAdmin.Update(quote);
        }
    }
}
