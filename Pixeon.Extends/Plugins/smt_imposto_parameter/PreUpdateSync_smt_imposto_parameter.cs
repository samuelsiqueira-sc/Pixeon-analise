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
    /// <summary>
    /// inicio
    /// </summary>
   public class PreUpdateSync_smt_imposto_parameter : PluginBase
    {
        public PreUpdateSync_smt_imposto_parameter() : base(typeof(PreUpdateSync_smt_imposto_parameter)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            smt_imposto_parameter target = localcontext.GetTarget<smt_imposto_parameter>();
            
            if (target.smt__year != null)
            {
                smt_imposto_parameterBusiness business = new smt_imposto_parameterBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
                VerifyIfExist(target, business);
            }
        }


        public void VerifyIfExist(smt_imposto_parameter target, smt_imposto_parameterBusiness business)
        {
            if (target.smt__year != null)
            {
                business.GetImposto(target);
            }
        }
    }
}
