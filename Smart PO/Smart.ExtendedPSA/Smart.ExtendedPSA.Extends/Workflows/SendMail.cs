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

namespace Smart.ExtendedPSA.Extends.Workflows
{
    /// <summary>
    /// asddas
    /// </summary>
    public sealed class SendMail : CodeActivity
    {
        /// <summary>
        /// Unidade organizacional do usuário atual. 
        /// </summary>

        [Input("UnidadeOrganizacional")]
        [ReferenceTarget("smt_holiday_request")]
        public InArgument<EntityReference> Smt_org_unity { get; set; }

        /// <summary>
        /// Output com os e-mails de todos os membros de um determinado time. 
        /// </summary>
        [Output("EmailTo")]
        public OutArgument<string> Smt_EmailTo { get; set; }

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
            var CheckSmartParameters = new smt_smartparameter();
            var CheckTeam = new List<SystemUser>();


            using (var orgContext = new OrganizationServiceContext(service))
            {

                CheckSmartParameters = (from parameter in orgContext.CreateQuery<smt_smartparameter>()
                                        where parameter.smt_name == "GET TEAM PROJECT MANAGER FOR HOLIDAY REQUEST"
                                        && parameter.smt_organizationalunit == unidadeOrganizacional
                                        select parameter).FirstOrDefault();

                if (CheckSmartParameters == null)
                {
                    CheckSmartParameters = (from parameter in orgContext.CreateQuery<smt_smartparameter>()
                                            where parameter.smt_name == "GET TEAM PROJECT MANAGER FOR HOLIDAY REQUEST"
                                            && parameter.smt_organizationalunit == null
                                            select parameter).FirstOrDefault();


                    if (CheckSmartParameters == null)
                    {
                        CheckTeam = (from teams in orgContext.CreateQuery<Team>()
                                     join member in orgContext.CreateQuery<TeamMembership>() on teams.TeamId equals member.TeamId
                                     join users in orgContext.CreateQuery<SystemUser>() on member.SystemUserId equals users.SystemUserId
                                     where teams.Name == CheckSmartParameters.smt_value
                                     && users.IsDisabled == false
                                     select users).ToList();
                    }
                    else
                    {
                        // CRIAR UM REGISTRO NA ENTIDADE SMT_LOG OU SMT_ALERT
                        throw new InvalidPluginExecutionException("ERRO AO PROCURAR O PARAMETRO");
                    }
                }

               var x = OpenTeam(CheckTeam);
               Smt_EmailTo.Set(executionContext, x); 

            }
        }

        /// <summary>
        /// Recebe os usuários do time em uma lista e insere todos os membros em uma ActivityParty. 
        /// </summary>
        /// <param name="teams">Lista dos membros do time</param>
        /// <returns>afffffff</returns>
        public EntityCollection OpenTeam(List<SystemUser> teams)
        {

            Entity ToParty = new Entity("activityparty");
            EntityCollection toFrom = new EntityCollection();

            foreach (var team in teams)
            {
                ToParty["partyid"] = new EntityReference(SystemUser.EntityLogicalName, team.Id);
                toFrom.Entities.Add(ToParty); 

            }


            return toFrom;

        }
    }
}
