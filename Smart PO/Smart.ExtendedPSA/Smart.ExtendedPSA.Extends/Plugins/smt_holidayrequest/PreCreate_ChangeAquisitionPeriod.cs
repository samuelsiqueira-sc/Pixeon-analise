using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smart.ExtendedPSA.Extends.Business;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_holidayrequest
{
    /// <summary>
    /// Classe principal
    /// </summary>
    public class PreCreate_ChangeAquisitionPeriod : PluginBase
    {

        /// <summary>
        /// qweqw
        /// </summary>
        public PreCreate_ChangeAquisitionPeriod() : base(typeof(PreCreate_ChangeAquisitionPeriod)) {}

            /// <summary>
            /// Execute
            /// </summary>
            /// <param name="localContext">Contecto de execução</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {

            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado"); }





        }


   
    }



}


