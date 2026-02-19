using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;

namespace Pixeon.Extends.Business
{
    public class msdyn_expenseBusiness : BaseBusiness
    {
        public msdyn_expenseBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="projectApproval">Aprovação do projeto.</param>
        /// <param name="expenseEntry">Entrada de Despesa.</param>
        public void UpdateProjectApproval(msdyn_projectapproval projectApproval, msdyn_expense expenseEntry)
        {
            int option = expenseEntry.smt_pl_billingtypee.Value;

            switch (option)
            {
                // Passível de Cobrança
                case 192350001:

                    option = 192350001;
                    break;

                // Não Passível de Cobrança
                case 192350000:

                    option = 192350000;
                    break;

                // Complementar
                case 180580002:

                    option = 192350002;
                    break;

                // Não Disponível   
                case 180580003:

                    option = 192350003;
                    break;

                default: break;
            }

            msdyn_projectapproval approval = new msdyn_projectapproval
            {
                Id = projectApproval.Id,
                msdyn_BillingType = new OptionSetValue(option),
                EntityState = EntityState.Changed,
            };
            Update(approval);
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="projectApproval">Aprovação do projeto.</param>
        /// <param name="expense">Entrada de despesa.</param>
        public void UpdateExpenseEntry(msdyn_projectapproval projectApproval, EntityReference expense)
        {
            int option = projectApproval.msdyn_BillingType.Value;

            switch (option)
            {
                // Passível de Cobrança
                case 192350001:

                    option = 192350001;
                    break;

                // Não Passível de Cobrança
                case 192350000:

                    option = 192350000;
                    break;

                // Complementar
                case 180580002:

                    option = 192350002;
                    break;

                // Não Disponível   
                case 180580003:

                    option = 192350003;
                    break;

                default: break;
            }

            msdyn_expense approval = new msdyn_expense
            {
                Id = expense.Id,
                smt_pl_billingtypee = new OptionSetValue(option),
                EntityState = EntityState.Changed,
            };
            Update(approval);
        }

        /// <summary>
        /// Doc.
        /// </summary>
        /// <returns>Aprovação.</returns>
        /// <param name="expense">Despesa.</param>
        public msdyn_projectapproval RetrieveProjectApproval(msdyn_expense expense)
        {
            try
            {
                using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
                {
                    return context.ProjectApprovalSet.Where(
                    approval => approval.msdyn_ExpenseEntry.Id == expense.Id).FirstOrDefault();
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
    }
}
