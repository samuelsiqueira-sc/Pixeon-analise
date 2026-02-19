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
using Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <summary>
    /// Plugins no Post update da entrada de horas que rodam de forma síncrona.
    /// </summary>
    public class PostUpdateSync_msdyn_timeentry : PluginBase
    {
        /// <summary>
        /// Construtor da classe
        /// </summary>
        public PostUpdateSync_msdyn_timeentry() : base(typeof(PreCreateSync_msdyn_timeentry)) { }
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

            #region Resx
            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            #endregion

            msdyn_timeentry target = localContext.GetTarget<msdyn_timeentry>();
            var timeentryBusiness = new Msdyn_timeentryBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);
            msdyn_timeentry preImg = localContext.GetPreImage<msdyn_timeentry>("preImage");
            MaxHoursWorkedUpdate(localContext, target, timeentryBusiness, preImg);

            RealStartAndFinish(localContext, target, timeentryBusiness);
        }

        /// <summary>
        /// Método para verificar se o usuário está tentando lançar mais horas do que o máximo permitido por dia, descrito no modelo de contrato dele.
        /// </summary>
        /// <param name="localContext">Contexto local.</param>
        /// <param name="timeEntry">Descrever.</param>
        /// <param name="business"> business de TimeEntry</param>
        /// <param name="preTimeEntry"> image de Entrada de horas </param>
        private void MaxHoursWorkedUpdate(LocalPluginContext localContext, msdyn_timeentry timeEntry, Msdyn_timeentryBusiness business, msdyn_timeentry preTimeEntry)
        {
            if (timeEntry.smt_lp_type_hours != null || timeEntry.msdyn_date != null || timeEntry.msdyn_duration != null)
            {
                CrmServiceContext context = new CrmServiceContext(localContext.OrganizationService);
                msdyn_timeentry time = new msdyn_timeentry();
                time.msdyn_timeentryId = preTimeEntry.Id;
                time.msdyn_type = timeEntry.msdyn_type != null ? timeEntry.msdyn_type : preTimeEntry.msdyn_type;
                time.smt_lp_type_hours = timeEntry.smt_lp_type_hours != null ? timeEntry.smt_lp_type_hours : preTimeEntry.smt_lp_type_hours;
                time.msdyn_date = timeEntry.msdyn_date != null ? timeEntry.msdyn_date : preTimeEntry.msdyn_date;
                time.msdyn_duration = timeEntry.msdyn_duration != null ? timeEntry.msdyn_duration : preTimeEntry.msdyn_duration;
                time.msdyn_bookableresource = localContext.OrganizationServiceAdmin.Retrieve(BookableResource.EntityLogicalName, preTimeEntry.msdyn_bookableresource.Id, new ColumnSet(BookableResource.Fields.Id, BookableResource.Fields.Name)).ToEntityReference();
                smt_model_contract workHours = business.GetCategoryWorkedHours(time.msdyn_bookableresource);
                business.CheckHours(workHours, time, context, preTimeEntry.msdyn_bookableresource);
            }
        }

        /// <summary>
        /// Atualiza o campo data de término e, se necessário data de início, em tarefa do projeto, após um lançamento de horas ser aprovado.
        /// </summary>
        /// <param name="localContext"> context </param>
        /// <param name="timeEntry"> entrada de horas do target</param>
        /// <param name="timeentryBusiness"> business de entrada de horas </param>
        private void RealStartAndFinish(LocalPluginContext localContext, msdyn_timeentry timeEntry, Msdyn_timeentryBusiness timeentryBusiness)
        {
            OptionSetValue tipoEntrada = new OptionSetValue();

            DateTime dataInicio;
            DateTime dataFinal;

            ColumnSet cs = new ColumnSet(new String[] { msdyn_timeentry.Fields.msdyn_projectTask, msdyn_timeentry.Fields.msdyn_type, msdyn_timeentry.Fields.msdyn_entryStatus });
            timeEntry = localContext.OrganizationServiceAdmin.Retrieve(msdyn_timeentry.EntityLogicalName, timeEntry.Id, cs).ToEntity<msdyn_timeentry>();

            if (timeEntry.msdyn_type != null && timeEntry.msdyn_projectTask != null && timeEntry.msdyn_entryStatusEnum != null)
            {
                // Se o tipo for trabalho e o status for aprovado.
                if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Work && timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Approved)
                {

                    try
                    {
                        Guid guidTarefaProjeto = ((EntityReference)timeEntry[msdyn_projecttask.EntityLogicalName]).Id;

                        msdyn_projecttask tarefaProjeto = new msdyn_projecttask();
                        ColumnSet projectCS = new ColumnSet(new String[] { msdyn_projecttask.Fields.smt_dt_real_start, msdyn_projecttask.Fields.smt_dt_real_end });
                        tarefaProjeto = localContext.OrganizationServiceAdmin.Retrieve(msdyn_projecttask.EntityLogicalName, guidTarefaProjeto, projectCS).ToEntity<msdyn_projecttask>();

                        msdyn_timeentry registerInitData = timeentryBusiness.DataDeInicio(guidTarefaProjeto);
                        msdyn_timeentry registerFinalData = timeentryBusiness.DataDeTermino(guidTarefaProjeto);

                        if (!tarefaProjeto.Contains("smt_dt_real_start") || tarefaProjeto.smt_dt_real_start == null)
                        {

                            // Verifica a primeira hora criada referente aquela tarefa do projeto
                            if (registerInitData.Contains("msdyn_date"))
                            {
                                dataInicio = (DateTime)registerInitData.msdyn_date;

                                // atribui o valor adquirido ao campo data real de inicio e data real de término
                                tarefaProjeto.smt_dt_real_start = dataInicio;

                            }

                        }
                        else if (registerInitData.Contains("msdyn_date"))
                        {
                            // Se a data da entrada de horas for menor do que a data de início da data de início real em tarefa do projeto, a data vai ser atualizada para a da entrada de horas.
                            if (registerInitData.msdyn_date.Value < tarefaProjeto.smt_dt_real_start.Value)
                                tarefaProjeto.smt_dt_real_start = registerInitData.msdyn_date;
                        }

                        // verifica a ultima hora criada referente a uma tarefa de projeto específica 
                        if (registerFinalData.Contains("msdyn_date"))
                        {
                            dataFinal = (DateTime)registerFinalData.msdyn_date.Value;

                            if (!tarefaProjeto.Contains("smt_dt_real_end") || tarefaProjeto.smt_dt_real_end < dataFinal)
                            {
                                // atribui o valor adquirido ao campo data real de inicio e data real de término
                                tarefaProjeto.smt_dt_real_end = dataFinal;
                            }

                            localContext.OrganizationServiceAdmin.Update(tarefaProjeto);
                        }
                    }
                    catch (Exception error)
                    {
                        throw new InvalidPluginExecutionException(error.Message);
                    }

                }
                else if (timeEntry.msdyn_typeEnum == msdyn_timeentrytype.Work && timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Returned)
                {
                    // Se for do tipo trabalho e o status for devolvido

                    // TODO: WESLEY - corrigido.
                    Guid guidTarefaProjeto = timeEntry.msdyn_projectTask.Id;

                    msdyn_projecttask tarefaProjeto = new msdyn_projecttask();
                    ColumnSet PorjectCs = new ColumnSet(new String[] { msdyn_projecttask.Fields.smt_dt_real_start, msdyn_projecttask.Fields.smt_dt_real_end });
                    tarefaProjeto = localContext.OrganizationServiceAdmin.Retrieve(msdyn_projecttask.EntityLogicalName, guidTarefaProjeto, PorjectCs).ToEntity<msdyn_projecttask>();
                    msdyn_timeentry registerInitData = timeentryBusiness.DataDeInicio(guidTarefaProjeto);
                    msdyn_timeentry registerFinalData = timeentryBusiness.DataDeTermino(guidTarefaProjeto);

                    if ((tarefaProjeto.Contains("smt_dt_real_start") || tarefaProjeto.smt_dt_real_start != null) && (tarefaProjeto.Contains("smt_dt_real_end") || tarefaProjeto.smt_dt_real_end != null))
                    {
                        // Verifica a primeira hora criada referente aquela tarefa do projeto
                        if (registerFinalData == null)
                        {
                            // atribui o valor adquirido ao campo data real de inicio e data real de término
                            tarefaProjeto.smt_dt_real_start = null;
                            tarefaProjeto.smt_dt_real_end = null;
                            localContext.OrganizationServiceAdmin.Update(tarefaProjeto);
                        }
                        else if (registerFinalData.Contains("msdyn_date"))
                        {
                            dataFinal = (DateTime)registerFinalData.msdyn_date;

                            // atribui o valor adquirido ao campo data real de inicio e data real de término
                            tarefaProjeto.smt_dt_real_start = registerInitData.msdyn_date;
                            tarefaProjeto.smt_dt_real_end = dataFinal;

                            localContext.OrganizationServiceAdmin.Update(tarefaProjeto);
                        }
                    }
                }
            }
        }

    }
}
