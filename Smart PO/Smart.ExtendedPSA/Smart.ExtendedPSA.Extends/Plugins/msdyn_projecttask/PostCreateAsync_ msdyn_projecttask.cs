using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Xrm.Sdk.Query;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// O plugin tem por finalidade associar o marco da tarefa de acordo com o modelo de projeto
    /// </summary>
    public class PostCreateAsync__msdyn_projecttask : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe.
        /// </summary>

        public PostCreateAsync__msdyn_projecttask() : base(typeof(PostCreateAsync__msdyn_projecttask)) { }

        /// <summary>
        /// Execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }
            /*
            #region Licença
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
            */
            #region Variaveis utilizadas.
            Guid userGuid = localContext.PluginExecutionContext.UserId;
            msdyn_projecttask projecttask = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_projecttask>();
            msdyn_projecttaskBusiness business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            #endregion

            #region Chamadas de funções 
           
            // AssociateRevenue(projecttask, business, localContext);
            #endregion
        }

        /// <summary>
        /// Método que associa o marco do projeto da tarefa de acordo com o modelo de projeto
        /// </summary>
        /// <param name="task">Target de tarefa</param>
        /// <param name="business">Classe de business</param>
        /// <param name="localContext">Contexto</param>
        private void AssociateRevenue(msdyn_projecttask task, msdyn_projecttaskBusiness business, LocalPluginContext localContext)
        {
            msdyn_project project = business.GetProject(task);
            if (project.msdyn_ProjectTemplate != null)
            {
                msdyn_projecttask taskTemplate = business.GetTaskTemplate(task, project);

                if (taskTemplate != null)
                {
                    msdyn_projecttask actualTask = new msdyn_projecttask();
                    actualTask.Id = task.Id;
                    actualTask.smt_lp_milestone = taskTemplate.smt_lp_milestone;
                    localContext.OrganizationServiceAdmin.Update(actualTask);
                }
                else if ((task.smt_lp_milestone != null && task.msdyn_finish != null) || task.smt_lp_milestone != null)
                {                    
                    business.SetEstimatedRevenueIfMilestoneIsNotNull(task, task, task);
                    business.UpdateAsAdmin(task);   
                }
                else if (task.smt_lp_milestone == null)
                {
                    task.smt_mn_estimated_revenue = project.smt_totaldespesasplanejadas;
                    task.smt_mn_total_revenue = project.smt_totaldespesasplanejadas;
                    business.UpdateAsAdmin(task);
                }
            }
        }
    }
}
