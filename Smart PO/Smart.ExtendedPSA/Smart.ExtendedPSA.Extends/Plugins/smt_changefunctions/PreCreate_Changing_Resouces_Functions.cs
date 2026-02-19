using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Smart.ExtendedPSA.Extends.Plugins.smt_changefunctions
{
    /// <summary>
    /// Plugin responsável por trocar a função do Recurso quando um registro for criado na entidade Alterações de Funções
    /// </summary>
    public class PreCreate_Changing_Resouces_Functions : PluginBase
    {
        /// <summary>
        /// Construtor da classe
        /// </summary>
        public PreCreate_Changing_Resouces_Functions() : base(typeof(PreCreate_Changing_Resouces_Functions)) { }
        /// <summary>
        /// Método que contém o código que vai mudar a Função do Recurso
        /// </summary>
        /// <param name="localContext">Variável que vai capturar o contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localContext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localContext.OrganizationService.Execute(ActionRequest);

            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localContext.LoadResxMessages(resxFileName);

            if (localContext.PluginExecutionContext.PrimaryEntityName.ToLower() == smt_change_functions.EntityLogicalName)
            {
                smt_change_functions func = localContext.GetTarget<smt_change_functions>();
                CrmServiceContext context = new CrmServiceContext(localContext.OrganizationServiceAdmin);
                smt_changefunctionsBusiness funcBus = new smt_changefunctionsBusiness();
                BookableResourceCategoryAssn categoryAss = funcBus.GetBookCatAssn(context, func.smt_lp_resourceid);
                BookableResource resource = funcBus.GetResource(localContext.OrganizationService, func.smt_lp_resourceid);
                ChangeFunction(func, categoryAss, resource, localContext.OrganizationService);
            }
            else
            {
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.NoTargPlugin));
            }
        }
        /// <summary>
        /// Método que irá mudar a função do ususario
        /// </summary>
        /// <param name="func">Entidade alvo</param>
        /// <param name="categoryAss">Registro de Associação de Categoria de Recurso Reservável referente ao recurso em questão</param>
        /// <param name="resource">Registro de Recurso em questão</param>
        /// <param name="orgService">Variável de serviço</param>
        public void ChangeFunction(smt_change_functions func, BookableResourceCategoryAssn categoryAss, BookableResource resource, IOrganizationService orgService)
        {
            EntityReference newFunc = func.smt_lp_function;
            Guid resourceId = func.smt_lp_resourceid.Id;

            // Se não for demissão
            if (func.smt_pl_type.Value != 100000001)
            {
                // Se o recurso tiver uma Associação de Categoria de Recurso
                if (categoryAss != null)
                {
                    // Alterando a função do usuario em Associação de Categoria de Recurso Reservavel
                    BookableResourceCategoryAssn assBook = new BookableResourceCategoryAssn();
                    assBook.Id = categoryAss.BookableResourceCategoryAssnId.Value;
                    assBook.ResourceCategory = newFunc;
                    orgService.Update(assBook);

                    // Testando se o Recurso foi demitido anteriormente ou se apenas tem sua Associação ou Recurso inatios para poder reatva-los
                    if (categoryAss.GetAttributeValue<OptionSetValue>(BookableResourceCategoryAssn.Fields.StateCode).Value != 0 &&
                        categoryAss.GetAttributeValue<OptionSetValue>(BookableResourceCategoryAssn.Fields.StatusCode).Value != 1)
                    {
                        assBook.StateCode = BookableResourceCategoryAssnState.Active;
                        assBook.StatusCode = new OptionSetValue(1);
                        orgService.Update(assBook);

                        BookableResource res = new BookableResource();
                        res.Id = categoryAss.Resource.Id;
                        res.StateCode = BookableResourceState.Active;
                        res.StatusCode = new OptionSetValue(1);
                        orgService.Update(res);
                    }
                    else if (resource.GetAttributeValue<OptionSetValue>(BookableResourceCategoryAssn.Fields.StateCode).Value != 0 &&
                        resource.GetAttributeValue<OptionSetValue>(BookableResourceCategoryAssn.Fields.StatusCode).Value != 1)
                    {
                        assBook.StateCode = BookableResourceCategoryAssnState.Active;
                        assBook.StatusCode = new OptionSetValue(1);
                        orgService.Update(assBook);

                        BookableResource res = new BookableResource();
                        res.Id = categoryAss.Resource.Id;
                        res.StateCode = BookableResourceState.Active;
                        res.StatusCode = new OptionSetValue(1);
                        orgService.Update(res);
                    }
                }
                else if (resource.GetAttributeValue<OptionSetValue>(BookableResourceCategoryAssn.Fields.StateCode).Value != 1 &&
                        resource.GetAttributeValue<OptionSetValue>(BookableResourceCategoryAssn.Fields.StatusCode).Value != 2)
                {
                    // Se o Recurso estiver ativo mas ele não tem Associação de Categoria de Recurso, basta criar uma associação
                    BookableResourceCategoryAssn assBook = new BookableResourceCategoryAssn();
                    assBook.Resource = func.smt_lp_resourceid;
                    assBook.ResourceCategory = newFunc;
                    assBook.msdyn_IsDefault = true;
                    orgService.Create(assBook);
                }
                else
                {
                    // Agora se o Recurso não estiver ativo, ativa-lo e criar a associação
                    BookableResource res = new BookableResource();
                    res.Id = func.smt_lp_resourceid.Id;
                    res.StateCode = BookableResourceState.Active;
                    res.StatusCode = new OptionSetValue(1);
                    orgService.Update(res);

                    BookableResourceCategoryAssn assBook = new BookableResourceCategoryAssn();
                    assBook.Resource = func.smt_lp_resourceid;
                    assBook.ResourceCategory = newFunc;
                    assBook.msdyn_IsDefault = true;
                    orgService.Create(assBook);
                }
            }
            // Se o Recurso tiver uma associação
            else if (categoryAss != null)
            {
                // Inativando o Recurso 
                BookableResourceCategoryAssn assBook = new BookableResourceCategoryAssn();
                assBook.Id = categoryAss.BookableResourceCategoryAssnId.Value;
                assBook.StateCode = BookableResourceCategoryAssnState.Inactive;
                assBook.StatusCode = new OptionSetValue(2);
                orgService.Update(assBook);

                // E a Associação de Categoria de Recurso
                BookableResource res = new BookableResource();
                res.Id = categoryAss.Resource.Id;
                res.StateCode = BookableResourceState.Inactive;
                res.StatusCode = new OptionSetValue(2);
                orgService.Update(res);
            }
            else
            {
                // Inativando apenas o Recurso
                BookableResource res = new BookableResource();
                res.Id = func.smt_lp_resourceid.Id;
                res.StateCode = BookableResourceState.Inactive;
                res.StatusCode = new OptionSetValue(2);
                orgService.Update(res);
            }
        }
    }
}
