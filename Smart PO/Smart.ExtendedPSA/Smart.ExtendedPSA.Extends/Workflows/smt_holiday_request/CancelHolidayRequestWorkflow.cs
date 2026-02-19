using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Workflow.Runtime.Tracking;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Workflows
{
    /// <summary>
    /// Cancela a Solicitação de férias
    /// </summary>
    public class CancelHolidayRequestWorkflow : CodeActivity
    {
        /// <summary>
        /// Solicitação de férias
        /// </summary>
        [RequiredArgument]
        [Input("Solicitação de Férias")]
        [ReferenceTarget(smt_holiday_request.EntityLogicalName)]
        public InOutArgument<EntityReference> HolidayRequestReference { get; set; }
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

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is smt_cancel_holiday_request)
            {
                smt_holiday_request holidayRequest = (smt_holiday_request)service.Retrieve(smt_holiday_request.EntityLogicalName, HolidayRequestReference.Get(executionContext).Id, new ColumnSet(smt_holiday_request.Fields.smt_lp_resource));
                if (holidayRequest != null && holidayRequest.smt_lp_resource != null)
                {
                    BookableResource resource = (BookableResource)service.Retrieve(BookableResource.EntityLogicalName, holidayRequest.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit));
                    
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}