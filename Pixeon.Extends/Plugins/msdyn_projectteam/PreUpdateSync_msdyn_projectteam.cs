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

namespace Pixeon.Extends.Plugins
{
    /// <summary>
    /// Plugin
    /// </summary>
    public class PreUpdateSync_msdyn_projectteam : PluginBase
    {
        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public PreUpdateSync_msdyn_projectteam() : base(typeof(PreUpdateSync_msdyn_projectteam)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis
            msdyn_projectteam projectTeam = localcontext.GetTarget<msdyn_projectteam>();
            msdyn_projectteam projectTeamImage = localcontext.GetPreImage<msdyn_projectteam>();

            ProjetoBusiness projetoBusiness = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if (projectTeam.msdyn_ProjectApprover == false)
            {
                ValidateManager(projectTeam, projectTeamImage, projetoBusiness);
            }
        }
        /// <summary>
        /// Cria equipe do projeto e atribui o projeto a ela.
        /// </summary>
        /// <param name="project">projeto do contexto </param>
        /// <param name="projetoBusiness"> Business de projeto </param>
        /// <param name="projecteamImage">Image</param>
        public void ValidateManager(msdyn_projectteam project, msdyn_projectteam projecteamImage, ProjetoBusiness projetoBusiness)
        {

            projetoBusiness.ValidateManager(project, projecteamImage);
        }
    }
}
