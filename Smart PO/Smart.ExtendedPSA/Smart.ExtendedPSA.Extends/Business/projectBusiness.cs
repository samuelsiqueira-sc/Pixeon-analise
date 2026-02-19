using CRM.Smart.ExtendedPSA.Extends.Business;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Business da entidade Projeto.
    /// </summary>
    public class ProjectBusiness : BaseBusiness
    {
        /// <summary>
        /// Método contrutor para receber alguns parametros precisos para a execução dos métodos da classe.
        /// </summary>
        /// <param name="service">Variavel de Serviço</param>
        /// <param name="serviceAdmin"> ServiceADM</param>
        /// <param name="tracingService">Atributo do tipo IorganizationService, utilizado para executar métodos do SDK do Dynamics CRM</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        public ProjectBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Classe utilizada para executar as regras de negócios da Action.
        /// </summary>
        /// <param name="userId">Id do user que executou a action</param>
        /// <param name="project">Referência ao projéto relacionado ao usuario</param>
        /// <param name="resxes">Mensagens resx para serem usadas em exceptions</param>
        public void ExecuteAction(Guid userId, EntityReference project, List<Resx> resxes)
        {
            ColumnSet columnSet = new ColumnSet("msdyn_bulkgenerationstatus");
            Entity shProj = Service.Retrieve("msdyn_project", project.Id, columnSet);

            int status;
            if (shProj.Contains("msdyn_bulkgenerationstatus"))
            {
                status = ((OptionSetValue)shProj["msdyn_bulkgenerationstatus"]).Value;
                if (status == 192350000)
                    throw new InvalidPluginExecutionException(resxes.GetMessageById(ResxExtension.PRJBLCR));
            }

            UpdateStatusProject(192350000, project.Id);
            Guid idBaseLine;
            var attributemap = GetMapAttributes("smt_msdyn_project_smt_baseline_projectid");
            Entity projeto = ReturnProjetcFields(project.Id);

            var newBaseline = new Entity("smt_baseline");

            foreach (var attribute in attributemap)
            {
                if (attribute.TargetAttributeName.ToLower().Equals("smt_projectid"))
                    continue;

                newBaseline[attribute.TargetAttributeName] = projeto.Contains(attribute.SourceAttributeName) ? projeto[attribute.SourceAttributeName] : null;
            }
            try
            {
                newBaseline["smt_projectid"] = new EntityReference("msdyn_project", project.Id);
                DateTime dtnow = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
                newBaseline["smt_name"] = projeto["msdyn_subject"] + " " + dtnow.ToString();
                idBaseLine = Service.Create(newBaseline.ToEntity<Entity>());
            }
            catch (Exception ex)
            {
                CreateLog(ex.Message, "Erro ao criar BaseLine", userId, " ", project.Id);
                UpdateStatusProject(1, project.Id);
                throw new InvalidPluginExecutionException(ex.Message);
            }

            var attributemapProjectTask = GetMapAttributes("smt_msdyn_projecttask_smt_tarefadabaseline_lp_projecttask");
            var enTarefa = ObterBy(projeto.Id);
            var tarefasBaseline = new List<Entity>();

            foreach (var tarefa in enTarefa)
            {
                var newBaselineTask = new Entity("smt_tarefadabaseline");
                newBaselineTask["smt_baselineid"] = new EntityReference("smt_baseline", idBaseLine);
                newBaselineTask["smt_name"] = tarefa.Attributes["msdyn_subject"];
                newBaselineTask["smt_lp_projecttask"] = tarefa.ToEntityReference();

                foreach (var attribute in tarefa.Attributes)
                {
                    var keyAttribute = attributemapProjectTask.FirstOrDefault(x => x.SourceAttributeName == attribute.Key);

                    if (keyAttribute == null) continue;

                    var targetField = keyAttribute.TargetAttributeName;

                    if (!string.IsNullOrEmpty(targetField) && (targetField == "smt_lp_projecttask" || targetField == "smt_lp_baseline"))
                        continue;

                    if (!string.IsNullOrEmpty(targetField) && targetField.ToLower().Equals("ctm_tarefaprincipalid"))
                    {
                        newBaselineTask[targetField] = new EntityReference("msdyn_projecttask", new Guid(attribute.Value.ToString()));
                        continue;
                    }

                    if (!string.IsNullOrEmpty(targetField))
                        newBaselineTask[targetField] = attribute.Value;
                }
                tarefasBaseline.Add(newBaselineTask);
            }

            try
            {
                foreach (var Tarefa in tarefasBaseline)
                    Tarefa.Id = Service.Create(Tarefa);
            }
            catch (Exception ex)
            {
                CreateLog(ex.Message, "Erro ao criar Tarefas BaseLine", userId, " ", project.Id);
                UpdateStatusProject(1, project.Id);
                throw new InvalidPluginExecutionException(resxes.GetMessageById(ResxExtension.PRJBLER));
            }
            UpdateStatusProject(1, project.Id);
        }

        /// <summary>
        /// Cria um ColumnSet com os campos passados no array de string, caso o array esteja vazio, retorna um ColumnSet(true), que pega todos o campos
        /// </summary>
        /// <param name="columns">Atributo que armazena as Colunas.</param>
        /// <returns>Retorna um CollumSet para utilizar no Retrieve.</returns>
        protected ColumnSet GetColumnSet(params string[] columns)
        {
         return (columns == null || columns.Length == 0) ? new ColumnSet("msdyn_subject") : new ColumnSet(columns);
        }

        /// <summary>
        /// Método utilizado para retornar
        /// </summary>
        /// <param name="idProjeto"> Atributo que contém o Id do projeto</param>
        /// <param name="columns">Atributo para receber as colunas</param>
        /// <returns>Retorna uma lista de registros</returns>
        public List<Entity> ObterBy(Guid idProjeto)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                return context.CreateQuery<msdyn_projecttask>().Where(t => t.msdyn_project.Id == idProjeto && t.StateCode == msdyn_projecttaskState.Active).ToList<Entity>();
            }

        }

        /// <summary>
        /// Método utilizado para executar uma query passada por parâmetro.
        /// </summary>
        /// <param name="query"> Quert em XML para ser executada.</param>
        /// <returns>Lista de registro,</returns>
        public List<Entity> RetrieveMultiple(QueryExpression query)
        {
            if (!query.TopCount.HasValue || query.TopCount.Value > 5000)
            {
                query.PageInfo = new PagingInfo
                {
                    Count = 5000,
                    PageNumber = 1
                };
            }

            var list = new List<Entity>();

            do
            {
                var result = Service.RetrieveMultiple(query);

                foreach (var entity in result.Entities)
                {
                    list.Add(entity.ToEntity<Entity>());
                }

                if (!result.MoreRecords) break;

                query.PageInfo.PagingCookie = result.PagingCookie;
                query.PageInfo.PageNumber++;
            }
            while (true);

            return list;
        }

        /// <summary>
        /// Método utilizado para buscar campos que serão utilizados para o processo de criação da baseline.
        /// </summary>
        /// <param name="idProject">Parâmetro utilizado para passar o Id do projeto.</param>
        /// <returns> Retornará uma objeto do tipo Entity</returns>
        public Entity ReturnProjetcFields(Guid idProject)
        {
            return Service.Retrieve("msdyn_project", idProject, new ColumnSet("msdyn_effort", "msdyn_plannedlaborcost", "smt_margemprojeto", "msdyn_finish", "msdyn_scheduledstart", "transactioncurrencyid", "msdyn_subject", "ownerid"));
        }

        /// <summary>
        /// Método utilizado para retornar o relacionamento entre as entidades
        /// </summary>
        /// <param name="relationShipName">Parâmetro que receberá o nome do mapeamento feito.</param>
        /// <returns>Retorno de um Enum</returns>
        protected IEnumerable<AttributeMap> GetMapAttributes(string relationShipName)
        {
            var relationShipRequest = new RetrieveRelationshipRequest()
            {
                Name = relationShipName
            };

            var relationShip = (RetrieveRelationshipResponse)Service.Execute(relationShipRequest);
            var relationMeta = (OneToManyRelationshipMetadata)relationShip.Results.FirstOrDefault().Value;

            var queryEntityMap = new QueryExpression("entitymap")
            {
                NoLock = true,
                ColumnSet = new ColumnSet("entitymapid")
            };
            queryEntityMap.Criteria.AddCondition("targetentityname", ConditionOperator.Equal, relationMeta.ReferencingEntity);
            queryEntityMap.Criteria.AddCondition("sourceentityname", ConditionOperator.Equal, relationMeta.ReferencedEntity);

            var link = queryEntityMap.AddLink("attributemap", "entitymapid", "entitymapid");
            link.LinkCriteria.AddCondition("targetattributename", ConditionOperator.Equal, relationMeta.ReferencingAttribute);

            var entityMapId = Service.RetrieveMultiple(queryEntityMap).Entities.FirstOrDefault().Id;

            var queryAttributeMap = new QueryExpression("attributemap")
            {
                NoLock = true,
                ColumnSet = new ColumnSet("sourceattributename", "targetattributename")
            };
            queryAttributeMap.Criteria.AddCondition("entitymapid", ConditionOperator.Equal, entityMapId.ToString());
            queryAttributeMap.Criteria.AddCondition("parentattributemapid", ConditionOperator.Null);
            var attributesMap = Service.RetrieveMultiple(queryAttributeMap);

            return attributesMap.Entities.Select(item => item.ToEntity<AttributeMap>());
        }

        /// <summary>
        /// Método utilizado para gerar log no Dynamics CRM em caso de erros.
        /// </summary>
        /// <param name="exception">String com o conteúdo da exception</param>
        /// <param name="title"> Título do registro</param>
        /// <param name="userId">Id do usuário que gerou a exception</param>
        /// <param name="recordName">Nome do registro que ocorreu o erro</param>
        /// <param name="idProject">Id do projeto que deu erro.</param>
        public void CreateLog(string exception, string title, Guid userId, string recordName, Guid idProject)
        {
            Entity log = new Entity();
            log["smt_name"] = title;
            log["smt_dt_eventdate"] = DateTime.Now;
            log["smt_lp_executinguser"] = new EntityReference("systemuser", userId);
            log["smt_st_entityname"] = "msdyn_project";
            log["smt_tx_message"] = exception;
            log["smt_pl_eventtype"] = "OnClick";
            log["smt_st_recordname"] = recordName;
            log["smt_st_eventorigin"] = "BOtão Criar BaseLine";
            log["smt_st_recordid"] = idProject.ToString();
            Service.Create(log);
        }
        /// <summary>
        /// Método utilizado para atualizar o status do projeto.
        /// </summary>
        /// <param name="status">Paramêtro utilizado para realizar a alteração do status</param>
        /// <param name="idProject">Guid do projeto utilizado para efetuar o update no registro.</param>
        public void UpdateStatusProject(int status, Guid idProject)
        {
            Entity updProject = new Entity("msdyn_project");
            updProject.Id = idProject;
            updProject["msdyn_bulkgenerationstatus"] = status == 192350000 ? new OptionSetValue(192350000) : null;
            Service.Update(updProject);
        }

        /// <summary>
        /// Cria uma entity collection vazia
        /// </summary>
        /// <param name="entries">Conjunto de registros</param>
        /// <returns>Entity Collection Vazia</returns>
        public EntityCollection TimeEntriesRecall(EntityCollection entries)
        {

            return new EntityCollection();
        }

        /// <summary>
        /// Método utilizado para criar um Service Impersonate.
        /// </summary>
        /// <param name="userId">Id do usuário que irá ser utilizado para criar o impersonate </param>
        /// <param name="factory"> IOrganizationServiceFactory para realizar o Impersonate</param>
        /// <returns>Retorno é um novo Iorganization Service</returns>
        public IOrganizationService GetService(Guid userId, IOrganizationServiceFactory factory)
        {
            return (IOrganizationService)factory.CreateOrganizationService(userId);
        }

        /// <summary>
        /// Criar Logs
        /// </summary>
        /// <param name="eventType"> tipo de Evento </param>
        /// <param name="recordid">id do registro </param>
        /// <param name="message"> mensagem do Throw </param>
        /// <param name="name"> nome </param>
        /// <param name="userId"> id do Usuário </param>
        public void CreateLogs(OptionSetValue eventType, String recordid, String message, String name, Guid userId)
        {
            smt_log log = new smt_log();
            log.smt_name = name;
            log.smt_st_entityname = msdyn_timeentry.EntityLogicalName;
            log.smt_pl_eventtype = eventType;
            log.smt_st_eventorigin = "s";
            log.smt_dt_eventdate = DateTime.Now;
            log.smt_lp_executinguser = new EntityReference("systemuser", userId);
            log.smt_st_recordname = " Teste ";
            log.smt_st_recordid = recordid;
            log.smt_tx_message = message;
            Service.Create(log);
        }

        /// <summary>
        /// Buscar um registro de recurso reservável em que o campo "usuário" seja igual ao "Gerente de Projetos" do projeto que está sendo criado / atualizado.
        /// </summary>
        /// <param name="userId">GUID do usuário.</param>
        /// <returns>Retorno é um novo Iorganization Service</returns>
        public BookableResource RetrieveBookableResource(Guid userId)
        {
            try
            {
                using (CrmServiceContext crmContext = new CrmServiceContext(Service))
                {
                    return crmContext.BookableResourceSet.Where(user => user.UserId.Id == userId).Select(user => user).FirstOrDefault();
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
            return null;
        }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="bookResourceId">doc.</param>
        /// <returns>doc.</returns>
        public BookableResourceCategoryAssn RetrieveCategoryAssign(Guid bookResourceId)
        {
            try
            {
                using (CrmServiceContext crmContext = new CrmServiceContext(Service))
                {
                    return crmContext.BookableResourceCategoryAssn.Where(user => user.Resource.Id == bookResourceId).Select(user => user).FirstOrDefault();
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
            return null;
        }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="resourceId">doc</param>
        /// <param name="projectId">doc</param>
        /// <returns>doc.</returns>
        public msdyn_projectteam RetrieveManagerTeam(Guid resourceId, Guid projectId)
        {
            try
            {
                using (CrmServiceContext crmContext = new CrmServiceContext(Service))
                {
                    return crmContext.msdyn_projectteam.Where(user => user.msdyn_bookableresourceid.Id == resourceId && user.msdyn_project.Id == projectId).Select(user => user).FirstOrDefault();
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
            return null;
        }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="managerTeam">doc</param>
        /// <param name="managerCategoryAssn">doc</param>
        public void SetNewFunction(msdyn_projectteam managerTeam, BookableResourceCategoryAssn managerCategoryAssn)
        {
            msdyn_projectteam manager = new msdyn_projectteam();
            manager.Id = managerTeam.Id;
            manager.msdyn_resourcecategory = managerCategoryAssn.ResourceCategory;
            manager.EntityState = EntityState.Changed;
            Update(manager);
        }

        /// <summary>
        /// Método que busca todas as tarefas da baseline relacionadas com a baseline preenchida no campo "Baseline de Referência".
        /// </summary>
        /// <param name="baselineGuid">.</param>
        /// <returns>.</returns>
        public List<smt_tarefadabaseline> RetrieveBaselineTask(Guid baselineGuid)
        {
            List<smt_tarefadabaseline> baselineTask = new List<smt_tarefadabaseline>();

            using (CrmServiceContext localContext = new CrmServiceContext(Service))
            {
                baselineTask = (from baselineTasks in localContext.CreateQuery<smt_tarefadabaseline>()
                                where baselineTasks.smt_BaselineId.Id == baselineGuid
                                select new smt_tarefadabaseline
                                {
                                    Id = baselineTasks.Id,
                                }).ToList();
            }

            return baselineTask;
        }

        /// <summary>
        /// Método que percorre a lista de "Tarefas da Baseline" e seta no campo "Tarefa da Baseline" na entidade "Tarefa do Projeto".
        /// </summary>
        /// <param name="baselineTask">.</param>
        public void SetBaselineTask(List<smt_tarefadabaseline> baselineTask)
        {
            foreach (smt_tarefadabaseline task in baselineTask)
            {
                msdyn_projecttask tarefa = FindProjectTask(task.smt_lp_projecttask.Id);
                tarefa.smt_lp_baseline_task = task.ToEntityReference();
                tarefa.EntityState = EntityState.Changed;
                Update(tarefa);
            }
        }

        /// <summary>
        /// Método que busca todas as "Tarefas do projeto".
        /// </summary>
        /// <param name="tarefaGuid">.</param>
        /// <returns>.</returns>
        public msdyn_projecttask FindProjectTask(Guid tarefaGuid)
        {
            using (CrmServiceContext localContext = new CrmServiceContext(Service))
            {
                return localContext.msdyn_projecttaskSet.Where(task => task.Id == tarefaGuid).FirstOrDefault();
            }        
        }
    }
}
