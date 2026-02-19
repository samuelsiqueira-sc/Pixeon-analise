using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Pixeon.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Business
{
    /// <summary>
    /// Classe padrão para e entidade "Projeto".  
    /// </summary>
    public class ProjetoBusiness : BaseBusiness
    {
        /// <summary>
        /// Método contrutor para receber alguns parametros precisos para a execução dos métodos da classe.
        /// </summary>
        /// <param name="service">Variavel de Serviço</param>
        /// <param name="serviceAdmin"> ServiceADM</param>
        /// <param name="tracingService">Atributo do tipo IorganizationService, utilizado para executar métodos do SDK do Dynamics CRM</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        public ProjetoBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Executa a requisição que atribui privilégios ao gerente secundário do projeto.
        /// </summary>
        /// <param name="project">Referência do projeto.</param>
        public void GrantAcessRequest(msdyn_project project)
        {
            try
            {
                var accessRequest = new GrantAccessRequest
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.AppendAccess | AccessRights.AppendToAccess | AccessRights.AssignAccess | AccessRights.CreateAccess | AccessRights.ReadAccess |
                        AccessRights.ShareAccess | AccessRights.WriteAccess,
                        Principal = project.smt_lp_temporary_manager
                    },
                    Target = project.ToEntityReference()
                };
                ServiceAdmin.Execute(accessRequest);
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException pluginException)
            {
                throw pluginException;
            }
            catch (Exception genericException)
            {
                throw genericException;
            }
        }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="projectGuid">GUID do projeto.</param>
        /// <returns>Lista de tarefas do projeto relacionadas ao projeto.</returns>
        public List<msdyn_projecttask> RetrieveProjectTasks(Guid projectGuid)
        {
            try
            {
                using (CrmServiceContext smartContext = new CrmServiceContext(ServiceAdmin))
                {
                    return smartContext.projecttaskSet.Where(tasks => tasks.msdyn_project.Id == projectGuid).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Executa a requisição que atribui privilégios ao gerente secundário do projeto.
        /// </summary>
        /// <param name="project">Referência do projeto.</param>
        /// <param name="projectTasks">Lista de tarefas do projeto relacionada ao projeto.</param>
        public void GrantAcessRequestOnTasks(msdyn_project project, List<msdyn_projecttask> projectTasks)
        {

            foreach (msdyn_projecttask task in projectTasks)
            {
                try
                {
                    var accessRequest = new GrantAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.AppendAccess | AccessRights.AppendToAccess | AccessRights.AssignAccess | AccessRights.CreateAccess | AccessRights.ReadAccess |
                            AccessRights.ShareAccess | AccessRights.WriteAccess,
                            Principal = project.smt_lp_temporary_manager
                        },
                        Target = task.ToEntityReference()
                    };
                    ServiceAdmin.Execute(accessRequest);

                }
                catch (ArgumentNullException argumentNullException)
                {
                    throw argumentNullException;
                }
                catch (InvalidPluginExecutionException pluginException)
                {
                    throw pluginException;
                }
                catch (Exception genericException)
                {
                    throw genericException;
                }
            }
        }

        /// <summary>
        /// Chamada da action de revogação de direitos de acesso do responsável temporário. 
        /// </summary>
        /// <param name="projectTasks">Doc.</param>
        /// <param name="project">Doc.</param>
        public void ModifyAccessRequestOnTasks(List<msdyn_projecttask> projectTasks, msdyn_project project)
        {

            foreach (msdyn_projecttask task in projectTasks)
            {
                try
                {
                    ModifyAccessRequest ModifyRequest = new ModifyAccessRequest
                    {
                        PrincipalAccess = new PrincipalAccess
                        {
                            AccessMask = AccessRights.ReadAccess,
                            Principal = project.smt_lp_temporary_manager
                        },
                        Target = task.ToEntityReference()
                    };
                    ServiceAdmin.Execute(ModifyRequest);
                }
                catch (ArgumentNullException argumentNullException)
                {
                    throw argumentNullException;
                }
                catch (InvalidPluginExecutionException pluginException)
                {
                    throw pluginException;
                }
                catch (Exception genericException)
                {
                    throw genericException;
                }
            }

        }



        /// <summary>
        /// Chamada da action de revogação de direitos de acesso do responsável temporário. 
        /// </summary>
        /// <param name="project">Doc.</param>
        public void ModifyAccessRequest(msdyn_project project)
        {
            try
            {
                ModifyAccessRequest ModifyRequest = new ModifyAccessRequest
                {
                    PrincipalAccess = new PrincipalAccess
                    {
                        AccessMask = AccessRights.ReadAccess,
                        Principal = project.smt_lp_temporary_manager
                    },
                    Target = project.ToEntityReference()
                };
                ServiceAdmin.Execute(ModifyRequest);
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException pluginException)
            {
                throw pluginException;
            }
            catch (Exception genericException)
            {
                throw genericException;
            }
        }

        /// <summary>
        /// Busca um usuário através do GUID.
        /// </summary>
        /// <param name="userGuid">GUID do usuário.</param>
        /// <returns>Usuário que possua o GUID passado como parâmetro.</returns>
        public SystemUser RetrieveUser(Guid userGuid)
        {
            try
            {
                using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
                {
                    return serviceContext.SystemUser.Where(user => user.Id == userGuid)
                        .FirstOrDefault();
                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException pluginException)
            {
                throw pluginException;
            }
            catch (Exception genericException)
            {
                throw genericException;
            }
            return null;
        }

        /// <summary>
        /// Retornadoc. 
        /// </summary>
        /// <param name="userId">Guid do usuário.</param>
        /// <param name="projectId">Guid do projeto.</param>
        /// <returns>Doc.</returns>
        public msdyn_projectteam SearchProjectTeam(Guid userId, Guid projectId)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.ProjectTeam.Where(member => member.msdyn_bookableresourceid.Id == userId && member.msdyn_project.Id == projectId)
                    .FirstOrDefault();

                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                throw invalidPluginExecutionException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Caso o recurso não seja membro da equipe do projeto, essa associação será criada.
        /// </summary>
        /// <param name="project">Projeto</param>
        /// <param name="userId"> id do usuário </param>
        public void CreateTeamMembership(msdyn_project project, Guid userId)
        {
            // Guid userId = project.smt_lp_temporary_manager.Id; // Guid do usuário. 
            string userEmail = RetrieveUser(userId).InternalEMailAddress; // E-mail do recurso. 
            BookableResource resource = RetrieveBookablersc(userId); // Retrieve no Recurso Reservavel do usuário. 

            if (resource != null)
            {
                BookableResourceCategoryAssn userCategory = RetrieveAssgnCategory(resource.Id); // Categoria do recurso.

                if (userCategory != null)
                {
                    msdyn_projectteam teamMembership = new msdyn_projectteam()
                    {
                        msdyn_project = project.ToEntityReference(),
                        smt_str_email_resource = userEmail,
                        msdyn_resourcecategory = userCategory.ResourceCategory,
                        msdyn_bookableresourceid = resource.ToEntityReference(),
                        msdyn_ProjectApprover = true,
                    };
                    ServiceAdmin.Create(teamMembership);
                }
                else { throw new InvalidPluginExecutionException("O usuário não possui uma Associação de Categoria de Recurso Reservavel."); }
            }
            else
            {
                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.Hi));  // "O usuário não possui uma Associação Recurso Reservavel."
            }
        }

        /// <summary>
        /// Inativa o registro de membro da equipe do projeto. 
        /// </summary>
        /// <param name="projectTeam">Equipe do projeto.</param>
        public void UpdateTeamMembership(msdyn_projectteam projectTeam)
        {
            try
            {
                msdyn_projectteam prjTeam = new msdyn_projectteam
                {
                    Id = projectTeam.Id,
                    msdyn_ProjectApprover = false,
                    EntityState = EntityState.Changed,
                };
                Update(prjTeam);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// Busca, na entidade "Associação de Categoria de Recurso Reservável" a categoria do recurso que será usada para preencher o campo "Função" ao inseri-lo na equipe do projeto. 
        /// </summary>
        /// <param name="userId">Para realizar a busca, utiliza-se o guid do usuário.</param>
        /// <returns>Referência da categoria do recurso.</returns>
        public BookableResourceCategoryAssn RetrieveAssgnCategory(Guid userId)
        {
            BookableResourceCategoryAssn bkResource = new BookableResourceCategoryAssn();

            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                bkResource = (from category in crmService.CategoryAssgn
                              where category.Resource.Id == userId
                              && category.msdyn_IsDefault == true
                              select category).FirstOrDefault();
            }
            return bkResource;
        }

        /// <summary>
        /// Busca por um Recurso Reservavel através do GUID passado como parâmetro. 
        /// </summary>
        /// <param name="userGuid">GUID do SystemUser.</param>
        /// <returns>Retorna o GUID do Recurso Reservavel que será utilizado na criação de membro da equipe do projeto.</returns>
        public BookableResource RetrieveBookablersc(Guid userGuid)
        {
            try
            {
                using (CrmServiceContext smartContext = new CrmServiceContext(ServiceAdmin))
                {
                    return smartContext.BookableResourceSet.Where(resource => resource.UserId.Id == userGuid).FirstOrDefault();
                }
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Caso o usuário possua um registro de Membro da Equioe do projeto com StateCode inativo, reativa o registro. 
        /// </summary>
        /// <param name="membership">Registro de Membro da Equipe do Projeto.</param>
        public void ActivateMembership(msdyn_projectteam membership)
        {
            msdyn_projectteam projectteam = new msdyn_projectteam
            {
                Id = membership.Id,
                StateCode = msdyn_projectteamState.Active,
                EntityState = EntityState.Changed,
                msdyn_ProjectApprover = true,
            };
            Update(projectteam);
        }

        /// <summary>
        /// Retorna as informações do contrato do projeto
        /// </summary>
        /// <param name="project">Dados do contrato do projeto</param>
        /// <returns>Projeto</returns>
        public SalesOrder GetSalesOrder(msdyn_project project)
        {
            return ServiceAdmin.Retrieve(SalesOrder.EntityLogicalName, project.msdyn_salesorderid.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("smt_pl_type_rr", "smt_dt_access")).ToEntity<SalesOrder>();
        }

        /// <summary>
        /// Atualiza os produtos do projeto
        /// </summary>
        /// <param name="salesOrder">SaleOrder</param>
        public void GetProductsSalesOrder(SalesOrder salesOrder)
        {
            using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
            {
                List<Guid> products = (from sales in crmContext.CreateQuery<SalesOrderDetail>()
                                       where sales.ProductTypeCode.Value == 1
                                       select sales.Id).ToList();

                foreach (Guid product in products)
                {
                    SalesOrderDetail updateProduct = new SalesOrderDetail
                    {
                        Id = product,
                        msdyn_BillingStatus = new OptionSetValue(192350004),
                    };
                    ServiceAdmin.Update(updateProduct);
                }

                SalesOrder updateSales = new SalesOrder
                {
                    Id = salesOrder.Id,
                    smt_dt_access = DateTime.Now,
                };
                Update(updateSales);
            }
        }


        /// <summary>
        /// Tarefas do projeto relacionadas ao projeto
        /// </summary>
        /// <param name="projectGuid">projectGuid</param>
        /// <returns>retorna lita das tarefas de projeto relacionadas ao projeto</returns>    
        private List<msdyn_projecttask> taskList(Guid projectGuid)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.projecttaskSet.Where(tasks => tasks.msdyn_project.Id == projectGuid && tasks.smt_lp_milestone != null).ToList();
                }
            }
            catch (ArgumentNullException argumentNull)
            {
                throw argumentNull;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Tetetetete
        /// </summary>
        /// <param name="projectGuid">tetetete</param> 
        /// <returns>tetetetete</returns>
        public decimal MarcoList(Guid projectGuid)
        {
            List<msdyn_projecttask> tarefas = taskList(projectGuid);
            Guid marcoAtual = Guid.Empty;
            decimal result = 0;
            smt_milestone_project marcoRetornado = null;

            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                foreach (msdyn_projecttask tarefa in tarefas)
                {
                    marcoAtual = tarefa.smt_lp_milestone.Id;

                    marcoRetornado = context.milestone_projectSet.Where(marco => marco.smt_milestone_projectId == marcoAtual).FirstOrDefault();

                    result += Convert.ToDecimal(marcoRetornado.smt_dc_stage);
                }
            }
            return result;
        }

        /// <summary>
        /// Método principal da criação da equipe proprietária do projeto
        /// </summary>
        /// <param name="project"> projeto do contexto </param>
        public void CreateTeam(msdyn_project project)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                msdyn_project projeto = new msdyn_project();
                projeto.Id = project.Id;

                Team team = new Team();
                BusinessUnit bu = new BusinessUnit();

                SystemUser projectManager = context.CreateQuery<SystemUser>().Where(u => u.Id == project.msdyn_projectmanager.Id).FirstOrDefault();
                msdyn_organizationalunit unit = context.CreateQuery<msdyn_organizationalunit>().Where(u => u.Id == project.msdyn_ContractOrganizationalUnitId.Id).FirstOrDefault();

                if (unit.smt_bt_channel == false)
                {
                    team = context.CreateQuery<Team>().Where(u => u.Id == unit.smt_equipedaunidade.Id).FirstOrDefault();
                    bu = context.CreateQuery<BusinessUnit>().Where(b => b.Id == team.BusinessUnitId.Id).FirstOrDefault();
                }
                else if (unit.smt_bt_channel == true)
                {
                    team = context.CreateQuery<Team>().Where(u => u.smt_st_teamdomain == unit.smt_st_domain).FirstOrDefault();
                    bu = context.CreateQuery<BusinessUnit>().Where(b => b.Id == team.BusinessUnitId.Id).FirstOrDefault();
                }

                if (project.msdyn_salesorderid != null)
                {
                    SalesOrder salesOrder = context.CreateQuery<SalesOrder>().Where(s => s.Id == project.msdyn_salesorderid.Id).FirstOrDefault();

                    projeto.smt_Programa = salesOrder.smt_program;
                    projeto.smt_st_additional_comments = salesOrder.smt_st_comments_addicional;
                    projeto.smt_dt_project_creation_date = salesOrder.smt_dt_project_creation_date;
                    projeto.smt_dc_executed_sf = salesOrder.smt_dc_executed_sf;
                    projeto.smt_bl_migration = salesOrder.smt_bt_migration;
                    if (salesOrder.smt_pl_deployment_status != null)
                    {
                        String deploymentLabel = GetOptionsSetLabelByValue("salesorder", "smt_pl_deployment_status", salesOrder.smt_pl_deployment_status.Value);
                        int? deployment = GetOptionSetValueByLabel("msdyn_project", "smt_pl_deployment_status", deploymentLabel);
                        if (deployment != null)
                            projeto.smt_pl_deployment_status = new OptionSetValue(deployment.Value);
                    }
                }
                // msdyn_organizationalunit unit = RetrieveAsAdmin(msdyn_organizationalunit.EntityLogicalName, project.msdyn_ContractOrganizationalUnitId.Id, new ColumnSet(true)).ToEntity<msdyn_organizationalunit>();
                String name = $"{bu.Name}: {project.msdyn_subject}";
                Team createdTeam = CreateOwnerTeam(name, bu);

                if (!unit.smt_bt_channel.Value)
                {
                    AppendRoleToTeam(createdTeam, "Gerentes e Analistas");
                }
                else if (unit.smt_bt_channel.Value)
                {
                    AppendRoleToTeam(createdTeam, "Canais");
                }

                projeto.OwnerId = new EntityReference(Team.EntityLogicalName, createdTeam.Id);
                ServiceAdmin.Update(projeto);
                AddUsersTeam(createdTeam.Id, project.msdyn_projectmanager.Id); // associa o gerente do projeto no time.

                if (projectManager.ParentSystemUserId != null) // Associa o time do projeto para o gerente vertical.
                {
                    AddUsersTeam(createdTeam.Id, projectManager.ParentSystemUserId.Id);
                }
            }
        }

        /// <summary>
        /// Atribui o direito de acesso de "Gerentes e Analistas de Projetos" para a equipe proprietário do projeto.
        /// </summary>
        /// <param name="team"> equipe </param>
        /// <param name="rolename"> nome do direito de acesso que será atribuido a equipe</param>
        public void AppendRoleToTeam(Team team, string rolename)
        {
            using (CrmServiceContext crmService = new CrmServiceContext(Service))
            {
                var role = (from roleset in crmService.RoleSet
                            where roleset.Name == rolename
                            && roleset.BusinessUnitId.Id == team.BusinessUnitId.Id
                            select roleset).FirstOrDefault();

                EntityReferenceCollection roles = new EntityReferenceCollection();
                if (role != null)
                {
                    roles.Add(role.ToEntityReference());
                }

                ServiceAdmin.Associate(Team.EntityLogicalName, team.Id, new Relationship("teamroles_association"), roles);
            }
        }

        /// <summary>
        /// Cria a equipe proprietária do projeto
        /// </summary>
        /// <param name="name"> nome da equipe </param>
        /// <param name="bu"> bu do gerente </param>
        /// <param name="manager"> gerente de projetos </param>
        public Team CreateOwnerTeam(String name, BusinessUnit bu)
        {
            Team team = new Team();
            team.Name = name;
            team.BusinessUnitId = new EntityReference(bu.LogicalName, bu.Id);
            team.TeamType = new OptionSetValue(0);
            Guid idTeam = ServiceAdmin.Create(team);
            Team createdTeam = RetrieveAsAdmin(Team.EntityLogicalName, idTeam, new ColumnSet(true)).ToEntity<Team>();

            return createdTeam;
        }

        /// <summary>
        /// Retrieve como Admin
        /// </summary>
        /// <param name="entityName"> nome lógico da entidade </param>
        /// <param name="idRecord"> id do Registro </param>
        /// <param name="columnSet"> campos desejados </param>
        /// <returns> Registro encontrado </returns>
        public Entity RetrieveAsAdmin(String entityName, Guid idRecord, ColumnSet columnSet)
        {
            Entity result = new Entity();
            try
            {
                result = ServiceAdmin.Retrieve(entityName, idRecord, columnSet);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Erro ao tentar buscar uma {entityName} com o id {idRecord}. Mensagem da exceção {ex.Message}");
            }
            return result;
        }

        /// <summary>
        /// Adicionar memrbo a equipe do projeto
        /// </summary>
        /// <param name="team"> equipe </param>
        /// <param name="user"> usuário </param>
        public void AddUsersTeam(Guid team, Guid user)
        {
            using (var crmService = new CrmServiceContext(Service))
            {
                try
                {
                    AddMembersTeamRequest addRequest = new AddMembersTeamRequest();
                    {
                        Guid[] members = new[] { user };
                        addRequest.TeamId = team;
                        addRequest.MemberIds = members;
                        ServiceAdmin.Execute(addRequest);
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }

        /// <summary>
        /// remove um usuário da equipe
        /// </summary>
        /// <param name="team"> id da equipe </param>
        /// <param name="user">id do usuário </param>
        public void RemoveUsersTeam(Guid team, Guid user)
        {
            using (var crmService = new CrmServiceContext(Service))
            {
                try
                {
                    RemoveMembersTeamRequest addRequest = new RemoveMembersTeamRequest();
                    {
                        Guid[] members = new[] { user };
                        addRequest.TeamId = team;
                        addRequest.MemberIds = members;
                        ServiceAdmin.Execute(addRequest);
                    };
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Atribui o novo gerente a equipe proprietária do projeto e remove o antigo gerente da mesma.
        /// </summary>
        /// <param name="target"> target do projeto </param>
        /// <param name="preImage"> preImage do projeto </param>
        public void ChangeProjectManager(msdyn_project target, msdyn_project preImage)
        {
            var manager = target.msdyn_projectmanager != null ? target.msdyn_projectmanager : preImage.msdyn_projectmanager;
            msdyn_project project = RetrieveAsAdmin(msdyn_project.EntityLogicalName, preImage.Id, new ColumnSet("ownerid")).ToEntity<msdyn_project>();
            // Verifica se o proprietário é uma equipe
            if (project.OwnerId.LogicalName == "team")
            {
                AddUsersTeam(project.OwnerId.Id, manager.Id);
                RemoveUsersTeam(project.OwnerId.Id, preImage.msdyn_projectmanager.Id);
            }
        }

        /// <summary>
        /// Atribui o novo gerente temporário a equipe proprietária do projeto e romove o antigo, se houver, da mesma.
        /// </summary>
        /// <param name="target"> target do projeto </param>
        /// <param name="preImage"> pre Image do projeto </param>
        public void ChangeTemporaryProjectManager(msdyn_project target, msdyn_project preImage)
        {
            msdyn_project project = RetrieveAsAdmin(msdyn_project.EntityLogicalName, preImage.Id, new ColumnSet("ownerid")).ToEntity<msdyn_project>();

            if (target.smt_lp_temporary_manager != null)
            {
                if (project.OwnerId.LogicalName == "team")
                    AddUsersTeam(project.OwnerId.Id, target.smt_lp_temporary_manager.Id);

                var resource = returnResource(target);

                if (resource != null)
                {
                    var function = returnFunction();

                    if (function != null)
                    {
                        var exist = SearchMember(resource, project);

                        if (exist == null)
                        {
                            msdyn_projectteam projectTeam = new msdyn_projectteam();
                            projectTeam.msdyn_name = "Gerente de Projetos";
                            projectTeam.msdyn_project = preImage.ToEntityReference();
                            projectTeam.msdyn_resourcecategory = function.ToEntityReference();
                            projectTeam.msdyn_ProjectApprover = true;
                            projectTeam.msdyn_bookableresourceid = resource.ToEntityReference();
                            projectTeam.msdyn_organizationalunit = resource.msdyn_organizationalunit;

                            ServiceAdmin.Create(projectTeam);
                        }
                        else
                        {
                            msdyn_projectteam projectTeam2 = new msdyn_projectteam();
                            projectTeam2.Id = exist.Id;
                            projectTeam2.msdyn_ProjectApprover = true;

                            ServiceAdmin.Update(projectTeam2);

                        }
                    }
                }
            }

            if (preImage.smt_lp_temporary_manager != null)
            {
                if (project.OwnerId.LogicalName == "team")
                    RemoveUsersTeam(project.OwnerId.Id, preImage.smt_lp_temporary_manager.Id);
                var resource = returnResource(preImage);
                if (resource != null)
                {
                    var exist = SearchMember(resource, project);

                    if (exist != null)
                    {
                        msdyn_projectteam projectTeam2 = new msdyn_projectteam();
                        projectTeam2.Id = exist.Id;
                        projectTeam2.msdyn_ProjectApprover = false;

                        ServiceAdmin.Update(projectTeam2);
                    }
                }

            }
        }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <param name="project">Entidade de Projeto</param>
        /// <param name="salesOrder">Contrato do projeto.</param>
        public void SetProjectName(msdyn_project project, SalesOrder salesOrder)
        {
            if (salesOrder.msdyn_ContractOrganizationalUnitId.Id != project.msdyn_ContractOrganizationalUnitId.Id)
                throw new InvalidPluginExecutionException("A unidade de contratação do projeto não pode ser diferente da unidade de contratação do contrato.");

            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                 
                string opNumber = salesOrder.smt_st_opp_number; // Número da OP
                string clientContact = salesOrder.Contains("customerid") ? salesOrder.CustomerId.Name : string.Empty; // Contato Administrativo do Cliente  
                string family = salesOrder.Contains("smt_smt_pl_family") ? salesOrder.FormattedValues["smt_smt_pl_family"] : string.Empty; // Familia
                string focusedProduct = salesOrder.Contains("smt_pl_focused_product") ? salesOrder.FormattedValues["smt_pl_focused_product"] : string.Empty;  // Produto Concentrado
                string productItens = salesOrder.Contains("smt_pl_items") ? salesOrder.FormattedValues["smt_pl_items"] : string.Empty; // Produto/Itens
                string nome;

                if (opNumber == null && clientContact == null && family == null && focusedProduct == null && productItens == null)
                {
                    nome = salesOrder.Name; // Caso todos os campos que compõe a regra estejam vazios, o nome do projeto
                                            // será o nome do contrato. 
                }
                else
                {
                    // nome = $"{opNumber}-{clientContact}-{family}-{focusedProduct}-{productItens}"; Do contrário, o nome do projeto é a concatenação     // dos campos Número da OP, Contato Administrativo do Cliente , Família, Produto Concentrado, Produto/Item.
                    nome = salesOrder.Name; // nova solicitação - nome do projeto seja igual ao nome da notificação
                }
                // Busca algum projeto com o mesmo nome
                msdyn_project projectWithSameName = context.CreateQuery<msdyn_project>().Where(p => p.msdyn_subject == nome).FirstOrDefault();

                if (projectWithSameName == null)
                {
                    project.msdyn_subject = nome;
                    project.msdyn_description = salesOrder.Name;
                }
                else
                    throw new InvalidPluginExecutionException("Não foi possível criar o projeto porque já existe um projeto com esse nome. Por favor, verifique se já há algum projeto para esse contrato, ou se há outro contrato com o mesmo nome.");
            }

        }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <param name="project">Entidade alvo.</param>
        /// <returns>Retorna a cotação da qual o projeto foi originado.</returns>
        public SalesOrder RetrieveProjectContract(msdyn_project project)
        {
            try
            {
                using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
                {
                    return crmContext.SalesOrderSet.Where(contract => contract.Id == project.msdyn_salesorderid.Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// retorna o recurso
        /// </summary>
        /// <param name="target">retorna o recurso</param>
        /// <returns>retorna o recurso</returns>
        public BookableResource returnResource(msdyn_project target)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var resource = (from a in crmService.CreateQuery<BookableResource>()
                                where a.UserId.Id == target.smt_lp_temporary_manager.Id
                                select a).FirstOrDefault();

                return resource;
            }
        }

        /// <summary>
        /// retorna o recurso
        /// </summary>
        /// <param name="target">retorna o recurso</param>
        /// <returns>retorna o recurso</returns>
        public BookableResourceCategory returnFunction()
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var category = (from a in crmService.CreateQuery<BookableResourceCategory>()
                                where a.Name == "Gerente de Projeto" || a.Name == "Gerente de Projetos"
                                select a).FirstOrDefault();

                return category;
            }
        }

        /// <summary>
        /// retorna o recurso
        /// </summary>
        /// <param name="member">retorna o recurso</param>
        /// <param name="project">projeto</param>
        /// <returns>retorna o recurso</returns>
        public msdyn_projectteam SearchMember(BookableResource member, msdyn_project project)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var exist = (from a in crmService.CreateQuery<msdyn_projectteam>()
                             where a.msdyn_bookableresourceid == member.ToEntityReference()
                             && a.msdyn_project == project.ToEntityReference()
                             select a).FirstOrDefault();

                return exist;
            }
        }
        /// <summary>
        /// teste
        /// </summary>
        /// <param name="project">teste</param>
        /// <param name="salesOrder">teste</param>
        /// <param name="project1">project1</param>
        public void CreateSalesLine(msdyn_project project, SalesOrder salesOrder, msdyn_project project1)
        {
            ServiceAdmin.Update(project1);

            var salesLine = new SalesOrderDetail();
            salesLine.IsProductOverridden = true;
            salesLine.ProductDescription = salesOrder.Name;
            salesLine.SalesOrderId = project.msdyn_salesorderid;
            salesLine.ProductTypeCode = new Microsoft.Xrm.Sdk.OptionSetValue(5);
            salesLine.msdyn_BillingMethod = new Microsoft.Xrm.Sdk.OptionSetValue(192350001);
            salesLine.msdyn_Project = project.ToEntityReference();
            salesLine.PricePerUnit = salesOrder.smt_mn_value_eventual_final;

            ServiceAdmin.Create(salesLine);
        }

        public void DeleteOwnerTeam(msdyn_project project)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                Team ownerTeam = context.TeamSet.Where(t => t.Id == project.OwnerId.Id).FirstOrDefault();

                ServiceAdmin.Delete(ownerTeam.LogicalName, ownerTeam.Id);
            }
        }
        private string GetOptionsSetLabelByValue(string entityName, string attributeName, int selectedValue)
        {

            RetrieveAttributeRequest retrieveAttributeRequest = new
            RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };
            // Execute the request.
            RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)ServiceAdmin.Execute(retrieveAttributeRequest);
            // Access the retrieved attribute.
            PicklistAttributeMetadata retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)
            retrieveAttributeResponse.AttributeMetadata;// Get the current options list for the retrieved attribute.
            OptionMetadata[] optionList = retrievedPicklistAttributeMetadata.OptionSet.Options.ToArray();
            string selectedOptionLabel = null;
            foreach (OptionMetadata oMD in optionList)
            {
                if (oMD.Value == selectedValue)
                {
                    selectedOptionLabel = oMD.Label.LocalizedLabels[0].Label.ToString();
                    break;
                }
            }
            return selectedOptionLabel;
        }

        public int? GetOptionSetValueByLabel(string entityName, string attributeName, string label)
        {
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };

            try
            {
                var attributeResponse = (RetrieveAttributeResponse)ServiceAdmin.Execute(attributeRequest);
                var attributeMetadata = (EnumAttributeMetadata)attributeResponse.AttributeMetadata;

                var optionLabels = (from options in attributeMetadata.OptionSet.Options
                                    select new { options.Value, Text = options.Label.UserLocalizedLabel.Label }).ToList();

                return optionLabels.Where(a => a.Text == label).Select(a => a.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string name = $"Erro ao executar retrieve no atributo {attributeName} da entidade {entityName}";

                // CreateLog(name, entityName, $"{attributeName}", $"{recordId}", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// retorna o recurso
        /// </summary>
        /// <param name="target">retorna o recurso</param>
        public void CreateDocumentSharePoint(SharePointDocumentLocation target)
        {
            try
            {
                using (var crmService = new CrmServiceContext(ServiceAdmin))
                {
                    var project = (from s in crmService.CreateQuery<msdyn_project>()
                                   where s.msdyn_projectId == target.RegardingObjectId.Id
                                   select new msdyn_project
                                   {
                                       smt_bl_projectInternal = s.smt_bl_projectInternal,
                                       msdyn_istemplate = s.msdyn_istemplate,
                                       msdyn_customer = s.msdyn_customer
                                   }).FirstOrDefault();

                    if (project.smt_bl_projectInternal == false && project.msdyn_istemplate == false)
                    {
                        var parameter = GetDocumentDefault();

                        if (parameter != null)
                        {
                            Guid parameterValue = new Guid(parameter.smt_value);

                            var document = (from s in crmService.CreateQuery<SharePointDocumentLocation>()
                                            where s.RegardingObjectId == project.msdyn_customer
                                            && s.ParentSiteOrLocation.Id == parameterValue
                                            select s).OrderByDescending(a => a.CreatedOn).FirstOrDefault();

                            if (document != null)
                            {
                                target.ParentSiteOrLocation = document.ToEntityReference();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// retorna o recurso
        /// </summary>
        /// <returns>retorna o recurso</returns>
        public smt_smartparameter GetDocumentDefault()
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var parameter = (from p in crmService.CreateQuery<smt_smartparameter>()
                                 where p.smt_name == "CREATE SHAREPOINT DOCUMENT"
                                 select p).FirstOrDefault();

                return parameter;
            }
        }
        /// <summary>
        /// Valida o gerente de projeto
        /// </summary>
        /// /// <param name="project">projectteam</param>
        /// <param name="projecteamImage"> image</param>
        public void ValidateManager(msdyn_projectteam project, msdyn_projectteam projecteamImage)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var manager = (from a in crmService.CreateQuery<msdyn_project>()
                               where a.msdyn_projectId == projecteamImage.msdyn_project.Id
                               select a.msdyn_projectmanager).FirstOrDefault();

                var user = (from b in crmService.CreateQuery<BookableResource>()
                            where b.BookableResourceId.Value == projecteamImage.msdyn_bookableresourceid.Id
                            select b.UserId).FirstOrDefault();
                if (user != null)
                {
                    if (manager.Id == user.Id)
                    {
                        throw new InvalidPluginExecutionException($"Não é possível alterar o campo de aprovador, pois o recurso {manager.Name} e  {user.Name} é gerente do projeto.");
                    }
                }
            }
        }

        /// <summary>
        /// Valida o gerente de projeto
        /// </summary>
        /// /// <param name="task">projectteam</param>
        public void UpdateTarefa(msdyn_projecttask task)
        {
            ServiceAdmin.Update(task);
        }

        /// <summary>
        /// Valida o gerente de projeto
        /// </summary>
        /// /// <param name="documentation">projectteam</param>
        public void ValidateDocumentAccount(SharePointDocumentLocation documentation)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var parameter = (from a in crmService.CreateQuery<smt_smartparameter>()
                               where a.smt_name == "PrincipalFolder"
                               select a).FirstOrDefault();

                if (parameter != null)
                {
                    var locations = (from a in crmService.CreateQuery<SharePointDocumentLocation>()
                                     where a.ParentSiteOrLocation.Id == new Guid(parameter.smt_value)
                                     && a.RegardingObjectId == documentation.RegardingObjectId
                                     select a).ToList();

                    if (locations != null && locations.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("Já existe uma pasta local para esse cliente criado. Por favor, validar a informação.");
                    }
                    else
                    {
                        if (parameter.smt_value.ToString() != documentation.ParentSiteOrLocation.Id.ToString())
                        {
                            documentation.ParentSiteOrLocation = new EntityReference(SharePointDocumentLocation.EntityLogicalName, new Guid(parameter.smt_value));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Valida o preenchimento do campo Término Real quando a Razão do Status do projeto
        /// for alterada para Concluído ou Cancelado.
        /// </summary>
        /// <param name="target">Entidade alvo do projeto.</param>
        /// <param name="preImage">PreImage do projeto.</param>
        public void ValidateActualEndOnProjectStatusChange(msdyn_project target, msdyn_project preImage)
        {
            // Verifica se está tentando desativar O projeto
            bool isDeactivating = target.Contains("statecode") &&
                                  target.StateCode != null &&
                                  target.StateCode.Value == msdyn_projectState.Inactive;

            if (!isDeactivating)
                return;

            // Pega o StatusCode - pode vir no target ou na preImage
            OptionSetValue status = null;

            if (target.Contains("statuscode") && target.StatusCode != null)
                status = target.StatusCode;
            else if (preImage != null && preImage.StatusCode != null)
                status = preImage.StatusCode;

            if (status == null)
                return;

            bool isConcluido = status.Value == 180580004;
            bool isCancelado = status.Value == 192350000;

            if (!isConcluido && !isCancelado)
                return;

            var actualEnd = target.Contains("msdyn_actualend") && target.msdyn_actualend != null
                            ? target.msdyn_actualend
                            : preImage?.msdyn_actualend;

            if (!actualEnd.HasValue)
            {
                throw new InvalidPluginExecutionException(
                    "Para concluir ou cancelar o projeto, é obrigatório preencher o campo 'Término Real'."
                );
            }
        }
    }
}
