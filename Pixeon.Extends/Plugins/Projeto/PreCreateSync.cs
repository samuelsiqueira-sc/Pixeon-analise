using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Projeto
{
    /// <summary>
    /// Doc. 
    /// </summary>
    public class PreCreateSync : PluginBase
    {
        /// <summary>
        /// Doc. 
        /// </summary>
        public PreCreateSync() : base(typeof(PreCreateSync)) { }

        /// <summary>
        /// Classe principal de execução. 
        /// </summary>
        /// <param name="localContext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            // Validação da Licença 

            // Declaração das variaveis
            msdyn_project TargetEntity = localContext.GetTarget<msdyn_project>();
            ProjetoBusiness business = new ProjetoBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

            // Invocação dos métodos 
            if (TargetEntity.smt_bl_projectInternal == false && TargetEntity.msdyn_istemplate == false)
            {
                SetProjectName(TargetEntity, business);
            }
            if (TargetEntity.smt_bl_migration == true)
            {
              // CreateLine(TargetEntity, business);
            }

            SetModalidadeCanal(TargetEntity, business);
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="project">Entidade alvo.</param>
        /// <param name="business">Business de projeto.</param>
        public void SetProjectName(msdyn_project project, ProjetoBusiness business)
        {
            SalesOrder salesOrder = business.RetrieveProjectContract(project); // Retrieve no Contrato do Projeto 

            if (salesOrder != null)
            {
                business.SetProjectName(project, salesOrder); // Método que atribui o nome do Projeto. 
            }
        }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="project">Entidade alvo.</param>
        /// <param name="business">Business de projeto.</param>
        public void CreateLine(msdyn_project project, ProjetoBusiness business)
        {
            SalesOrder salesOrder = business.RetrieveProjectContract(project); // Retrieve no Contrato do Projeto 
            project.msdyn_ContractOrganizationalUnitId = salesOrder.msdyn_ContractOrganizationalUnitId;
            project.msdyn_customer = salesOrder.CustomerId;
            if (salesOrder.smt_program != null)
            {
                project.smt_Programa = salesOrder.smt_program;
            }

            if (salesOrder != null)
            {
               // business.CreateSalesLine(project, salesOrder, );
            }
        }

        /// <summary>
        /// Preenche o campo Modalidade Canal
        /// </summary>
        /// <param name="projeto">Projeto a ser alterado</param>
        /// <param name="busines">Business contendo os métodos necessários</param>
        public void SetModalidadeCanal(msdyn_project projeto, ProjetoBusiness busines)
        {
            if (projeto.msdyn_salesorderid != null)
            {
                var modalidadeCanal = busines.RetrieveProjectContract(projeto);

                if (modalidadeCanal != null)
                {
                    projeto.smt_op_modalidade_canal = modalidadeCanal.smt_op_modalidade_canal;
                }

            }
            
        }
    }
}
