using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Classe que contém os retrieves necessários para o funcionamento do plugin 
    /// </summary>
    public class smt_changefunctionsBusiness
    {
        /// <summary>
        /// Método responsável por trazer o registro de associação de categoria de recurso reservável relacionado ao recurso em questão
        /// </summary>
        /// <param name="context">Variável contendo o contexto</param>
        /// <param name="resourceId">Referência ao recurso</param>
        /// <returns>Registro de associação de categoria de recurso reservável relacionado ao recurso em questão</returns>
        public BookableResourceCategoryAssn GetBookCatAssn(CrmServiceContext context, EntityReference resourceId)
        {
            BookableResourceCategoryAssn categoryAss = new BookableResourceCategoryAssn();
            using (context)
            {
                BookableResourceCategoryAssn result = (from book in context.CreateQuery<BookableResourceCategoryAssn>()
                                                       where book.Resource.Id == resourceId.Id
                                                       select new BookableResourceCategoryAssn
                                                       {
                                                           BookableResourceCategoryAssnId = book.BookableResourceCategoryAssnId,
                                                           Resource = book.Resource,
                                                           LogicalName = book.LogicalName,
                                                           StatusCode = book.StatusCode,
                                                           StateCode = book.StateCode
                                                       }).FirstOrDefault();
                categoryAss = result;
            }
            return categoryAss;
        }
        /// <summary>
        /// Método responsável por retirar 
        /// </summary>
        /// <param name="orgService">Variável de serviço</param>
        /// <param name="lpResource">Referência ao recurso em questão</param>
        /// <returns>Registro referente ao Recurso, com os campos StateCode e StatusCode preenchidos</returns>
        public BookableResource GetResource(IOrganizationService orgService, EntityReference lpResource)
        {
            BookableResource resource = orgService.Retrieve(lpResource.LogicalName, lpResource.Id, new ColumnSet(BookableResource.Fields.StateCode, BookableResource.Fields.StatusCode)).ToEntity<BookableResource>();
            return resource;
        }
    }
}
