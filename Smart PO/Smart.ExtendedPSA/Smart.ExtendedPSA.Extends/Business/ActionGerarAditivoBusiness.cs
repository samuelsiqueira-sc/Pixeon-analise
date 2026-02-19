using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.Earlybound;

namespace CRM.Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Métodos de ActionGerarAditivo
    /// </summary>
    public class ActionGerarAditivoBusiness : BaseBusiness
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name='service'>Service InitialUser</param>
        /// <param name='serviceAdmin'>Service Admin</param>
        /// <param name='tracingService'>Trace</param>
        /// <param name="messages">Resx Messages</param>
        public ActionGerarAditivoBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        #region BO

        /// <summary>
        /// Criar o adtivo em contrato
        /// </summary>
        /// <param name="contratoRef">Referencia de Contrato</param>
        /// <param name="cotacaoRef">Referencia de Cotacao</param>
        public void GerarAdtivo(EntityReference contratoRef, EntityReference cotacaoRef)
        {

            List<QuoteDetail> linhasCotacao = GetQuotesDetail(cotacaoRef);

            if (linhasCotacao != null)
            {
                CriarLinhasOrdem(linhasCotacao, contratoRef);
            }
        }
        /// <summary>
        /// Cria linhas da ordem 
        /// </summary>
        /// <param name="linhasCotacao">Referencia de linha de contação</param>
        /// <param name="contratoRef">referencia de contrato</param>
        public void CriarLinhasOrdem(List<QuoteDetail> linhasCotacao, EntityReference contratoRef)
        {
            foreach (var linhaCotacao in linhasCotacao)
            {
                OptionSetValue tipoLinha = new OptionSetValue();
                tipoLinha.Value = linhaCotacao.Contains("smt_pl_contract_line_type") ? ((OptionSetValue)linhaCotacao["smt_pl_contract_line_type"]).Value : 9999;
                OptionSetValue metodoCobranca = new OptionSetValue();
                metodoCobranca.Value = linhaCotacao.Contains("msdyn_billingmethod") ? ((OptionSetValue)linhaCotacao["msdyn_billingmethod"]).Value : 9999;

                SalesOrderDetail aditivo = null;
                aditivo = new SalesOrderDetail();
                EntityReference newLinhaContrato = new EntityReference("salesorderdetail");

                // Tipo da linha == nova
                if (tipoLinha.Value == 100000000)
                {
                    aditivo.SalesOrderId = contratoRef;
                    aditivo = PreencherCamposLinhaOrdem(linhaCotacao, aditivo);
                    newLinhaContrato.Id = Service.Create(aditivo);

                    // O campo frequencia da fatura é limpo por algum plugin do PSA por isso é adiconado pós criação
                    aditivo.Id = newLinhaContrato.Id;
                    aditivo.msdyn_invoicefrequency = linhaCotacao.msdyn_invoicefrequency;
                    Service.Update(aditivo);

                }

                // Tipo da linha == Existente
                else if (tipoLinha.Value == 100000001)
                {
                    aditivo.Id = linhaCotacao.Contains("smt_lp_contract_line") ? ((EntityReference)linhaCotacao["smt_lp_contract_line"]).Id : new Guid();
                    newLinhaContrato.Id = aditivo.Id;
                }

                if (newLinhaContrato.Id != new Guid())
                {
                    // NOVA
                    if (tipoLinha.Value == 100000000)
                    {
                        #region Preço Fixo
                        // Detalhe
                        List<msdyn_quotelinetransaction> detalhesLinhaCotacao = GetDetalhesLinhaCotacao(linhaCotacao.ToEntityReference());
                        CriarDetalheLinhaContratoProjeto(newLinhaContrato, detalhesLinhaCotacao, contratoRef);
                        UpdateValoresLinhaContrato(newLinhaContrato, SomaValoresLinhaContrato(newLinhaContrato));
                        #endregion Preço Fixo
                    }

                    // Tipo da linha == Existente
                    else if (tipoLinha.Value == 100000001)
                    {
                        // Preço Fixo
                        if (metodoCobranca.Value == 192350001)
                        {
                            #region Hora e Material

                            // Detalhe
                            List<msdyn_quotelinetransaction> detalhesLinhaCotacao = GetDetalhesLinhaCotacao(linhaCotacao.ToEntityReference());
                            CriarDetalheLinhaContratoProjeto(newLinhaContrato, detalhesLinhaCotacao, contratoRef);
                            UpdateValoresLinhaContrato(newLinhaContrato, SomaValoresLinhaContrato(newLinhaContrato));

                            // Etapa
                            List<msdyn_quotelinescheduleofvalue> etapasDaLinhaCotacao = GetEtapasLinhaCotacao(linhaCotacao.ToEntityReference());
                            CriarEtapaLinhaContrato(newLinhaContrato, etapasDaLinhaCotacao, contratoRef);
                            #endregion
                        }

                        // Hora e Material
                        else if (metodoCobranca.Value == 192350000)
                        {
                            #region Hora e Material
                            // Detalhe
                            List<msdyn_quotelinetransaction> detalhesLinhaCotacao = GetDetalhesLinhaCotacao(linhaCotacao.ToEntityReference());
                            CriarDetalheLinhaContratoProjeto(newLinhaContrato, detalhesLinhaCotacao, contratoRef);
                            UpdateValoresLinhaContrato(newLinhaContrato, SomaValoresLinhaContrato(newLinhaContrato));

                            // Etapa
                            List<msdyn_quotelinescheduleofvalue> etapasDaLinhaCotacao = GetEtapasLinhaCotacao(linhaCotacao.ToEntityReference());
                            CriarEtapaLinhaContrato(newLinhaContrato, etapasDaLinhaCotacao, contratoRef);

                            // Criar Agenda
                            List<msdyn_quotelineinvoiceschedule> agendasfaturaLinhaCotacao = GetAgendaFaturaLinhaCotacao(linhaCotacao.ToEntityReference());
                            CriarAgendaFaturasLinhaContrato(newLinhaContrato, agendasfaturaLinhaCotacao, contratoRef);
                            #endregion
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Cria Detalhe da Linha do Contrato de Projeto
        /// </summary>
        /// <param name="newLinhaContrato">Referencia da nova linha de contrato</param>
        /// <param name="detalhesLinhaCotacao">Detalhe da linha de cotação</param>
        /// <param name="contratoRef">Referencia de contrato</param>
        public void CriarDetalheLinhaContratoProjeto(EntityReference newLinhaContrato, List<msdyn_quotelinetransaction> detalhesLinhaCotacao, EntityReference contratoRef)
        {
            foreach (var detalhe in detalhesLinhaCotacao)
            {
                msdyn_orderlinetransaction detalheLinhaProjeto = new msdyn_orderlinetransaction();
                detalheLinhaProjeto["msdyn_salescontractlineid"] = newLinhaContrato;
                detalheLinhaProjeto.msdyn_SalesContract = contratoRef;
                detalheLinhaProjeto = PreencherCamposDetalheOrdem(detalhe, detalheLinhaProjeto);

                Service.Create(detalheLinhaProjeto);
            }
        }
        /// <summary>
        /// Cria Etapa da Linha do Contrato
        /// </summary>
        /// <param name="LinhaContrato">Referencia da Linha do COntrato</param>
        /// <param name="EtapaFaturaLinhaCotacao">EtapaFaturaLinhaCotacao</param>
        /// <param name="contratoRef">Referencia do contrato</param>
        public void CriarEtapaLinhaContrato(EntityReference LinhaContrato, List<msdyn_quotelinescheduleofvalue> EtapaFaturaLinhaCotacao, EntityReference contratoRef)
        {
            foreach (var EtapalinhaCotacao in EtapaFaturaLinhaCotacao)
            {
                msdyn_contractlinescheduleofvalue EtapaLinhaContrato = new msdyn_contractlinescheduleofvalue();
                EtapaLinhaContrato["msdyn_contractlineid"] = LinhaContrato;
                EtapaLinhaContrato.msdyn_ContractLineDescription = LinhaContrato.Name;
                EtapaLinhaContrato["msdyn_contract"] = contratoRef;
                EtapaLinhaContrato = PreencherCamposEtapasLinhaOrdem(EtapalinhaCotacao, EtapaLinhaContrato);

                Service.Create(EtapaLinhaContrato);
            }
        }

        /// <summary>
        /// CriarAgendaFaturasLinhaContrato
        /// </summary>
        /// <param name="LinhaContrato">Referencia da Linha do Contrato</param>
        /// <param name="AgendasFaturasLinhaCotacao">AgendasFaturasLinhaCotacao</param>
        /// <param name="contratoRef">Referencia do Contrato</param>
        public void CriarAgendaFaturasLinhaContrato(EntityReference LinhaContrato, List<msdyn_quotelineinvoiceschedule> AgendasFaturasLinhaCotacao, EntityReference contratoRef)
        {
            foreach (var AgendaFaturaLinhaCotacao in AgendasFaturasLinhaCotacao)
            {
                msdyn_contractlineinvoiceschedule AgendafaturaLinhaContrato = new msdyn_contractlineinvoiceschedule();
                AgendafaturaLinhaContrato["msdyn_contractlineid"] = LinhaContrato;
                AgendafaturaLinhaContrato = PreencherCamposAgendaFaturasLinhaContrato(AgendaFaturaLinhaCotacao, AgendafaturaLinhaContrato);

                AgendafaturaLinhaContrato.Id = Service.Create(AgendafaturaLinhaContrato);

                AgendafaturaLinhaContrato.msdyn_transactioncutoffdate = AgendaFaturaLinhaCotacao.msdyn_transactioncutoffdate;
                Service.Update(AgendafaturaLinhaContrato);
            }
        }

        /// <summary>
        /// SetStateQuote
        /// </summary>
        /// <param name="contratoRef">Referencia do Contrato</param>
        public void SetStateQuote(EntityReference contratoRef)
        {
            SetStateRequest state = new SetStateRequest();

            state.State = new OptionSetValue(2);
            state.Status = new OptionSetValue(100000000);

            EntityReference caseReference = new EntityReference(Quote.EntityLogicalName, contratoRef.Id);

            state.EntityMoniker = caseReference;

            SetStateResponse stateSet = (SetStateResponse)Service.Execute(state);

            /*CloseQuoteRequest req = new CloseQuoteRequest();
            Entity quoteClose = new Entity("quoteclose");
            quoteClose.Attributes.Add("quoteid", new EntityReference("quote", contratoRef.Id));
            quoteClose.Attributes.Add("subject", ".");
            req.QuoteClose = quoteClose;
            req.RequestName = "CloseQuote";
            OptionSetValue status = new OptionSetValue();
            status.Value = 2;
            req.Status = status;
            CloseQuoteResponse resp = (CloseQuoteResponse)Service.Execute(req);

            SetStateRequest stateAditivo = new SetStateRequest();

            stateAditivo.State = new OptionSetValue(2);
            stateAditivo.Status = new OptionSetValue(100000000);

            EntityReference caseReferencestateAditivo = new EntityReference(Quote.EntityLogicalName, contratoRef.Id);

            stateAditivo.EntityMoniker = caseReferencestateAditivo;

            SetStateResponse stateSetstateAditivo = (SetStateResponse)Service.Execute(stateAditivo);*/
        }
        #endregion

        #region DAO
        /// <summary>
        /// GetQuotesDetail
        /// </summary>
        /// <param name="cotacaoRef">Referncia do Contrato</param>
        /// <returns>Lista de Cotação</returns>
        public List<QuoteDetail> GetQuotesDetail(EntityReference cotacaoRef)
        {
            QueryExpression filter = new QueryExpression();
            filter.EntityName = QuoteDetail.EntityLogicalName;
            filter.ColumnSet = new ColumnSet(true);
            filter.Criteria.AddCondition(QuoteDetail.Fields.QuoteId, ConditionOperator.Equal, cotacaoRef.Id);

            EntityCollection quotesDetail = Service.RetrieveMultiple(filter);

            List<QuoteDetail> retorno = new List<QuoteDetail>();

            foreach (var quoteDetail in quotesDetail.Entities)
            {
                retorno.Add((QuoteDetail)quoteDetail);
            }
            return retorno;
        }
        /// <summary>
        /// GetEtapasLinhaCotacao
        /// </summary>
        /// <param name="linhaCotacaoRef">Referencia da linha de cotação</param>
        /// <returns>Lisa de msdyn_quotelinescheduleofvalue</returns>
        // Detalhe
        public List<msdyn_quotelinescheduleofvalue> GetEtapasLinhaCotacao(EntityReference linhaCotacaoRef)
        {
            QueryExpression filter = new QueryExpression();
            filter.EntityName = msdyn_quotelinescheduleofvalue.EntityLogicalName;
            filter.ColumnSet = new ColumnSet("msdyn_name", "msdyn_projecttask", "msdyn_amount", "msdyn_tax", "msdyn_invoicedate");
            filter.Criteria.AddCondition("msdyn_quotelineid", ConditionOperator.Equal, linhaCotacaoRef.Id);

            EntityCollection colecao = Service.RetrieveMultiple(filter);

            List<msdyn_quotelinescheduleofvalue> retorno = new List<msdyn_quotelinescheduleofvalue>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_quotelinescheduleofvalue)c);
            }

            return retorno;
        }
        /// <summary>
        /// GetDetalhesLinhaCotacao
        /// </summary>
        /// <param name="linhaCotacaoRef">referencia da linha de cotacao</param>
        /// <returns>Lista de msdyn_quotelinetransaction</returns>
        public List<msdyn_quotelinetransaction> GetDetalhesLinhaCotacao(EntityReference linhaCotacaoRef)
        {
            QueryExpression filter = new QueryExpression();
            filter.EntityName = msdyn_quotelinetransaction.EntityLogicalName;
            filter.ColumnSet = new ColumnSet(true);
            filter.Criteria.AddCondition("msdyn_quotelineid", ConditionOperator.Equal, linhaCotacaoRef.Id);
            filter.Criteria.AddCondition("msdyn_origin", ConditionOperator.NotNull);

            EntityCollection DetalhesLinhaCotaca = Service.RetrieveMultiple(filter);

            List<msdyn_quotelinetransaction> retorno = new List<msdyn_quotelinetransaction>();

            foreach (var detalhe in DetalhesLinhaCotaca.Entities)
            {
                retorno.Add((msdyn_quotelinetransaction)detalhe);
            }
            return retorno;
        }
        /// <summary>
        /// GetAgendaFaturaLinhaCotacao
        /// </summary>
        /// <param name="linhaCotacaoRef">Referencia da linha de cotacao</param>
        /// <returns>Lista de msdyn_quotelineinvoiceschedule</returns>
        public List<msdyn_quotelineinvoiceschedule> GetAgendaFaturaLinhaCotacao(EntityReference linhaCotacaoRef)
        {
            QueryExpression filter = new QueryExpression();
            filter.EntityName = msdyn_quotelineinvoiceschedule.EntityLogicalName;
            filter.ColumnSet = new ColumnSet("msdyn_InvoiceRunDate", "msdyn_name");
            filter.Criteria.AddCondition("msdyn_quotelineid", ConditionOperator.Equal, linhaCotacaoRef.Id);

            EntityCollection DetalhesLinhaCotaca = Service.RetrieveMultiple(filter);

            List<msdyn_quotelineinvoiceschedule> retorno = new List<msdyn_quotelineinvoiceschedule>();

            foreach (var detalhe in DetalhesLinhaCotaca.Entities)
            {
                retorno.Add((msdyn_quotelineinvoiceschedule)detalhe);
            }
            return retorno;
        }

        /// <summary>
        /// PreencherCamposLinhaOrdem
        /// </summary>
        /// <param name="linhaCotacao">linha de cotação</param>
        /// <param name="linhaOrdem">linha da Ordem</param>
        /// <returns>SalesOrderDetail</returns>
        public SalesOrderDetail PreencherCamposLinhaOrdem(QuoteDetail linhaCotacao, SalesOrderDetail linhaOrdem)
        {
            linhaOrdem.BaseAmount = linhaCotacao.BaseAmount;
            linhaOrdem.ShipTo_ContactName = linhaCotacao.ShipTo_ContactName;
            linhaOrdem.ShipTo_Line1 = linhaCotacao.ShipTo_Line1;
            linhaOrdem.ShipTo_Line2 = linhaCotacao.ShipTo_Line2;
            linhaOrdem.ShipTo_Line3 = linhaCotacao.ShipTo_Line3;
            linhaOrdem.LineItemNumber = linhaCotacao.LineItemNumber;
            linhaOrdem["quotedetailid"] = linhaCotacao.ToEntityReference();
            linhaOrdem.msdyn_BillingMethod = linhaCotacao.msdyn_BillingMethod;
            linhaOrdem.ProductDescription = linhaCotacao.ProductDescription;
            linhaOrdem.msdyn_Project = linhaCotacao.msdyn_Project;
            linhaOrdem.msdyn_IncludeTime = linhaCotacao.msdyn_IncludeTime;
            linhaOrdem.msdyn_IncludeExpense = linhaCotacao.msdyn_IncludeExpense;
            linhaOrdem.msdyn_IncludeFee = linhaCotacao.msdyn_IncludeFee;
            linhaOrdem.PricePerUnit = linhaCotacao.PricePerUnit;
            linhaOrdem.Tax = linhaCotacao.Tax;
            linhaOrdem.ExtendedAmount = linhaCotacao.ExtendedAmount;
            linhaOrdem.msdyn_BudgetAmount = linhaCotacao.msdyn_BudgetAmount;

            // linhaOrdem.msdyn_invoicefrequency = linhaCotacao.msdyn_invoicefrequency;           
            linhaOrdem.msdyn_BillingStartDate = linhaCotacao.msdyn_BillingStartDate;
            linhaOrdem.Quantity = linhaCotacao.Quantity;
            linhaOrdem.ProductTypeCode = linhaCotacao.ProductTypeCode;
            linhaOrdem.IsProductOverridden = linhaCotacao.IsProductOverridden;
            linhaOrdem.RequestDeliveryBy = linhaCotacao.RequestDeliveryBy;
            linhaOrdem.ProductId = linhaCotacao.ProductId;
            linhaOrdem.UoMId = linhaCotacao.UoMId;
            linhaOrdem.msdyn_CostPricePerUnit = linhaCotacao.msdyn_CostPricePerUnit;
            linhaOrdem.msdyn_CostAmount = linhaCotacao.msdyn_CostAmount;
            linhaOrdem.msdyn_LineType = linhaCotacao.msdyn_LineType;
            linhaOrdem["salesorderdetailname"] = linhaCotacao.Contains("quotedetailname") ? linhaCotacao["quotedetailname"] : String.Empty;

            return linhaOrdem;
        }
        /// <summary>
        /// PreencherCamposDetalheOrdem
        /// </summary>
        /// <param name="DetalhelinhaCotacao">msdyn_quotelinetransaction</param>
        /// <param name="DetalheLinhaOrdem">msdyn_orderlinetransaction</param>
        /// <returns>msdyn_orderlinetransaction</returns>
        public msdyn_orderlinetransaction PreencherCamposDetalheOrdem(msdyn_quotelinetransaction DetalhelinhaCotacao, msdyn_orderlinetransaction DetalheLinhaOrdem)
        {

            DetalheLinhaOrdem.msdyn_description = DetalhelinhaCotacao.msdyn_description;
            DetalheLinhaOrdem.msdyn_Project = DetalhelinhaCotacao.msdyn_Project;
            DetalheLinhaOrdem.msdyn_ResourceCategory = DetalhelinhaCotacao.msdyn_ResourceCategory;
            DetalheLinhaOrdem.msdyn_TransactionCategory = DetalhelinhaCotacao.msdyn_TransactionCategory;
            DetalheLinhaOrdem.msdyn_TransactionClassification = DetalhelinhaCotacao.msdyn_TransactionClassification;
            DetalheLinhaOrdem.msdyn_TransactionTypeCode = DetalhelinhaCotacao.msdyn_TransactionTypeCode;
            DetalheLinhaOrdem.msdyn_StartDateTime = DetalhelinhaCotacao.msdyn_StartDateTime;
            DetalheLinhaOrdem.msdyn_EndDateTime = DetalhelinhaCotacao.msdyn_EndDateTime;
            DetalheLinhaOrdem.msdyn_ResourceOrganizationalUnitId = DetalhelinhaCotacao.msdyn_ResourceOrganizationalUnitId;
            DetalheLinhaOrdem.msdyn_UnitSchedule = DetalhelinhaCotacao.msdyn_UnitSchedule;
            DetalheLinhaOrdem.msdyn_Unit = DetalhelinhaCotacao.msdyn_Unit;
            DetalheLinhaOrdem.msdyn_BillingType = DetalhelinhaCotacao.msdyn_BillingType;
            DetalheLinhaOrdem.msdyn_Quantity = DetalhelinhaCotacao.msdyn_Quantity;
            DetalheLinhaOrdem.msdyn_Price = DetalhelinhaCotacao.msdyn_Price;
            DetalheLinhaOrdem.msdyn_PriceList = DetalhelinhaCotacao.msdyn_PriceList;
            DetalheLinhaOrdem.msdyn_Amount = DetalhelinhaCotacao.msdyn_Amount;
            DetalheLinhaOrdem.msdyn_tax = DetalhelinhaCotacao.msdyn_tax;

            // colocar o campo da ordem ordermid

            return DetalheLinhaOrdem;

        }

        // Etapa 
        /// <summary>
        /// PreencherCamposEtapasLinhaOrdem
        /// </summary>
        /// <param name="EtapalinhaCotacao">EtapalinhaCotacao</param>
        /// <param name="EtapaLinhaContrato">EtapaLinhaContrato</param>
        /// <returns>msdyn_contractlinescheduleofvalue</returns>
        public msdyn_contractlinescheduleofvalue PreencherCamposEtapasLinhaOrdem(msdyn_quotelinescheduleofvalue EtapalinhaCotacao, msdyn_contractlinescheduleofvalue EtapaLinhaContrato)
        {
            EtapaLinhaContrato.msdyn_name = EtapalinhaCotacao.msdyn_name;
            EtapaLinhaContrato.msdyn_projecttask = EtapalinhaCotacao.msdyn_projecttask;
            EtapaLinhaContrato.msdyn_amount = EtapalinhaCotacao.msdyn_amount;
            EtapaLinhaContrato.msdyn_tax = EtapalinhaCotacao.msdyn_tax;
            EtapaLinhaContrato.msdyn_Invoicedate = EtapalinhaCotacao.msdyn_invoicedate;
            EtapaLinhaContrato.msdyn_startdatetime = EtapalinhaCotacao.msdyn_invoicedate;

            // EtapaLinhaContrato.msdyn_startdatetime = EtapalinhaCotacao.msdyns;
            // EtapaLinhaContrato.msdyn_startdatetime = EtapalinhaCotacao.msdyn_;
            // EtapaLinhaContrato.msdyn_description = EtapalinhaCotacao.msdyn_;
            // EtapaLinhaContrato.msdyn_externaldescription = EtapalinhaCotacao.msdyn_ex xternaldescription;
            // EtapaLinhaContrato.msdyn_price = EtapalinhaCotacao.msdyn_;
            // EtapaLinhaContrato.msdyn_price_Base = EtapalinhaCotacao.msdyn_;
            // EtapaLinhaContrato.msdyn_Invoicestatus = EtapalinhaCotacao.msdyn_Invoicestatus;    

            // msdyn_invoicedate
            return EtapaLinhaContrato;
        }
        /// <summary>
        /// PreencherCamposAgendaFaturasLinhaContrato
        /// </summary>
        /// <param name="AgendaFaturasLinhaCotacao">msdyn_quotelineinvoiceschedule</param>
        /// <param name="AgendaFaturasLinhaContrato">msdyn_contractlineinvoiceschedule</param>
        /// <returns>msdyn_contractlineinvoiceschedule</returns>
        public msdyn_contractlineinvoiceschedule PreencherCamposAgendaFaturasLinhaContrato(msdyn_quotelineinvoiceschedule AgendaFaturasLinhaCotacao, msdyn_contractlineinvoiceschedule AgendaFaturasLinhaContrato)
        {
            // AgendaFaturasLinhaContrato.msdyn_transactioncutoffdate = AgendaFaturasLinhaCotacao.msdyn_transactioncutoffdate;
            AgendaFaturasLinhaContrato.msdyn_InvoiceRunDate = AgendaFaturasLinhaCotacao.msdyn_InvoiceRunDate;
            AgendaFaturasLinhaContrato.msdyn_name = AgendaFaturasLinhaCotacao.msdyn_name;

            // AgendaFaturasLinhaContrato.msdyn_Invoice = AgendaFaturasLinhaCotacao.msdyn_Invoice;
            // AgendaFaturasLinhaContrato.msdyn_InvoiceRunDate = AgendaFaturasLinhaCotacao.msdyn_InvoiceRunDate;
            // AgendaFaturasLinhaContrato.msdyn_InvoiceRunStatus = AgendaFaturasLinhaCotacao.msdyn_InvoiceRunStatus;
            return AgendaFaturasLinhaContrato;
        }
        /// <summary>
        /// SomaValoresLinhaContrato
        /// </summary>
        /// <param name="linhaContrato">Referencia da linha do contrato</param>
        /// <returns>List Double</returns>
        public List<Double> SomaValoresLinhaContrato(EntityReference linhaContrato)
        {
            Double ValorLinha = 0;
            Double imposto = 0;

            QueryExpression filter = new QueryExpression();
            filter.EntityName = msdyn_orderlinetransaction.EntityLogicalName;
            filter.ColumnSet = new ColumnSet("msdyn_amount", "msdyn_tax");
            filter.Criteria.AddCondition("msdyn_salescontractlineid", ConditionOperator.Equal, linhaContrato.Id);
            filter.Criteria.AddCondition("msdyn_origin", ConditionOperator.NotNull);


            EntityCollection estapasLinhaContrato = Service.RetrieveMultiple(filter);

            foreach (var etapa in estapasLinhaContrato.Entities)
            {

                ValorLinha += etapa.Contains("msdyn_amount") ? (Double)((Money)etapa["msdyn_amount"]).Value : 0;
                imposto += etapa.Contains("msdyn_tax") ? (Double)((Money)etapa["msdyn_tax"]).Value : 0;
            }

            List<Double> retorno = new List<Double>();

            retorno.Add(ValorLinha);
            retorno.Add(imposto);


            return retorno;
        }
        /// <summary>
        /// UpdateValoresLinhaContrato
        /// </summary>
        /// <param name="linhaContrato">linha do Contrato Referencia</param>
        /// <param name="valores">valores</param>
        public void UpdateValoresLinhaContrato(EntityReference linhaContrato, List<Double> valores)
        {
            SalesOrderDetail updateLinhaCotacao = new SalesOrderDetail();
            updateLinhaCotacao.Id = linhaContrato.Id;
            updateLinhaCotacao.PricePerUnit = new Money((Decimal)valores[0]);
            updateLinhaCotacao.Tax = new Money((Decimal)valores[1]);

            Service.Update(updateLinhaCotacao);
        }

        #endregion
        /// <summary>
        /// a
        /// </summary>
        /// <param name="columns">a</param>
        /// <returns>a</returns>
        protected ColumnSet GetColumnSet(params string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                throw new Exception("conjunto de colunas nulo ou inválido");
            }
            return new ColumnSet(columns);
        }
    }
}
