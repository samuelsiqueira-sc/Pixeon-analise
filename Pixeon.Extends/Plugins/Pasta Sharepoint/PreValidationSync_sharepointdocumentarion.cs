using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Projeto
{
    /// <summary>
    /// Plugin
    /// </summary>
    public class PreValidationSync_sharepointdocumentarion : PluginBase
    {

        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public PreValidationSync_sharepointdocumentarion() : base(typeof(PreValidationSync_sharepointdocumentarion)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis
            SharePointDocumentLocation documentation = localcontext.GetTarget<SharePointDocumentLocation>();
            ProjetoBusiness projetoBusiness = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos.
            ValidateDocumentation(documentation, projetoBusiness);
            // CreateDocuments(project, projetoBusiness);
        }

        /// <summary>
        /// Cria equipe do projeto e atribui o projeto a ela.
        /// </summary>
        /// <param name="documentation">projeto do contexto </param>
        /// <param name="projetoBusiness"> Business de projeto </param>
        public void ValidateDocumentation(SharePointDocumentLocation documentation, ProjetoBusiness projetoBusiness)
        {
            if(documentation.RegardingObjectId.LogicalName == "account" && !documentation.Name.Contains("Documentos em Site Padrão"))
            {
                projetoBusiness.ValidateDocumentAccount(documentation);
            }
        }

    }
}
