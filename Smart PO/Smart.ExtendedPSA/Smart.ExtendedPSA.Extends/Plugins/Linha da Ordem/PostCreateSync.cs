using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Plugins.Linha_da_Ordem
{
    /// <summary>
    /// Construtor padrão.
    /// </summary>
    public class PostCreateSync : PluginBase
    {
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public PostCreateSync() : base(typeof(PostCreateSync)) { }

        /// <summary>
        /// Classe padrão de execução de plugins. 
        /// </summary>
        /// <param name="localcontext">Contexto local de execução.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
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

            // Multilinguagem.
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localcontext.LoadResxMessages(resxFileName);

            // Declaração de variáveis. 
            SalesOrderDetail target = localcontext.GetTarget<SalesOrderDetail>();
            LinhadaOrdemBusiness business = new LinhadaOrdemBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos. 
            ContractedAmount(target, business);         
        }

        /// <summary>
        /// Documentar.
        /// </summary>
        /// <param name="target">Documentar.</param>
        /// <param name="business">Documentar.</param>
        public void ContractedAmount(SalesOrderDetail target, LinhadaOrdemBusiness business)
        {
            if (target.msdyn_Project != null)
            {
                string firstParameterName = "% valor contratado";
                string secondParameterName = "Margem de venda padrão";

                msdyn_project salesOrderDetailProject = business.GetProject(target.msdyn_Project.Id); // Busca o projeto da linha da ordem. 
                Guid businessUnit = salesOrderDetailProject.msdyn_ContractOrganizationalUnitId.Id; // GUID da unidade organizacional do projeto. Utilizado para realizar a busca dos pârametros.

                smt_smartparameter contractedAmountPercentage = business.GetSmartParameter(firstParameterName, businessUnit); // busca o parâmetro "% valor contratado".
                smt_smartparameter defaultSalesMargin = business.GetSmartParameter(secondParameterName, businessUnit); // Busca o pârametro "Margem de venda padrão".

                if (contractedAmountPercentage == null || contractedAmountPercentage.smt_value == null || contractedAmountPercentage.smt_value == "0")
                {
                    business.ChangeValue(salesOrderDetailProject, target);

                    if (!salesOrderDetailProject.Contains("smt_margemprojeto") && defaultSalesMargin != null)
                    {
                        decimal value = Math.Round(Convert.ToDecimal(defaultSalesMargin.smt_value), 2);
                        business.SetValue(salesOrderDetailProject, value);
                    }
                }
                else
                {
                    // No projeto, será atribuido no campo "Valor Contratado" o valor da linha do contrato multiplicado pelo valor encontrado
                    // anteriormente no parâmetro "% valor contratado"
                    decimal percentage = Convert.ToDecimal(contractedAmountPercentage.smt_value); // string > decimal, valor vindo do parâmetro.
                    decimal priceperunit = (target.PricePerUnit.Value * percentage) / 100; // valor contratado * porcentagem do pârametro.
                    business.ChangeContractedAmountNull(salesOrderDetailProject, priceperunit);

                    if (!salesOrderDetailProject.Contains("smt_margemprojeto") && defaultSalesMargin != null)
                    {
                        decimal value = Math.Round(Convert.ToDecimal(defaultSalesMargin.smt_value), 2);
                        business.SetValue(salesOrderDetailProject, value);
                    }
                }
            }
        }
    }
}
