using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using CRM.Pixeon.Extends.Earlybound;

namespace Pixeon.Extends.Plugins
{
    public class PreCreateSync_SalesOrder : PluginBase
    {
        public PreCreateSync_SalesOrder() : base(typeof(PreCreateSync_SalesOrder)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            SalesOrder target = localcontext.GetTarget<SalesOrder>();
            SalesOrderBusiness business = new SalesOrderBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            DuplicateSalesOrder(target, business);
            AssociateOrganizationUnit(target, business);
            SetModalidadeCanal(target,business);
            // GetArqsSalesForce(target, business);
        }

        public void AssociateOrganizationUnit(SalesOrder target, SalesOrderBusiness business)
        {
            var uoCotacao = business.GetOrganizationUnit(target.QuoteId.Id);
            target.msdyn_ContractOrganizationalUnitId = uoCotacao.msdyn_ContractOrganizationalUnitId;
            
            /*
            if (target.smt_st_license != null && target.smt_st_license.ToUpper() == "GOLD PARTNER")
            {
                if (target.smt_st_email != String.Empty)
                {
                    string channel = target.smt_st_email; // Campo Canal da cotação

                    // Busca a unidade organizacional com o "domínio" igual ao canal da cotação
                    msdyn_organizationalunit unitOrganization = business.GetUOByDomain(channel);

                    if (unitOrganization != null)
                    {
                        target.msdyn_ContractOrganizationalUnitId = unitOrganization.ToEntityReference();                        
                    }
                       
                }
            }
            else
            {
                // Busca um parâmetro de alocação de acordo com a família, produto/itens, produto concentrado, classificação de venda e a região do cliente
                smt_parameter_alocation parameter = business.GetUO(target);
                msdyn_organizationalunit unidadeOrganizacional = business.GetOrganizationUnit(target);
                if (parameter != null && parameter.smt_lp_organizationalunit != null)
                    target.msdyn_ContractOrganizationalUnitId = parameter.smt_lp_organizationalunit;

            }
            */
        }

        public void GetArqsSalesForce(SalesOrder target, SalesOrderBusiness business)
        {
            if (target.QuoteId != null)
            {
                business.GetArqs(target);
            }
        }
        /// <summary>
        /// Verifica se existe já contrato do projeto existente,
        /// </summary>
        /// <param name="target">Target de Contrato do Projeto</param>
        /// <param name="business">Business da Ordem</param>
        public void DuplicateSalesOrder(SalesOrder target, SalesOrderBusiness business)
        {
            if (target.smt_st_opp_number != null)
            {
                business.GetDuplicate(target);
            }
        }
        /// <summary>
        /// Define o campo Modalidade Canal
        /// </summary>
        /// <param name="target">.</param>
        /// <param name="busines">.</param>
        public void SetModalidadeCanal(SalesOrder target, SalesOrderBusiness busines)
        {
            var modalidadeCanal = busines.getModalidadeCanal(target);

            if (modalidadeCanal != null)
            {
                target.smt_op_modalidade_canal = modalidadeCanal.smt_op_modalidade_canal;
            }
        }
    }
}
