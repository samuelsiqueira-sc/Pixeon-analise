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
    /// Classe padrão para PostUpdate. 
    /// </summary>
    public class PostUpdateSync_msdyn_projecttask : PluginBase
    {

        /// <summary>
        /// Chamada  padrão da classe.
        /// </summary>
        public PostUpdateSync_msdyn_projecttask() : base(typeof(PostUpdateSync_msdyn_projecttask)) { }

        /// <summary>
        /// Execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
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

            // Variaveis utilizadas.
            msdyn_projecttask target = localContext.GetTarget<msdyn_projecttask>();
            msdyn_projecttask preImage = localContext.GetPreImage<msdyn_projecttask>();
            msdyn_projecttask merged = localContext.GetMergePreImage<msdyn_projecttask>();

            // Chamada dos métodos. 
            // AssociateResourceWithProjectTask(localContext, target, preImage, merged);
           
            // AuditoriaAlteracao(localContext, target, preImage, messages);
            // CopyWBSId();
        }

        private void AssociateResourceWithProjectTask(LocalPluginContext localContext, msdyn_projecttask target, msdyn_projecttask preImage, msdyn_projecttask merged)
        {
            // O plugin tem por finalidade associar automaticamente um recurso a uma tarefa do projeto. 
            msdyn_projecttaskBusiness Business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            Guid userGuid = localContext.PluginExecutionContext.UserId;
            Guid projecttaskGuid = target.Id;
            EntityCollection EncVazio = new EntityCollection();
            EncVazio.EntityName = "msdyn_projectteam";

            var WorkItem = target.smt_int_work_item_id != null ? target.smt_int_work_item_id : preImage.smt_int_work_item_id;
            var result_atribuicao = Business.BuscarAtribuicaoDeRecurso(target.Id);

            if (merged.smt_st_resourceemail != null && merged.smt_st_resourceemail != string.Empty && preImage.smt_int_work_item_id != null)
            {
                if (merged.smt_st_resourceemail != preImage.smt_st_resourceemail)
                {
                    BookableResource user = Business.SearchBookableResource(merged);

                    if (user != null)
                    {
                        EntityCollection foundMembers = Business.FindTeamMember(merged, user);

                        // Caso o usuário com aquele e-mail não seja encontrada na equipe do projeto, inserir na equipe do projeto antes da atribuição. 
                        if (foundMembers.Entities.Count == 0)
                        {
                            msdyn_project project = new msdyn_project();
                            project.Id = merged.msdyn_project.Id;

                            BookableResourceCategoryAssn AssignCategory = Business.SearchAssignCategory(user);

                            if (AssignCategory == null)
                            {
                                // Caso o usuário não possua uma categoria de associação de recurso, um log é criado. A tarefa será criada
                                // porém nenhum usuário sera atribuido. 
                                Business.CreateLog("O usuário não possui uma associação de recurso.", projecttaskGuid, userGuid);
                                Business.CallActionAssociateResourceWithProjectTask(EncVazio, merged);
                            }
                            else
                            {
                                // No entanto, caso o usuário possua associação, será incluido no na equipe do projeto e será atribuido a uma 
                                // determinada tarefa.
                                EntityCollection BookableResource = Business.CreateTeamMembership(merged, project, merged, AssignCategory);
                                Business.CallActionAssociateResourceWithProjectTask(BookableResource, merged);
                            }
                        }
                        else
                        {
                            Business.CallActionAssociateResourceWithProjectTask(foundMembers, merged);
                        }
                    }
                    else
                    {
                        // Erro ao não encontrar email cadastrado.                          
                        Business.CreateLog("E-mail não cadastrado no sistema.", projecttaskGuid, userGuid);
                        Business.CallActionAssociateResourceWithProjectTask(EncVazio, merged);
                    }
                }
                else
                {
                    if (preImage.smt_int_work_item_id != null)
                    {
                        // No update, ao deixar o e-mail em branco, o recurso anterior será removido da tarefa que então ficará sem atribuição de recurso.
                        // Business.CallActionAssociateResourceWithProjectTask(EncVazio, projecttask);
                    }
                }
            }
        }

        /// <summary>
        /// Método que ira chamar a Business e validar a regra de Auditoria de Alteração.
        /// </summary>
        /// <param name="localContext">Contexto do Plugin</param>
        /// <param name="projecttask">Tarefa do Projeto</param>
        /// <param name="preImg"> Pre Image daTarefa do Projeto</param>
        /// <param name="messages">RESX</param>
        private void AuditoriaAlteracao(LocalPluginContext localContext, msdyn_projecttask projecttask, msdyn_projecttask preImg, List<Resx> messages)
        {
            if (projecttask.smt_st_resourceemail != null && projecttask.smt_st_resourceemail != String.Empty && preImg.smt_int_work_item_id != null)
            {
                msdyn_projecttask tarefaContext = (msdyn_projecttask)localContext.OrganizationService.Retrieve(projecttask.LogicalName, projecttask.Id,
                new ColumnSet("smt_st_work_item_id", "msdyn_effort", "smt_int_work_item_id", "smt_st_resourceemail", "smt_st_project_task_type", "msdyn_parenttask", "msdyn_subject", "msdyn_project"));
                msdyn_projecttaskBusiness Business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);

                // Valida contexto
                if (tarefaContext.Contains("smrkt_st_wo_item_id"))
                {
                    // Verfica se o campo está vazio ou nulo
                    if (tarefaContext.smt_st_work_item_id != null && tarefaContext.smt_st_work_item_id != string.Empty)
                    {
                        smt_project_alerts_smt_pl_taskchangedfield campoAlterado;
                        string actualvalue, oldvalue;

                        // Verifica se os campos alterados são os específicos
                        // Estimativa
                        if (tarefaContext.Contains("msdyn_effort") && preImg.Contains("msdyn_effort"))
                        {
                            if (tarefaContext.msdyn_Effort != preImg.msdyn_Effort)
                            {
                                campoAlterado = smt_project_alerts_smt_pl_taskchangedfield.Estimativa;
                                actualvalue = tarefaContext.msdyn_Effort.ToString();
                                oldvalue = preImg.msdyn_Effort.ToString();
                                Business.AuditoriaAlteracao(tarefaContext, campoAlterado, actualvalue, oldvalue);
                            }
                        }
                        // Item da Sprint
                        if (tarefaContext.Contains("smt_int_work_item_id") && preImg.Contains("smt_int_work_item_id"))
                        {
                            if (tarefaContext.smt_int_work_item_id != preImg.smt_int_work_item_id)
                            {
                                campoAlterado = smt_project_alerts_smt_pl_taskchangedfield.Item;
                                actualvalue = tarefaContext.smt_int_work_item_id.ToString();
                                oldvalue = preImg.smt_int_work_item_id.ToString();
                                Business.AuditoriaAlteracao(tarefaContext, campoAlterado, actualvalue, oldvalue);
                            }
                        }
                        // Recurso
                        if (tarefaContext.Contains("smt_st_resourceemail") && preImg.Contains("smt_st_resourceemail"))
                        {
                            if (tarefaContext.smt_st_resourceemail != preImg.smt_st_resourceemail)
                            {
                                campoAlterado = smt_project_alerts_smt_pl_taskchangedfield.Recurso;
                                actualvalue = tarefaContext.smt_st_resourceemail.ToString();
                                oldvalue = preImg.smt_st_resourceemail.ToString();
                                Business.AuditoriaAlteracao(tarefaContext, campoAlterado, actualvalue, oldvalue);
                            }
                        }

                    }
                }

                // Bloqueio de Alteração dos campos "Tipo do WorkItem" e "Tarefa Principal" para tarefas do tipo Iteration
                // Validar Object set
                if (tarefaContext.Contains("smt_st_project_task_type") && preImg.Contains("smt_st_project_task_type"))
                {
                    // Verifica se o tipo da task contém "Interation"
                    if (preImg.smt_st_project_task_type.Contains("Iteration") || preImg.msdyn_subject.Contains("Iteration"))
                    {
                        int alt = 0;
                        // Verifica se houve alteração no tipo da Task
                        if (preImg.smt_st_project_task_type != tarefaContext.smt_st_project_task_type)
                        {
                            // Mensagem de Trava de alteração.
                            alt = 1;
                            Business.TravaAlteracao(alt);
                            // throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF06));
                        }
                        if (preImg.msdyn_parenttask != tarefaContext.msdyn_parenttask)
                        {
                            // Mensagem de Trava de alteração.
                            alt = 2;
                            Business.TravaAlteracao(alt);
                        }
                        if (preImg.msdyn_subject != tarefaContext.msdyn_subject)
                        {
                            // Mensagem de Trava de alteração.
                            alt = 3;
                            Business.TravaAlteracao(alt);
                        }

                    }
                }
            }
        }

        private void CopyWbsId(msdyn_projecttask target)
        {

        }
    }
}
