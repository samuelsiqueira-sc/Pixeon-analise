using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Projeto
{
    /// <summary>
    /// Herda a pluginbase.
    /// </summary>
    public class PreUpdateSync : PluginBase
    {
        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public PreUpdateSync() : base(typeof(PreUpdateSync)) { }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_project project = localcontext.GetTarget<msdyn_project>();
            msdyn_project projectImage = localcontext.GetPreImage<msdyn_project>();
            ProjetoBusiness projetoBusiness =
                new ProjetoBusiness(
                    localcontext.OrganizationService,
                    localcontext.OrganizationServiceAdmin,
                    localcontext.TracingService,
                    null);

            ValidateActualEndOnProjectStatusChange(project, projetoBusiness, projectImage);
            ChangeProjectManager(project, projetoBusiness, projectImage);
            ChangeTemporaryProjectManager(project, projetoBusiness, projectImage);
            ChangeOwnerId(project, projectImage);
        }


        /// <summary>
        /// Atribui o novo gerente do projeto a equipe proprietária
        /// </summary>
        /// <param name="target"> target de projeto </param>
        /// <param name="projetoBusiness"> business de projeto </param>
        /// <param name="preImage"> pre Image de projeto </param>
        public void ChangeProjectManager(msdyn_project target, ProjetoBusiness projetoBusiness, msdyn_project preImage)
        {
            if (target.msdyn_projectmanager != null)
            {
                projetoBusiness.ChangeProjectManager(target, preImage);
            }
        }

        /// <summary>
        /// Atribui o novo gerente temporário a equipe proprietária
        /// </summary>
        /// <param name="target"> target de projeto </param>
        /// <param name="projetoBusiness"> business de projeto </param>
        /// <param name="preImage"> pre image de projeto </param>
        public void ChangeTemporaryProjectManager(msdyn_project target, ProjetoBusiness projetoBusiness, msdyn_project preImage)
        {
            if (target.Contains("smt_lp_temporary_manager"))
                projetoBusiness.ChangeTemporaryProjectManager(target, preImage);
        }

        /// <summary>
        /// Classe que concede direitos de acesso para o gerente temporário.  
        /// </summary>
        /// <param name="project">Contexto de execução local.</param>
        /// <param name="projetoBusiness">Business da entidad e de projetos.</param>
        public void GrantAcess(msdyn_project project, ProjetoBusiness projetoBusiness)
        {
            Guid userId = projetoBusiness.RetrieveBookablersc(project.smt_lp_temporary_manager.Id).Id; // GUID do responsavel temporário. Recurso Reservavel. 
            Guid projectId = project.Id; // GUID do projeto. 
            msdyn_projectteam teamMembership = projetoBusiness.SearchProjectTeam(userId, projectId); // Verifica se o usuário possui um registro de Membro da Equipe do Projeto.
            List<msdyn_projecttask> projectTasks = projetoBusiness.RetrieveProjectTasks(projectId);

            // Caso o usuário já possua um registro de Membro da Equipe do Projeto, concede acesso ao registro de projeto para o usuário. 
            if (teamMembership != null)
            {
                projetoBusiness.GrantAcessRequest(project); // Concede acesso ao projeto para o responsável temporário. 
            }
            else
            {
                projetoBusiness.CreateTeamMembership(project, project.smt_lp_temporary_manager.Id); // Caso o usuário não possua registro de "Membro da equipe do Projeto", será criado um. 
                projetoBusiness.GrantAcessRequest(project); // Concede acesso ao projeto para o responsável temporário. 
                if (projectTasks != null)
                {
                    projetoBusiness.GrantAcessRequestOnTasks(project, projectTasks);
                }
            }
        }

        /// <summary>
        /// Método para revogação dos direitos de acesso anteriormente concedidos a um gerente temporário. 
        /// </summary>
        /// <param name="project">Contexto de execução local.</param>
        /// <param name="projetoBusiness">Business da entidade de projeto.</param>
        public void ModifyAccess(msdyn_project project, ProjetoBusiness projetoBusiness)
        {
            Guid userId = project.smt_lp_temporary_manager.Id; // Guid do usuário. 
            Guid projectId = project.Id;  // GUID do projeto. 
            BookableResource bkReesource = projetoBusiness.RetrieveBookablersc(userId);
            projetoBusiness.ModifyAccessRequest(project);

            List<msdyn_projecttask> projectTasks = projetoBusiness.RetrieveProjectTasks(projectId);
            if (projectTasks != null)
            {
                projetoBusiness.ModifyAccessRequestOnTasks(projectTasks, project);
            }

            msdyn_projectteam projectTeam = projetoBusiness.SearchProjectTeam(bkReesource.Id, projectId); // Busca pelo registro de "Membro da Equipe do Projeto"
            projetoBusiness.UpdateTeamMembership(projectTeam);

            // Revoga os direitos de acesso anteriormente concedidos a um determinado usuário. 
            // projetoBusiness.DeactivateTeamMembership(projectTeam); // Inativa o registro de "Membro da Equipe do Projeto" do usuário especificado.
        }

        /// <summary>
        /// Classe que concede direitos de acesso para o gerente temporário.  
        /// </summary>
        /// <param name="project">Contexto de execução local.</param>
        /// <param name="projetoBusiness">Business da entidad e de projetos.</param>
        public void ProjectMilestoneSum(msdyn_project project, ProjetoBusiness projetoBusiness)
        {
            decimal resultadoMarco = projetoBusiness.MarcoList(project.Id);

            if (resultadoMarco != 100) 
            {
                throw new InvalidPluginExecutionException("A soma dos marcos de reconhecimento não correspondem a 100%. Favor revisar o projeto.");
            }

        }

        /// <summary>
        /// Atribui o novo gerente temporário a equipe proprietária
        /// </summary>
        /// <param name="target"> target de projeto </param>
        /// <param name="preImage"> pre image de projeto </param>
        public void ChangeOwnerId(msdyn_project target, msdyn_project preImage)
        {
            if (target.OwnerId != null)
            {
                if (target.OwnerId.LogicalName == Team.EntityLogicalName && preImage.OwnerId.LogicalName == Team.EntityLogicalName && target.OwnerId.Id != preImage.OwnerId.Id)
                    throw new InvalidPluginExecutionException("O proprietário do Projeto não pode ser alterado. Caso queira alterar o gerente, por favor alterar o campo 'Gerente de Projeto'");

                if (target.OwnerId.LogicalName != Team.EntityLogicalName)
                    throw new InvalidPluginExecutionException("O proprietário do Projeto não pode ser alterado. Caso queira alterar o gerente, por favor alterar o campo 'Gerente de Projeto'");
            }
        }

        /// <summary>
        /// Valida o preenchimento do campo Término Real na alteração de status do projeto.
        /// </summary>
        /// <param name="target">Target do projeto.</param>
        /// <param name="projetoBusiness">Business da entidade Projeto.</param>
        /// <param name="preImage">PreImage do projeto.</param>
        public void ValidateActualEndOnProjectStatusChange(msdyn_project target,ProjetoBusiness projetoBusiness,msdyn_project preImage)
        {
            projetoBusiness.ValidateActualEndOnProjectStatusChange(target, preImage);
        }
    }
}