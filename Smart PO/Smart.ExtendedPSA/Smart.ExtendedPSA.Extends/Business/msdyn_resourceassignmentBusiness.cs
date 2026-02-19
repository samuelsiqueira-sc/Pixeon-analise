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
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Data.Entity.Core.Objects.DataClasses;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Metodo Construtor
    /// </summary>
    public class msdyn_resourceassignmentBusiness : BaseBusiness
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="service">service</param>
        /// <param name="serviceAdmin">serviceAdmin</param>
        /// <param name="tracingService">tracingService</param>
        /// <param name="messages">messages</param>
        public msdyn_resourceassignmentBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Retorna uma lista de Trabalho Planejado
        /// </summary>
        /// <param name="json">Campo Trabalho Planejado de Atribuição de Recurso</param>
        /// <returns>returns</returns>
        public List<TrabalhoPlanejado> GetObjectTrabalhoPlanejado(string json)
        {
            List<TrabalhoPlanejado> saida = new List<TrabalhoPlanejado>();
            saida = ReadToObjectT<List<TrabalhoPlanejado>>(json);
            return saida;

        }

        /// <summary>
        /// Retorna uma lista de Custo Planejado
        /// </summary>
        /// <param name="json">Campo Contorno do Custo Planejado de Atribuição de Recurso</param>
        /// <returns>returns</returns>
        public List<CustoPlanejado> GetObjectCustoPlanejado(string json)
        {
            List<CustoPlanejado> saida = new List<CustoPlanejado>();
            saida = ReadToObjectT<List<CustoPlanejado>>(json);

            return saida;
        }     

        /// <summary>
        /// Soma Custo de Atribuições de Recurso
        /// </summary>
        /// <param name="custoPlanejado">custoPlanejado</param>
        /// <param name="esforcoPlanejado">esforcoPlanejado</param>
        /// <returns>Retorna a soma do custo</returns>
        public Money SomaCusto(List<CustoPlanejado> custoPlanejado, List<TrabalhoPlanejado> esforcoPlanejado)
        {
            Decimal soma = 0;

            foreach (var esforco in esforcoPlanejado)
            {
                if (esforco.End < DateTime.Now.ToLocalTime())
                {
                    foreach (var custo in custoPlanejado)
                    {
                        if (esforco.Start > custo.StartDateTime && esforco.Start < custo.EndDateTime)
                        {
                            soma += esforco.Hours * custo.UnitPrice;
                        }
                    }
                }
            }       

            return new Money(soma);
        }
        /// <summary>
        /// Conta a quantidade de dias úteis entre duas datas informadas
        /// </summary>
        /// <param name="begin">Data de início</param>
        /// <param name="end">Data do fim</param>
        /// <returns>Quantidade de dias úteis entre as datas informadas</returns>
        public int CountDays(DateTime begin, DateTime end)
        {
            // Para evitar que a iterações comece no fim de semana:
            // Verifica se a data de inicio é em um domingo, caso afirmativo pula 1 dia
            // Se começar no sábado pula 2 dias.
            if (begin.DayOfWeek == DayOfWeek.Sunday)
                begin = begin.AddDays(1);
            else if (begin.DayOfWeek == DayOfWeek.Saturday)
                begin = begin.AddDays(2);

            int qtdDays = 0;
            while (begin < end)
            {
                // To-Do: validar datas que são feriados

                // Verifica se a data de inicio é em um sábado, caso afirmativo pula 2 dias (final de semana)
                if (begin.DayOfWeek == DayOfWeek.Saturday)
                    begin = begin.AddDays(2);
                else
                {
                    qtdDays++;
                    begin = begin.AddDays(1);
                }
            }
            return qtdDays;
        }
        /// <summary>
        /// Atualza os campos Esforço Presente Planejado, Custo Presente Planejado e Última Atualização Planejamento Presente de Atribuições de Recurso
        /// 
        /// </summary>
        /// <param name="dataAtual">Hora atual</param>
        /// <param name="inicio">Campo Data de Unicio de Atribuição e Recurso</param>
        /// <param name="termino">Campo Data de Unicio de Atribuição e Recurso</param>
        /// <param name="entity">Entity Context</param>
        /// <param name="preImg">PreImagem Context</param>
        /// <returns>Se metodo atulizar os campos ele retorna true</returns>
        public Boolean AtualizaPlanejamento(DateTime dataAtual, DateTime inicio, DateTime termino, msdyn_resourceassignment entity, msdyn_resourceassignment preImg)
        {
            if (inicio < dataAtual && termino > dataAtual)
            {
                int qtdDays = CountDays(inicio, termino);
                var progressPerct = (decimal)CountDays(inicio.AddDays(1), dataAtual) / (decimal)(qtdDays >= 0 ? qtdDays : 1); // Para contabilizar os 100% o código anda em uma janela de 1 dia, por isso CountDays. Não conta o 1º dia, mas conta o último.
                var qtdHours = entity.msdyn_effort == null ? preImg.msdyn_effort : entity.msdyn_effort;
                var plannedCost = entity.msdyn_plannedcost == null ? preImg.msdyn_plannedcost : entity.msdyn_plannedcost;

                entity.smt_epp = qtdHours * progressPerct;
                entity.smt_cpp = new Money((decimal)plannedCost.Value * progressPerct);
                entity.smt_ultima_atualizacao_pp = DateTime.Now.ToUniversalTime();

                return true;
            }
            else if (inicio < dataAtual && termino < dataAtual)
            {
                entity.smt_epp = preImg.msdyn_effort;
                entity.smt_cpp = preImg.msdyn_plannedcost;
                entity.smt_ultima_atualizacao_pp = DateTime.Now.ToUniversalTime();

                return true;
            }
            else if (inicio > dataAtual && termino > dataAtual)
            {
                entity.smt_epp = 0;
                entity.smt_cpp = new Money(0);
                entity.smt_ultima_atualizacao_pp = DateTime.Now.ToUniversalTime();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Calcula o valor do Progresso Físico Planejado das Tarefas Pais
        /// </summary>
        /// <param name="entity">Entidade em que houve a alteração do Esforço Presente Planejado</param>
        /// <param name="preImg">Pre Imagem</param>
        /// <param name="tarefaBusiness">Business da Tarefa do Projeto</param>
        public void AtualizaEsforçoPresenteTarefasPais(msdyn_resourceassignment entity, msdyn_resourceassignment preImg, msdyn_projecttaskBusiness tarefaBusiness)
        {
            // Obtém a tarefa vinculada à atribuição
            msdyn_projecttask tarefa = (msdyn_projecttask)Service.Retrieve(msdyn_projecttask.EntityLogicalName, preImg.msdyn_taskid.Id, new ColumnSet("msdyn_parenttask"));

            // Chama a ação para atualizar o Esforço Presente Planejado da Tarefa vinculada a atribuição
            CalculateRollupFieldRequest request = new CalculateRollupFieldRequest
            {
                Target = tarefa.ToEntityReference(),
                FieldName = "smt_esforcopresenteplanejado"
            };
            Service.Execute(request);
            
            // Para cada tarefa pai, calcula seu esforço presente planejado
            while (tarefa.msdyn_parenttask != null)
            {
                // Busca as suas tarefa irmãs (que possuem o mesmo parenttask)
                var query = new QueryExpression(msdyn_projecttask.EntityLogicalName);
                query.Criteria.AddCondition("msdyn_parenttask", ConditionOperator.Equal, tarefa.msdyn_parenttask.Id.ToString());
                query.ColumnSet.AddColumns("msdyn_projecttaskid", "smt_esforcopresenteplanejado", "smt_esforcopresenteplanejadotarefaspais", "msdyn_summary", "msdyn_subject");
                var tarIrmas = Service.RetrieveMultiple(query);

                Decimal esfocoPresente = 0;

                foreach (var tar in tarIrmas.Entities)
                {
                    var tmp = tar.ToEntity<msdyn_projecttask>();

                    // Se a tarefa  que sofreu alteração NÃO FOR PAI, pega o valor do campo "Esforço Presente Planejado"
                    if (tmp.msdyn_issummary == false)
                    {
                        esfocoPresente += tmp.smt_EsforcoPresentePlanejado.Value;
                    }
                    else // Pega o valor do campo "Esforço Presente Planejado das tarefas Pais"
                    {
                        esfocoPresente += tmp.smt_esforcopresenteplanejadotarefaspais.Value;
                    }
                    
                }

                // Atualiza o esforço presente da tarefa pai
                msdyn_projecttask updateTask = new msdyn_projecttask();
                updateTask.Id = tarefa.msdyn_parenttask.Id;
                updateTask.smt_esforcopresenteplanejadotarefaspais = esfocoPresente;

                Service.Update(updateTask);

                // Busca as informaçãoes da tarefa pai, para saber se ela tem algum parenttask
                // Caso verdadeiro o while executara os cálculo
                tarefa = (msdyn_projecttask)Service.Retrieve(msdyn_projecttask.EntityLogicalName, tarefa.msdyn_parenttask.Id, new ColumnSet("msdyn_parenttask"));
            }
        }
    }
    
    /// <summary>
    /// Classe esquema do campo Trabalho Planejado de Atribuição de Recurso
    /// </summary>
    public class TrabalhoPlanejado
    {
        /// <summary>
        /// End
        /// </summary>
        public DateTime End { get; set; }
        /// <summary>
        /// Start
        /// </summary>
        public DateTime Start { get; set; }
        /// <summary>
        /// Hours
        /// </summary>
        public Decimal Hours { get; set; }
    }

    /// <summary>
    /// Classe esquema do campo Contorno do Custo Planejado de Atribuição de Recurso
    /// </summary>
    public class CustoPlanejado
    {
        /// <summary>
        /// BillingType
        /// </summary>
        public int BillingType { get; set; }
        /// <summary>
        /// EndDateTime
        /// </summary>
        public DateTime EndDateTime { get; set; }
        /// <summary>
        /// PriceListId
        /// </summary>
        public string PriceListId { get; set; }
        /// <summary>
        /// StartDateTime
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// TransactionCurrencyId
        /// </summary>
        public string TransactionCurrencyId { get; set; }
        /// <summary>
        /// UnitId
        /// </summary>
        public string UnitId { get; set; }
        /// <summary>
        /// UnitPrice
        /// </summary>
        public Decimal UnitPrice { get; set; }
    }
}
