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
    public class PostCreateAsync : PluginBase
    {

        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public PostCreateAsync() : base(typeof(PostCreateAsync)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis
            msdyn_project project = localcontext.GetTarget<msdyn_project>();
            ProjetoBusiness projetoBusiness = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos.
            UpdateProject(project, projetoBusiness);
           // CreateDocuments(project, projetoBusiness);
        }

        /// <summary>
        /// Cria equipe do projeto e atribui o projeto a ela.
        /// </summary>
        /// <param name="project">projeto do contexto </param>
        /// <param name="projetoBusiness"> Business de projeto </param>
        public void UpdateProject(msdyn_project project, ProjetoBusiness projetoBusiness)
        {
            projetoBusiness.CreateTeam(project);
        }

    }
}
