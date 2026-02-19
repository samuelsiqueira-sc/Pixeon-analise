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
    /// doc.
    /// </summary>
    public class PostUpdateSync : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe.
        /// </summary>
        public PostUpdateSync() : base(typeof(PostUpdateSync)) { }

        /// <summary>
        /// Execução do plugin
        /// </summary>
        /// <param name="localcontext">doc.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            // Declaração de variáveis. 
            msdyn_project entity = localcontext.GetTarget<msdyn_project>();
            ProjectBusiness business = new ProjectBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            // Invocação dos métodos
            FunctionChange(entity, business);
        }

        /// <summary>
        /// doc.
        /// </summary>
        /// <param name="entity">doc.</param>
        /// <param name="business">doc.</param>
        public void FunctionChange(msdyn_project entity, ProjectBusiness business)
        {
            if (entity.Contains("msdyn_projectmanager"))
            {
                Guid managerId = entity.msdyn_projectmanager.Id; // ID do usuário contido no campo "Gerente de Projeto". 
                BookableResource managerResource = business.RetrieveBookableResource(managerId);

                if (managerResource != null)
                {
                    Guid bookableResourceId = managerResource.Id;   // ID do recurso reservavel do usuário do sistema.
                    BookableResourceCategoryAssn managerCategoryAssn = business.RetrieveCategoryAssign(bookableResourceId);

                    if (managerCategoryAssn != null)
                    {
                        msdyn_projectteam managerTeam = business.RetrieveManagerTeam(managerResource.Id, entity.Id);

                        if (managerTeam != null)
                        {
                            {
                                business.SetNewFunction(managerTeam, managerCategoryAssn);
                            }
                        }
                    }
                }
            }
        }

    }
}