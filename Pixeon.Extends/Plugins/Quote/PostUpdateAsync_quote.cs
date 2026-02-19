using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using Microsoft.Xrm.Sdk;

namespace Pixeon.Extends.Plugins
{
    public class PostUpdateAsync_quote : PluginBase
    {
        public PostUpdateAsync_quote() : base(typeof(PostUpdateAsync_quote)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            Quote target = localcontext.GetTarget<Quote>();
            Quote preImage = localcontext.GetPreImage<Quote>();
            QuoteBusiness business = new QuoteBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            // SetOwnerTeam(target, preImage, business);
            // SetOrganizationalUnit(target, preImage, business);
        }

        public void SetOwnerTeam(Quote target, Quote preImage, QuoteBusiness business)
        {
            var quoteUpdate = new Quote
            {
                Id = target.Id
            };

            if (target.smt_pl_businessunitEnum != null && target.smt_pl_businessunitEnum == Quote_smt_pl_businessunit.DISTRIBUIDOR) // caso seja distribuidor
            {
                Team team = new Team();
                string isGoldPartner = preImage.smt_st_license.ToUpper(); // Campo "Licença" contido na cotação. Valida se a Licença é Gold Partner. 

                if (isGoldPartner == "GOLD PARTNER")
                {
                    string channel = preImage.smt_st_email; // Campo "E-mail" da Cotação. 
                   // string domain = email.Substring(email.IndexOf('@') + 1).Split('.')[0].ToLower(); // Isola a parte da string que contém o dominio do e-mail.

                    team = business.GetTeamByDomain(channel); // Retorna um time baseado no campo "Dominio da Equipe" contido na entidade "Equipe". 
                                                                  // business.UpdateQuote(target.Id, team.ToEntityReference()); // Atualiza o proprietário da cotação. 
                }
                else
                {
                   team = business.RetrieveServiceTeam();
                    // business.UpdateQuote(target.Id, serviceTeam.ToEntityReference());
                }
                business.UpdateQuote(target.Id, team.ToEntityReference());

            }
            else if (target.smt_pl_classification != null || target.smt_pl_family != null || target.smt_pl_focused_product != null || target.smt_pl_items != null) // Verifica se algum dos campos que definem um parâmetro foi alterado
            {
                
                if (preImage.smt_pl_businessunitEnum == null)
                {
                    business.UpdateOwnerQuote(target, preImage, quoteUpdate);
                }
                else if (preImage.smt_pl_businessunitEnum != Quote_smt_pl_businessunit.DISTRIBUIDOR)
                {
                    business.UpdateOwnerQuote(target, preImage, quoteUpdate);
                }

            }
        }

        /// <summary>
        /// Insere a Unidade organizacional do parâmetro encontrado
        /// </summary>
        /// <param name="target"> target </param>
        /// <param name="preImage"> pre Image </param>
        /// <param name="business"> business </param>
        public void SetOrganizationalUnit(Quote target, Quote preImage, QuoteBusiness business)
        {
            Quote quoteUpdate = new Quote
            {
                Id = target.Id
            };

            if (target.OwnerId != null)
            {
                bool? isService = business.OwnerTeamIsService(target);

                if (isService == true)
                {
                    Quote mergedQuote = preImage;
                    mergedQuote.OwnerId = target.OwnerId;

                    EntityReference OrganizationUnit = business.GetUnitOrganizationServiceTeam(mergedQuote);

                    if (OrganizationUnit != null)
                        quoteUpdate.msdyn_ContractOrganizationalUnitId = OrganizationUnit;
                }
                else
                {
                    smt_parameter_alocation parameter = business.GetTeam(preImage);

                    if (parameter != null && parameter.smt_lp_organizationalunit != null)
                        quoteUpdate.msdyn_ContractOrganizationalUnitId = parameter.smt_lp_organizationalunit;
                }

                business.UpdateUnitOrganizationQuote(target.Id, quoteUpdate.msdyn_ContractOrganizationalUnitId);
            }

        }
    }
}
