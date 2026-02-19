using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins.PreUpdateSync_msdyn_projectapproval
{
    public class PreUpdateSync_msdyn_projectapproval : PluginBase
    {
        public PreUpdateSync_msdyn_projectapproval() : base(typeof(PreUpdateSync_msdyn_projectapproval)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis
            msdyn_projectapproval project = localcontext.GetTarget<msdyn_projectapproval>();
            msdyn_projectapproval entry = localcontext.GetPreImage<msdyn_projectapproval>();

            TimeEntryBusiness timeEntryBusiness = new TimeEntryBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            msdyn_expenseBusiness expenseEntryBusiness = new msdyn_expenseBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if (project.msdyn_BillingType != null)
            {
                if (project.msdyn_TimeEntry != null)
                {
                    // Invocação dos métodos.
                    timeEntryBusiness.UpdateTimeEntry(project, entry.msdyn_TimeEntry);
                }
                if (project.msdyn_ExpenseEntry != null)
                {
                    // Invocação dos métodos.
                    expenseEntryBusiness.UpdateExpenseEntry(project, entry.msdyn_ExpenseEntry);
                }
            }
        }
    }
}
