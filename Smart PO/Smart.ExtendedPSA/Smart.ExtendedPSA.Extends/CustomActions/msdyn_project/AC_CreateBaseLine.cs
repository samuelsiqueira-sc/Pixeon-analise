using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.CustomActions.Msdyn_project
{
    /// <summary>
    /// Classe utilizada para uma CustomAction, onde é utilizada para gerar os dados da baseline, utilizando as tarefas do projeto para a criação das tarefas da baseline.
    /// </summary>
    public class AC_CreateBaseLine : PluginBase
    {
       /// <summary>
       /// a
       /// </summary>
        public AC_CreateBaseLine() : base(typeof(AC_CreateBaseLine))
        {

        }

        /// <inheritdoc/>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);
            EntityReference project = (EntityReference)localContext.PluginExecutionContext.InputParameters["Target"];
            var quoteBusiness = new ProjectBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            quoteBusiness.ExecuteAction(localContext.PluginExecutionContext.UserId, project, messages);
        }

    }
}
