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

namespace Pixeon.Extends.Plugins
{
    /// <summary>
    /// Plugin
    /// </summary>
    public class PostCreateAsync_smt_task_history : PluginBase
    {

        /// <summary>
        /// Construtor padrão. 
        /// </summary>
        public PostCreateAsync_smt_task_history() : base(typeof(PostCreateAsync_smt_task_history)) { }

        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação de Licença 

            // RESX

            // Declaração das variaveis
            smt_history_task history = localcontext.GetTarget<smt_history_task>();
            ProjetoBusiness projetoBusiness = new ProjetoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos.
            UpdateTask(history, projetoBusiness);
            // CreateDocuments(project, projetoBusiness);
        }

        /// <summary>
        /// Cria equipe do projeto e atribui o projeto a ela.
        /// </summary>
        /// <param name="history">projeto do contexto </param>
        /// <param name="projetoBusiness"> projeto business</param>
        public void UpdateTask(smt_history_task history, ProjetoBusiness projetoBusiness)
        {
            msdyn_projecttask taskUpdate = new msdyn_projecttask();
            taskUpdate.Id = history.smt_lp_task.Id;
            var value = history.smt_mn_wrong_value == null ? new Money(0) : history.smt_mn_wrong_value;
            if (history.smt_dc_eventual == null)
            {
                if (history.smt_mn_value_real != value)
                {
                    taskUpdate.smt_mn_recognized_revenue = history.smt_mn_value_real;
                }
            }
            else
            {
                var valorTotal = history.smt_mn_valor_contrato.Value * (history.smt_dc_eventual / 100);

                taskUpdate.smt_mn_recognized_revenue = new Money(valorTotal.Value);
                taskUpdate.smt_mn_estimated_revenue = new Money(valorTotal.Value);
            }

            projetoBusiness.UpdateTarefa(taskUpdate);

        }
    }
}
