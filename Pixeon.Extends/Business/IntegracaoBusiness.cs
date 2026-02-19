using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Business;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using Microsoft.Xrm.Sdk.Metadata;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;

namespace Pixeon.Extends.Business
{
    public class IntegracaoBusiness : BaseBusiness
    {

        public IntegracaoBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, null) { }

        /// <summary>
        /// Busca o valor do option set pela label vinda do salesforce.
        /// </summary>
        /// <param name="entityName"> nome lógico da Entidade do atributo </param>
        /// <param name="attributeName">nome lógio de atributo </param>
        /// <param name="label"> Label do campo </param>
        /// <param name="recordId"> Id da integração </param>
        /// <returns> Valor lógico do option set</returns>
        public int? GetOptionSetValueByLabel(string entityName, string attributeName, string label, Guid recordId)
        {
            var attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityName,
                LogicalName = attributeName,
                RetrieveAsIfPublished = true
            };

            try
            {
                var attributeResponse = (RetrieveAttributeResponse)ServiceAdmin.Execute(attributeRequest);
                var attributeMetadata = (EnumAttributeMetadata)attributeResponse.AttributeMetadata;

                var optionLabels = (from options in attributeMetadata.OptionSet.Options
                                    select new { options.Value, Text = options.Label.UserLocalizedLabel.Label }).ToList();

                return optionLabels.Where(a => a.Text == label).Select(a => a.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string name = $"Erro ao executar retrieve no atributo {attributeName} da entidade {entityName}";
                
                CreateLog(name, entityName, $"{attributeName}", $"{recordId}", ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Cria Logs
        /// </summary>
        /// <param name="name"> nome do log</param>
        /// <param name="entityname"> nome da entidade em que ocorreu o erro</param>
        /// <param name="recordName"> nome do registro </param>
        /// <param name="recordId"> id do registro </param>
        /// <param name="message"> mensagem de erro </param>
        public void CreateLog(string name, string entityname, string recordName, string recordId, string message)
        {
            smt_log log = new smt_log()
            {
                smt_name = name,
                smt_st_entityname = entityname,
                smt_pl_eventtype = new OptionSetValue(100000000), // Erro
                smt_st_eventorigin = "PostCreateAsync_smt_integration",
                smt_dt_eventdate = DateTime.Now,
                smt_st_recordname = recordName,
                smt_st_recordid = recordId,
                smt_tx_message = message
            };

            ServiceAdmin.Create(log);
        }
    }
}
