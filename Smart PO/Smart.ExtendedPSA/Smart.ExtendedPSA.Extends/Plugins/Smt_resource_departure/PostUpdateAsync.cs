using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Xrm.Sdk;

namespace Smart.ExtendedPSA.Extends.Plugins.Smt_resource_departure
{
    /// <summary>
    /// Plugin de Post Update do Afastamento do Recurso.
    /// </summary>
    public class PostUpdateAsync : PluginBase
    {
        /// <summary>
        /// Base.
        /// </summary>
        public PostUpdateAsync() : base(typeof(PostUpdateAsync)) { }
        /// <summary>
        /// Método para chamada de variáveis não locais.
        /// </summary>
        /// <param name="localcontext">Contexto local do formulário.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            smt_resource_departure_eb target = localcontext.GetTarget<smt_resource_departure_eb>();
            smt_resource_departure_eb preimage = localcontext.GetPreImage<smt_resource_departure_eb>();
            List<Resx> messages;
            string restFileName = ResxExtension.webResourceName;
            messages = localcontext.LoadResxMessages(restFileName);
            ResourceDepartureBusiness business = new ResourceDepartureBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService);

            StatusChanged(target, business, messages, localcontext, preimage);
        }

        /// <summary>
        /// Médoto que após verificar a razão de status, o contrato, o recurso e o tipo de recurso, cria os afastamentos nos dias selecionados. 
        /// </summary>
        /// <param name="target">Entidade local - Afastamento do Recurso.</param>
        /// <param name="business">Business de Afastamento do Recurso.</param>
        /// <param name="messages">mensagens de erros.</param>
        /// <param name="localcontext">Contexto local do formulário.</param>
        /// <param name="preimage">preimage da entidade atual.</param>
        private void StatusChanged(smt_resource_departure_eb target, ResourceDepartureBusiness business, List<Resx> messages, LocalPluginContext localcontext, smt_resource_departure_eb preimage)
        {
            Guid resourceId = preimage.smt_lp_resource.Id;
            BookableResource resource = business.SearchResource(resourceId);
            EntityReference userId = new EntityReference(SystemUser.EntityLogicalName, resource.UserId.Id);
            business.DepartureDates(preimage, resource, messages);
           
            // verifica se usuário precisa lançar horas
            if (business.WorkWithoutHours(resource.Id) == null)
            {
                // Se o usuário - que deve possuir gerente - e o modelo de contrato do recurso não forem nulos
                if (userId != null && resource.smt_lp_model_contract != null)
                {
                    smt_model_contract contractModel = business.FindContractModel(resource.smt_lp_model_contract.Id);
                    SystemUser manager = business.ManagerId(userId.Id);

                    // Se status do afastamento for aprovado.
                    if (target.StatusCodeEnum == smt_resource_departure_StatusCode.Aprovado)
                    {
                        if (resource.ResourceType.Value == 3 && contractModel != null && manager != null)
                        {
                            // Chamada do método que verifica datas do afastamento.
                            business.CreateDepartureForDays(manager, messages, localcontext.OrganizationServiceFactory, preimage, resource, contractModel);
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException("Erro ao atualizar registro de afastamento de horas, pois seu contrato/gerente está nulo ou o tipo do recurso não é usuário.");
                            OptionSetValue eventType = new OptionSetValue(100000000);
                            String recordid = preimage.Id.ToString();
                            String message = "Erro ao atualizar registro de afastamento de horas, pois seu contrato/gerente está nulo ou o tipo do recurso não é usuário. " + messages.GetMessageById(ResxExtension.CUTDR111);
                            String name = "Erro ao atualizar registro de afastamento de horas.";
                            business.CreateLogs(eventType, localcontext.OrganizationService, recordid, message, name, manager.Id);
                        }
                    }
                    else if (target.StatusCodeEnum == smt_resource_departure_StatusCode.Cancelado || preimage.StatusCodeEnum == smt_resource_departure_StatusCode.Cancelado)
                    {
                        if (manager != null)
                        {
                            IOrganizationService managerService = business.GetService(manager.Id, localcontext.OrganizationServiceFactory);
                            business.Canceltarget(preimage, managerService);
                        }
                        else
                        {
                            IOrganizationService managerService = business.GetService(userId.Id, localcontext.OrganizationServiceFactory);
                            business.Canceltarget(preimage, managerService);
                        }
                    }
                }
                else
                {
                    throw new InvalidPluginExecutionException("Erro ao atualizar registro de afastamento, pois o usuário do recurso ou o modelo decontrato do mesmo está nulo");
                    OptionSetValue eventType = new OptionSetValue(100000000);
                    String recordid = preimage.Id.ToString();
                    String message = "Erro ao atualizar registro de afastamento, pois o usuário do recurso ou o modelo decontrato do mesmo está nulo. " + messages.GetMessageById(ResxExtension.CUTDR111);
                    String name = "Erro ao atualizar registro de afastamento, pois o usuário do recurso ou o modelo decontrato do mesmo está nulo.";
                    business.CreateLogs(eventType, localcontext.OrganizationService, recordid, message, name, resource.UserId.Id);
                }
            }
        }
    }
}
