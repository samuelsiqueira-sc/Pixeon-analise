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

namespace Smart.ExtendedPSA.Extends.Plugins
{

    // NOME ANTIGO: PG_ControlarProgressoFísico...

    /// <summary>
    /// Iniciaçaõ da Classe
    /// </summary>
    public class PreUpdateSync_msdyn_projecttask : PluginBase
    {
        /// <summary>
        /// PreUpdateSync_msdyn_projecttask
        /// </summary>
        public PreUpdateSync_msdyn_projecttask() : base(typeof(PreUpdateSync_msdyn_projecttask))
        {
        }

        /// <summary>
        /// Chamada da classe que executa a lógica.
        /// </summary>
        /// <param name="localContext">TODO</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {

            #region ResX

            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            #region Licença

            // Chamada da Action de Licenças
            /*WhoAmIRequest request = new WhoAmIRequest();
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

            var target = localContext.GetTarget<msdyn_projecttask>();
            var projectTaskPreImg = localContext.GetPreImage<msdyn_projecttask>();

            CheckProjectTask(target, projectTaskPreImg, localContext, null);
            // InsertWBSLevel(target, localContext);

        }

        /// <summary>
        /// Método que ira chamar a Business e validar a regra do Progresso Físico da Tarefa.
        /// </summary>
        /// <param name="target">Tarefa do Projeto</param>
        /// <param name="projectTaskPreImg_"> Pre Image daTarefa do Projeto</param>
        /// <param name="localContext">Contexto do Plugin</param>
        /// <param name="messages">RESX</param>
        private void CheckProjectTask(msdyn_projecttask target, msdyn_projecttask projectTaskPreImg_, LocalPluginContext localContext, List<Resx> messages)
        {
            if (target.smt_progressofisico != null || target.msdyn_Effort != null)
            {
                msdyn_projecttaskBusiness projectTaskBusiness = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);
                var tarefaContext = target; // localContext.GetTarget<msdyn_projecttask>();

                // Valida contexto
                projectTaskPreImg_.smt_progressofisico = tarefaContext.smt_progressofisico != null ? tarefaContext.smt_progressofisico : projectTaskPreImg_.smt_progressofisico;
                projectTaskPreImg_.msdyn_Effort        = tarefaContext.msdyn_Effort        != null ? tarefaContext.msdyn_Effort        : projectTaskPreImg_.msdyn_Effort;

                if (tarefaContext.smt_progressofisico != null || tarefaContext.msdyn_Effort != null)
                    projectTaskBusiness.ControlarProgressoFisico(projectTaskPreImg_);
            }

        }

        /// <summary>
        /// Método para preencher o nível da WBS
        /// </summary>
        /// <param name="target"> target </param>
        /// <param name="localContext"> contexto </param>
        private void InsertWBSLevel(msdyn_projecttask target, LocalPluginContext localContext)
        {
            if (target.msdyn_WBSID != null)
            {
                msdyn_projecttaskBusiness business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
                business.SetWBSLevel(target);
            }
        }

    }
}
