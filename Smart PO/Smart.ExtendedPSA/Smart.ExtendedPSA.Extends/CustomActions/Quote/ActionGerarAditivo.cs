using CRM.Smart.ExtendedPSA.Extends.Business;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Smart.ExtendedPSA.Extends.CustomActions.Quote
{
    /// <DynamicsCE.Deploy>CRM.Smart.ExtendedPSA.Extends.CustomActions.Quote.ActionGerarAditivo</DynamicsCE.Deploy>
    public class ActionGerarAditivo : PluginBase
    {
        private List<Resx> messages;

        /// <summary>
        ///  Initializes a new instance of the <see cref='Plugin'/> class.
        /// </summary>
        public ActionGerarAditivo() : base(typeof(ActionGerarAditivo))
        {
        }

        /// <summary>
        ///  Classe padrão de execução de plugins 
        /// </summary>
        /// <param name="localContext">Contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext.PluginExecutionContext.InputParameters.Contains("Target") && localContext.PluginExecutionContext.InputParameters["Target"] is EntityReference)
            {
                if (localContext == null) { throw new InvalidPluginExecutionException("localContext"); }

                /// <summary>
                /// Utilizado para realizar a busca de exceções cadastradas
                /// Exemplo de Utilização: throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.Hi));
                /// </summary>
                string resxFileName = ResxExtension.webResourceName;
                messages = localContext.LoadResxMessages(resxFileName);

                try
                {

                    EntityReference entityReference = (EntityReference)localContext.PluginExecutionContext.InputParameters["Target"];
                    String idContrato = localContext.PluginExecutionContext.InputParameters["Contrato"].ToString();

                    if (idContrato != null && idContrato != String.Empty)
                    {
                        EntityReference contratoRef = new EntityReference("salesorder", new Guid(idContrato));

                        ActionGerarAditivoBusiness GerarAditivoBusiness = new ActionGerarAditivoBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, messages);

                        GerarAditivoBusiness.GerarAdtivo(contratoRef, entityReference);

                        // Atualiza o campo tipo de contrato no contrato                  
                        SalesOrder contrato = new SalesOrder();
                        contrato.Id = contratoRef.Id;
                        contrato["smt_pl_type_contract"] = new OptionSetValue(100000000);

                        localContext.OrganizationService.Update(contrato);

                        // Fechar Cotação Como Ganha e Aditivo Gerado
                        GerarAditivoBusiness.SetStateQuote(entityReference);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException(ex.Message);
                }
            }
        }
    }
}