using CRM.Pixeon.Extends.Earlybound;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Entrada_de_Hora
{
    /// <summary>
    /// Doc.
    /// </summary>
    public class PostUpdateAsync : PluginBase
    {
        /// <summary>
        /// Doc.
        /// </summary>
        public PostUpdateAsync() : base(typeof(PostUpdateAsync)) { }

        /// <summary>
        /// Classe principal de execução do Plugin. 
        /// </summary>
        /// <param name="localcontext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Validação da Licença.

            // Declaração das variaveis. 
            msdyn_timeentry timeEntry = localcontext.GetTarget<msdyn_timeentry>();
            msdyn_timeentry PreImage = localcontext.GetPreImage<msdyn_timeentry>();
            TimeEntryBusiness business = new TimeEntryBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos. 
            if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Aprovado && PreImage.msdyn_typeEnum == msdyn_timeentrytype.Trabalho)
            {
                SetMapping(PreImage, business);
            }
            if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Aprovado && PreImage.msdyn_typeEnum == msdyn_timeentrytype.Trabalho)
            {
                SetProjectFields(PreImage, business);
            }

            if (PreImage.msdyn_entryStatusEnum == msdyn_timeentrystatus.RecuperacaoSolicitada && timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Devolvido && PreImage.msdyn_typeEnum == msdyn_timeentrytype.Trabalho)
            {
                SetProjectFieldsRecuperado(PreImage, business);
            }
        }

        /// <summary>
        /// Este método tem por finalidade mapear o campo 
        /// </summary>
        /// <param name="preImage">Entrada de horas</param>
        /// <param name="business">Business.</param>
        public void SetMapping(msdyn_timeentry preImage, TimeEntryBusiness business)
        {
            msdyn_projectapproval approval = business.RetrieveProjectApproval(preImage); // Retrieve da  aprovação da tarefa do projeto. 

            if (approval != null)
            {
                business.UpdateTimeEntry(approval, preImage); // Atualiza a aprovação do projeto de acordo com o que foi inserido na Tarefa do Projeto. 
            }
        }

        public void SetProjectFields(msdyn_timeentry preImage, TimeEntryBusiness business)
        {
            business.UpdateProject(preImage);
        }

        public void SetProjectFieldsRecuperado(msdyn_timeentry preImage, TimeEntryBusiness business)
        {
            business.UpdateProjectDevolvido(preImage);
        }
    }
}
