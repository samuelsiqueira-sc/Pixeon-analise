// TODO: QUE ISSO????????

/*using System;
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
    public class PostUpdate_PostCreate_AssociateResourceWithProjectTask : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe.
        /// </summary>
        public PostUpdate_PostCreate_AssociateResourceWithProjectTask() : base(typeof(PostUpdate_PostCreate_AssociateResourceWithProjectTask)) { }

        /// <summary>
        /// Execução do plugin.
        /// </summary>
        /// <param name="localContext">Contexto Local</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }
 
            // Variaveis utilizadas.
            msdyn_projecttaskBusiness Business = new msdyn_projecttaskBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            msdyn_projecttask projecttask = localContext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_projecttask>();
           

            Business.ValidateLicense();

            switch (localContext.PluginExecutionContext.MessageName.ToUpper())
            {

            case "UPDATE":
                    msdyn_projecttask projecttaskPreImage = localContext.GetPreImage<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_projecttask>();
                    if (projecttask.smt_st_resourceemail != null && projecttask.smt_st_resourceemail != String.Empty)
                    {
                        BookableResource user = Business.SearchBookableResource(projecttask);

                        if (user != null)
                        {
                            EntityCollection foundMembers = Business.FindTeamMember(projecttaskPreImage, user);

                            // Caso o usuário com aquele e-mail não seja encontrada na equipe do projeto, inserir na equipe do projeto antes da atribuição. 
                            if (foundMembers.Entities.Count == 0)
                            {
                                msdyn_project project = new msdyn_project();
                                project.Id = projecttaskPreImage.msdyn_project.Id;
                                msdyn_projectteam createTeamMembership = new msdyn_projectteam();

                                BookableResourceCategoryAssn AssignCategory = Business.SearchAssignCategory(user);

                                if (AssignCategory == null)
                                {
                                    throw new InvalidPluginExecutionException("O usuário não possui uma associação de recurso.");
                                }
                                else
                                {
                                    EntityCollection BookableResource = Business.CreateTeamMembership(projecttask, project, projecttaskPreImage, createTeamMembership, AssignCategory);
                                    Business.CallActionAssociateResourceWithProjectTask(BookableResource, projecttask);
                                }

                            }
                            else
                            {
                                Business.CallActionAssociateResourceWithProjectTask(foundMembers, projecttask);
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Não há usuário cadastrado com o e-mail informado.");
                        }     
                    }
                    break;

                case "CREATE":
                    if (projecttask.smt_st_resourceemail != null && projecttask.smt_st_resourceemail != String.Empty)
                    {
                        BookableResource user = Business.SearchBookableResource(projecttask);

                        if (user != null)
                        {
                            EntityCollection foundMembers = Business.FindTeamMember(projecttask, user);

                            // Caso o usuário com aquele e-mail não seja encontrada na equipe do projeto, inserir na equipe do projeto antes da atribuição. 
                            if (foundMembers.Entities.Count == 0)
                            {
                                msdyn_project project = new msdyn_project();
                                project.Id = projecttask.msdyn_project.Id;
                                msdyn_projectteam createTeamMembership = new msdyn_projectteam();

                                BookableResourceCategoryAssn AssignCategory = Business.SearchAssignCategory(user);

                                if (AssignCategory == null)
                                {
                                    throw new InvalidPluginExecutionException("O usuário não possui uma associação de recurso.");
                                }
                                else
                                {
                                    EntityCollection BookableResource = Business.PostCreate_CreateTeamMembership(projecttask, project, createTeamMembership, AssignCategory);
                                    Business.CallActionAssociateResourceWithProjectTask(BookableResource, projecttask);
                                }

                            }
                            else
                            {
                                Business.CallActionAssociateResourceWithProjectTask(foundMembers, projecttask);
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Não há usuário cadastrado com o e-mail informado.");
                        }
                    }
                    break; 
         }
      }
   }
}*/