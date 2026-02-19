using System;
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
    /// Chamada do plugin
    /// </summary>
    public class PostUpdateAsync_msdyn_projecttask : PluginBase
    {
        /// <summary>
        /// Método que libera o reconhecimento de receita
        /// </summary>
        public PostUpdateAsync_msdyn_projecttask() : base(typeof(PostUpdateAsync_msdyn_projecttask)) { }
        /// <summary>
        /// Execução do plugin
        /// </summary>
        /// <param name="localContext">Contexto</param>
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
           /* WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);*/
            #endregion
            
            msdyn_projecttask target = localContext.GetTarget<msdyn_projecttask>();
            msdyn_projecttask preImage = localContext.GetPreImage<msdyn_projecttask>();
            msdyn_projecttask mergeTask = localContext.GetMergePreImage<msdyn_projecttask>();
            msdyn_projecttaskBusiness business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
                       
            // UpdateEstimatedRevenue(target, preImage, business, localContext, mergeTask);
            // RecognizedRevenue(target, preImage, mergeTask, localContext, business);
            MarkAsCompleted(mergeTask, target, business);
        }
        /// <summary>
        /// Método que realiza o reconhecimento de receita
        /// </summary>
        /// <param name="target">Target da tarefa do projeto</param>
        /// <param name="preImage">PreImage da tarefa do projeto</param>
        /// <param name="mergeTask">Merge PreImage</param>
        /// <param name="localContext">Contexto</param>
        /// <param name="business"> business de tarefa do projeto </param>
        private void RecognizedRevenue(msdyn_projecttask target, msdyn_projecttask preImage, msdyn_projecttask mergeTask, LocalPluginContext localContext, msdyn_projecttaskBusiness business)
        {
            if (target.smt_statusprojetoEnum == msdyn_projecttask_smt_statusprojeto.Concluido ||
                target.smt_statusprojetoEnum == msdyn_projecttask_smt_statusprojeto.AprovadoAutomaticamente)
            {
                if (preImage.smt_lp_milestone != null)
                {
                    var valueTask = business.SetRevenue(preImage);
                    msdyn_projecttask taskActual = new msdyn_projecttask();
                    taskActual.Id = target.Id;
                    taskActual.smt_mn_recognized_revenue = valueTask;
                    // taskActual.smt_lp_milestone = target.smt_lp_milestone;

                    taskActual.smt_dt_recognized = DateTime.Now;

                    localContext.OrganizationService.Update(taskActual);
                    business.SetTotalRevenueProject(preImage, valueTask);
                }
            }
        }

        /// <summary>
        /// Marcar a tarefa pai como concluída
        /// </summary>
        /// <param name="mergeTask"> merge de tarefa do projeto </param>
        /// <param name="targetTask"> target de tarefa do projeto </param>
        /// <param name="business"> business </param>
        private void MarkAsCompleted(msdyn_projecttask mergeTask, msdyn_projecttask targetTask, msdyn_projecttaskBusiness business)
        {
            if (targetTask.smt_statusprojetoEnum == msdyn_projecttask_smt_statusprojeto.Concluido && mergeTask.msdyn_parenttask != null)
            {
                business.GetTasksWithSameParent(mergeTask);
            }
        }

        /// <summary>
        /// Verifica se o marco contém dados e se sim, atualiza o valor de Receita Estimada 
        /// </summary>
        /// <param name="targetTask">Tarefa</param>
        /// <param name="preImageTask">Tarefa - Pre Image</param>
        /// <param name="business">Classe Business</param>
        /// <param name="localContext">Contexto</param>
        /// <param name="mergeTask"> merge da tarefa do projeto </param>
        private void UpdateEstimatedRevenue(msdyn_projecttask targetTask, msdyn_projecttask preImageTask, msdyn_projecttaskBusiness business, LocalPluginContext localContext, msdyn_projecttask mergeTask)
        {
            if ((targetTask.smt_lp_milestone != null && targetTask.msdyn_finish != null) &&
                targetTask.smt_statusprojetoEnum != msdyn_projecttask_smt_statusprojeto.Concluido ||
                targetTask.smt_statusprojetoEnum != msdyn_projecttask_smt_statusprojeto.AprovadoAutomaticamente)
            {

                business.SetEstimatedRevenueIfMilestoneIsNotNull(targetTask, preImageTask, mergeTask);
            }
        }
    }
}