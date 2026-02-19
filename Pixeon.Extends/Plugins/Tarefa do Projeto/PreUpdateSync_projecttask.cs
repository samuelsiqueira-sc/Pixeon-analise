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

namespace Pixeon.Extends.Plugins
{
    /// <summary>
    /// Classe dos plugins no PreUpdate síncrono em tarefa do projeto
    /// </summary>
    public class PreUpdateSync_projecttask : PluginBase
    {
        /// <summary>
        /// Método Construtor da classe
        /// </summary>
        public PreUpdateSync_projecttask() : base(typeof(PreUpdateSync_projecttask)) { }

        /// <summary>
        /// Método principal de execução do plugin
        /// </summary>
        /// <param name="localcontext"> Contexto de execução </param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_projecttask task = localcontext.GetTarget<msdyn_projecttask>();
            msdyn_projecttask preImage = localcontext.GetTarget<msdyn_projecttask>();
            projecttaskBusiness business = new projecttaskBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if(task.msdyn_Effort != null && preImage.msdyn_WBSID.ToString() == "1")
            {
                UpdateProjeto(task, preImage, localcontext);
            }
            // RecurrentRevenue(mergeTask, business);
        }

        /// <summary>
        /// Plugin de reconhecimento recorrente dos tipos padrão e instalação
        /// </summary>
        /// <param name="taskTarget"> Target da tarefa do projeto </param>
        /// <param name="taskImage">Pre Image de tarefa do projeto </param>
        /// <param name="mergeTask"> merge do target e pre image</param>
        /// <param name="business"> business de projecttask </param>
        protected void RecurrentRevenue(msdyn_projecttask mergeTask, projecttaskBusiness business)
        {
            business.ValidateRecurrentRevenue(mergeTask);
        }

        /// <summary>
        /// Plugin de reconhecimento recorrente dos tipos padrão e instalação
        /// </summary>
        /// <param name="mergeTask"> Target da tarefa do projeto </param>
        /// <param name="preImage">Pre Image de tarefa do projeto </param>
        /// <param name="localContext"> business de projecttask </param>
        protected void UpdateProjeto(msdyn_projecttask mergeTask, msdyn_projecttask preImage, LocalPluginContext localContext)
        {
            var project = new msdyn_project();
            project.msdyn_projectId = preImage.msdyn_project.Id;
            project.smt_dc_efforttotal = new Decimal(mergeTask.msdyn_Effort.Value);

            localContext.OrganizationServiceAdmin.Update(project);
        }
    }
}
