using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// Plugin que irá checar se o recurso está tentando lançar mais horas que o expediente dele
    /// </summary>
    public class PreCreateSync_msdyn_timeentry : PluginBase
    {
        /// <summary>
        /// Construtor da classe
        /// </summary>
        public PreCreateSync_msdyn_timeentry() : base(typeof(PreCreateSync_msdyn_timeentry)) { }
        /// <summary>
        /// Método que contém o código que vai checar se o recurso está tentando lançar mais horas que o expediente dele
        /// </summary>
        /// <param name="localContext">Variável que vai capturar o contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
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

            #region ResX
            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            msdyn_timeentry timeEntry = localContext.GetTarget<msdyn_timeentry>();
            Msdyn_timeentryBusiness Business = new Msdyn_timeentryBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);

            if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Absence && timeEntry.msdyn_description == null)
            {
                throw new InvalidPluginExecutionException("Para lançar uma entrada de hora de ausência é necessário informar a descrição.");
            }
            if (timeEntry.msdyn_projectTask != null && timeEntry.smt_lp_type_hours != null)
            {
                IfIsParentTask(timeEntry, Business);
                CheckProjectTaskPercentage(localContext, timeEntry, messages);
                MaxHoursWorked(localContext, timeEntry, messages, Business);
            }
            else if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Work)
            {
                setImportHours(localContext, timeEntry, messages, Business);
            }
            else if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Absence || timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Vacation) 
            {
                MaxHoursWorked(localContext, timeEntry, messages, Business);
            }
        }

        /// <summary>
        /// Valida o lançamento de horas de acordo com o modelo de contrato do recurso reservavel. 
        /// </summary>
        /// <param name="localContext">Contexto local.</param>
        /// <param name="timeEntry">Target, entrada de horas.</param>
        /// <param name="messages">Lista de mensagens RESX.</param>
        /// <param name="business"> business de entrada de horas</param>
        private void MaxHoursWorked(LocalPluginContext localContext, msdyn_timeentry timeEntry, List<Resx> messages, Msdyn_timeentryBusiness business)
        {
            if (localContext.PluginExecutionContext.PrimaryEntityName.ToLower() == msdyn_timeentry.EntityLogicalName)
            {
                if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Work || timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Vacation || timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Absence)
                {
                    CrmServiceContext context = new CrmServiceContext(localContext.OrganizationServiceAdmin);
                    smt_model_contract workHours = business.GetCategoryWorkedHours(timeEntry.msdyn_bookableresource);
                    business.CheckHours(workHours, timeEntry, context, timeEntry.msdyn_bookableresource);
                }
            }
        }

        /// <summary>
        /// Verificar se a entrada de horas, a tarefa do projeto relacionada tem 100%.
        /// </summary>
        /// <param name="localContext">Contexto do Plugin.</param>
        /// <param name="timeEntry">Target do Plugin.</param>
        /// <param name="messages">Lista de mensagens do RESX.</param>
        private void CheckProjectTaskPercentage(LocalPluginContext localContext, msdyn_timeentry timeEntry, List<Resx> messages)
        {
            if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Work)
            {
                var project_task = localContext.OrganizationServiceAdmin.Retrieve(msdyn_projecttask.EntityLogicalName, timeEntry.msdyn_projectTask.Id, new ColumnSet(msdyn_projecttask.Fields.smt_progressofisico)).ToEntity<msdyn_projecttask>();

                if (project_task != null)
                {
                    if (project_task.smt_progressofisico == 100)
                    {
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.TIMEENTRY1));
                    }
                }
            }
        }

        /// <summary>
        /// Valida o lançamento de horas de acordo com o modelo de contrato do recurso reservavel. 
        /// </summary>
        /// <param name="localContext">Contexto local.</param>
        /// <param name="timeEntry">Target, entrada de horas.</param>
        /// <param name="messages">Lista de mensagens RESX.</param>
        /// <param name="business"> business de entrada de horas</param>
        private void setImportHours(LocalPluginContext localContext, msdyn_timeentry timeEntry, List<Resx> messages, Msdyn_timeentryBusiness business)
        {
            var type = business.GetTypeHours();

            if (type != null)
            {
                timeEntry.smt_lp_type_hours = type.ToEntityReference();
                var projectName = localContext.OrganizationServiceAdmin.Retrieve(msdyn_project.EntityLogicalName, timeEntry.msdyn_project.Id, new ColumnSet("msdyn_subject")).ToEntity<msdyn_project>();

                if (timeEntry.msdyn_projectTask != null)
                {
                    var taskName = localContext.OrganizationServiceAdmin.Retrieve(msdyn_projecttask.EntityLogicalName, timeEntry.msdyn_projectTask.Id, new ColumnSet(msdyn_projecttask.Fields.msdyn_subject, msdyn_projecttask.Fields.smt_progressofisico)).ToEntity<msdyn_projecttask>();
                    
                    timeEntry.msdyn_description = taskName != null ? $"{taskName.msdyn_subject.ToString()}" : $"{projectName.msdyn_subject.ToString()}";
                    timeEntry.smt_dc_task_percentage = taskName.smt_progressofisico;
                }
                else
                {
                    timeEntry.msdyn_description = $"{projectName.msdyn_subject.ToString()}";
                }
            }
        }

        private void IfIsParentTask(msdyn_timeentry target, Msdyn_timeentryBusiness business)
        {
            business.IfProjectTaskIsParent(target);
        }
    }
}
