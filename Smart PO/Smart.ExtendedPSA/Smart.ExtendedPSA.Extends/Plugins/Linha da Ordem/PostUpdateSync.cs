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
    /// doc.
    /// </summary>
    public class PostUpdateSync : PluginBase
    {
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public PostUpdateSync() : base(typeof(PostUpdateSync)) { }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="localcontext">doc.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localcontext.OrganizationService.Execute(request);
            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",//
            };
            localcontext.OrganizationService.Execute(ActionRequest);

            // Declaração de variáveis. 
            SalesOrderDetail target = localcontext.GetTarget<SalesOrderDetail>();
            SalesOrderDetail entityImage = localcontext.GetPreImage<SalesOrderDetail>();
            LinhadaOrdemBusiness business = new LinhadaOrdemBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            SalesOrderDetail salesOrderActual = new SalesOrderDetail
            {
                Id = target.Id,
                msdyn_Project = target.msdyn_Project != null ? target.msdyn_Project : entityImage.msdyn_Project,
                PricePerUnit = target.PricePerUnit != null ? target.PricePerUnit : entityImage.PricePerUnit,
            };

            if (salesOrderActual.PricePerUnit != null && salesOrderActual.msdyn_Project != null)
            {
                // Invocação dos métodos. 
                ChangeContractedAmountOnProject(salesOrderActual, business);
            }
        }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="salesOrder">doc</param>
        /// <param name="business">doc</param>
        public void ChangeContractedAmountOnProject(SalesOrderDetail salesOrder, LinhadaOrdemBusiness business)
        {
            string firstParameterName = "% valor contratado";
            string secondParameterName = "Margem de venda padrão";

            msdyn_project salesOrderDetailProject = business.GetProject(salesOrder.msdyn_Project.Id);  // Busca o projeto da linha da ordem. 
            msdyn_projecttask task = new msdyn_projecttask();

            if (salesOrderDetailProject != null)
            {
                Guid businessUnit = salesOrderDetailProject.msdyn_ContractOrganizationalUnitId.Id;
                smt_smartparameter contractedAmountPercentage = business.GetSmartParameter(firstParameterName, businessUnit); // busca o parâmetro "% valor contratado".
                smt_smartparameter defaultSalesMargin = business.GetSmartParameter(secondParameterName, businessUnit); // Busca o pârametro "Margem de venda padrão".

                if (contractedAmountPercentage == null || contractedAmountPercentage.smt_value == null || contractedAmountPercentage.smt_value == "0")
                {
                    business.ChangeValue(salesOrderDetailProject, salesOrder);
                }
                else
                {

                        // No projeto, será atribuido no campo "Valor Contratado" o valor da linha do contrato multiplicado pelo valor encontrado
                        // anteriormente no parâmetro "% valor contratado"
                        decimal percentage = Convert.ToDecimal(contractedAmountPercentage.smt_value); // string > decimal, valor vindo do parâmetro.
                        decimal priceperunit = (salesOrder.PricePerUnit.Value * percentage) / 100; // valor contratado * porcentagem do pârametro.
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
