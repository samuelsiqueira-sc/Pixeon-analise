using CRM.Smart.ExtendedPSA.Extends.Business;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Smart.ExtendedPSA.Extends.Models;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Smart.ExtendedPSA.Extends.Earlybound
{
    /// <summary>
    /// Operações do operationset
    /// </summary>
    public class OperationSetBO : BaseBusiness
    {
        /// <summary>
        /// dsada
        /// </summary>
        /// <param name="service">.</param>
        /// <param name="serviceAdmin">.</param>
        /// <param name="tracingService">.</param>
        /// <param name="messages">.</param>
        public OperationSetBO(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Calls the action to create an operationSet
        /// </summary>        
        /// <param name="projectId">project id for the operations to be included in this operationSet</param>
        /// <param name="description">description of this operationSet</param>
        /// <returns>operationSet id</returns>
        public string CallCreateOperationSetAction(Guid projectId, string description)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_CreateOperationSetV1");
            operationSetRequest["ProjectId"] = projectId.ToString();
            operationSetRequest["Description"] = description;
            OrganizationResponse response = Service.Execute(operationSetRequest);
            return response["OperationSetId"].ToString();
        }

        /// <summary>
        /// Calls the action to create an entity
        /// </summary>
        /// <param name="entity">Scheduling entity</param>
        /// <param name="operationSetId">operationSet id</param>
        /// <returns>OperationSetResponse</returns>

        public OperationSetResponse CallPssCreateAction(Entity entity, string operationSetId)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssCreateV1");
            operationSetRequest["Entity"] = entity;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(Service.Execute(operationSetRequest));
        }

        /// <summary>
        /// Calls the action to update an entity
        /// </summary>
        /// <param name="entity">Scheduling entity</param>
        /// <param name="operationSetId">operationSet Id</param>
        /// <returns>OperationSetResponse</returns>
        public OperationSetResponse CallPssUpdateAction(Entity entity, string operationSetId)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssUpdateV1");
            operationSetRequest["Entity"] = entity;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(Service.Execute(operationSetRequest));
        }

        /// <summary>
        /// Calls the action to update an entity
        /// </summary>
        /// <param name="recordId">Id of the record to be deleted</param>
        /// <param name="entityLogicalName">Entity logical name of the record</param>
        /// <param name="operationSetId">OperationSet Id</param>
        /// <returns>OperationSetResponse</returns>
        public OperationSetResponse CallPssDeleteAction(string recordId, string entityLogicalName, string operationSetId)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssDeleteV1");
            operationSetRequest["RecordId"] = recordId;
            operationSetRequest["EntityLogicalName"] = entityLogicalName;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(Service.Execute(operationSetRequest));
        }

        /// <summary> 
        /// Calls the action to update a Resource Assignment contour
        /// </summary> 
        /// <param name="resourceAssignmentId">Id of the resource assignment to be updated</param> 
        /// <param name="serializedUpdates">JSON formatted contour updates</param>
        /// <param name="operationSetId">operationSet id</param> 
        /// <returns>OperationSetResponse</returns> 
        public OperationSetResponse CallPssUpdateContourAction(string resourceAssignmentId, string serializedUpdates, string operationSetId)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_PssUpdateResourceAssignmentContourV1");
            operationSetRequest["ResourceAssignmentId"] = resourceAssignmentId;
            operationSetRequest["UpdatedContours"] = serializedUpdates;
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(Service.Execute(operationSetRequest));
        }

        /// <summary>
        /// Calls the action to execute requests in an operationSet
        /// </summary>
        /// <param name="operationSetId">operationSet id</param>
        /// <returns>OperationSetResponse</returns>
        public OperationSetResponse CallExecuteOperationSetAction(string operationSetId)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_ExecuteOperationSetV1");
            operationSetRequest["OperationSetId"] = operationSetId;
            return GetOperationSetResponseFromOrgResponse(Service.Execute(operationSetRequest));
        }

        /// <summary>
        /// This can be used to abandon an operationSet that is no longer needed
        /// </summary>
        /// <param name="operationSetId">operationSet id</param>
        /// <returns>OperationSetResponse</returns>
        public OperationSetResponse CallAbandonOperationSetAction(Guid operationSetId)
        {
            OrganizationRequest operationSetRequest = new OrganizationRequest("msdyn_AbandonOperationSetV1");
            operationSetRequest["OperationSetId"] = operationSetId.ToString();
            return GetOperationSetResponseFromOrgResponse(Service.Execute(operationSetRequest));
        }


        /// <summary>
        /// Calls the action to create a new project
        /// </summary>
        /// <param name="project">Project</param>
        /// <returns>project Id</returns>
        public Guid CallCreateProjectAction(Entity project)
        {
            OrganizationRequest createProjectRequest = new OrganizationRequest("msdyn_CreateProjectV1");
            createProjectRequest["Project"] = project;
            OrganizationResponse response = Service.Execute(createProjectRequest);
            var projectId = Guid.Parse((string)response["ProjectId"]);
            return projectId;
        }

        /// <summary>
        /// Calls the action to create a new project team member
        /// </summary>
        /// <param name="teamMember">Project team member</param>
        /// <returns>project team member Id</returns>
        public string CallCreateTeamMemberAction(Entity teamMember)
        {
            OrganizationRequest request = new OrganizationRequest("msdyn_CreateTeamMemberV1");
            request["TeamMember"] = teamMember;
            OrganizationResponse response = Service.Execute(request);
            return (string)response["TeamMemberId"];
        }

        /// <summary>
        /// Desserealiza a resposta do operation set
        /// </summary>
        /// <param name="orgResponse">OperationSet Response</param>
        /// <returns>Json</returns>
        public OperationSetResponse GetOperationSetResponseFromOrgResponse(OrganizationResponse orgResponse)
        {
            return JsonConvert.DeserializeObject<OperationSetResponse>((string)orgResponse.Results["OperationSetResponse"]);
        }

        /// <summary>
        /// Obtém os Bucket do projeyo
        /// </summary>
        /// <param name="projectReference">Projeto</param>
        /// <returns>Lista dos Buckets</returns>
        public EntityCollection GetDefaultBucket(EntityReference projectReference)
        {
            var columnsToFetch = new ColumnSet("msdyn_project", "msdyn_name");
            var getDefaultBucket = new QueryExpression("msdyn_projectbucket")
            {
                ColumnSet = columnsToFetch,
                Criteria =
        {
            Conditions =
            {
                new ConditionExpression("msdyn_project", ConditionOperator.Equal, projectReference.Id)
            }
        }
            };

            return Service.RetrieveMultiple(getDefaultBucket);
        }

        /// <summary>
        /// Cria um membro da equipe do projeto
        /// </summary>
        /// <param name="projectReference">Projeto</param>
        /// <returns>Membro da equipe do projeto</returns>
        public Entity GetTeamMember(EntityReference projectReference)
        {
            var teamMember = new Entity("msdyn_projectteam", Guid.NewGuid());
            teamMember["msdyn_name"] = $"TM {DateTime.Now.ToShortTimeString()}";
            teamMember["msdyn_project"] = projectReference;

            return teamMember;

        }

        /// <summary>
        /// Cria o bucket
        /// </summary>
        /// <param name="projectReference">Projeto</param>
        /// <returns>Bucket Padrão</returns>
        public EntityReference GetBucket(EntityReference projectReference)
        {
            var bucketCollection = GetDefaultBucket(projectReference);
            if (bucketCollection.Entities.Count > 0)
            {
                return bucketCollection[0].ToEntityReference();
            }
            else
            {
                return null;
            }

            throw new Exception($"Please open project with id {projectReference.Id} in the Dynamics UI and navigate to the Tasks tab");
        }

        /// <summary>
        /// Cria o projeto
        /// </summary>
        /// <returns>Projeto</returns>
        public Entity CreateProject()
        {
            var project = new Entity("msdyn_project", Guid.NewGuid());
            project["msdyn_subject"] = $"Proj {DateTime.Now.ToShortTimeString()}";

            return project;
        }

        /// <summary>
        /// Cria as tarefas do projeto
        /// </summary>
        /// <param name="name">Nome da Tarefa</param>
        /// <param name="projectReference">Projeto</param>
        /// <param name="parentReference">Tarefa Pai</param>
        /// <param name="bucketReference">Bucket do Projeto</param>
        /// <returns>Tarefa do Projeto</returns>
        public Entity GetTask(string name, EntityReference projectReference, EntityReference parentReference = null, EntityReference bucketReference = null)
        {
            var task = new Entity("msdyn_projecttask", Guid.NewGuid());
            task["msdyn_project"] = projectReference;
            task["msdyn_subject"] = name;
            task["msdyn_effort"] = 4d;
            task["msdyn_scheduledstart"] = DateTime.Today;
            task["msdyn_scheduledend"] = DateTime.Today.AddDays(5);
            task["msdyn_start"] = DateTime.Now.AddDays(1);
            task["msdyn_projectbucket"] = bucketReference;
            task["msdyn_LinkStatus"] = new OptionSetValue(192350000);
            task["smt_marcodaentrega"] = false;
            
            // msdyn_LinkStatus, msdyn_project, msdyn_projectbucket, msdyn_subject, smt_marcodaentrega.

            // Custom field handling
            /*
            task["new_custom1"] = "Just my test";
            task["new_age"] = 98;
            task["new_amount"] = 591.34m;
            task["new_isready"] = new OptionSetValue(100000000);
            */

            if (parentReference == null)
            {
                task["msdyn_outlinelevel"] = 1;
            }
            else
            {
                task["msdyn_parenttask"] = parentReference;
            }

            return task;
        }
        /// <summary>
        /// Cria a atribuição de recurso
        /// </summary>
        /// <param name="name">Nome</param>
        /// <param name="teamMember">Equipe do projeto</param>
        /// <param name="task">Tarefa</param>
        /// <param name="project">Projeto</param>
        /// <returns>Atribuição de recurso</returns>
        public Entity GetResourceAssignment(string name, Entity teamMember, Entity task, Entity project)
        {
            var assignment = new Entity("msdyn_resourceassignment", Guid.NewGuid());
            assignment["msdyn_projectteamid"] = teamMember.ToEntityReference();
            assignment["msdyn_taskid"] = task.ToEntityReference();
            assignment["msdyn_projectid"] = project.ToEntityReference();
            assignment["msdyn_name"] = name;

            return assignment;
        }

        /// <summary>
        /// Cria o registro de Dependência da Tarefa do Projeto
        /// </summary>
        /// <param name="project">Projeto</param>
        /// <param name="predecessor">Tarefa Predecessora</param>
        /// <param name="successor">Tarefa Sucessora</param>
        /// <returns>Dependência da Tarefa do Projeto</returns>
        public Entity GetTaskDependency(Entity project, Entity predecessor, Entity successor)
        {
            var taskDependency = new Entity("msdyn_projecttaskdependency", Guid.NewGuid());
            taskDependency["msdyn_project"] = project.ToEntityReference();
            taskDependency["msdyn_predecessortask"] = predecessor.ToEntityReference();
            taskDependency["msdyn_successortask"] = successor.ToEntityReference();
            taskDependency["msdyn_linktype"] = new OptionSetValue(192350000);

            return taskDependency;
        }
    }
}
