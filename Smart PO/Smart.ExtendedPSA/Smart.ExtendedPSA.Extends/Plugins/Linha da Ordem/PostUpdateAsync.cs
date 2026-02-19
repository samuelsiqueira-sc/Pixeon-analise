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
    public class PostUpdateAsync : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe.
        /// </summary>
        public PostUpdateAsync() : base(typeof(PostUpdateAsync)) { }
        /// <summary>
        /// Execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }

            #region Messages

            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

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

            // Declaração de variáveis. 
            SalesOrderDetail target = localContext.GetTarget<SalesOrderDetail>();
            SalesOrderDetail preImage = localContext.GetPreImage<SalesOrderDetail>();
            LinhadaOrdemBusiness business = new LinhadaOrdemBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

            ////SalesOrderDetail salesOrderActual = new SalesOrderDetail
            ////{
            ////    Id = targetDetail.Id,
            ////    msdyn_Project = targetDetail.msdyn_Project != null ? targetDetail.msdyn_Project : preImageDetail.msdyn_Project,
            ////    PricePerUnit = targetDetail.PricePerUnit != null ? targetDetail.PricePerUnit : preImageDetail.PricePerUnit,
            ////};

            // Invocação dos métodos. 
            UpdateEstimatedRevenue(target, preImage, business);

        }

        /// <summary>
        /// Método que realiza o reconhecimento de receita
        /// </summary>
        /// <param name="target">Target da Linha de Ordem </param>
        /// <param name="preImage">preImage da Linha de Ordem</param>
        /// <param name="business"> Business da Linha de Ordem</param>
        private void UpdateEstimatedRevenue(SalesOrderDetail target, SalesOrderDetail preImage, LinhadaOrdemBusiness business)
         {
            var projectGuid = target.msdyn_Project != null ? target.msdyn_Project : preImage.msdyn_Project != null ? preImage.msdyn_Project : null;
            // Retrieve do projeto associado à linha de ordem 

            if (projectGuid != null)
            {
                msdyn_project project = business.RetrieveProject(projectGuid.Id);

                if (project != null)
                {
                    Money value = target.PricePerUnit != null ? target.PricePerUnit : preImage.PricePerUnit;

                    if (value != null)
                    {
                        // Listar todas as tarefas do projeto 
                        List<msdyn_projecttask> tasks = business.RetrieveProjectTasks(project.Id);  // Retrieve nas tarefas do projeto relacionadas ao projeto da linha da ordem

                        foreach (msdyn_projecttask task in tasks)
                        {

                            smt_milestone_project marco = business.RetrieveTaskMilestone(task); // Retrieve no marco relacionado a essa tarefa

                            if (marco != null)
                            {
                                msdyn_projecttask taskToUpdate = new msdyn_projecttask();
                                taskToUpdate.Id = task.Id;
                                taskToUpdate.smt_mn_estimated_revenue = task.smt_mn_estimated_revenue;
                                taskToUpdate.smt_mn_estimated_revenue = value != null ? new Money((decimal)(value.Value * (marco.smt_dc_stage / 100))) : null;
                                business.UpdateAsAdmin(taskToUpdate);


                            }
                        }
                    }
                }
            }

        }

    }
}
