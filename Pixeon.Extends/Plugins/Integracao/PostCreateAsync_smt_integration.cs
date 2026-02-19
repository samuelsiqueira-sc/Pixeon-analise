using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Plugins.Integracao
{
    /// <summary>
    /// Inicio
    /// </summary>
    public class PostCreateAsync_smt_integration : PluginBase
    {
        /// <summary>
        /// Inicio
        /// </summary>
        public PostCreateAsync_smt_integration() : base(typeof(PostCreateAsync_smt_integration))
        { }
        /// <summary>
        /// Chamada
        /// </summary>
        /// <param name="localcontext">Contexto</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            smt_integration target = localcontext.GetTarget<smt_integration>();
            IntegracaoBusiness business = new IntegracaoBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

            if (target.smt_st_entitylogicalname == "account")
            {
                TreatDataAccount(target, localcontext, business);
            }

            if (target.smt_st_entitylogicalname == "quote")
            {
                TreatDataQuote(target, localcontext, business);
            }

            if (target.smt_st_entitylogicalname == "contact")
            {
                TreatDataContact(target, localcontext, business);
            }
        }
        /// <summary>
        /// Target
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="localcontext">local</param>
        /// <param name="business"> business de Integração </param>
        protected void TreatDataAccount(smt_integration target, LocalPluginContext localcontext, IntegracaoBusiness business)
        {
            Account account = new Account();
            account.Id = new Guid(target.smt_smt_st_registerId);

            #region Active Channels
            if (target.smt_name == "smt_mc_active_channels")
            {
                OptionSetValueCollection channels = new OptionSetValueCollection();

                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "P01 - FLOW":
                            channels.Add(new OptionSetValue(180580000));
                            break;

                        case "P01 - PACS":
                            channels.Add(new OptionSetValue(180580001));
                            break;

                        case "P02 - FLOW":
                            channels.Add(new OptionSetValue(180580002));
                            break;

                        case "P02 - PACS":
                            channels.Add(new OptionSetValue(180580003));
                            break;

                        case "P03 - FLOW":
                            channels.Add(new OptionSetValue(180580004));
                            break;

                        case "P03 - PACS":
                            channels.Add(new OptionSetValue(180580005));
                            break;
                        case "P04 - PACS":
                            channels.Add(new OptionSetValue(180580006));
                            break;

                        case "P05 - FLOW":
                            channels.Add(new OptionSetValue(180580007));
                            break;

                        case "P05 - PACS":
                            channels.Add(new OptionSetValue(180580008));
                            break;

                        case "P11 - PACS":
                            channels.Add(new OptionSetValue(180580009));
                            break;

                        case "P12 - PACS":
                            channels.Add(new OptionSetValue(180580010));
                            break;
                        case "P13 - FLOW":
                            channels.Add(new OptionSetValue(180580011));
                            break;

                        case "P20 - PAC":
                            channels.Add(new OptionSetValue(180580012));
                            break;

                        case "IAN - FLOW":
                            channels.Add(new OptionSetValue(180580013));
                            break;

                        case "IAN - CV":
                            channels.Add(new OptionSetValue(180580014));
                            break;

                        case "SendReport - CV":
                            channels.Add(new OptionSetValue(180580015));
                            break;

                        default:
                            break;
                    }
                }
                account.smt_mc_active_channels = channels;
            }
            #endregion

            #region Modulo PACS
            if (target.smt_name == "smt_pl_module_pacs")
            {
                OptionSetValueCollection pacs = new OptionSetValueCollection();
                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "AuroraMaster":
                            pacs.Add(new OptionSetValue(1));
                            break;

                        case "Distribuição Interna":
                            pacs.Add(new OptionSetValue(2));
                            break;

                        case "WebPortal":
                            pacs.Add(new OptionSetValue(3));
                            break;

                        case "MedReport":
                            pacs.Add(new OptionSetValue(4));
                            break;

                        case "Worklist":
                            pacs.Add(new OptionSetValue(5));
                            break;

                        case "VirtualWorklist":
                            pacs.Add(new OptionSetValue(6));
                            break;
                        case "Fluens":
                            pacs.Add(new OptionSetValue(7));
                            break;

                        case "Lumine":
                            pacs.Add(new OptionSetValue(8));
                            break;

                        case "LTA":
                            pacs.Add(new OptionSetValue(9));
                            break;

                        default:
                            break;
                    }
                }
                account.smt_pl_module_pacs = pacs;
            }
            #endregion

            #region ClickVita
            if (target.smt_name == "smt_pl_clickvita")
            {
                OptionSetValueCollection clickVita = new OptionSetValueCollection();
                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "Distribuição de Imagens e Laudos":
                            clickVita.Add(new OptionSetValue(1));
                            break;

                        case "Distribuição de Laudos":
                            clickVita.Add(new OptionSetValue(2));
                            break;

                        default:
                            break;
                    }
                }
                account.smt_pl_clickvita = clickVita;
            }
            #endregion

            #region Modulo SMART
            if (target.smt_name == "smt_pl_module_smart")
            {
                OptionSetValueCollection smart = new OptionSetValueCollection();

                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "AGENDAMENTO":
                            smart.Add(new OptionSetValue(1));
                            break;

                        case "ATENDIMENTO AO PACIENTE":
                            smart.Add(new OptionSetValue(2));
                            break;

                        case "CENTRO CIRÚRGICO E OBSTÉTRICO":
                            smart.Add(new OptionSetValue(3));
                            break;

                        case "INTERNAÇÃO HOSPITALAR":
                            smart.Add(new OptionSetValue(4));
                            break;

                        case "PAINEL / TOTEM":
                            smart.Add(new OptionSetValue(5));
                            break;

                        case "PRE-INTERNAÇÃO (CAPTAÇÃO)":
                            smart.Add(new OptionSetValue(6));
                            break;
                        case "ASSISTÊNCIA MEDICA E DE ENFERMAGEM":
                            smart.Add(new OptionSetValue(7));
                            break;

                        case "CCIH":
                            smart.Add(new OptionSetValue(8));
                            break;

                        case "COMUNICAÇÃO COM APARELHOS":
                            smart.Add(new OptionSetValue(9));
                            break;

                        case "CONSULTÓRIO MEDICO":
                            smart.Add(new OptionSetValue(10));
                            break;

                        case "LABORATÓRIO CLINICO":
                            smart.Add(new OptionSetValue(11));
                            break;
                        case "NUTRIÇÃO":
                            smart.Add(new OptionSetValue(12));
                            break;

                        case "PESQUISA CIENTIFICA":
                            smart.Add(new OptionSetValue(13));
                            break;

                        case "WEB LAUDOS":
                            smart.Add(new OptionSetValue(14));
                            break;

                        case "CONTABILIDADE":
                            smart.Add(new OptionSetValue(15));
                            break;

                        case "FATURAMENTO":
                            smart.Add(new OptionSetValue(16));
                            break;

                        case "MODULO SUS":
                            smart.Add(new OptionSetValue(17));
                            break;

                        case "ORÇAMENTO E CUSTOS":
                            smart.Add(new OptionSetValue(18));
                            break;

                        case "TESOURARIA":
                            smart.Add(new OptionSetValue(19));
                            break;

                        case "ESTOQUE":
                            smart.Add(new OptionSetValue(20));
                            break;

                        case "PESQUISA DE AVALIAÇÃO":
                            smart.Add(new OptionSetValue(21));
                            break;

                        case "SISTEMA DE INFORMAÇÕES EXECUTIVAS":
                            smart.Add(new OptionSetValue(22));
                            break;
                        case "CONTROLE DE FLUXO DE PRONTUARIO":
                            smart.Add(new OptionSetValue(23));
                            break;

                        case "LAVANDERIA":
                            smart.Add(new OptionSetValue(24));
                            break;

                        case "MANUTENÇÃO":
                            smart.Add(new OptionSetValue(25));
                            break;

                        default:
                            break;
                    }
                }
                account.smt_pl_module_smart = smart;
            }
            #endregion

            #region Modulo XCLINIC
            if (target.smt_name == "smt_pl_module_xclinic")
            {
                OptionSetValueCollection moduleXClinic = new OptionSetValueCollection();

                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "Worklist":
                            moduleXClinic.Add(new OptionSetValue(1));
                            break;

                        case "Integração PACS - Centro de Laudos":
                            moduleXClinic.Add(new OptionSetValue(2));
                            break;

                        case "Integração PACS - Envio de Laudos":
                            moduleXClinic.Add(new OptionSetValue(3));
                            break;

                        case "Integração Orizon":
                            moduleXClinic.Add(new OptionSetValue(4));
                            break;

                        case "LIS Interfaceamento":
                            moduleXClinic.Add(new OptionSetValue(5));
                            break;

                        case "LabGroup":
                            moduleXClinic.Add(new OptionSetValue(6));
                            break;

                        case "BI":
                            moduleXClinic.Add(new OptionSetValue(7));
                            break;

                        case "Emissão NFS-e":
                            moduleXClinic.Add(new OptionSetValue(8));
                            break;

                        case "Central de Comunicação":
                            moduleXClinic.Add(new OptionSetValue(9));
                            break;

                        case "Indicadores de ONA":
                            moduleXClinic.Add(new OptionSetValue(10));
                            break;

                        case "Dashboard Web":
                            moduleXClinic.Add(new OptionSetValue(11));
                            break;
                        case "Totem":
                            moduleXClinic.Add(new OptionSetValue(12));
                            break;

                        case "+ Emissor de Senhas":
                            moduleXClinic.Add(new OptionSetValue(13));
                            break;

                        case "+ Painel de chamada TV":
                            moduleXClinic.Add(new OptionSetValue(14));
                            break;

                        case "+ Biometria":
                            moduleXClinic.Add(new OptionSetValue(15));
                            break;

                        case "Voice Explorer":
                            moduleXClinic.Add(new OptionSetValue(16));
                            break;

                        case "+ Reconhecimento de Voz":
                            moduleXClinic.Add(new OptionSetValue(17));
                            break;

                        case "Laudos.NET":
                            moduleXClinic.Add(new OptionSetValue(18));
                            break;

                        case "Laudos.NET - RIS":
                            moduleXClinic.Add(new OptionSetValue(19));
                            break;

                        case "Laudos.NET - LIS":
                            moduleXClinic.Add(new OptionSetValue(20));
                            break;

                        default:
                            break;
                    }
                }
                account.smt_pl_module_xclinic = moduleXClinic;
            }
            #endregion

            #region Moment XCLINIC
            if (target.smt_name == "smt_pl_moment_xclinic")
            {
                OptionSetValueCollection momentClickVita = new OptionSetValueCollection();
                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "Controle":
                            momentClickVita.Add(new OptionSetValue(1));
                            break;

                        case "Gestão":
                            momentClickVita.Add(new OptionSetValue(2));
                            break;

                        case "Produtividade":
                            momentClickVita.Add(new OptionSetValue(3));
                            break;

                        case "Relacionamento":
                            momentClickVita.Add(new OptionSetValue(4));
                            break;
                        default:
                            break;
                    }
                }
                account.smt_pl_moment_xclinic = momentClickVita;
            }
            #endregion

            #region Tratar picklists
            if (target.smt_name != "smt_pl_module_pacs" && target.smt_name != "smt_pl_clickvita" && target.smt_name != "smt_pl_module_smart" && target.smt_name != "smt_pl_module_xclinic" && target.smt_name != "smt_pl_moment_xclinic")
            {
                if (target.smt_name.Contains("smt_pl_"))
                {
                    int? value = business.GetOptionSetValueByLabel(Account.EntityLogicalName, target.smt_name, target.smt_st_jsonbody, new Guid(target.smt_smt_st_registerId));

                    if (value != null)
                        account.Attributes[$"{target.smt_name}"] = new OptionSetValue((int)value);
                    else
                    {
                        string name = $"Erro ao atualizar {target.smt_st_entitylogicalname}";
                        string message = $"Erro ao atribuir o valor {target.smt_st_jsonbody} ao campo {target.smt_name} da entidade {target.smt_st_entitylogicalname}";
                        business.CreateLog(name, target.smt_st_entitylogicalname, target.smt_name, $"{new Guid(target.smt_smt_st_registerId)}", message);
                    }
                }
            }
            #endregion

            try
            {
                account.EntityState = EntityState.Changed;
                localcontext.OrganizationServiceAdmin.Update(account);
            }
            catch (Exception ex)
            {
                business.CreateLog("Erro ao atualizar optionset da integração de conta", Account.EntityLogicalName, target.smt_name, $"{new Guid(target.smt_smt_st_registerId)}", $"{ex.Message}");
            }
        }

        /// <summary>
        /// Tratar os picklist de cotação
        /// </summary>
        /// <param name="target">target de integração</param>
        /// <param name="localcontext">Contexto</param>
        /// <param name="business"> business de Integração </param>
        protected void TreatDataQuote(smt_integration target, LocalPluginContext localcontext, IntegracaoBusiness business)
        {
            Quote quote = new Quote();
            quote.Id = new Guid(target.smt_smt_st_registerId);

            #region Responsible
            if (target.smt_name == "smt_pl_responsible_coordination")
            {
                OptionSetValueCollection responsible = new OptionSetValueCollection();
                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "CONNE":
                            responsible.Add(new OptionSetValue(1));
                            break;

                        case "SUDESTE":
                            responsible.Add(new OptionSetValue(2));
                            break;

                        case "SUL":
                            responsible.Add(new OptionSetValue(3));
                            break;

                        case "SMART":
                            responsible.Add(new OptionSetValue(4));
                            break;

                        case "FLOW":
                            responsible.Add(new OptionSetValue(5));
                            break;

                        case "Pleres":
                            responsible.Add(new OptionSetValue(6));
                            break;

                        case "Communis":
                            responsible.Add(new OptionSetValue(7));
                            break;

                        case "Serviços Especiais":
                            responsible.Add(new OptionSetValue(8));
                            break;

                        case "Customizações":
                            responsible.Add(new OptionSetValue(9));
                            break;

                        default:
                            break;
                    }
                }
                quote.smt_pl_responsible_coordination = responsible;
            }
            #endregion

            #region Produtos
            if (target.smt_name == "smt_pl_products")
            {
                OptionSetValueCollection products = new OptionSetValueCollection();
                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "PACS":
                            products.Add(new OptionSetValue(1));
                            break;

                        case "RIS":
                            products.Add(new OptionSetValue(2));
                            break;

                        case "LIS":
                            products.Add(new OptionSetValue(3));
                            break;

                        case "Produtos Complementares":
                            products.Add(new OptionSetValue(4));
                            break;

                        case "Clickvita":
                            products.Add(new OptionSetValue(5));
                            break;

                        case "LabLink":
                            products.Add(new OptionSetValue(6));
                            break;

                        case "Flow Performance":
                            products.Add(new OptionSetValue(7));
                            break;

                        case "MW - SmartLab":
                            products.Add(new OptionSetValue(8));
                            break;

                        case "MW - SmartClin":
                            products.Add(new OptionSetValue(9));
                            break;

                        case "MW - SmartRis":
                            products.Add(new OptionSetValue(10));
                            break;

                        case "MW - SmartHealth":
                            products.Add(new OptionSetValue(11));
                            break;

                        case "MW - Aplicativo - Produto Complementar":
                            products.Add(new OptionSetValue(12));
                            break;

                        case "MW - Produto Complementar":
                            products.Add(new OptionSetValue(13));
                            break;

                        case "MW - Produto Complementar Lab e Health":
                            products.Add(new OptionSetValue(14));
                            break;

                        case "MW - Serviço":
                            products.Add(new OptionSetValue(15));
                            break;

                        case "MW - Módulo Complementar":
                            products.Add(new OptionSetValue(16));
                            break;

                        default:
                            break;
                    }
                }
                quote.smt_pl_products = products;
            }
            #endregion

            #region Pendências para Fechamento
            if (target.smt_name == "smt_pl_pending_closing")
            {
                OptionSetValueCollection pendencies = new OptionSetValueCollection();
                string[] values = target.smt_st_jsonbody.Split(',');

                foreach (var value in values)
                {
                    switch (value)
                    {
                        case "[A] - Tipo (Conta) - Inconsistência Cliente Novo/Cliente Base":
                            pendencies.Add(new OptionSetValue(1));
                            break;

                        case "[A] - Cliente Desde - Inconsistência Data/Ano correspondente à primeira venda":
                            pendencies.Add(new OptionSetValue(2));
                            break;

                        case "[A] - Unidade de Negócio - Inconsistência Empresa/Produto":
                            pendencies.Add(new OptionSetValue(3));
                            break;

                        case "[A] - Descrição da Necessidade do Cliente - Inconsistência na descrição da venda":
                            pendencies.Add(new OptionSetValue(4));
                            break;

                        case "[A] - Status Projeto Técnico - Sem Flag":
                            pendencies.Add(new OptionSetValue(5));
                            break;

                        case "[A] - Status Aprovação Projeto Técnico - Sem Aprovação":
                            pendencies.Add(new OptionSetValue(6));
                            break;

                        case "[A] - Desconto - Sem apontamento do desconto":
                            pendencies.Add(new OptionSetValue(7));
                            break;

                        case "[A] - Carência - Sem apontamento da carência":
                            pendencies.Add(new OptionSetValue(8));
                            break;

                        case "[A] - Número Parcelas Eventual - Sem preenchimento parcelamento":
                            pendencies.Add(new OptionSetValue(9));
                            break;

                        case "[A] - Status Alçada de Aprovação - Sem Flag":
                            pendencies.Add(new OptionSetValue(10));
                            break;

                        case "[A] - Status da Alçada de Aprovação - Sem aprovação":
                            pendencies.Add(new OptionSetValue(11));
                            break;

                        case "[A] - Outros (dados de faturamento) - Insconsistência na descrição do faturamento":
                            pendencies.Add(new OptionSetValue(12));
                            break;

                        case "[A] - Fase Finder - Houve participação - Inconsistência nas informações":
                            pendencies.Add(new OptionSetValue(13));
                            break;

                        case "[A] - Fase Representante - Houve representante - Inconsistência nas informações":
                            pendencies.Add(new OptionSetValue(14));
                            break;

                        case "[A] - Papéis de Contato - Inconsistência nas informações":
                            pendencies.Add(new OptionSetValue(15));
                            break;

                        case "[B] - Documentos Anexados - Falta Documento":
                            pendencies.Add(new OptionSetValue(16));
                            break;

                        case "[B] - Liberação Price - Não foi liberada Price":
                            pendencies.Add(new OptionSetValue(17));
                            break;

                        case "[B] - Autorização Chatter - Documentação padrão":
                            pendencies.Add(new OptionSetValue(18));
                            break;

                        case "[C] - Horas Projeto - Incosistência nas horas do projeto":
                            pendencies.Add(new OptionSetValue(19));
                            break;

                        case "[C] - Price Versão - Versão desatualizada":
                            pendencies.Add(new OptionSetValue(20));
                            break;

                        case "[C] - Price Valores - Inconsistência nos valores Price/OP":
                            pendencies.Add(new OptionSetValue(21));
                            break;

                        case "[C] - Price Produtos - Inconsistência nos produtos Price/Proposta":
                            pendencies.Add(new OptionSetValue(22));
                            break;

                        case "[C] - Price Horas - Inconsistência nas horas Price/OP/Calculadora":
                            pendencies.Add(new OptionSetValue(23));
                            break;

                        case "[C] - Calculadora Versão - Versão desatualizada":
                            pendencies.Add(new OptionSetValue(24));
                            break;

                        case "[C] - Calculadora Informações - Inconsistência nas informações Calculadora/OP/Proposta":
                            pendencies.Add(new OptionSetValue(25));
                            break;

                        case "[C] - Autorização Chatter - Versão antiga Price/Calculadora":
                            pendencies.Add(new OptionSetValue(26));
                            break;

                        case "[C] - Autorização Chatter - Horas projeto divergente Price/Calculadora":
                            pendencies.Add(new OptionSetValue(27));
                            break;

                        case "[C] - Valor Recorrente Mensal Price - Inconsistência com a Price":
                            pendencies.Add(new OptionSetValue(28));
                            break;

                        case "[C] - Valor Eventual Price - Inconsistência com a Price":
                            pendencies.Add(new OptionSetValue(29));
                            break;

                        case "[C] - Valor de implantação e treinamento - Incosistência valor (LUT/CDU)":
                            pendencies.Add(new OptionSetValue(30));
                            break;

                        case "[D] - Conta (nome/cnpj/end.) - Falta preenchimento/Inconsistência na informação":
                            pendencies.Add(new OptionSetValue(31));
                            break;

                        case "[D] - Tipo (OP) - Inconsistência Produto/Proposta":
                            pendencies.Add(new OptionSetValue(32));
                            break;

                        case "[D] - Período Cobrança - Inconsistência informação/Proposta":
                            pendencies.Add(new OptionSetValue(33));
                            break;

                        case "[D] - Valor Recorrente Mensal Final - Inconsistência com Proposta/Overprice":
                            pendencies.Add(new OptionSetValue(34));
                            break;

                        case "[D] - Valor Eventual Final - Inconsistência com Proposta/Overprice":
                            pendencies.Add(new OptionSetValue(35));
                            break;

                        case "[D] - Dados para Faturamento - Inconsistência na Opção escolhida/Proposta/Price":
                            pendencies.Add(new OptionSetValue(36));
                            break;

                        case "[D] - Condições Especiais Oferecidas - Inconsistência na informação":
                            pendencies.Add(new OptionSetValue(37));
                            break;

                        case "[D] - Data Prevista Cobrança Recorrente - Inconsistência com negociação":
                            pendencies.Add(new OptionSetValue(38));
                            break;

                        case "[D] - Data Prevista Cobrança Eventual - Inconsistência com negociação":
                            pendencies.Add(new OptionSetValue(39));
                            break;

                        case "[D] - Proposta Anexada - Versão desatualizada":
                            pendencies.Add(new OptionSetValue(40));
                            break;

                        case "[D] - Proposta Anexada - Número de OP incorreto/OP vinculada":
                            pendencies.Add(new OptionSetValue(41));
                            break;

                        case "[D] - Proposta Anexada - Nome da instituição":
                            pendencies.Add(new OptionSetValue(42));
                            break;

                        case "[D] - Proposta Anexada - Inconsistência produto/OP":
                            pendencies.Add(new OptionSetValue(43));
                            break;

                        case "[D] - Proposta Anexada - Cláusula LUT - descrita conforme negociação":
                            pendencies.Add(new OptionSetValue(44));
                            break;

                        case "[D] - Aceite Comercial - Versão desatualizada":
                            pendencies.Add(new OptionSetValue(45));
                            break;

                        case "[D] - Aceite Comercial - Número de OP incorreto/OP vinculada":
                            pendencies.Add(new OptionSetValue(46));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência dados solicitante":
                            pendencies.Add(new OptionSetValue(47));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência dados faturamento":
                            pendencies.Add(new OptionSetValue(48));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência dados representante legal":
                            pendencies.Add(new OptionSetValue(49));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência dados responsável setor de contratos":
                            pendencies.Add(new OptionSetValue(50));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência Valor RR/OP/Proposta":
                            pendencies.Add(new OptionSetValue(51));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência Valor EV/OP/Proposta":
                            pendencies.Add(new OptionSetValue(52));
                            break;

                        case "[D] - Aceite Comercial - Inconsistência informação responsável por custos":
                            pendencies.Add(new OptionSetValue(53));
                            break;

                        case "[D] - Aceite Comercial - Falta Assinatura/Data":
                            pendencies.Add(new OptionSetValue(54));
                            break;

                        case "[D] - PT - Número de OP incorreto/OP vinculada":
                            pendencies.Add(new OptionSetValue(55));
                            break;

                        case "[D] - PT - Nome da instituição":
                            pendencies.Add(new OptionSetValue(56));
                            break;

                        case "[D] - PT - Inconsistência produto/OP":
                            pendencies.Add(new OptionSetValue(57));
                            break;

                        case "[D] - PT - Falta versão DOC":
                            pendencies.Add(new OptionSetValue(58));
                            break;

                        case "[D] - PT - Falta versão validada pelo STV":
                            pendencies.Add(new OptionSetValue(59));
                            break;

                        case "[D] - Aceite PT - Aceite versão final":
                            pendencies.Add(new OptionSetValue(60));
                            break;

                        case "[D] - Aceite PT - Número de OP incorreto/OP vinculada":
                            pendencies.Add(new OptionSetValue(61));
                            break;

                        case "[D] - Aceite PT - Falta Assinatura/Dat":
                            pendencies.Add(new OptionSetValue(62));
                            break;

                        case "[D] - Autorização Chatter - RR pós instalação":
                            pendencies.Add(new OptionSetValue(63));
                            break;

                        case "[D] - Autorização Chatter - Carência EV":
                            pendencies.Add(new OptionSetValue(64));
                            break;

                        case "[E] - Autorização Chatter - Negociações não prevista":
                            pendencies.Add(new OptionSetValue(65));
                            break;

                        case "[E] - Autorização Chatter - Autorização Perído de Cobrança":
                            pendencies.Add(new OptionSetValue(66));
                            break;

                        case "[E] - Autorização Chatter - OP zerada":
                            pendencies.Add(new OptionSetValue(67));
                            break;

                        default:
                            break;
                    }
                }
                quote.smt_pl_pending_closing = pendencies;
            }
            #endregion

            #region Tratar picklists
            if (target.smt_name != "smt_pl_responsible_coordination" && target.smt_name != "smt_pl_products" && target.smt_name != "smt_pl_pending_closing")
            {
                if (target.smt_name.Contains("smt_pl_") || target.smt_name == "smt_st_reason_comment")
                {
                    int? value = business.GetOptionSetValueByLabel(Quote.EntityLogicalName, target.smt_name, target.smt_st_jsonbody, new Guid(target.smt_smt_st_registerId));

                    if (value != null)
                        quote.Attributes[$"{target.smt_name}"] = new OptionSetValue((int)value);
                    else
                    {
                        string name = $"Erro ao atualizar {target.smt_st_entitylogicalname}";
                        string message = $"Erro ao atribuir o valor {target.smt_st_jsonbody} ao campo {target.smt_name} da entidade {target.smt_st_entitylogicalname}";
                        business.CreateLog(name, target.smt_st_entitylogicalname, target.smt_name, $"{new Guid(target.smt_smt_st_registerId)}", message);
                    }
                }
            }
            #endregion
            try
            {
                quote.EntityState = EntityState.Changed;
                localcontext.OrganizationServiceAdmin.Update(quote);
            }
            catch (Exception ex)
            {
                business.CreateLog("Erro ao atualizar optionset da integração de cotação", Quote.EntityLogicalName, target.smt_name, $"{new Guid(target.smt_smt_st_registerId)}", $"{ex.Message}");
            }
        }

        /// <summary>
        /// Trata os picklists de contato
        /// </summary>
        /// <param name="target"> target de integração </param>
        /// <param name="localcontext"> contexto de execução </param>
        /// <param name="business"> business de integração </param>
        protected void TreatDataContact(smt_integration target, LocalPluginContext localcontext, IntegracaoBusiness business)
        {
            Contact contato = new Contact();
            contato.Id = new Guid(target.smt_smt_st_registerId);

            if (target.smt_name.Contains("smt_pl_"))
            {
                int? value = business.GetOptionSetValueByLabel(Contact.EntityLogicalName, target.smt_name, target.smt_st_jsonbody, new Guid(target.smt_smt_st_registerId));

                if (value != null)
                    contato.Attributes[$"{target.smt_name}"] = new OptionSetValue((int)value);
                else
                {
                    string name = $"Erro ao atualizar {target.smt_st_entitylogicalname}";
                    string message = $"Erro ao atribuir o valor {target.smt_st_jsonbody} ao campo {target.smt_name} da entidade {target.smt_st_entitylogicalname}";
                    business.CreateLog(name, target.smt_st_entitylogicalname, target.smt_name, $"{new Guid(target.smt_smt_st_registerId)}", message);
                }
            }

            try
            {
                contato.EntityState = EntityState.Changed;
                localcontext.OrganizationServiceAdmin.Update(contato);
            }
            catch (Exception ex)
            {
                business.CreateLog("Erro ao atualizar optionset da integração de contato", Contact.EntityLogicalName, target.smt_name, $"{new Guid(target.smt_smt_st_registerId)}", $"{ex.Message}");
            }

        }
    }
}
