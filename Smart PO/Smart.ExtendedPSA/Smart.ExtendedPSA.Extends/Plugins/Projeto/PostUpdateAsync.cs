using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Plugins.Projeto
{
    /// <summary>
    ///  doc.
    /// </summary>
    public class PostUpdateAsync : PluginBase
    {
        /// <summary>
        /// Chamada padrão da classe.
        /// </summary>
        public PostUpdateAsync() : base(typeof(PostUpdateAsync)) { }

        /// <summary>
        /// Execução do plugin.
        /// </summary>
        /// <param name="localcontext">.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Declaração de variáveis. 
            msdyn_project entity = localcontext.GetTarget<msdyn_project>();
            ProjectBusiness business = new ProjectBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos.
           InsertBaselineTask(entity, business);
        }

        /// <summary>
        /// Método que verifica se o campo "Baseline de referência" contém dados e insere a tarefa da baseline no campo "Tarefa da baseline" da "tarefa do projeto".
        /// </summary>
        /// <param name="entity">.</param>
        /// <param name="business">.</param>
        public void InsertBaselineTask(msdyn_project entity, ProjectBusiness business)
        {
            business.SetBaselineTask(business.RetrieveBaselineTask(entity.smt_lp_lastbaselinecreated.Id));
        }
    }
}