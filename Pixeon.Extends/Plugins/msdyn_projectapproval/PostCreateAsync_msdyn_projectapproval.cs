using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins.PostCreateAsync_msdyn_projectapproval
{
    public class PostCreateAsync_msdyn_projectapproval : PluginBase
    {
        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public PostCreateAsync_msdyn_projectapproval() : base(typeof(PostCreateAsync_msdyn_projectapproval)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis
            msdyn_projectapproval project = localcontext.GetTarget<msdyn_projectapproval>();
            TimeEntryBusiness timeEntryBusiness = new TimeEntryBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            msdyn_expenseBusiness expenseBusiness = new msdyn_expenseBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if (project.msdyn_TimeEntry != null)
            {
                var time = localcontext.OrganizationServiceAdmin.Retrieve(msdyn_timeentry.EntityLogicalName, project.msdyn_TimeEntry.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("smt_pl_billingtypee")).ToEntity<msdyn_timeentry>();
                // Invocação dos métodos.
                timeEntryBusiness.UpdateProjectApproval(project, time);
            }
            if (project.msdyn_ExpenseEntry != null)
            {
                var expense = localcontext.OrganizationServiceAdmin.Retrieve(msdyn_expense.EntityLogicalName, project.msdyn_ExpenseEntry.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("smt_pl_billingtypee")).ToEntity<msdyn_expense>();
                expenseBusiness.UpdateProjectApproval(project, expense);
            }
        }
    }
}
