using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Pixeon.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Business
{
    /// <summary>
    /// cHAMADA
    /// </summary>
    /// 
    public class Smt_issueBusiness : BaseBusiness
    {
        /// <summary>
        /// a
        /// </summary>
        /// <param name="fields">a</param>
        /// <returns>a</returns>
        public static string GenerateJsonFormat(List<Field> fields)
        {
            StringBuilder JSON = new StringBuilder();
            JSON.Append(@"{""fields"":{");

            for (Int32 i = 0; i < fields.Count; i++)
            {
                JSON.Append(String.Format(@"""{0}"": {1}", fields[i].Name, fields[i].Value));
                if (i < fields.Count - 1)
                    JSON.Append(",");
            }

            JSON.Append("}}");

            return JSON.ToString();
        }

        /// <summary>
        /// Método contrutor para receber alguns parametros precisos para a execução dos métodos da classe.
        /// </summary>
        /// <param name="service">Variavel de Serviço</param>
        /// <param name="serviceAdmin"> ServiceADM</param>
        /// <param name="tracingService">Atributo do tipo IorganizationService, utilizado para executar métodos do SDK do Dynamics CRM</param>
        /// <param name="messages">Mensagens do SDK do Dynamics CRM</param>
        public Smt_issueBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        public string GenerateIssueURL(String dynamicsURI, String logicalname, Guid targetID, Guid appID)
        {
            // https://pixeon.crm2.dynamics.com/main.aspx?appid=1dd779cf-72f2-e911-a811-000d3ac08add&pagetype=entityrecord&etn=smt_issue&id=906cd7a9-8568-ea11-a812-000d3ac06f14
            return String.Format("{0}/main.aspx?pagetype=entityrecord&etn={1}&id={2}", dynamicsURI, logicalname, targetID);

            // return String.Format("{0}/main.aspx?appid={1}&pagetype={2}&etn=smt_issue&id={3}", dynamicsURI, appID, logicalname, targetID);
        }

        public string GetParamDynamicsURI(String parameter)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='smt_smartparameter'>
                                <attribute name='smt_smartparameterid' />
                                <attribute name='smt_name' />
                                <attribute name='smt_value' />
                                <order attribute='smt_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='smt_name' operator='eq' value='" + parameter + @"' />
                                </filter>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            String value = result[0]["smt_value"].ToString();
            return value;
        }

        public void GenerateJson(smt_issue target, smt_issue postImage, Smt_issueBusiness issueBusiness)
        {
            if ((bool)target.smt_bl_integrar)
            {
                smt_issue updateIssue = new smt_issue();
                updateIssue.Id = target.Id;

                string issueURL = GenerateIssueURL(GetParamDynamicsURI("DYNAMICS URI"), updateIssue.LogicalName, target.Id, Guid.Empty);

                switch (postImage.smt_lp_project.Name.ToLower())
                {
                    case "service desk pixeon": 
                        {
                            smt_issue dadosIssue = GetIssueFieldsSSD(target.Id);
                            EntityCollection issueRequestParticipants = issueBusiness.GetIssueApprovers(target.Id);
                            EntityCollection issueApprovals = issueBusiness.GetIssueRequestParticipants(target.Id);
                            updateIssue.smt_jiraapirequestbody = issueBusiness.GenerateJiraBodyCreateSSD(dadosIssue, issueRequestParticipants, issueApprovals, issueURL);
                            updateIssue.smt_jiraapirequestbody2 = issueBusiness.GenerateJiraBodyUpdate(dadosIssue);
                            break;
                        }
                    case "implantação":
                        {
                            smt_issue dadosIssue = GetIssueFieldsIMP(target.Id, issueURL);

                            EntityCollection issueProducts = issueBusiness.GetIssueProducts(target.Id);
                            EntityCollection issueClients = issueBusiness.GetIssueClients(target.Id);
                            updateIssue.smt_jiraapirequestbody = issueBusiness.GenerateJiraBodyCreateIMP(dadosIssue, issueProducts, issueClients, issueURL);
                            break;
                        }
                }

                updateIssue.smt_st_dynrecordurl = issueURL;
                ServiceAdmin.Update(updateIssue);
            }
        }

        public smt_issue GetIssueFieldsSSD(Guid issueId)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='smt_issue'>
                                <attribute name='smt_issueid' />
                                <attribute name='smt_lp_project' />
                                <attribute name='smt_lp_issuetype' />
                                <attribute name='smt_st_summary' />
                                <attribute name='smt_st_version' />
                                <attribute name='smt_lp_tipo' />
                                <attribute name='smt_pl_sistema' />
                                <attribute name='smt_lp_priority' />
                                <attribute name='smt_lp_modulo' />
                                <attribute name='smt_st_description' />
                                <attribute name='smt_dt_datagolive' />
                                <attribute name='smt_lp_projectcontract' />
                                <attribute name='smt_st_dynrecordurl' />
                                <attribute name='smt_pl_conexaotermliberada' />
                                <attribute name='smt_pl_clienteemprod' />
                                <attribute name='createdby' />
                                <order attribute='smt_lp_project' descending='false' />
                                <filter type='and'>
                                  <condition attribute='smt_issueid' operator='eq' uiname='Teste' uitype='smt_issue' value='" + issueId.ToString() + @"' />
                                </filter>
                                <link-entity name='contact' from='contactid' to='smt_pl_requestingperson' visible='false' link-type='outer' alias='contact_jiraid'>
                                  <attribute name='smt_st_jiraid' />
                                </link-entity>
                                <link-entity name='account' from='accountid' to='smt_pl_account' visible='false' link-type='outer' alias='account_jiraid'>
                                  <attribute name='smt_st_jiraid' />
                                  <attribute name='smt_st_document' />
                                </link-entity>
                                <link-entity name='systemuser' from='systemuserid' to='smt_pl_consultant' visible='false' link-type='outer' alias='user_jiraid'>
                                  <attribute name='smt_st_jiraid' />
                                </link-entity>
                                    <link-entity name='systemuser' from='systemuserid' to='createdby' visible='false' link-type='outer' alias='createdby_jiraid'>
                                    <attribute name='smt_st_jiraid' />
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            return (smt_issue)result[0];
        }
        public EntityCollection GetIssueRequestParticipants(Guid issueId)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='systemuser'>
                                <attribute name='fullname' />
                                <attribute name='businessunitid' />
                                <attribute name='title' />
                                <attribute name='address1_telephone1' />
                                <attribute name='positionid' />
                                <attribute name='systemuserid' />
                                <attribute name='smt_st_jiraid' />
                                <order attribute='fullname' descending='false' />
                                <link-entity name='smt_request_participants' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
                                  <link-entity name='smt_issue' from='smt_issueid' to='smt_issueid' alias='ak'>
                                    <filter type='and'>
                                      <condition attribute='smt_issueid' operator='eq' uiname='Teste' uitype='smt_issue' value='" + issueId.ToString() + @"' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            return result;
        }
        public EntityCollection GetIssueApprovers(Guid issueId)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='systemuser'>
                                <attribute name='fullname' />
                                <attribute name='businessunitid' />
                                <attribute name='title' />
                                <attribute name='address1_telephone1' />
                                <attribute name='positionid' />
                                <attribute name='systemuserid' />
                                <attribute name='smt_st_jiraid' />
                                <order attribute='fullname' descending='false' />
                                <link-entity name='smt_smt_issue_systemuser' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
                                  <link-entity name='smt_issue' from='smt_issueid' to='smt_issueid' alias='ai'>
                                    <filter type='and'>
                                      <condition attribute='smt_issueid' operator='eq' uiname='Teste' uitype='smt_issue' value='" + issueId.ToString() + @"' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            return result;
        }

        public SalesOrder GetIssueSalesOrder(Guid salesOrderId)
        {
            ColumnSet cs = new ColumnSet(
                nameof(SalesOrder.smt_st_region),
                nameof(SalesOrder.smt_mn_value_recurrence_final),
                nameof(SalesOrder.smt_st_opp_number),
                nameof(SalesOrder.smt_st_notificationid));

            SalesOrder projectContract = (SalesOrder)ServiceAdmin.Retrieve("salesorder", salesOrderId, cs);

            return projectContract;

        }
        public smt_issue GetIssueFieldsIMP(Guid issueId, String issueURL)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='smt_issue'>
                                <attribute name='smt_issueid' />
                                <attribute name='smt_lp_project' />
                                <attribute name='smt_lp_issuetype' />
                                <attribute name='smt_st_summary' />
                                <attribute name='smt_lp_priority' />
                                <attribute name='smt_pl_regiao' />
                                <attribute name='smt_lp_projectcontract' />
                                <attribute name='smt_dt_dataauditoria' />
                                <attribute name='smt_st_valorrecorrentemensal' />
                                <attribute name='smt_pl_tipolaudo' />
                                <attribute name='smt_lp_tipoimplantacao' />
                                <attribute name='smt_st_numeroop' />
                                <attribute name='smt_st_linknotificacao' />
                                <attribute name='smt_lp_linhadeprodutos' />
                                <attribute name='smt_pl_celulaatendimento' />
                                <attribute name='smt_st_qtdlicencas' />
                                <attribute name='smt_st_pendenciasprojeto' />
                                <attribute name='smt_pl_momento' />
                                <attribute name='smt_st_detalhesprojeto' />
                                <order attribute='smt_lp_project' descending='false' />
                                <filter type='and'>
                                  <condition attribute='smt_issueid' operator='eq' uiname='Teste IMP' uitype='smt_issue' value='" + issueId.ToString() + @"' />
                                </filter>
                                <link-entity name='contact' from='contactid' to='smt_pl_requestingperson' visible='false' link-type='outer' alias='user_jiraid'>
                                  <attribute name='smt_st_jiraid' />
                                </link-entity>
                                <link-entity name='systemuser' from='systemuserid' to='createdby' visible='false' link-type='outer' alias='a_c630b514ce4dea11a812000d3a88793f'>
                                  <attribute name='smt_st_jiraid' />
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            return (smt_issue)result[0];
        }
        public EntityCollection GetIssueProducts(Guid issueId)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='smt_produtoissue'>
                                <attribute name='smt_produtoissueid' />
                                <attribute name='smt_name' />
                                <attribute name='createdon' />
                                <order attribute='smt_name' descending='false' />
                                <link-entity name='smt_imp_produtos' from='smt_produtoissueid' to='smt_produtoissueid' visible='false' intersect='true'>
                                  <link-entity name='smt_issue' from='smt_issueid' to='smt_issueid' alias='ae'>
                                    <filter type='and'>
                                      <condition attribute='smt_issueid' operator='eq' uiname='Teste IMP' uitype='smt_issue' value='" + issueId.ToString() + @"' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            return result;
        }
        public EntityCollection GetIssueClients(Guid issueId)
        {
            String xml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
                              <entity name='systemuser'>
                                <attribute name='fullname' />
                                <attribute name='systemuserid' />
                                <attribute name='smt_st_jiraid' />
                                <order attribute='fullname' descending='false' />
                                <link-entity name='smt_imp_clientes' from='systemuserid' to='systemuserid' visible='false' intersect='true'>
                                  <link-entity name='smt_issue' from='smt_issueid' to='smt_issueid' alias='ac'>
                                    <filter type='and'>
                                      <condition attribute='smt_issueid' operator='eq' uiname='Teste IMP' uitype='smt_issue' value='" + issueId.ToString() + @"' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                              </entity>
                            </fetch>";

            EntityCollection result = ServiceAdmin.RetrieveMultiple(new FetchExpression(xml));

            return result;

        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="issue">a</param>
        /// <param name="requestParticipants">a</param>
        /// <param name="approvals">a</param>
        /// <param name="issueURL">a</param>
        /// <returns>a</returns>
        public string GenerateJiraBodyCreateSSD(smt_issue issue, EntityCollection requestParticipants, EntityCollection approvals, String issueURL)
        {
            #region comentado Corpo da requisição SSD
            /* CORPO DA REQUISIÇÃO SSD
            {
                "fields": {
                    "project": { "key": "SSD"},
                   "issuetype": { "name": "Implantações"},
                   "customfield_32705": [{"name":"android@pixeon.com"}],
                   "summary": "Teste POSTMAN",
                   "customfield_33408": {"id":"sys-adm"},
                   "description": "Creating of an issue using project keys and issue type names using the REST API",
                   "customfield_31739": ["Beira Leito" ], 
                   "customfield_31740": ["Beira Leito" ], 
                   "customfield_31738": ["Beira Leito" ], 
                   "priority": {"name":"Baixo"},
                   "customfield_31529": "0.0.0.0",
                   "customfield_31467": {"id":"36197"},
                   "customfield_31578": {"id":"36394"},
                   "customfield_31579": "2020-02-21",
                   "customfield_32101": ["05.668.779/0001-89"], 
                   "customfield_15100": [{"id":"sys-adm"},{"id":"felipe.carlos"}],
                   "customfield_21101": [{"id":"sys-adm"},{"id":"felipe.carlos"}]
               }
             } 
             */
            #endregion

            String Body = string.Empty;
            List<Field> fieldsSSD = new List<Field>();

            fieldsSSD.Add(new Field("project", String.Format(@"{{""key"": ""{0}""}}", "SSD")));                                                             // Projeto, (Obrigatório)
            fieldsSSD.Add(new Field("issuetype", String.Format(@"{{""name"": ""{0}""}}", issue.smt_lp_issuetype.Name)));                                    // Tipo de Issue (Obrigatório)

            if (issue.Contains("createdby_jiraid.smt_st_jiraid"))
                fieldsSSD.Add(new Field("customfield_32705", String.Format(@"[{{""name"": ""{0}""}}]", ((AliasedValue)issue["createdby_jiraid.smt_st_jiraid"]).Value)));            // Solicitante
            fieldsSSD.Add(new Field("summary", String.Format(@"""{0}""", issue.smt_st_summary)));                                                             // Resumo (Obrigatório)
            if (issue.Contains("user_jiraid.smt_st_jiraid"))
                fieldsSSD.Add(new Field("customfield_33408", String.Format(@"{{""id"": ""{0}""}}", ((AliasedValue)issue["user_jiraid.smt_st_jiraid"]).Value)));               // Contato Consultor
            if (issue.Contains(nameof(issue.smt_st_description)))
                fieldsSSD.Add(new Field("description", String.Format(@"""{0}\n\nRegistro no PSA: {1}""", issue.smt_st_description, issueURL)));                                               // Descrição
            else
                fieldsSSD.Add(new Field("description", String.Format(@"""{0}""", issue.smt_st_dynrecordurl)));                                               // Descrição
            if (issue.Contains(nameof(issue.smt_pl_sistema)))
                fieldsSSD.Add(new Field("customfield_31739", String.Format(@"[""{0}""]", issue.smt_pl_sistema.Name)));                                      // Sistema
            if (issue.Contains(nameof(issue.smt_lp_tipo)))
                fieldsSSD.Add(new Field("customfield_31740", String.Format(@"[""{0}""]", issue.smt_lp_tipo.Name)));                                         // Tipo
            if (issue.Contains(nameof(issue.smt_lp_modulo)))
                fieldsSSD.Add(new Field("customfield_31738", String.Format(@"[""{0}""]", issue.smt_lp_modulo.Name)));                                       // Modulo
            if (issue.Contains(nameof(issue.smt_st_version)))
                fieldsSSD.Add(new Field("customfield_31529", String.Format(@"""{0}""", issue.smt_st_version)));                                           // Versão
            if (issue.Contains(nameof(issue.smt_lp_priority)))
                fieldsSSD.Add(new Field("priority", String.Format(@"{{""name"": ""{0}""}}", issue.smt_lp_priority.Name)));                                  // Prioridade
            if (issue.Contains(nameof(issue.smt_pl_conexaotermliberada)))
                fieldsSSD.Add(new Field("customfield_31467", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_pl_conexaotermliberada)])));                   // Conexão Tera Term Liberada?
            if (issue.Contains(nameof(issue.smt_pl_clienteemprod)))
                fieldsSSD.Add(new Field("customfield_31578", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_pl_clienteemprod)])));     // Cliente está em produção?
            if (issue.Contains(nameof(issue.smt_dt_datagolive)))
                fieldsSSD.Add(new Field("customfield_31579", String.Format(@"""{0}""", ((DateTime)issue.smt_dt_datagolive).ToString("yyyy-MM-dd"))));     // Data do Golive
            if (issue.Contains("account_jiraid.smt_st_document"))
                fieldsSSD.Add(new Field("customfield_32101", String.Format(@"[""{0}""]", ((AliasedValue)issue["account_jiraid.smt_st_document"]).Value)));                        // Organização (CNPJ Pesquisa)

            if (requestParticipants.Entities.Count > 0)
            {
                StringBuilder strParticipants = new StringBuilder();
                strParticipants.Append("[");

                for (Int32 i = 0; i < requestParticipants.Entities.Count; i++)
                {
                    strParticipants.Append(String.Format(@"{{""id"": ""{0}""}}", requestParticipants.Entities[i]["smt_st_jiraid"]));
                    if (i < requestParticipants.Entities.Count - 1)
                        strParticipants.Append(",");
                }

                strParticipants.Append("]");

                fieldsSSD.Add(new Field("customfield_15100", strParticipants.ToString()));
            }

            if (approvals.Entities.Count > 0)
            {
                StringBuilder strApprovals = new StringBuilder();
                strApprovals.Append("[");

                for (Int32 i = 0; i < approvals.Entities.Count; i++)
                {
                    strApprovals.Append(String.Format(@"{{""id"": ""{0}""}}", approvals.Entities[i]["smt_st_jiraid"]));
                    if (i < requestParticipants.Entities.Count - 1)
                        strApprovals.Append(",");
                }

                strApprovals.Append("]");
                fieldsSSD.Add(new Field("customfield_21101", strApprovals.ToString()));
            }

            Body = GenerateJsonFormat(fieldsSSD);
            Body = Body.Replace("\n", "\\n");
            return Body;
        }

        public string GenerateJiraBodyCreateIMP(smt_issue issue, EntityCollection products, EntityCollection clients, String issueURL)
        {
            /* CORPO DA REQUISIÇÃO IMP
            {
                "fields": {
                   "project": {"key": "IMP"},
                   "issuetype": {"name": "Transição"},
                   "summary": "Teste Hand Over POSTMAN",
                   "reporter": {"name":"android@pixeon.com"},
                   "priority": {"name":"NAB"},
                   "customfield_30800": "2020-02-21",
                   "customfield_20407": [{"id":"sys-adm"},{"id":"felipe.carlos"}],
                   "customfield_14402": {"value":"Sul"},
                   "customfield_21702": "http://teste",
                   "customfield_19701": "asd123",
                   "customfield_29000": "asdf1234",
                   "customfield_21706": [{"value":"PACS AURORA"},{"value":"SMARTCLIN"},{"value":"PIXSAFE"}],
                   "customfield_12801": {"value":"SMARTCLIN"},
                   "customfield_24500": {"value":"Com imagem"},
                   "customfield_21701": {"value":"G1"},
                   "customfield_21703": {"value":"Suporte"},
                   "customfield_21801": [{"name":"Controle"},{"name":"Produtividade"}],
                   "customfield_21705": "00 licencas",
                   "customfield_22500": "pendenciasdoprojeto",
                   "customfield_17500": "detalhedoprojeto"
               }
            }
         */

            String Body = string.Empty;
            List<Field> fieldsIMP = new List<Field>();

            fieldsIMP.Add(new Field("project", String.Format(@"{{""key"": ""{0}""}}", "IMP")));                                                             // Projeto, (Obrigatório)
            fieldsIMP.Add(new Field("issuetype", String.Format(@"{{""name"": ""{0}""}}", issue.smt_lp_issuetype.Name)));                                    // Tipo de Issue (Obrigatório)
            fieldsIMP.Add(new Field("summary", String.Format(@"""{0}""", issue.smt_st_summary)));                                                           // Resumo (Obrigatório)
            if (issue.Contains("createdby_jiraid.smt_st_jiraid"))
                fieldsIMP.Add(new Field("reporter", String.Format(@"[{{""name"": ""{0}""}}]", ((AliasedValue)issue["createdby_jiraid.smt_st_jiraid"]).Value)));    // Solicitante
            if (issue.Contains(nameof(issue.smt_lp_priority)))
                fieldsIMP.Add(new Field("priority", String.Format(@"{{""name"": ""{0}""}}", issue.smt_lp_priority.Name)));                                  // Prioridade
            if (issue.Contains(nameof(issue.smt_dt_dataauditoria)))
                fieldsIMP.Add(new Field("customfield_30800", String.Format(@"""{0}""", ((DateTime)issue.smt_dt_dataauditoria).ToString("yyyy-MM-dd"))));    // Data da Auditoria
            if (issue.Contains(nameof(issue.smt_pl_regiao)))
               fieldsIMP.Add(new Field("customfield_14402", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_pl_regiao)]))); // Região
            // if (issue.Contains(nameof(issue.smt_st_linknotificacao)))
            //    fieldsIMP.Add(new Field("customfield_21702", String.Format(@"""{0}""", issue.smt_st_linknotificacao)));                                      // Link da notificação (Salesforce)
            // if (issue.Contains(nameof(issue.smt_st_numeroop)))
            //    fieldsIMP.Add(new Field("customfield_19701", String.Format(@"""{0}""", issue.smt_st_numeroop)));                                            // Numero OP
            // if (issue.Contains(nameof(issue.smt_st_valorrecorrentemensal)))
            //    fieldsIMP.Add(new Field("customfield_29000", String.Format(@"""{0}""", issue.smt_st_valorrecorrentemensal)));                               // Valor Recorrente Mensal
            if (issue.Contains(nameof(issue.smt_lp_linhadeprodutos)))
                fieldsIMP.Add(new Field("customfield_12801", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_lp_linhadeprodutos)]))); // Linha de Produtos            
            if (issue.Contains(nameof(issue.smt_pl_tipolaudo)))
                fieldsIMP.Add(new Field("customfield_24500", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_pl_tipolaudo)]))); // Tipo de Laudo 
            if (issue.Contains(nameof(issue.smt_pl_celulaatendimento)))
                fieldsIMP.Add(new Field("customfield_21701", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_pl_celulaatendimento)]))); // Célula de Atendimento            
            if (issue.Contains(nameof(issue.smt_lp_tipoimplantacao)))
                fieldsIMP.Add(new Field("customfield_21703", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(issue.smt_lp_tipoimplantacao)]))); // Tipo de Implantação           
            if (issue.Contains(nameof(issue.smt_st_qtdlicencas)))
                fieldsIMP.Add(new Field("customfield_21705", String.Format(@"""{0}""", issue.smt_st_qtdlicencas)));                                            // Quantidade de Licenças
            if (issue.Contains(nameof(issue.smt_st_pendenciasprojeto)))
                fieldsIMP.Add(new Field("customfield_22500", String.Format(@"""{0}""", issue.smt_st_pendenciasprojeto)));                                    // Pendências do projeto
            if (issue.Contains(nameof(issue.smt_st_detalhesprojeto)))
                fieldsIMP.Add(new Field("customfield_17500", String.Format(@"""{0}\n\nRegistro no PSA: {1}""", issue.smt_st_detalhesprojeto, issueURL)));                                  // Detalhes ou Informações adicionais do Projeto
            else
                fieldsIMP.Add(new Field("customfield_17500", String.Format(@"""{0}""", issue.smt_st_dynrecordurl)));                                  // Detalhes ou Informações adicionais do Projeto

            if (issue.Contains("account_jiraid.smt_st_document"))
                fieldsIMP.Add(new Field("customfield_32101", String.Format(@"[""{0}""]", ((AliasedValue)issue["account_jiraid.smt_st_document"]).Value)));                        // Organização (CNPJ Pesquisa)

            if (issue.Contains(nameof(issue.smt_lp_projectcontract)))
            {
                SalesOrder projectContract = GetIssueSalesOrder(issue.smt_lp_projectcontract.Id);

                // if (issue.Contains(nameof(projectContract.smt_st_region)))
                //    fieldsIMP.Add(new Field("customfield_14402", String.Format(@"{{""value"": ""{0}""}}", issue.FormattedValues[nameof(projectContract.smt_st_region)]))); // Região
                if (projectContract.Contains(nameof(projectContract.smt_st_notificationid)))
                        fieldsIMP.Add(new Field("customfield_21702", String.Format(@"""{0}""", projectContract.smt_st_notificationid)));                                      // Link da notificação (Salesforce)
                if (projectContract.Contains(nameof(projectContract.smt_st_opp_number)))
                    fieldsIMP.Add(new Field("customfield_19701", String.Format(@"""{0}""", projectContract.smt_st_opp_number)));                                            // Numero OP
                if (projectContract.Contains(nameof(projectContract.smt_mn_value_recurrence_final)))
                    fieldsIMP.Add(new Field("customfield_29000", String.Format(@"""{0}""", projectContract.FormattedValues[nameof(projectContract.smt_mn_value_recurrence_final)])));                               // Valor Recorrente Mensal
            }

            if (issue.Contains(nameof(issue.smt_pl_momento)))
            {
                StringBuilder strMomento = new StringBuilder();
                strMomento.Append("[");

                String[] momento = issue.FormattedValues[nameof(issue.smt_pl_momento)].ToString().Split(';');
                
                for (Int32 i = 0; i < momento.Length; i++)
                {
                    strMomento.Append(String.Format(@"{{""value"": ""{0}""}}", momento[i].Trim()));
                    if (i < momento.Length - 1)
                        strMomento.Append(",");
                }
                strMomento.Append("]");

                fieldsIMP.Add(new Field("customfield_21801", strMomento.ToString()));
            }

            if (products.Entities.Count > 0)
            {
                StringBuilder strProducts = new StringBuilder();
                strProducts.Append("[");

                for (Int32 i = 0; i < products.Entities.Count; i++)
                {
                    strProducts.Append(String.Format(@"{{""value"": ""{0}""}}", products.Entities[i]["smt_name"]));
                    if (i < products.Entities.Count - 1)
                        strProducts.Append(",");
                }

                strProducts.Append("]");

                fieldsIMP.Add(new Field("customfield_21706", strProducts.ToString()));
            }

            if (clients.Entities.Count > 0)
            {
                StringBuilder strClients = new StringBuilder();
                strClients.Append("[");

                for (Int32 i = 0; i < clients.Entities.Count; i++)
                {
                    strClients.Append(String.Format(@"{{""id"": ""{0}""}}", clients.Entities[i]["smt_st_jiraid"]));
                    if (i < clients.Entities.Count - 1)
                        strClients.Append(",");
                }

                strClients.Append("]");
                fieldsIMP.Add(new Field("customfield_20407", strClients.ToString()));
            }

            Body = GenerateJsonFormat(fieldsIMP);
            Body = Body.Replace("\n", "\\n");
            return Body;
        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="issue">a</param>
        /// <returns>retorna corpo da requisição</returns
        public string GenerateJiraBodyUpdate(smt_issue issue)
        {
            // TODO: [Elton] Método para obter o campo ID Jira da Conta
            Int32 jiraOrgCode = 2602;

            if (issue.Contains("account_jiraid.smt_st_jiraid"))
                jiraOrgCode = Int32.Parse(((AliasedValue)issue["account_jiraid.smt_st_jiraid"]).Value.ToString());

            StringBuilder sb = new StringBuilder();
            sb.Append(@"{""fields"": {""customfield_23900"": [" + jiraOrgCode.ToString() + "]}}");  // Campo "Organização"
            return sb.ToString();
        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="issue">a</param>
        /// <returns>retorna corpo da requisição</returns>
        public string GenerateJiraBodyAssign(smt_issue issue)
        {
            String jiraUser = String.Empty;

            if (issue.Contains("createdby_jiraid.smt_st_jiraid"))
               jiraUser = ((AliasedValue)issue["createdby_jiraid.smt_st_jiraid"]).Value.ToString();  

            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrEmpty(jiraUser))
            {
                sb.Append(String.Format(@"{{""name"": ""{0}""}}", jiraUser));  // Campo "Responsável"
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// a
    /// </summary>
    public struct Field
    {
        public string Name;
        public string Value;

        /// <summary>
        /// a
        /// 
        /// </summary>
        /// <param name="name">a</param>
        /// <param name="value">a</param>
        public Field(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

}
