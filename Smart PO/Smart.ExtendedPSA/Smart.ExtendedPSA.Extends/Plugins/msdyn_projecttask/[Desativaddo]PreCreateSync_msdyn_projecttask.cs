using CRM.Smart.ExtendedPSA.Extends.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.Business;

namespace Smart.ExtendedPSA.Extends.Plugins
{

    /// <summary>
    /// Classe principal do plugin
    /// </summary>
    public class PreCreateSync_msdyn_projecttask : PluginBase
    {
        /// <summary>
        /// Método construtor
        /// </summary>
        public PreCreateSync_msdyn_projecttask() : base(typeof(PreCreateSync_msdyn_projecttask)) { }

        /// <summary>
        /// Execute CRM
        /// </summary>
        /// <param name="localcontext">contexto de execução </param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_projecttask target = localcontext.GetTarget<msdyn_projecttask>();
            msdyn_projecttaskBusiness business = new msdyn_projecttaskBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            // InsertWBSLevel(target, business);
        }

        private void InsertWBSLevel(msdyn_projecttask target, msdyn_projecttaskBusiness business)
        {
            business.SetWBSLevel(target);
        }
    }
}
