using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Smart Parameter Business
    /// </summary>
    public class SmartParameterBusiness : BaseBusiness
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="serviceAdmin"> ServiceADM</param>
        /// <param name="tracingService">Atributo do tipo IorganizationService, utilizado para executar métodos do SDK do Dynamics CRM</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        public SmartParameterBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Busca o parâmetro de acordo com o nome e a unidade organizacional.
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro</param>
        /// <param name="organizationUnitId">Unidade organizacional</param>
        /// <returns>Retorna o registro com o nome e unidade organizacional de entrada, retorna null se a combinação não existir</returns>
        public smt_smartparameter FindParameter(string parameterName, EntityReference organizationUnitId)
        {
            if (parameterName != null && parameterName != String.Empty)
            {
                if (organizationUnitId == null)
                    return ParameterGlobal(parameterName);
                else
                    return ParameterOrganizationalUnit(parameterName, organizationUnitId);
            }
            return null;
        }

        /// <summary>
        /// Verifica se o nome do parâmetro é único para o context dele. Usado no momento de criação do Parâmetro.
        /// </summary>
        /// <param name="smartParameter">Verifica se existe um parâmetro com o mesmo nome dentro da unidade organizacional(ou entre os parâmetros globais)</param>
        /// <param name="messages">y</param>
        public void ParameterName_IsUnique_Creation(smt_smartparameter smartParameter, List<Resx> messages)
        {
            if (smartParameter != null)
            {
                smt_smartparameter parameter = FindParameter(smartParameter.smt_name, smartParameter.smt_organizationalunit);
                if (parameter != null)
                    throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.ParameterAlreadyExist));
            }
        }

        /// <summary>
        /// Busca o parâmetro de unidade organizacional, de acordo com o nome.
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro</param>
        /// <param name="organizationUnitId">Unidade Organizacional</param>
        /// <returns>Retorna o registro com esse nome na unidade organizacional</returns>
        public smt_smartparameter ParameterOrganizationalUnit(string parameterName, EntityReference organizationUnitId)
        {
            if (parameterName != null && parameterName != String.Empty)
            {
                var query = new QueryExpression(smt_smartparameter.EntityLogicalName);
                query.ColumnSet = new ColumnSet(new string[] { "smt_value" });
                query.Criteria.AddCondition("smt_name", ConditionOperator.Equal, parameterName);
                query.Criteria.AddCondition("smt_organizationalunit", ConditionOperator.Equal, organizationUnitId.Id);
                EntityCollection entityCollection = Service.RetrieveMultiple(query);

                if (entityCollection.Entities.Count > 0)
                {
                    smt_smartparameter parameter = (smt_smartparameter)entityCollection.Entities.FirstOrDefault();
                    return parameter;
                }
            }
            return null;
        }

        /// <summary>
        /// Busca o parâmetro global, de acordo com o nome.
        /// </summary>
        /// <param name="parameterName">Nome do Parâmetro</param>
        /// <returns>Retorna o parâmetro global com o nome dado, retorna null se ele não existir.</returns>
        public smt_smartparameter ParameterGlobal(string parameterName)
        {
            if (parameterName != null && parameterName != string.Empty)
            {
                var query = new QueryExpression(smt_smartparameter.EntityLogicalName);
                query.ColumnSet = new ColumnSet(new string[] { "smt_value" });
                query.Criteria.AddCondition("smt_name", ConditionOperator.Equal, parameterName);
                query.Criteria.AddCondition("smt_organizationalunit", ConditionOperator.Null);
                EntityCollection entityCollection = Service.RetrieveMultiple(query);

                if (entityCollection.Entities.Count > 0)
                {
                    smt_smartparameter parameter = (smt_smartparameter)entityCollection.Entities.FirstOrDefault();
                    return parameter;
                }
            }
            return null;
        }

        /// <summary>
        /// Retorna os registros de parametro de acordo com a unidade organizacional do recurso 
        /// </summary>
        /// <param name="UnidadeOrg">OrgUnidade</param>
        /// <returns>Return</returns>
        /// 
        public String ParametroSmartHoraProjeto (Guid UnidadeOrg)
        {
            // Verifica se o parametro possuí a mesma unidade organizacional do recurso reservável (Referente a horário mínimo do projeto)

            String HorasProjeto = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='smt_smartparameter'>
                                        <attribute name='smt_smartparameterid' />
                                        <attribute name='smt_name' />
                                        <attribute name='smt_value' />
                                        <attribute name='smt_organizationalunit' />
                                        <order attribute='smt_name' descending='false' />
                                        <filter type='and'>
                                          <condition attribute='smt_name' operator='eq' value='GET MINIMUM HOURS PER PROJECT' />
                                          <condition attribute='smt_organizationalunit' operator='eq' uitype='msdyn_organizationalunit' value='{"+ UnidadeOrg + @"}' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            Entity RetornoParametroSmartHoraProjeto = Service.RetrieveMultiple(new FetchExpression(HorasProjeto)).Entities.FirstOrDefault();

            // Verifica o parametro que não possuí a unidade organizacional (Referente a horário mínimo do projeto)
            String HorasProjetoSemUnidade = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                                  <entity name='smt_smartparameter'>
                                                    <attribute name='smt_smartparameterid' />
                                                    <attribute name='smt_name' />
                                                    <attribute name='smt_value' />
                                                    <attribute name='smt_organizationalunit' />
                                                    <order attribute='smt_name' descending='false' />
                                                    <filter type='and'>
                                                      <condition attribute='smt_name' operator='eq' value='GET MINIMUM HOURS PER PROJECT' />
                                                      <condition attribute='smt_organizationalunit' operator='null' />
                                                    </filter>
                                                  </entity>
                                                </fetch>";

            Entity RetornoParametroSmartHoraProjetoSemUnidade = Service.RetrieveMultiple(new FetchExpression(HorasProjetoSemUnidade)).Entities.FirstOrDefault();

            // Verifica qual dos dois fetchs será usado
            if (RetornoParametroSmartHoraProjeto != null)
            {
                return RetornoParametroSmartHoraProjeto["smt_value"].ToString();
            }
            else if (RetornoParametroSmartHoraProjetoSemUnidade != null)
            {
                return RetornoParametroSmartHoraProjetoSemUnidade["smt_value"].ToString();
            }
            else
            {
                return null;
            }
           
        }
    }
}
