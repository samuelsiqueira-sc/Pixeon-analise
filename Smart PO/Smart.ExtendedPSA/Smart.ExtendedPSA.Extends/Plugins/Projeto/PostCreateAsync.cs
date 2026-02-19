using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Plugins.Projeto
{
    /// <summary>
    /// doc.
    /// </summary>
    public class PostCreateAsync : PluginBase
    {
        /// <summary>
        /// doc
        /// </summary>
        public PostCreateAsync() : base(typeof(PostCreateAsync)) { }

        /// <summary>
        /// doc
        /// </summary>
        /// <param name="localcontext">doc.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Declaração de variáveis. 
            msdyn_project entity = localcontext.GetTarget<msdyn_project>();
            ProjectBusiness business = new ProjectBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos. 
            FunctionChange(entity, business);            
        }

        /// <summary>
        /// Cria tarefas no projeto especificado.
        /// </summary>
        /// <param name="project">Projeto</param>
        /// <param name="service">Service</param>
        /// <param name="serviceAdmin">ServiceAdmin</param>
        /// <param name="tracingService">Trace</param>
        public static void InsertTasks(Entity project, IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService)
        {
            OperationSetBO scheduleAPI = new OperationSetBO(service, serviceAdmin, tracingService, null);
            EntityReference projectReference = new EntityReference("msdyn_project", project.Id);

            var teamMember = scheduleAPI.GetTeamMember(projectReference);
            var createTeamMemberResponse = scheduleAPI.CallCreateTeamMemberAction(teamMember);
            var description = $"My demo {DateTime.Now.ToString("t", new CultureInfo("pt-BR"))}";
            var operationSetId = scheduleAPI.CallCreateOperationSetAction(project.Id, description);

            // var bucket = scheduleAPI.GetBucket(projectReference);
            
            var bucket = new Entity("msdyn_projectbucket", Guid.NewGuid());
            bucket["msdyn_project"] = projectReference;
            bucket["msdyn_name"] = "Bucket via Plugin";
            var bucketResponse = scheduleAPI.CallPssCreateAction(bucket, operationSetId);
            
            
            var bucketReference = bucket.ToEntityReference();
            var task1 = scheduleAPI.GetTask("Task 1", projectReference, null, bucketReference);
            var task2 = scheduleAPI.GetTask("Task 1.1", projectReference, task1.ToEntityReference(), bucketReference);
            var task3 = scheduleAPI.GetTask("Task 2", projectReference, null, bucketReference);

            // var assignment1 = scheduleAPI.GetResourceAssignment("R1", teamMember, task2, project);
            // var assignment2 = scheduleAPI.GetResourceAssignment("R2", teamMember, task3, project);
            var task1Response = scheduleAPI.CallPssCreateAction(task1, operationSetId);
            var task2Response = scheduleAPI.CallPssCreateAction(task2, operationSetId);
            var task3Response = scheduleAPI.CallPssCreateAction(task3, operationSetId);
            /*
            var assignment1Response = scheduleAPI.CallPssCreateAction(assignment1, operationSetId);
            var assignment2Response = scheduleAPI.CallPssCreateAction(assignment2, operationSetId);
            var task2Update = new Entity(task2.LogicalName, task2.Id);

            task2Update["msdyn_subject"] = "Updated Task";
            task2Update["msdyn_scheduledstart"] = DateTime.Today.AddDays(1);
            task2Update["msdyn_scheduledend"] = DateTime.Today.AddDays(3);

            var task2UpdateResponse = scheduleAPI.CallPssUpdateAction(task2Update, operationSetId);
            var dependency1 = scheduleAPI.GetTaskDependency(project, task2, task3);
            var dependency1Response = scheduleAPI.CallPssCreateAction(dependency1, operationSetId);
            var deleteDependencyResponse = scheduleAPI.CallPssDeleteAction(dependency1.Id.ToString(), dependency1.LogicalName, operationSetId);
            */
            Console.WriteLine("Calling ExecuteOperationSet...");
            scheduleAPI.CallExecuteOperationSetAction(operationSetId);
            Console.WriteLine("Operation Set Executed");

        }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="entity">doc.</param>
        /// <param name="business">doc.</param>
        /// <param name="localcontext">doc.</param>
        public void FunctionChange(msdyn_project entity, ProjectBusiness business)
        {
            if (entity.Contains("msdyn_projectmanager"))
            {
                Guid managerId = entity.msdyn_projectmanager.Id; // ID do usuário contido no campo "Gerente de Projeto". 
                BookableResource managerResource = business.RetrieveBookableResource(managerId);

                if (managerResource != null)
                {
                    Guid bookableResourceId = managerResource.Id;   // ID do recurso reservavel do usuário do sistema.
                    BookableResourceCategoryAssn managerCategoryAssn = business.RetrieveCategoryAssign(bookableResourceId);

                    if (managerCategoryAssn != null)
                    {
                        msdyn_projectteam managerTeam = business.RetrieveManagerTeam(managerResource.Id, entity.Id);

                        if (managerTeam != null)
                        {
                            business.SetNewFunction(managerTeam, managerCategoryAssn);
                        }
                    }
                }
            }
        }
    }
}
