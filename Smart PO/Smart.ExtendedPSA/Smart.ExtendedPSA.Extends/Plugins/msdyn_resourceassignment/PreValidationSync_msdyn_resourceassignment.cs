using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.Earlybound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    /// <DynamicsCE.Deploy>Smart.ExtendedPSA.Extends.Plugins._PreValidationSync</DynamicsCE.Deploy>    
    public class PreValidationSync_msdyn_resourceassignment : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='Plugin'/> class.
        /// </summary>
        public PreValidationSync_msdyn_resourceassignment()
            : base(typeof(PreValidationSync_msdyn_resourceassignment)) { }

        /// <summary>
        /// Classe padrão de execução de plugins 
        /// </summary>
        /// <param name='localContext'>Contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            /// <summary>
            /// Plugin é executado no Update dos campos "Última Atualização Planejamento Presente", "Data de Início" e "Data de Término"
            /// O Flow "Atualizar Planejamento Presente - Plugin", é executado uma vez por dia as 00h01 e altera o campo "Última Atualização Planejamento Presente"
            /// Esse Fluxo atualiza o Planejamento Presente (altera os campos Esforço Presente Planejado, Custo Presente Planejado e Última Atualização Planejamento)
            /// </summary>
            if (localContext == null) { throw new InvalidPluginExecutionException("Contexto não localizado!"); };

            try
            {
                // WhoAmIRequest request = new WhoAmIRequest();
                // WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

                // Guid OrgId = response.OrganizationId;

                //// Request da action de validação de licenças
                // var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
                // {
                //    ["OrgID"] = OrgId.ToString(),
                //    ["ProductID"] = "EXPSA",
                // };

                // localContext.OrganizationService.Execute(ActionRequest);

                msdyn_resourceassignment entity = localContext.GetTarget<msdyn_resourceassignment>();
                var preImg = localContext.GetPreImage<msdyn_resourceassignment>();
                var business = new msdyn_resourceassignmentBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);

                DateTime dataAtual = DateTime.Now.ToUniversalTime();
                DateTime inicio = entity.msdyn_start != null ? (DateTime)entity.msdyn_start : (DateTime)preImg.msdyn_start;
                DateTime termino = entity.msdyn_finish != null ? (DateTime)entity.msdyn_finish : (DateTime)preImg.msdyn_finish;

                if (entity.msdyn_start != null || entity.msdyn_finish != null)
                {
                    if (entity.msdyn_start != preImg.msdyn_start || entity.msdyn_finish != preImg.msdyn_finish)
                    {
                        if (business.AtualizaPlanejamento(dataAtual, inicio.ToUniversalTime(), termino.ToUniversalTime(), entity, preImg))
                            return;
                    }
                }
                else
                {
                    // Se o metodo não atualizar o Planejamento é executado o bloco seguinte
                    if (!business.AtualizaPlanejamento(dataAtual, inicio.ToUniversalTime(), termino.ToUniversalTime(), entity, preImg))
                    {
                        if (entity.smt_ultima_atualizacao_pp != null && entity.smt_ultima_atualizacao_pp != preImg.smt_ultima_atualizacao_pp)
                        {
                            string json1 = entity.msdyn_plannedwork != null ? entity.msdyn_plannedwork.ToString() : preImg.msdyn_plannedwork.ToString();
                            json1 = @json1.Replace("'", "'");
                            string json2 = entity.msdyn_plannedwork != null ? entity.msdyn_plannedcostcontour.ToString() : preImg.msdyn_plannedcostcontour.ToString();
                            json2 = @json2.Replace("'", "'");

                            List<TrabalhoPlanejado> esforcoPlanejado = business.GetObjectTrabalhoPlanejado(json1);
                            List<CustoPlanejado> custoPlanejado = business.GetObjectCustoPlanejado(json2);
                            entity.smt_epp = esforcoPlanejado.Where(x => x.End <= DateTime.Now.ToUniversalTime()).Sum(x => x.Hours);

                            if (custoPlanejado.Count > 0)
                                entity.smt_cpp = business.SomaCusto(custoPlanejado, esforcoPlanejado);

                            entity.smt_ultima_atualizacao_pp = DateTime.Now.ToUniversalTime();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}