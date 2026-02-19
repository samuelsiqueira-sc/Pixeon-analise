using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Documents;
using System.Workflow.Runtime.Tracking;

namespace Smart.ExtendedPSA.Extends.Workflows
{
    /// <summary>
    /// O objetivo deste workflow é retornar o e-mail que está contido num parametro previamente cadastrado, que neste caso tem o nome de "GET DEFAULT USER TO SEND EMAIL", buscar o recurso
    /// relacionado a este e-mail e então inseri-lo no "From" ao enviar um e-mail. 
    /// </summary>
    public class SendEmailToDefaultUser_ResourceDeparture : CodeActivity
    {
        /// <summary>
        /// Nome do parâmetro previamente cadastrado. 
        /// </summary>
        [Input("Nome do parâmetro")]
        public InArgument<String> SmartParameter { get; set; }

        /// <summary>
        /// Output, retorna o usuário do sistema.
        /// </summary>
        [Output("Default User E-mail")]
        [ReferenceTarget("systemuser")]
        public OutArgument<EntityReference> DefaultUserEmail { get; set; }

        /// <summary>
        /// Execute. 
        /// </summary>
        /// <param name="executionContext">Contexto de execução.</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            // Variaveis utilizadas. 
            SystemUser defaultUser = new SystemUser();
            string parameterName = SmartParameter.Get(executionContext);
            String defaultUserEmail = String.Empty;
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            using (OrganizationServiceContext svcContext = new OrganizationServiceContext(service))
            {
                defaultUserEmail = (from parameter in svcContext.CreateQuery<smt_smartparameter>()
                                    where parameter.smt_name == parameterName
                                    select parameter.smt_value).FirstOrDefault();

                if (defaultUserEmail == null)
                {
                    smt_log log = new smt_log();
                    log.smt_name = "Falha ao buscar parâmetro";
                    log.smt_st_entityname = $"Solicitação de Afastamento (smt_resource_departure)";
                    log.smt_pl_eventtype = new OptionSetValue(100000000);
                    log.smt_dt_eventdate = DateTime.Now;
                    log.smt_lp_executinguser = service.Retrieve(SystemUser.EntityLogicalName, context.UserId, new ColumnSet("systemuserid")).ToEntityReference();
                    log.smt_st_recordname = "Erro ao enviar e-mail";
                    log.smt_st_recordid = "custom wf";
                    log.smt_tx_message = "Não foi possível localizar um parâmetro que corresponda ao parâmetro especificado";
                    log.smt_st_eventorigin = "Origem do erro: WF customizado - SendEmailToDefaultUser_ResourceDeparture";
                    service.Create(log);
                }
                else
                {
                    defaultUser = (from user in svcContext.CreateQuery<SystemUser>()
                                   where user.InternalEMailAddress == defaultUserEmail
                                   select user).FirstOrDefault();

                    DefaultUserEmail.Set(executionContext, defaultUser.ToEntityReference());
                }
            }
        }
    }
}
