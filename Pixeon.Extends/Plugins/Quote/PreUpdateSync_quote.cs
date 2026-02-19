using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Earlybound;

namespace Pixeon.Extends.Plugins
{
    /// <summary>
    /// Chamada
    /// </summary>
    public class PreUpdateSync_quote : PluginBase
    {
        /// <summary>
        /// chamada
        /// </summary>
        public PreUpdateSync_quote() : base(typeof(PreUpdateSync_quote)) { }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação da licença. 

            // Declaração de variaveis
            Quote target = localcontext.GetTarget<Quote>();
            Quote preImage = localcontext.GetPreImage<Quote>();
            Quote mergedQuote = localcontext.GetMergePreImage<Quote>();
            QuoteBusiness business = new QuoteBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            ReasonsRepproval(target, preImage);
            SetOrganizationalUnit(target, preImage, business, mergedQuote);
            CloseQuote(target, preImage, localcontext);

        }

        /// <summary>
        /// Função que verifica se a cotação está sendo fechada como perdida e se os campos foram preenchidos.
        /// </summary>
        /// <param name="target">Target</param>
        /// <param name="preImage">PreImage</param>
        public void ReasonsRepproval(Quote target, Quote preImage)
        {
            if (target.StateCode != null && target.StateCode == QuoteState.Closed && target.StatusCode != null && target.StatusCode.Value == 5)
            {
                if (preImage.smt_st_reason_comment == null && target.smt_st_reason_comment == null)
                {
                    throw new InvalidPluginExecutionException("Não foi possível fechar a cotação, pois os campos de reprovação não foram preenchidos.");
                }
                else
                {
                    target.smt_dt_repprove = DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Insere a Unidade organizacional do parâmetro encontrado
        /// </summary>
        /// <param name="target"> target </param>
        /// <param name="preImage"> pre Image </param>
        /// <param name="business"> business </param>
        /// <param name="mergedQuote"> merge da Cotação</param>
        public void SetOrganizationalUnit(Quote target, Quote preImage, QuoteBusiness business, Quote mergedQuote)
        {

            if (target.OwnerId != null)
            {

                // if (preImage.smt_st_license != null && preImage.smt_st_license.ToUpper() == "GOLD PARTNER")
                // {
                //    string channel = preImage.smt_st_email; // Campo "E-mail" da Cotação. 
                //    /// string domain = email.Substring(email.IndexOf('@') + 1).Split('.')[0].ToLower(); // Isola a parte da string que contém o dominio do e-mail.

                // msdyn_organizationalunit unitOrganization = business.GetUOByDomain(channel);

                // if (unitOrganization != null)
                //        target.msdyn_ContractOrganizationalUnitId = unitOrganization.ToEntityReference();
                //    else
                //    {
                //        unitOrganization = business.GetPixeonOrganization();

                // if (unitOrganization != null)
                //            target.msdyn_ContractOrganizationalUnitId = unitOrganization.ToEntityReference();
                //        // business.UpdateQuote(target.Id, serviceTeam.ToEntityReference());
                //    }
                // }
                // else
                // {
                //    bool? isService = business.OwnerTeamIsService(target);

                // if (isService == true)
                //    {
                //        EntityReference OrganizationUnit = business.GetUnitOrganizationServiceTeam(mergedQuote);

                // if (OrganizationUnit != null)
                //            target.msdyn_ContractOrganizationalUnitId = OrganizationUnit;
                //    }
                //    else
                //    {
                //        smt_parameter_alocation parameter = business.GetTeam(preImage);

                // if (parameter != null && parameter.smt_lp_organizationalunit != null)
                //            target.msdyn_ContractOrganizationalUnitId = parameter.smt_lp_organizationalunit;
                //    }
                // }
                if (target.OwnerId.LogicalName == "team")
                {
                    var team = business.GetTeamOwner(target);
                    if (team != null)
                    {
                        target.msdyn_ContractOrganizationalUnitId = team.ToEntityReference();
                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("Não é possível atribuir cotação para um usuário. Por favor, atribuir a uma equipe do sistema.");
                }

            }

        }


        /// <summary>
        /// Fecha Cotação se Fase da Oportunidade for Venda Cancelada
        /// </summary>
        /// <param name="target"> target </param>
        /// <param name="preImage"> pre Image </param>
        /// <param name="localContext"> localContext </param>
        private void CloseQuote(Quote target, Quote preImage, LocalPluginContext localContext)
        {

            if (target.smt_pl_fase_oportunidade != null && target.smt_pl_fase_oportunidade == smt_pl_fase_oportunidade.VendaCancelada)
            {

                QuoteBusiness business = new QuoteBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
                
                business.CloseQuote(target);
                ///target.StateCode = QuoteState.Closed;
                /// business.UpdateQuote(target.Id, target.OwnerId);
            }

        }
    }
}
