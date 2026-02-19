using CRM.Smart.ExtendedPSA.Extends.Business;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk.Client;


namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Business de Ultimo Projeto Trabalhado
    /// </summary>
    public class smt_latest_project_worked_Business : BaseBusiness
    {
        /// <summary>
        /// Chamada de recursos da Business
        /// </summary>
        /// <param name="service">service</param>
        /// <param name="serviceAdmin">serviceAdmin</param>
        /// <param name="tracingService">tracingService</param>
        /// <param name="messages">messages</param>
        public smt_latest_project_worked_Business(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Verifica registros da entidade Ultimo Projeto Trabalhado
        /// </summary>
        /// <param name="projeto">guid</param>
        /// <param name="recurso">guid</param>
        /// <returns>registers</returns>
        public Entity RegistrosUltimosProjetosTrabalhados(Guid projeto, Guid recurso)
        {
            String ProjetosRecursos = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='smt_latest_project_worked'>
                                        <attribute name='smt_lp_resource' />
                                        <attribute name='smt_lp_project' />
                                        <attribute name='smt_latest_project_workedid' />
                                        <order attribute='smt_lp_resource' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='smt_lp_project' operator='eq' value='{" + projeto + @"}' />
                                          <condition attribute='smt_lp_resource' operator='eq' uitype='bookableresource' value='{" + recurso + @"}' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            Entity RetornoRegistrosUltimosProjetosTrabalhados = Service.RetrieveMultiple(new FetchExpression(ProjetosRecursos)).Entities.FirstOrDefault();

            if (RetornoRegistrosUltimosProjetosTrabalhados != null)
            {
                return RetornoRegistrosUltimosProjetosTrabalhados;
            }

            return null;
        }

    }
}
