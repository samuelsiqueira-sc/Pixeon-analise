using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CRM.Pixeon.Extends.Business;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk;
using CRM.Pixeon.Extends.Earlybound;
using System.Security.Cryptography.X509Certificates;
using Smart.ExtendedPSA.Extends.Models;
using System.IdentityModel.Tokens;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// a
    /// </summary>
    public class PostCreateSync_Quote : PluginBase
    {
        /// <summary>
        /// Classe construtora
        /// </summary>
        public PostCreateSync_Quote() : base(typeof(PostCreateSync_Quote)) { }

        /// <summary>
        /// Método utilizado para receber o contexto da execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto do Plugin</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            QuoteBusiness business = new QuoteBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

            Quote quote = localContext.GetTarget<Quote>();
            AssociateTeam(localContext, quote, business);
            StoryQuotes(localContext, quote, business);
            
            SetEmailDates(quote, business);
        }

        /// <summary>
        /// Método que verifica se a nova cotação já faz parte de uma mesma oportunidade relacionada no SF
        /// </summary>
        /// <param name="localContext">contexto</param>
        /// <param name="quote">target</param>
        /// <param name="business">business</param>
        private void StoryQuotes(LocalPluginContext localContext, Quote quote, QuoteBusiness business)
        {
            var quotes = business.RetrieveQuotesByNumberOpp(quote);

            if (quotes != null)
            {
                business.Associate(quote, quotes);
            }
        }

        /// <summary>
        /// Associa o time a cotação
        /// </summary>
        /// <param name="localContext">contexto</param>
        /// <param name="quote">target</param>
        /// <param name="business">business</param>
        private void AssociateTeam(LocalPluginContext localContext, Quote quote, QuoteBusiness business)
        {
            Quote quoteUpdate = new Quote();
            quoteUpdate.Id = quote.Id;

            // smt_smartparameter parameter = business.GetDefaultUnit();

            // if (quote.msdyn_ContractOrganizationalUnitId.Id.ToString() != parameter.smt_value)
            // {
            //    var teamDefault = business.GetTeamDefault(quote);

            // if (teamDefault != null && teamDefault.smt_lp_team != null)
            //    {
            //        quoteUpdate.OwnerId = new EntityReference(Team.EntityLogicalName, teamDefault.smt_lp_team.Id);
            //        business.UpdateQuote(quote.Id, quoteUpdate.OwnerId);
            //    }
            // }
            // else
            // {
            if (quote.smt_st_license != null && quote.smt_st_license.ToUpper() == "GOLD PARTNER") // se a licença for diferente de null
            {
                Team team = new Team();
                msdyn_organizationalunit contractOrganization = new msdyn_organizationalunit();

                string isGoldPartner = quote.smt_st_license.ToUpper(); // Campo "Licença" contido na cotação. Valida se a Licença é Gold Partner. 

                if (isGoldPartner == "GOLD PARTNER")
                {
                    // Verificar qual é o tipo de modalidade de canal e direcionar conforme o tipo
                    
                    // Se tipo 2: Direciona para o canal ao qual ela pertence

                    if (quote.smt_op_modalidade_canal == smt_op_modalidade_canal.Tipo2)
                    {
                        string channel = quote.smt_st_email; // Campo "E-mail" da Cotação. Esse campo contem a informação do CANAL
                        team = business.GetTeamByDomain(channel); // Retorna um time baseado no campo "Dominio da Equipe" contido na entidade "Equipe". 
                        quoteUpdate.OwnerId = team != null ? new EntityReference("team", team.Id) : null;
                        business.UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
                    }                    
                    else if (quote.smt_op_modalidade_canal == smt_op_modalidade_canal.Tipo1)
                    {
                        // Se tipo 1: Direciona para os GPs definido nas regras de Distribuição de GPs
                        var alocation = business.GetTeam(quote);

                        if (alocation != null && alocation.smt_lp_team != null)
                        {
                            // quoteUpdate.msdyn_contractorganizationalunitid = alocation.smt_lp_organizationalunit;
                            quoteUpdate.OwnerId = new EntityReference("team", alocation.smt_lp_team.Id);
                            business.UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
                            // localContext.OrganizationServiceAdmin.Update(quoteUpdate);
                        }
                        else
                        {
                            team = business.RetrieveServiceTeam();
                            quoteUpdate.OwnerId = team != null ? new EntityReference("team", team.Id) : null;

                            if (team != null)
                                business.UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
                        }
                    }
                }
            }
            else if (quote.smt_pl_family != null && 
                    quote.smt_pl_classification != null && 
                    quote.smt_txt_integrador != null) // Verifica se algum dos campos que definem um parâmetro foi alterado
            {
                var alocation = business.GetTeam(quote);

                if (alocation != null && alocation.smt_lp_team != null)
                {
                    // quoteUpdate.msdyn_contractorganizationalunitid = alocation.smt_lp_organizationalunit;
                    quoteUpdate.OwnerId = new EntityReference("team", alocation.smt_lp_team.Id);
                    business.UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
                    // localContext.OrganizationServiceAdmin.Update(quoteUpdate);
                }
                else
                {
                    var team = business.RetrieveServiceTeam();
                    quoteUpdate.OwnerId = team != null ? new EntityReference("team", team.Id) : null;

                    if (team != null)
                        business.UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
                }
            }
            else
            {
                var team = business.RetrieveServiceTeam();
                quoteUpdate.OwnerId = team != null ? new EntityReference("team", team.Id) : null;

                if (team != null)
                    business.UpdateQuote(quoteUpdate.Id, quoteUpdate.OwnerId);
            }            
        }

        private void SetEmailDates(Quote target, QuoteBusiness business)
        {
            DateTime inicioSLA = DateTime.Now.ToUniversalTime();

            String nomeCalendario = business.GetCalendarName();

            List<Calendario> calendarios = business.GetCalendarios(inicioSLA, inicioSLA.AddDays(10), nomeCalendario);
            target.smt_dt_email_ownermanager = business.TempoSLA(calendarios, inicioSLA, 3);
            target.smt_dt_email_verticalmanager = business.TempoSLA(calendarios, inicioSLA, 4);
            target.smt_dt_email_director = business.TempoSLA(calendarios, inicioSLA, 5);
            business.UpdateEmailDates(target);
        }

    }

}
