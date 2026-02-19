using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// classe principal do plugin
    /// </summary>
    public class PostCreateAsync_smt_monthly_revenue_recognition : PluginBase
    {

        /// <summary>
        /// construtor
        /// </summary>
        public PostCreateAsync_smt_monthly_revenue_recognition() : base(typeof(PostCreateAsync_smt_monthly_revenue_recognition)) { }

        /// <summary>
        /// execute
        /// </summary>
        /// <param name="localContext"> contexto de execução </param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            #region Messages
            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            #region Licença

            // Chamada da Action de Licenças
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

            smt_monthly_revenue_recognition revenueRecognition = localContext.GetTarget<smt_monthly_revenue_recognition>();
            MonthlyRevenueRecognized(localContext, revenueRecognition, messages);
        }

        /// <summary>
        ///  Atualiza os valores do Reconhecimento de Receita
        /// </summary>
        /// <param name="localContext"> contexto de execução </param>
        /// <param name="revenueRecognition"> Reconhecimento de receita que disparou o plugin </param>
        /// <param name="messages"> mensagens resx </param>
        private void MonthlyRevenueRecognized(LocalPluginContext localContext, smt_monthly_revenue_recognition revenueRecognition, List<Resx> messages)
        {
            smt_monthly_revenue_recognitionBusiness business = new smt_monthly_revenue_recognitionBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);

            // Valor da soma de todas as tarefas filhas da tarefa do reconhecimento de receita
            decimal monthlyRevenue = business.SumTotalRevenueToProjectTaskThisMonth(revenueRecognition, revenueRecognition.smt_lp_projecttask.Id);

            // Atualiza ou deleta o reconhecimento de receita
            business.UpdateMonthlyRevenueRecognition(revenueRecognition, monthlyRevenue, localContext.PluginExecutionContext);
        }
    }
}
