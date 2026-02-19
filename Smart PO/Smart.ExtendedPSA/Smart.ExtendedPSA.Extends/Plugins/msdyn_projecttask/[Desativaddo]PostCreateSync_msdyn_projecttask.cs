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
    /// O plugin tem por finalidade associar automaticamente um recurso a uma tarefa do projeto. 
    /// </summary>
    public class PostCreateSync_msdyn_projecttask : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe.
        /// </summary>
        public PostCreateSync_msdyn_projecttask() : base(typeof(PostCreateSync_msdyn_projecttask)) { }

        /// <summary>
        /// Execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }

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

            #region Variaveis utilizadas.
            Guid userGuid = localContext.PluginExecutionContext.UserId;
            msdyn_projecttask projecttask = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_projecttask>();
            msdyn_projecttaskBusiness business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            #endregion

            #region Chamadas de funções
           // AssociateResourceWithProjectTask(projecttask, business, userGuid);

            #endregion
        }

        /// <summary>
        /// Método que irá invocar a business e realizar as validações necessárias. 
        /// </summary>
        /// <param name="projecttask">Tarefa do projeto</param>
        /// <param name="business">Contexto de execução local</param>
        /// <param name="userGuid">Contexto de execução local</param>
        private void AssociateResourceWithProjectTask(msdyn_projecttask projecttask, msdyn_projecttaskBusiness business, Guid userGuid)
        {
            // Variaveis utilizadas.
            Guid projecttaskGuid = projecttask.Id;
            EntityCollection EncVazio = new EntityCollection();
            EncVazio.EntityName = "msdyn_projectteam";

            if (projecttask.smt_st_resourceemail != null && projecttask.smt_st_resourceemail != String.Empty)
            {
                BookableResource user = business.SearchBookableResource(projecttask);

                if (user != null)
                {
                    EntityCollection foundMembers = business.FindTeamMember(projecttask, user);

                    // Caso o usuário com aquele e-mail não seja encontrada na equipe do projeto, inserir na equipe do projeto antes da atribuição. 
                    if (foundMembers.Entities.Count == 0)
                    {
                        msdyn_project project = new msdyn_project();
                        project.Id = projecttask.msdyn_project.Id;
                        BookableResourceCategoryAssn AssignCategory = business.SearchAssignCategory(user);

                        if (AssignCategory == null)
                        {
                            business.CreateLog("UNPCRR", projecttaskGuid, userGuid);
                            business.CallActionAssociateResourceWithProjectTask(EncVazio, projecttask);
                        }
                        else
                        {
                            EntityCollection BookableResource = business.PostCreate_CreateTeamMembership(projecttask, project, AssignCategory);
                            business.CallActionAssociateResourceWithProjectTask(BookableResource, projecttask);
                        }
                    }
                    else
                    {
                        business.CallActionAssociateResourceWithProjectTask(foundMembers, projecttask);
                    }
                }
                else
                {
                    // Erro ao não encontrar email cadastrado.
                    business.CreateLog("ENCS", projecttaskGuid, userGuid);
                    business.CallActionAssociateResourceWithProjectTask(EncVazio, projecttask);
                }
            }
        }
    }
}
