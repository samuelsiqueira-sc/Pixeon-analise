using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins
{
    public class PreUpdateSync_SalesOrder : PluginBase
    {
     public PreUpdateSync_SalesOrder() : base(typeof(PreUpdateSync_SalesOrder)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            SalesOrder target = localcontext.GetTarget<SalesOrder>();
            SalesOrderBusiness business = new SalesOrderBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            CloseSalesOrder(target, business);
            SetOrganizationalUnit(target, business);
        }

        public void CloseSalesOrder(SalesOrder target, SalesOrderBusiness business)
        {
            if (target.smt_pl_fase_oportunidadeEnum == Salesorder_smt_pl_fase_oportunidade.VendaCancelada)
            {
                business.CloseSalesOrder(target);
            }
        }

        /// <summary>
        /// Insere a Unidade organizacional do parâmetro encontrado
        /// </summary>
        /// <param name="target"> target </param>
        /// <param name="business"> business </param>
        public void SetOrganizationalUnit(SalesOrder target, SalesOrderBusiness business)
        {

            if (target.OwnerId != null)
            {
                if (target.OwnerId.LogicalName == Team.EntityLogicalName)
                {
                    var team = business.GetTeamOwnerSales(target);
                    if (team != null)
                    {
                        target.msdyn_ContractOrganizationalUnitId = team.ToEntityReference();
                    }
                }
                else
                {
                    var team = business.GetUserOwnerSales(target);
                    if (team != null)
                    {
                        target.msdyn_ContractOrganizationalUnitId = team.ToEntityReference();
                    }
                }

            }

        }

    }
}
