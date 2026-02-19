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
    /// plugin
    /// </summary>
    public class CreateProject : PluginBase
    {

        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public CreateProject() : base(typeof(CreateProject)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis 
            msdyn_project project = localcontext.GetTarget<msdyn_project>();
            ProjetoBusiness projetoBusiness = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos.
            CheckAssociatedContract(project, projetoBusiness);
        }

        /// <summary>
        /// Método que verifica se o campo "Projeto Interno" é igual a "Não" e se campo "Contrato" está vazio e apresenta uma mensagem de erro, caso o retorno dessa condição seja verdadeiro.
        /// </summary>
        /// <param name="project">Projeto.</param>
        /// <param name="projetoBusiness">Business de projeto.</param>
        public void CheckAssociatedContract(msdyn_project project, ProjetoBusiness projetoBusiness)
        {
            try
            {
                if ((project.smt_bl_projectInternal == false) && (project.msdyn_salesorderid == null) && (project.smt_bl_migration == false) && (project.msdyn_istemplate == false))
                {
                    throw new InvalidPluginExecutionException("É necessário que tenha um contrato associado ao projeto.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Erro: " + ex.Message);
            }
        }

    }
}
