using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Workflow.Runtime.Tracking;
using System.Linq;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System.Collections.Generic;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Query;
using System.Windows.Documents;
using Microsoft.Crm.Sdk.Messages;
namespace Smart.ExtendedPSA.Extends.Workflows
{
    /// <summary>
    /// doc.
    /// </summary>
    public sealed class SendEmail_ResourceDeparture : CodeActivity
    {
        /// <summary>
        /// Unidade organizacional do usuário atual. 
        /// </summary>
        [Input("UnidadeOrganizacional")]
        [ReferenceTarget("msdyn_organizationalunit")]
        public InArgument<EntityReference> Smt_org_unity { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Input("Email")]
        [ReferenceTarget("email")]
        public InArgument<EntityReference> InEmail { get; set; }

        /// <summary>
        /// Afastamento do Recurso
        /// </summary>
        [Input("Solicitação de Afastamento")]
        [ReferenceTarget("smt_resource_departure")]
        public InArgument<EntityReference> ResourceDeparture { get; set; }

        /// <summary>
        /// Parâmetro Smart 
        /// </summary>
        [Input("Nome do parâmetro")]
        public InArgument<String> SmtParameter { get; set; }


        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="executionContext">ExecutionContext</param>
        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);
            var excContext = executionContext.GetExtension<IExecutionContext>();

            var unidadeOrganizacional = Smt_org_unity.Get(executionContext);
            var email = InEmail.Get(executionContext);
            var parameterName = SmtParameter.Get(executionContext);
            var departure = ResourceDeparture.Get(executionContext);

            var CheckSmartParameters = new smt_smartparameter();
            var CheckTeam = new List<SystemUser>();


            using (var orgContext = new OrganizationServiceContext(service))
            {

                CheckSmartParameters = (from parameter in orgContext.CreateQuery<smt_smartparameter>()
                                        where parameter.smt_name == parameterName
                                        && parameter.smt_organizationalunit == unidadeOrganizacional
                                        select parameter).FirstOrDefault();

                if (CheckSmartParameters == null)
                {
                    CheckSmartParameters = (from parameter in orgContext.CreateQuery<smt_smartparameter>()
                                            where parameter.smt_name == parameterName
                                            && parameter.smt_organizationalunit == null
                                            select parameter).FirstOrDefault();
                }

                if (CheckSmartParameters != null)
                {
                    CheckTeam = (from teams in orgContext.CreateQuery<Team>()
                                 join member in orgContext.CreateQuery<TeamMembership>() on teams.TeamId equals member.TeamId
                                 join users in orgContext.CreateQuery<SystemUser>() on member.SystemUserId equals users.SystemUserId
                                 where teams.Name == CheckSmartParameters.smt_value
                                 && users.IsDisabled == false
                                 select users).ToList();

                    if (CheckTeam.Count > 0)
                    {
                        var x = OpenTeam(CheckTeam);
                        Entity emailEnt = new Entity(email.LogicalName, email.Id);
                        emailEnt["to"] = x;

                        service.Update(emailEnt);

                        var request = new SendEmailRequest
                        {
                            EmailId = emailEnt.Id,
                            TrackingToken = string.Empty,
                            IssueSend = true
                        };

                        service.Execute(request);
                    }
                    else
                    {
                        var log = new smt_log();
                        log.smt_name = $"Falha para equipe";
                        log.smt_st_entityname = $"Solicitação de Afastamento (smt_resource_departure)";
                        log.smt_pl_eventtype = new OptionSetValue(100000000);
                        log.smt_dt_eventdate = DateTime.Now;
                        log.smt_lp_executinguser = service.Retrieve(SystemUser.EntityLogicalName, context.UserId, new ColumnSet(false)).ToEntityReference();
                        log.smt_st_recordname = "Enviar e-mail para a equipe";
                        log.smt_st_recordid = departure.Id.ToString();
                        log.smt_tx_message = $"Não foi possível enviar o e-mail: não tem membros associados a equipe {parameterName}";

                        service.Create(log);
                    }
                }
                else
                {
                    var log = new smt_log();
                    log.smt_name = $"Falha para: {parameterName}";
                    log.smt_st_entityname = $"Solicitação de Afastamento (smt_resource_departure)";
                    log.smt_pl_eventtype = new OptionSetValue(100000000);
                    log.smt_dt_eventdate = DateTime.Now;
                    log.smt_lp_executinguser = service.Retrieve(SystemUser.EntityLogicalName, context.UserId, new ColumnSet(false)).ToEntityReference();
                    log.smt_st_recordname = "Enviar e-mail para a equipe";
                    log.smt_st_recordid = departure.Id.ToString();
                    log.smt_tx_message = $"Não foi possível enviar o e-mail: equipe de {parameterName} não foi configurada na entidade de Parâmetros Smart";

                    service.Create(log);
                }
            }
        }
        /// <summary>
        /// Recebe os usuários do time em uma lista e insere todos os membros em uma ActivityParty. 
        /// </summary>
        /// <param name="teams">Lista dos membros do time</param>
        /// <returns>Return</returns>
        public EntityCollection OpenTeam(List<SystemUser> teams)
        {
            EntityCollection toFrom = new EntityCollection();
            foreach (var team in teams)
            {
                Entity ToParty = new Entity("activityparty");
                ToParty["partyid"] = new EntityReference(SystemUser.EntityLogicalName, team.SystemUserId.Value);
                toFrom.Entities.Add(ToParty);
            }
            return toFrom;
        }
    }
}
