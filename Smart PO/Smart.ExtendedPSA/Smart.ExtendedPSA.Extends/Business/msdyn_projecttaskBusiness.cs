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

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Metodos do PG PreUpdateSync_msdyn_projecttask
    /// </summary>
    public class msdyn_projecttaskBusiness : BaseBusiness
    {
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name='service'>Service InitialUser</param>
        /// <param name='serviceAdmin'>Service Admin</param>
        /// <param name='tracingService'>Trace</param>
        /// <param name="messages">Resx Messages</param>
        public msdyn_projecttaskBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        #region BO
        /// <summary>
        /// AtualizaHabilidadeTarefaFilhas
        /// </summary>
        /// <param name="projectTask">projectTask_</param>
        public void AtualizaHabilidadeTarefaFilhas(msdyn_projecttask projectTask)
        {
            if (projectTask.msdyn_IsLineTask.HasValue && (projectTask.smt_lp_caracteristica != null || projectTask.smt_lp_valor_classificacao != null))
            {
                if (!projectTask.msdyn_IsLineTask.Value)
                {

                    var list = ListarPorTarefaPrincipal(projectTask.Id, true, new string[] { "smt_lp_caracteristica", "smt_lp_valor_classificacao" })
                                                            .Where(x => (x.smt_lp_caracteristica == null || x.smt_lp_valor_classificacao == null) || (x.smt_lp_caracteristica.Id != projectTask.smt_lp_caracteristica.Id || x.smt_lp_valor_classificacao != projectTask.smt_lp_valor_classificacao));

                    var updateRecords = list.Select(x => new msdyn_projecttask() { Id = x.Id, smt_lp_caracteristica = projectTask.smt_lp_caracteristica, smt_lp_valor_classificacao = projectTask.smt_lp_valor_classificacao });

                    foreach (var item in updateRecords)
                    {
                        Service.Update(item);
                    }

                }
            }
        }
        /// <summary>
        /// AtualizaNomeRecursoTarefaFilhas
        /// </summary>
        /// <param name="projectTask">projectTask_</param>
        public void AtualizaNomeRecursoTarefaFilhas(msdyn_projecttask projectTask)
        {
            if (projectTask.msdyn_IsLineTask.HasValue && (projectTask.smt_nomedorecurso != null))
            {
                if (!projectTask.msdyn_IsLineTask.Value)
                {

                    var list = ListarPorTarefaPrincipal(projectTask.Id, true, new string[] { "smt_nomedorecurso" })
                                                            .Where(x => (x.smt_nomedorecurso == null) || (x.smt_nomedorecurso != projectTask.smt_nomedorecurso));

                    var updateRecords = list.Select(x => new msdyn_projecttask() { Id = x.Id, smt_nomedorecurso = projectTask.smt_nomedorecurso });

                    foreach (var item in updateRecords)
                    {
                        Service.Update(item);
                    }

                }
            }
        }
        /// <summary>
        /// AutorizaTarefaMarco
        /// </summary>
        /// <param name="_TarefaProjeto">_TarefaProjeto</param>
        public void AutorizaTarefaMarco(msdyn_projecttask _TarefaProjeto)
        {
            if (_TarefaProjeto.msdyn_parenttask != null)
                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF09));
            // throw new MultipleLanguageExceptionBO("ProjectTask_Error_Frame2", "ExceptionPSA");
        }

        /// <summary>
        /// Trava de Alteração
        /// </summary>
        /// <param name="alt_">Codigo de mensagem de trava de alteração</param>
        public void TravaAlteracao(int alt_)
        {
            switch (alt_)
            {
                case 1:
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.TRVALTTASK1).ToString());
                    break;
                case 2:
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.TRVALTTASK2).ToString());
                    break;
                case 3:
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.TRVALTTASK3).ToString());
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// GetHabilidadeTarefa
        /// </summary>
        /// <param name="projectTask">projectTask_</param>
        /// <returns>msdyn_projecttask</returns>
        public msdyn_projecttask GetHabilidadeTarefa(msdyn_projecttask projectTask)
        {
            if (projectTask.msdyn_IsLineTask.HasValue && projectTask.msdyn_parenttask != null)
            {
                if (projectTask.msdyn_IsLineTask.Value)
                {

                    Entity parentTask = Service.Retrieve("msdyn_parenttask", projectTask.msdyn_parenttask.Id, new ColumnSet("smt_lp_caracteristica", "smt_lp_valor_classificacao", "smt_nomedorecurso"));

                    msdyn_projecttask parentTaskReturn = new msdyn_projecttask();
                    parentTaskReturn.Attributes = parentTask.Attributes;

                    return parentTaskReturn;
                }
            }

            return null;
        }
        /// <summary>
        /// ObterIdMarcoTarefa
        /// </summary>
        /// <param name="projectTaskId">projectTaskId</param>
        /// <returns>Guid</returns>
        public Guid ObterIdMarcoTarefa(Guid projectTaskId)
        {
            var columns = new string[] { "smt_marcodaentrega", "msdyn_parenttask" };
            var task = new msdyn_projecttask();

            if (projectTaskId != Guid.Empty)
            {
                task = Retrieve<msdyn_projecttask>(projectTaskId, columns);
            }

            while (task.msdyn_parenttask != null)
            {
                task = Retrieve<msdyn_projecttask>(task.msdyn_parenttask.Id, columns);

                if (task.smt_marcodaentrega.HasValue && task.smt_marcodaentrega.Value)
                {
                    return task.Id;
                }
            }

            return Guid.Empty;
        }
        /// <summary>
        /// ObterMarco
        /// </summary>
        /// <param name="projectTask">projectTask_</param>
        /// <param name="columns">columns</param>
        /// <returns>msdyn_projecttask</returns>
        public msdyn_projecttask ObterMarco(msdyn_projecttask projectTask, params string[] columns)
        {
            if (projectTask.smt_marcodaentrega.HasValue && projectTask.smt_marcodaentrega.Value)
            {
                return projectTask;
            }

            var parentId = projectTask.msdyn_parenttask;

            while (parentId != null)
            {
                var task = Retrieve<msdyn_projecttask>(parentId.Id, columns);

                if (task.smt_marcodaentrega.HasValue && task.smt_marcodaentrega.Value)
                {
                    return task;
                }

                parentId = task.msdyn_parenttask;
            }

            return null;
        }

        // ==========================================================================================
        // public void ValidaCriacaoTarefa(msdyn_projecttask _ProjectTask)
        // {
        //    var boProjeto = new ProjetoBO(Service);

        // if (_ProjectTask.msdyn_project != null)
        //    {
        //        if (!boProjeto.PermiteAlteracaoPorStatusProjeto(_ProjectTask.msdyn_project.Id, msdyn_projecttask.EntityLogicalName))
        //        {
        //            throw new InvalidPluginExecutionException("\r\nNão é possível criar a tarefa, verifique o Status do Projeto!");
        //        }
        //    }
        // }

        /// <summary>
        /// ValidaAtualizacaoTarefa
        /// </summary>
        /// <param name="projectTask">projectTask_</param>
        /// <param name="entityContext">entityContext</param>
        public void ValidaAtualizacaoTarefa(msdyn_projecttask projectTask, msdyn_projecttask entityContext)
        {
            if (projectTask.msdyn_project == null) return;

            var project = Retrieve<msdyn_project>(projectTask.msdyn_project.Id, "statuscode");

            int StatusCodeInt = ((OptionSetValue)project.statuscode).Value;

            if (StatusCodeInt == (int)msdyn_project_statuscode.Fechado || StatusCodeInt == (int)msdyn_project_statuscode.Finalizado || StatusCodeInt == (int)msdyn_project_statuscode.Inativoa || StatusCodeInt == (int)msdyn_project_statuscode.Suspenso)
            {
                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF02));
                // throw new MultipleLanguageExceptionBO("ProjectTask_Create_Error1", "ExceptionPSA");
            }

            // FieldsIsNotAllowedToEdit(entityContext, (msdyn_project_StatusCode)project.StatusCode.Value);
            if (project.statuscode.Value.Equals(msdyn_project_statuscode.EmPlanejamento.GetHashCode()))
            {
                // Não é possível ajustar/ excluir horas de tarefas já concluídas;
                // if ((projectTask_.smt_progressofisico.HasValue && projectTask_.smt_progressofisico.Value == 100) 
                // projectTask_.smt_statusprojeto.Value.Equals(msdyn_projecttask_smt_statusprojeto.Concluido.GetHashCode()))
                // {
                //    throw new InvalidPluginExecutionException("\r\nNão é possível alterar uma tarefa concluída!");
                // }

                // Não é possível ajustar/ excluir tarefas que já foram apontadas;
                if (projectTask.msdyn_Actualcost?.Value > 0)
                {
                    // se tem custo já ocorreu apontamento, permite alteração da tarefa, porém nunca pode diminuir o custo estimado (diminuir horas) do que o custo real.
                    if (projectTask.msdyn_plannedCost?.Value < projectTask.msdyn_Actualcost.Value)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF10));
                        // throw new MultipleLanguageExceptionBO("ProjectTask_Expencise1", "ExceptionPSA");
                    }
                }
            }
        }

        /// <summary>
        /// ValidarStatusMarco
        /// </summary>
        /// <param name="projecttask">projecttask</param>
        /// <param name="preimage">preimage</param>
        public void ValidarStatusMarco(msdyn_projecttask projecttask, msdyn_projecttask preimage)
        {
            if (projecttask.smt_marcodaentrega.HasValue && projecttask.smt_marcodaentrega.Value)
            {
                if (preimage.smt_statusprojeto == null)
                {
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF08));
                    // throw new MultipleLanguageExceptionBO("ProjectTask_Error_Status_Frame1", "ExceptionPSA");
                }

                if (((OptionSetValue)preimage.smt_statusprojeto).Value == (int)msdyn_projecttask_smt_statusprojeto.Aprovado)
                {
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF03));
                    // throw new MultipleLanguageExceptionBO("ProjectTask_Error_Frame1", "ExceptionPSA");
                }
            }
            else
            {
                var statusMarco = ObterStatusMarco(projecttask);
                if (!statusMarco.HasValue) return;

                if (statusMarco.Value.GetHashCode() == msdyn_projecttask_smt_statusprojeto.Aprovado.GetHashCode())
                {
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF03));
                    // throw new MultipleLanguageExceptionBO("ProjectTask_Error_Frame1", "ExceptionPSA");
                }
            }
        }

        private msdyn_projecttask_smt_statusprojeto? ObterStatusMarco(msdyn_projecttask projecttask)
        {
            if (projecttask.smt_marcodaentrega.HasValue && projecttask.smt_marcodaentrega.Value)
            {
                if (projecttask.smt_statusprojeto == null) return null;
                return (msdyn_projecttask_smt_statusprojeto)projecttask.smt_statusprojeto.Value;
            }
            else
            {
                var marco = ObterMarco(projecttask, "smt_statusprojeto", "smt_marcodaentrega", "msdyn_parenttask");

                if (marco == null) return null;
                if (marco.smt_statusprojeto == null) return null;

                return (msdyn_projecttask_smt_statusprojeto)marco.smt_statusprojeto.Value;
            }
        }

        #region Controle de Progresso Físico
        /// <summary>
        /// MIT041 - GEP_007
        /// 2.1.9 Inserir progresso físico das tarefas
        /// Não deverá ser possível alterar o progresso físico de atividades que estejam dentro de marcos com status “Em aprovação” e “Aprovado”.
        /// 2.1.10	Verificar se as atividades das EDTs foram concluídas
        /// Um marco de entrega (tarefa mãe) é concluído quando todas as suas atividades (tarefas filhas) atingirem 100% do seu progresso físico.
        /// O progresso físico do marco de entrega deverá ser atualizado automaticamente com base nas tarefas filhas.
        /// O cálculo do progresso físico deve ser proporcional ao tamanho (quantidade de horas) das tarefas. 
        /// 2.1.11	Excluir agendas não confirmadas do marco
        /// </summary>
        /// <param name="projectTask">ProjectTask PreImage</param>
        public void ControlarProgressoFisico(msdyn_projecttask projectTask)
        {
            var projeto = new msdyn_project();

            if (projectTask.msdyn_parenttask != null)
            {

                var tarefaPrincipal = ObterMarcoTarefa(projectTask.msdyn_parenttask.Id);

                var tarefasFilhas = ObterTarefasFilhas(tarefaPrincipal.Id);

                tarefaPrincipal = AtualizarProgressoMarco(projectTask, tarefaPrincipal, tarefasFilhas);

                // Chama o método recursivamente caso ainda hajam tarefas pai
                if (tarefaPrincipal.msdyn_parenttask != null)
                    ControlarProgressoFisico(tarefaPrincipal);
                else
                    AtualizarProgressoProjeto(tarefaPrincipal);

            }
            else
            {
                AtualizarProgressoProjeto(projectTask);
            }

        }
        /// <summary>
        /// AtualizarProgressoMarco
        /// </summary>
        /// <param name="projectTask">projectTask_</param>
        /// <param name="tarefaPrincipal">tarefaPrincipal</param>
        /// <param name="tarefasFilhas">tarefasFilhas</param>
        /// <returns>msdyn_projecttask</returns>
        public msdyn_projecttask AtualizarProgressoMarco(msdyn_projecttask projectTask, msdyn_projecttask tarefaPrincipal, IEnumerable<msdyn_projecttask> tarefasFilhas)
        {
            if (tarefasFilhas != null)
            {
                // Atualizar marco com progresso proporcional as atividades filhas
                var progressoMarco = CalcularProgressoFisico(projectTask, tarefasFilhas.ToList());

                var marcoEntrega = new msdyn_projecttask()
                {
                    Id = tarefaPrincipal.Id,
                    smt_progressofisico = tarefaPrincipal.smt_progressofisico = Convert.ToDecimal(progressoMarco)
                };

                if (progressoMarco == 100)
                {
                    if (tarefaPrincipal.smt_marcodaentrega != null && (bool)tarefaPrincipal.smt_marcodaentrega)
                    {
                        // Atualiza o status do marco para Concluído, caso todas as atividades tenham sido concluídas
                        marcoEntrega.smt_statusprojeto = new OptionSetValue((int)msdyn_projecttask_smt_statusprojeto.Concluido);
                        // Atualiza a data de conclusão do marco com a data atual
                        marcoEntrega.smt_datafimprojeto = GetdDataAtual();
                    }

                    // Exclui agendas não confirmadas
                    // ExcluirAgendasNaoConfirmadas(tarefasFilhas.ToList());
                }
                Update(marcoEntrega);
            }

            return tarefaPrincipal;
        }

        /// <summary>
        /// Atualiza o progresso físico do projeto com base nos marcos de entrega
        /// </summary>
        /// <param name="marcoEntrega">Marco de Entrega atualizado</param>
        public void AtualizarProgressoProjeto(msdyn_projecttask marcoEntrega)
        {
            if (marcoEntrega.msdyn_project != null)
            {
                var marcosEntrega = ObterTarefasMarco(marcoEntrega.msdyn_project.Id);

                if (marcosEntrega != null)
                {
                    var progressoProjeto = CalcularProgressoFisico(marcoEntrega, marcosEntrega.ToList());
                    var esforçoRestante = (1 - progressoProjeto) * marcoEntrega.msdyn_Effort;

                    var projeto = new msdyn_project()
                    {
                        Id = marcoEntrega.msdyn_project.Id,
                        smt_progressofisico = Convert.ToDecimal(progressoProjeto),
                        msdyn_effortremaining = esforçoRestante
                    };
                    Update(projeto);
                }
            }

        }

        private double CalcularProgressoFisico(msdyn_projecttask projectTask, List<msdyn_projecttask> tarefasFilhas)
        {
            double totalHoras = 0, horasConcluidas = 0, progressoMarco = 0;

            if (tarefasFilhas.Count > 0)
            {
                //// Verifica se a tarefa já está na lista ou se acabou de ser criada (e então adiciona na lista)
                // bool isCreate = projectTask_ == null ? false : (tarefasFilhas.Find(tar => tar.Id == projectTask_.Id) == null ? true : false);
                // if (isCreate)
                //    tarefasFilhas.Add(projectTask_);

                // Varre a lista de tarefas para atualizar o progresso físico
                foreach (var task in tarefasFilhas)
                {
                    var horasEstimadas = projectTask != null && task.Id == projectTask.Id ?
                        (projectTask.msdyn_Effort != null ? projectTask.msdyn_Effort : 0) :
                        (task.msdyn_Effort != null ? task.msdyn_Effort.Value : 0);
                    var pctConcluido = projectTask != null && task.Id == projectTask.Id ?
                        (projectTask.smt_progressofisico != null ? projectTask.smt_progressofisico : 0) :
                        (task.smt_progressofisico != null ? task.smt_progressofisico.Value : 0);
                    totalHoras += (double)horasEstimadas;
                    horasConcluidas += (double)horasEstimadas * ((double)pctConcluido / 100);
                }

                if (totalHoras > 0)
                    progressoMarco = (horasConcluidas * 100) / totalHoras;
            }

            return progressoMarco;
        }

        /// <summary>
        /// Exclui as agendas que não contém horas lançadas
        /// </summary>
        /// <param name="tarefasFilhas">lista de tarefas filhas da tarefa principal</param>
        /// 
        // =======================================================================================
        private void ExcluirAgendasNaoConfirmadas(List<msdyn_projecttask> tarefasFilhas)
        {
            if (tarefasFilhas.Count > 0)
            {

                foreach (var item in tarefasFilhas)
                {
                    // Retorna todos as reservas de recuro da tarefa
                    var reservas = GetReservaByTarefa(item.Id);
                    if (reservas != null)
                    {
                        foreach (var reserva in reservas)
                        {
                            // Cancela a reserva caso ela não possua apontamentos com status <> RASCUNHO
                            var entradasHora = ObterApontamentosByReserva(reserva.Id, 192350000);
                            if (entradasHora.Count == 0)
                                Service.Delete(BookableResourceBooking.EntityLogicalName, reserva.Id);
                        }
                    }
                }
            }
        }
        #endregion Controle de Progresso Físico
        /// <summary>
        /// EnviarMarcosParaAprovacao
        /// </summary>
        /// <param name="projectId">projectId</param>
        /// <param name="selectedItems">selectedItems</param>
        /// <returns>string</returns>
        public string EnviarMarcosParaAprovacao(Guid projectId, string selectedItems)
        {
            // Separa os IDs em um array
            string[] ids = selectedItems.Split(',');
            var result = string.Empty;

            foreach (string id in ids)
            {
                try
                {
                    var idProjectTask = new Guid(id);
                    string[] campos = { "smt_marcodaentrega", "smt_statusprojeto" };
                    var tarefaProjeto = Retrieve<msdyn_projecttask>(idProjectTask, campos);

                    #region Validações
                    if (tarefaProjeto.smt_marcodaentrega == null || !(bool)tarefaProjeto.smt_marcodaentrega)
                        return Messages.GetMessageById(ResxExtension.PGCPF04).ToString();
                    if (tarefaProjeto.smt_statusprojeto == null
                        || (tarefaProjeto.smt_statusprojeto.Value != (int)msdyn_projecttask_smt_statusprojeto.Concluido
                        && tarefaProjeto.smt_statusprojeto.Value != (int)msdyn_projecttask_smt_statusprojeto.Reprovado))
                        return Messages.GetMessageById(ResxExtension.PGCPF05).ToString();
                    #endregion Validações

                    // Atualiza o status do marco e seta a data de envio para aprovação
                    msdyn_projecttask projectTask = new msdyn_projecttask()
                    {
                        Id = idProjectTask,
                        smt_statusprojeto = new OptionSetValue((int)msdyn_projecttask_smt_statusprojeto.Emaprovacao),
                        smt_dataenvioaprovacao = GetdDataAtual()
                    };
                    Update(projectTask);
                }
                catch (Exception ex)
                {
                    result = "Erro: " + ex.ToString();
                }
            }

            return result;
        }
        /// <summary>
        /// FieldsIsNotAllowedToEdit
        /// </summary>
        /// <param name="projecttask">projecttask</param>
        /// <param name="projectStatusCode">projecttask</param>
        public void FieldsIsNotAllowedToEdit(msdyn_projecttask projecttask, msdyn_project_statuscode projectStatusCode)
        {

            var fields = GetFieldsIsNotAllowedToEdit(projectStatusCode);
            var fieldsEdited = new List<string>();

            // se veio custo real, ação é de sistema, deixa prosseguir.
            if (projecttask.msdyn_Actualcost != null && projecttask.msdyn_Actualcost.Value != 0)
                return;

            foreach (var field in fields)
            {
                if (projecttask.Contains(field))
                {
                    fieldsEdited.Add(field);
                }
            }

            if (fieldsEdited.Any())
            {
                var labelsField = GetLabels(msdyn_projecttask.EntityLogicalName, fieldsEdited.ToArray());
                string labels = string.Empty;
                labelsField.ForEach(x => labels += $", {x}");
                labels = labels.Substring(2);

                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF06) + labels + Messages.GetMessageById(ResxExtension.PGCPF06_2));
                // throw new MultipleLanguageExceptionBO("ProjectTask_Error_Labels", labels);
            }
        }

        private string[] GetFieldsIsNotAllowedToEdit(msdyn_project_statuscode status)
        {
            switch (status)
            {
                case msdyn_project_statuscode.Falhanaimportacaodoprojeto:
                    return new string[] { "smt_bt_preplanejamento", "smt_custo_despesa_estimada", "statuscode",
                        "smt_lp_caracteristica", "smt_lp_modulo", "smt_lp_processo",
                        "smt_lp_produto", "smt_lp_valor_classificacao", "smt_marcodaentrega", "smt_nomedorecurso", "smt_progressofisico",
                        "smt_tipoaprovacao", "msdyn_parenttask", "msdyn_resourcecategory", "msdyn_effort",
                        "msdyn_resourceorganizationalunitid", "msdyn_finish", "msdyn_scheduledhours", "msdyn_start"};

                case msdyn_project_statuscode.EmExecucao:
                    return new string[] { "smt_bt_preplanejamento", "smt_custo_despesa_estimada", "smt_bt_preplanejamento",
                        "smt_lp_caracteristica", "smt_lp_modulo", "smt_lp_processo", "msdyn_effort",
                        "smt_lp_produto", "smt_lp_valor_classificacao", "smt_marcodaentrega", "smt_nomedorecurso", "msdyn_parenttask",
                        "msdyn_resourcecategory", "msdyn_resourceorganizationalunitid", "msdyn_finish", "msdyn_scheduledhours",
                        "msdyn_start" };

                case msdyn_project_statuscode.EmPlanejamento:
                    return new string[] { };

                // "smt_datafimprojeto", "smt_justreprovacao" "smt_statusprojeto", 

                default:
                    return new string[] { };
            }
        }
        /// <summary>
        /// TodosMarcosAprovados
        /// </summary>
        /// <param name="tarefa">tarefa</param>
        /// <returns>Boolean</returns>
        public Boolean TodosMarcosAprovados(msdyn_projecttask tarefa)
        {

            bool result = true;
            msdyn_project projeto = Retrieve<msdyn_project>(tarefa.msdyn_project.Id, "smt_tipodeprojeto");

            if (projeto.smt_tipodeprojeto != null)
            {

                if (projeto.smt_tipodeprojeto.Value == msdyn_project_smt_tipodeprojeto.Faturavel.GetHashCode())
                {

                    var marcos = ObterMarcos(projeto, "smt_statusprojeto");

                    if (marcos != null)
                    {
                        foreach (var marco in marcos)
                        {
                            if (marco.smt_statusprojeto != null)
                            {
                                if (marco.smt_statusprojeto.Value != msdyn_projecttask_smt_statusprojeto.Aprovado.GetHashCode())
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Valida o perfil do usuário que está alterando o status do marco
        /// em aprovação, pré-aprovado, reprovado (CP). Aprovado e reprovado (PMO local)
        /// </summary>
        /// <param name="tarefa">Tarefa (marco) com status sendo alterado</param>
        public void ValidarAlteracaoStatusMarco(msdyn_projecttask tarefa)
        {

            var allRoles = GetAllRolesBySystemUserId(tarefa.ModifiedBy.Id);
            string[] perfisAcesso = { "TOTVS - Coordenador", "TOTVS - PMO" };
            var roles = new List<Role>();

            switch (tarefa.smt_statusprojeto.Value.GetHashCode())
            {

                case (int)msdyn_projecttask_smt_statusprojeto.Emaprovacao:
                    roles = (from role in allRoles
                             where role.Name.ToLower() == perfisAcesso[0].ToLower()
                             select role).ToList();

                    if (roles.Count() == 0)
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF07) + perfisAcesso[0] + Messages.GetMessageById(ResxExtension.PGCPF07_2));
                    // throw MultipleLanguageExceptionBO("ProjectTask_Roles_Aproved1", perfisAcesso[0]);
                    break;
                case (int)msdyn_projecttask_smt_statusprojeto.Preaprovado:
                    roles = (from role in allRoles
                             where role.Name.ToLower() == perfisAcesso[0].ToLower()
                             select role).ToList();

                    if (roles.Count() == 0)
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF07) + perfisAcesso[0] + Messages.GetMessageById(ResxExtension.PGCPF07_3));
                    // throw new MultipleLanguageExceptionBO("ProjectTask_Roles_Aproved2", perfisAcesso[0]);
                    break;
                case (int)msdyn_projecttask_smt_statusprojeto.Aprovado:
                    roles = (from role in allRoles
                             where role.Name.ToLower() == perfisAcesso[1].ToLower()
                             select role).ToList();

                    if (roles.Count() == 0)
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF07) + perfisAcesso[1] + Messages.GetMessageById(ResxExtension.PGCPF07_4));
                    // throw new MultipleLanguageExceptionBO("ProjectTask_Roles_Aproved3", perfisAcesso[1]);

                    break;
                case (int)msdyn_projecttask_smt_statusprojeto.Reprovado:
                    roles = (from role in allRoles
                             where perfisAcesso.Any(perfil => perfil.ToLower() == role.Name.ToLower())
                             select role).ToList();

                    if (roles.Count() == 0)
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.PGCPF07) + perfisAcesso[0] + Messages.GetMessageById(ResxExtension.PGCPF07_5) + perfisAcesso[1] + Messages.GetMessageById(ResxExtension.PGCPF07_4));
                    // throw new MultipleLanguageExceptionBO("ProjectTask_Roles_Aproved4", perfisAcesso[0], perfisAcesso[1]);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Cria registros de Alertas quando campos específicos das tarefas que contém dados no atributo "Work Item Id" forem alterados
        /// </summary>
        /// <param name="projectTask_">Tarefa que houve alteração</param>
        /// <param name="oldProjectTask_">Tarefa antes da alteração</param>
        /// <param name="campoalterado_">Valor Enum para indicar no Picklist qual campo foi alterado</param>
        /// <param name="actualvalue_">Valor atual do campo após a alteração</param>
        /// <param name="oldvalue_">Valor do campo antes da alteração</param>
        public void AuditoriaAlteracao(msdyn_projecttask projectTask_, smt_project_alerts_smt_pl_taskchangedfield campoalterado_, string actualvalue_, string oldvalue_)
        {
            DateTime dataatual = GetdDataAtual();
            smt_project_alerts alertadeProjeto = new smt_project_alerts();
            alertadeProjeto.smt_name = "Alteração do Campo " + Enum.GetName(typeof(smt_project_alerts_smt_pl_taskchangedfield), campoalterado_) + " na Tarefa do Projeto: " + projectTask_.msdyn_subject + " - " + dataatual;
            alertadeProjeto.smt_lp_project = new EntityReference(projectTask_.msdyn_project.LogicalName, projectTask_.msdyn_project.Id);
            alertadeProjeto.smt_lp_taskproject = new EntityReference(projectTask_.LogicalName, projectTask_.Id);
            alertadeProjeto.smt_pl_taskchangedfieldEnum = campoalterado_;
            alertadeProjeto.smt_st_type = "Alteração de Campo da Tarefa";
            alertadeProjeto.smt_tx_description = "Valor pré alteração: " + oldvalue_.ToString() + "\t Valor pós Alteração: " + actualvalue_.ToString() + "\n\n Data da Alteração: " + dataatual;
            ServiceAdmin.Create(alertadeProjeto);
        }

        #endregion

        #region DAO
        /// <summary>
        /// ListarPorTarefaPrincipal
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="isLineTask">isLineTask</param>
        /// <param name="columns">columns</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ListarPorTarefaPrincipal(Guid id, bool? isLineTask, params string[] columns)
        {
            QueryExpression query = new QueryExpression(msdyn_projecttask.EntityLogicalName);
            query.NoLock = true;
            query.ColumnSet = GetColumnSet(columns);

            query.Criteria.AddCondition("msdyn_parenttask", ConditionOperator.Equal, id.ToString());
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, msdyn_projecttaskState.Active.GetHashCode());

            if (isLineTask.HasValue)
            {
                query.Criteria.AddCondition("msdyn_islinetask", ConditionOperator.Equal, isLineTask);
            }

            EntityCollection colecao = Service.RetrieveMultiple(query);

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }
            return retorno;
        }
        /// <summary>
        /// Listar
        /// </summary>
        /// <param name="project">project</param>
        /// <param name="marco">marco</param>
        /// <param name="columns">columns</param>
        /// <returns>msdyn_projecttask</returns>
        public List<msdyn_projecttask> Listar(msdyn_project project, bool? marco, params string[] columns)
        {
            var query = new QueryExpression(msdyn_projecttask.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = GetColumnSet(columns)
            };

            query.Criteria.AddCondition("msdyn_project", ConditionOperator.Equal, project.Id.ToString());
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, msdyn_projecttaskState.Active.GetHashCode());

            if (marco.HasValue && marco.Value)
            {
                query.Criteria.AddCondition("smt_marcodaentrega", ConditionOperator.Equal, true);
            }
            EntityCollection colecao = Service.RetrieveMultiple(query);

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }
            return retorno;
        }
        /// <summary>
        /// ObterByProjetoSemBaseline
        /// </summary>
        /// <param name="_ProjetoId">_ProjetoId</param>
        /// <param name="columnsTarefaProjeto">columnsTarefaProjeto</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ObterByProjetoSemBaseline(Guid _ProjetoId, params string[] columnsTarefaProjeto)
        {
            if (columnsTarefaProjeto == null)
            {
                throw new Exception("conjunto de colunas nulo ou inválido");
            }
            var query = new QueryExpression(msdyn_projecttask.EntityLogicalName);
            query.NoLock = true;
            query.ColumnSet = GetColumnSet(columnsTarefaProjeto);

            query.Criteria.AddCondition("msdyn_project", ConditionOperator.Equal, _ProjetoId.ToString());
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, msdyn_projecttaskState.Active.GetHashCode());
            query.Criteria.AddCondition("msdyn_islinetask", ConditionOperator.Equal, false);

            var linkTarefaBaseline = query.AddLink("smt_tarefadabaseline", "msdyn_projecttaskid", "smt_tarefaprojetoid", JoinOperator.LeftOuter);
            linkTarefaBaseline.EntityAlias = "tbl";
            linkTarefaBaseline.Columns.AddColumn("smt_tarefadabaselineid");

            query.Criteria.AddCondition("tbl", "smt_tarefadabaselineid", ConditionOperator.Null);

            EntityCollection colecao = Service.RetrieveMultiple(query);

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }

            return retorno;
        }
        /// <summary>
        /// ObterUltimaTarefaWBSID
        /// </summary>
        /// <param name="_ProjetoId">_ProjetoId</param>
        /// <param name="columnsTarefaProjeto">columnsTarefaProjeto</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ObterUltimaTarefaWBSID(Guid _ProjetoId, params string[] columnsTarefaProjeto)
        {

            var query = new QueryExpression(msdyn_projecttask.EntityLogicalName);
            query.NoLock = true;
            query.ColumnSet = GetColumnSet(columnsTarefaProjeto);

            query.Criteria.AddCondition("msdyn_project", ConditionOperator.Equal, _ProjetoId.ToString());
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, msdyn_projecttaskState.Active.GetHashCode());

            query.AddOrder("msdyn_wbsid", OrderType.Descending);

            EntityCollection colecao = Service.RetrieveMultiple(query);

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }

            return retorno;
        }
        /// <summary>
        /// ObterTarefasSemMarco
        /// </summary>
        /// <param name="_ProjetoId">_ProjetoId</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ObterTarefasSemMarco(Guid _ProjetoId)
        {
            var FetchXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='msdyn_projecttask'>
                                    <attribute name='msdyn_subject' />
                                    <attribute name='createdon' />
                                    <attribute name='msdyn_projecttaskid' />
                                    <order attribute='msdyn_subject' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='msdyn_parenttask' operator='null' />
                                      <condition attribute='smt_marcodaentrega' operator='eq' value='0' />
                                      <condition attribute='msdyn_project' operator='eq' value='" + _ProjetoId + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";

            // return RetrieveMultiple(new FetchExpression(FetchXML));

            EntityCollection colecao = Service.RetrieveMultiple(new FetchExpression(FetchXML));

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }

            return retorno;

        }
        /// <summary>
        /// ObterTarefasMarco
        /// </summary>
        /// <param name="_ProjetoId">_ProjetoId</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ObterTarefasMarco(Guid _ProjetoId)
        {
            var FetchXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='msdyn_projecttask'>
                                    <attribute name='msdyn_subject' />
                                    <attribute name='createdon' />
                                    <attribute name='msdyn_projecttaskid' />
                                    <attribute name='msdyn_start' />
                                    <attribute name='msdyn_finish' />
                                    <attribute name='smt_progressofisico' />
                                    <attribute name='msdyn_effort' />
                                    <order attribute='msdyn_subject' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='msdyn_parenttask' operator='null' />
                                      <condition attribute='msdyn_project' operator='eq' value='" + _ProjetoId + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";

            EntityCollection colecao = Service.RetrieveMultiple(new FetchExpression(FetchXML));

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }

            return retorno;

        }
        /// <summary>
        /// ObterTarefasFilhas
        /// </summary>
        /// <param name="_tarefaProjetoId">_tarefaProjetoId</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ObterTarefasFilhas(Guid _tarefaProjetoId)
        {
            var FetchXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='msdyn_projecttask'>
                                    <attribute name='msdyn_subject' />
                                    <attribute name='createdon' />
                                    <attribute name='msdyn_projecttaskid' />
                                    <attribute name='msdyn_start' />
                                    <attribute name='msdyn_finish' />
                                    <attribute name='msdyn_effort' />
                                    <attribute name='smt_progressofisico' />
                                    <order attribute='msdyn_subject' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='msdyn_parenttask' operator='eq' value='" + _tarefaProjetoId + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";

            EntityCollection colecao = Service.RetrieveMultiple(new FetchExpression(FetchXML));

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }

            return retorno;
        }

        /// <summary>
        /// Valida regra:  Não pode haver uma atividade que não tenha um marco na cadeia superior
        /// </summary>
        /// <param name="_IdProjeto">_IdProjeto</param>
        /// <returns>True se exite taefa fora da cadeia de marco, False se estiver ok.</returns>
        public bool ExisteTarefaSemMarcoCadeiaSuperior(Guid _IdProjeto)
        {
            var fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                          <entity name='msdyn_projecttask'>
                            <attribute name='msdyn_projecttaskid' />
                            <filter type='and'>
                              <condition attribute='smt_marcodaentrega' operator='ne' value='1' />
                              <condition attribute='msdyn_parenttask' operator='null' />
                              <condition attribute='msdyn_project' operator='eq' value='" + _IdProjeto + @"' />
                            </filter>
                          </entity>
                        </fetch>";

            var _Result = Service.RetrieveMultiple(new FetchExpression(fetch));

            if (_Result.Entities.Count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// MarcoCadeiaOutroMarco
        /// </summary>
        /// <param name="_ProjetoId">_ProjetoId</param>
        /// <returns>True se Existe, False se não</returns>
        public bool MarcoCadeiaOutroMarco(Guid _ProjetoId)
        {
            var fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                      <entity name='msdyn_projecttask'>
                        <attribute name='msdyn_subject' />
                        <attribute name='createdon' />
                        <attribute name='msdyn_projecttaskid' />
                        <order attribute='msdyn_subject' descending='false' />
                        <filter type='and'>
                          <condition attribute='smt_marcodaentrega' operator='eq' value='1' />
                          <condition attribute='msdyn_parenttask' operator='not-null' />
                          <condition attribute='msdyn_project' operator='eq' value='" + _ProjetoId + @"'/>
                        </filter>
                      </entity>
                    </fetch>";

            var result = Service.RetrieveMultiple(new FetchExpression(fetch));

            if (result.Entities.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtem ID tarefa MARCO que cobre a tarefa referenciada
        /// </summary>
        /// <param name="taskId_">_TaskId</param>
        /// <returns>msdyn_projecttask</returns>
        public msdyn_projecttask ObterMarcoTarefa(Guid taskId_)
        {
            Entity saida = Service.Retrieve(msdyn_projecttask.EntityLogicalName, taskId_, new ColumnSet("msdyn_parenttask", "smt_progressofisico", "smt_marcodaentrega", "msdyn_project", "smt_statusprojeto", "msdyn_effort"));

            return (msdyn_projecttask)saida;
        }

        /// <summary>
        /// ObterMarcos
        /// </summary>
        /// <param name="projeto">projeto</param>
        /// <param name="columns">columns</param>
        /// <returns>msdyn_projecttask</returns>
        public IEnumerable<msdyn_projecttask> ObterMarcos(msdyn_project projeto, params string[] columns)
        {
            var query = new QueryExpression(msdyn_projecttask.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = GetColumnSet(columns)
            };

            query.Criteria.AddCondition("msdyn_project", ConditionOperator.Equal, projeto.Id.ToString());
            query.Criteria.AddCondition("smt_marcodaentrega", ConditionOperator.Equal, true);

            EntityCollection colecao = Service.RetrieveMultiple(query);

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();

            foreach (var c in colecao.Entities)
            {
                retorno.Add((msdyn_projecttask)c);
            }

            return retorno;
        }
        /// <summary>
        /// ObterTarefasByProjeto
        /// </summary>
        /// <param name="projeto">projeto</param>
        /// <param name="columns">columns</param>
        /// <returns>List msdyn_projecttask</returns>
        public List<msdyn_projecttask> ObterTarefasByProjeto(msdyn_project projeto, params string[] columns)
        {
            var queryAll = new QueryExpression(msdyn_projecttask.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = GetColumnSet(columns)
            };

            queryAll.Criteria.AddCondition("msdyn_project", ConditionOperator.Equal, projeto.Id.ToString());
            queryAll.Criteria.AddCondition("msdyn_parenttask", ConditionOperator.NotNull);
            queryAll.Criteria.AddCondition("smt_marcodaentrega", ConditionOperator.Equal, false);

            EntityCollection allTasks = Service.RetrieveMultiple(queryAll);

            List<msdyn_projecttask> retorno = new List<msdyn_projecttask>();
            foreach (var task in allTasks.Entities)
            {
                var childTasks = (List<msdyn_projecttask>)ListarPorTarefaPrincipal(task.Id, null, columns);
                if (childTasks == null || childTasks.Count == 0)
                    retorno.Add((msdyn_projecttask)task);
            }

            return retorno;
        }
        #endregion

        #region Util
        /// <summary>
        /// GetdDataAtual
        /// </summary>
        /// <returns>DateTime</returns>
        public static DateTime GetdDataAtual()
        {
            #region Horario Brasilia
            // conversor de horário para comparar data de vencimento com o horário de brasilia
            var timeUtc = DateTime.UtcNow;
            var kstZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); // Brasilia/BRA
            var dateTimeBrasilia = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, kstZone);
            #endregion
            return dateTimeBrasilia;
        }

        #endregion

        /// <summary>
        /// getReservaByTarefa
        /// Método que retorna Reservas de Recursos relacionados à Tarefa do Projeto do parâmetro
        /// </summary>
        /// <param name="tarefaProjetoId">tarefaProjetoId</param>
        /// <returns>List BookableResourceBooking</returns>
        public List<BookableResourceBooking> GetReservaByTarefa(Guid tarefaProjetoId)
        {
            string fetchXML = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='bookableresourcebooking'>
                                    <attribute name='bookableresourcebookingid' />
                                    <filter type='and'>
                                      <condition attribute='smt_projecttask' operator='eq' value='" + tarefaProjetoId + @"' />
                                    </filter>
                                  </entity>
                                </fetch>";
            EntityCollection colecao = Service.RetrieveMultiple(new FetchExpression(fetchXML));

            List<BookableResourceBooking> saida = new List<BookableResourceBooking>();

            foreach (var e in colecao.Entities)
            {
                saida.Add((BookableResourceBooking)e);
            }
            return saida;
        }

        /// <summary>
        /// obterApontamentosByReserva
        /// Método que retorna os Apontamentos (Entradas de Hora) realcionados à Reserva de Recurso do Parâmetro e com Status Específico informado no Parâmetro
        /// </summary>
        /// <param name="reservaId">reservaId</param>
        /// <param name="statusExcecao">statusExcecao</param>
        /// <returns>List msdyn_timeentry</returns>
        /// 
        public List<msdyn_timeentry> ObterApontamentosByReserva(Guid reservaId, int statusExcecao)
        {
            string fetch = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='msdyn_timeentry'>
                                <attribute name='msdyn_timeentryid' />
                                <filter type='and'>
                                  <condition attribute='smt_reserva_originadoraid' operator='eq' value='" + reservaId + @"' />
                                  <condition attribute='msdyn_entrystatus' operator='ne' value='" + statusExcecao + @"' />
                                </filter>
                              </entity>
                            </fetch>";

            EntityCollection colecao = Service.RetrieveMultiple(new FetchExpression(fetch));

            List<msdyn_timeentry> saida = new List<msdyn_timeentry>();

            foreach (var e in colecao.Entities)
            {
                saida.Add((msdyn_timeentry)e);
            }
            return saida;
        }

        /// <summary>
        /// GetAllRolesBySystemUserId
        /// </summary>
        /// <param name="_UserId">_UserId</param>
        /// <returns>List Role</returns>
        public List<Role> GetAllRolesBySystemUserId(Guid _UserId)
        {
            CrmServiceContext OrgContext = new CrmServiceContext(Service);
            List<Role> roles = (from su in OrgContext.SystemUserRolesSet
                                join r in OrgContext.RoleSet on su.RoleId equals r.RoleId
                                where su.SystemUserId == _UserId
                                select new Role
                                {
                                    Id = r.Id,
                                    Name = r.Name

                                }).ToList();
            return roles;
        }

        /// <summary>
        /// GetColumnSet
        /// </summary>
        /// <param name="columns">columns</param>
        /// <returns>ColumnSet</returns>
        protected ColumnSet GetColumnSet(params string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                throw new Exception("conjunto de colunas nulo ou inválido");
            }
            return new ColumnSet(columns);
        }

        /// <summary>
        /// Busca a categoria de associação do recurso reservavel. 
        /// </summary>
        /// <param name="user">Recurso Reservavel</param>
        /// <returns>Associação de categoria do recurso reservavel</returns>
        public BookableResourceCategoryAssn SearchAssignCategory(BookableResource user)
        {
            CrmServiceContext context = new CrmServiceContext(Service);

            BookableResourceCategoryAssn AssignCategory = (from resourceCategory in context.CreateQuery<BookableResourceCategoryAssn>()
                                                           where resourceCategory.Resource.Id == user.BookableResourceId
                                                           select resourceCategory).FirstOrDefault();

            return AssignCategory;
        }

        /// <summary>
        /// Busca um usuário com o mesmo e-mail inserido em "Email do Recurso" na tarefa do projeto. Depois, busca um recurso reservavel com o mesmo ID deste usuário. 
        /// </summary>
        /// <param name="projecttask">Tarefa do projeto</param>
        /// <returns>Recurso Reservavel</returns>
        public BookableResource SearchBookableResource(msdyn_projecttask projecttask)
        {
            CrmServiceContext context = new CrmServiceContext(Service);

            BookableResource user = (from systemuser in context.CreateQuery<SystemUser>()
                                     join bookableResource in context.BookableResourceSet on systemuser.SystemUserId equals bookableResource.UserId.Id
                                     where systemuser.InternalEMailAddress == projecttask.smt_st_resourceemail
                                     select bookableResource).FirstOrDefault();

            return user;
        }

        /// <summary>
        /// Busca um membro da equipe do projeto onde os campos ID do recurso reservavel e o ID do projeto sejam iguais. 
        /// </summary>
        /// <param name="projecttask">Pre image da tarefa do projeto</param>
        /// <param name="user">Recurso reservavel referente a tarefa do projeto</param>
        /// <returns>Membro da equipe do projeto</returns>
        public EntityCollection FindTeamMember(msdyn_projecttask projecttask, BookableResource user)
        {
            QueryExpression findTeamMember = new QueryExpression(msdyn_projectteam.EntityLogicalName);
            findTeamMember.ColumnSet = GetColumnSet("msdyn_projectteamid");
            findTeamMember.Criteria.AddCondition(msdyn_projectteam.Fields.msdyn_bookableresourceid, ConditionOperator.Equal, user.BookableResourceId);
            findTeamMember.Criteria.AddCondition(msdyn_projectteam.Fields.msdyn_project, ConditionOperator.Equal, projecttask.msdyn_project.Id);
            EntityCollection foundMembers = Service.RetrieveMultiple(findTeamMember);

            return foundMembers;
        }

        /// <summary>
        /// Busca a atribuição de Recurso! 
        /// </summary>
        /// <param name="taskId">Passa a Tarefa do Projeto do Contexto</param>
        /// <returns>Retorna a Atribuição</returns>
        public msdyn_resourceassignment BuscarAtribuicaoDeRecurso(Guid taskId)
        {
            using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
            {
                return (from types in serviceContext.msdyn_projecttaskSet
                        join atribuicao in serviceContext.msdyn_resourceassignmentSet on types.Id equals atribuicao.msdyn_taskid.Id
                        where atribuicao.msdyn_taskid.Id == taskId
                        select new msdyn_resourceassignment
                        {
                            Id = atribuicao.Id
                        }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Chamada da Action nativa que associa o recurso a tarefa do projeto.
        /// </summary>
        /// <param name="entityCollection">Entity Collection que recebe o recurso da tarefa</param>
        /// <param name="projecttask">Tarefa do Projeto</param>
        public void CallActionAssociateResourceWithProjectTask(EntityCollection entityCollection, msdyn_projecttask projecttask)
        {
            OrganizationRequest actionRequest = new OrganizationRequest("msdyn_AssignResourcesForTask")
            {
                ["TeamCollection"] = entityCollection,
                ["Target"] = projecttask.ToEntityReference()
            };

            ServiceAdmin.Execute(actionRequest);
        }

        /// <summary>
        /// Caso o recurso não seja membro da equipe do projeto, essa associação será criada.
        /// </summary>
        /// <param name="projecttask">Tarefa do projeto</param>
        /// <param name="project">Projeto</param>
        /// <param name="projecttaskPreImage">Pre Image da tarefa do projeto</param>
        /// <param name="assignCategory">Categoria de associação do recurso reservavel</param>
        /// <returns>Recurso Reservavel</returns>
        public EntityCollection CreateTeamMembership(msdyn_projecttask projecttask, msdyn_project project, msdyn_projecttask projecttaskPreImage, BookableResourceCategoryAssn assignCategory)
        {
            msdyn_projectteam createTeamMembership = new msdyn_projectteam()
            {
                msdyn_From = project.msdyn_scheduledstart,
                msdyn_To = project.msdyn_finish,
                msdyn_project = projecttaskPreImage.msdyn_project,
                smt_str_email_resource = projecttask.smt_st_resourceemail,
                msdyn_resourcecategory = assignCategory.ResourceCategory,
                msdyn_bookableresourceid = assignCategory.Resource,
            };

            Guid TeamMembershipGuid = Service.Create(createTeamMembership);

            QueryExpression TeamMembership = new QueryExpression(msdyn_projectteam.EntityLogicalName);
            TeamMembership.ColumnSet = GetColumnSet("msdyn_projectteamid");
            TeamMembership.Criteria.AddCondition(msdyn_projectteam.Fields.msdyn_projectteamId, ConditionOperator.Equal, TeamMembershipGuid);

            EntityCollection BookableResource = Service.RetrieveMultiple(TeamMembership);

            return BookableResource;
        }

        /// <summary>
        /// Caso o recurso não seja membro da equipe do projeto, essa associação será criada.
        /// </summary>
        /// <param name="projecttask">Tarefa do projeto</param>
        /// <param name="project">Projeto</param>
        /// <param name="assignCategory">Categoria de associação do recurso reservavel</param>
        /// <returns>Recurso Reservavel</returns>
        public EntityCollection PostCreate_CreateTeamMembership(msdyn_projecttask projecttask, msdyn_project project, BookableResourceCategoryAssn assignCategory)
        {
            msdyn_projectteam createTeamMembership = new msdyn_projectteam()
            {
                msdyn_From = project.msdyn_scheduledstart,
                msdyn_To = project.msdyn_finish,
                msdyn_project = projecttask.msdyn_project,
                smt_str_email_resource = projecttask.smt_st_resourceemail,
                msdyn_resourcecategory = assignCategory.ResourceCategory,
                msdyn_bookableresourceid = assignCategory.Resource,
            };

            Guid TeamMembershipGuid = Service.Create(createTeamMembership);

            QueryExpression TeamMembership = new QueryExpression(msdyn_projectteam.EntityLogicalName);
            TeamMembership.ColumnSet = new ColumnSet("msdyn_projectteamid");
            TeamMembership.Criteria.AddCondition(msdyn_projectteam.Fields.msdyn_projectteamId, ConditionOperator.Equal, TeamMembershipGuid);

            EntityCollection BookableResource = Service.RetrieveMultiple(TeamMembership);

            return BookableResource;
        }

        /// <summary>
        /// Criação de log de erro. 
        /// </summary>
        /// <param name="error">Nome do erro</param>
        /// <param name="projecttask">ID da tarefa do projeto</param>
        /// <param name="user">ID do usuário</param>
        public void CreateLog(string error, Guid projecttask, Guid user)
        {
            smt_log log = new smt_log()
            {
                smt_name = Messages.GetMessageById(error),
                smt_st_entityname = "Tarefa do projeto (msdyn_projecttask)",
                smt_pl_eventtypeEnum = smt_log_smt_pl_eventtype.Erro,
                smt_dt_eventdate = DateTime.Now,
                smt_lp_executinguser = Service.Retrieve(SystemUser.EntityLogicalName, user, GetColumnSet("systemuserid")).ToEntityReference(),
                smt_st_recordname = Messages.GetMessageById(error),
                smt_st_recordid = projecttask.ToString(),
                smt_tx_message = Messages.GetMessageById(error),
                smt_st_eventorigin = "Plugin AssociateResourceWithProjectTask",
            };
            Service.Create(log);
        }

        #region Recognized Revenue

        /// <summary>
        /// Liberar receita
        /// </summary>
        /// <param name="task">target da tarefa</param>
        public Money SetRevenue(msdyn_projecttask task)
        {
            using (var crmContext = new CrmServiceContext(ServiceAdmin))
            {
                var marco = (from m in crmContext.CreateQuery<smt_milestone_project>()
                             where m.Id == task.smt_lp_milestone.Id
                             select m).FirstOrDefault();

                if (marco != null)
                {
                    var valueProject = (from p in crmContext.CreateQuery<SalesOrderDetail>()
                                        where p.msdyn_Project.Id == task.msdyn_project.Id
                                        select p.PricePerUnit).FirstOrDefault();

                    var totalRevenue = valueProject != null ? valueProject.Value * (marco.smt_dc_stage / 100) : null;
                    task.smt_mn_recognized_revenue = new Money(totalRevenue.Value);
                }

                return task.smt_mn_recognized_revenue;
            }
        }
        /// <summary>
        /// Inserir total da receita reconhecida
        /// </summary>
        /// <param name="preImage">PreImage</param>
        /// <param name="valueTask"> Valor para adicionar na receita da tarefa raíz </param>
        public void SetTotalRevenueProject(msdyn_projecttask preImage, Money valueTask)
        {
            // Guid idpai = preImage.msdyn_parenttask.Id;

            // Retorna o id da tarefa Raiz 
            Guid idParentTask = preImage.msdyn_parenttask != null ? GetParentTask(preImage.msdyn_parenttask.Id) : preImage.Id;

            // Valor somado das receitas de todas as tarefas filhas da mesma raiz
            decimal tasksFilhas = GetChildTasks(preImage, idParentTask);

            // Valor a ser adicionado na tarefa raiz.
            valueTask.Value += tasksFilhas;

            msdyn_projecttask taskRaiz = ServiceAdmin.Retrieve(msdyn_projecttask.EntityLogicalName, idParentTask, new ColumnSet("smt_mn_total_revenue")).ToEntity<msdyn_projecttask>();
            msdyn_projecttask taskRaizUp = taskRaiz;
            taskRaizUp.smt_mn_total_revenue = valueTask;
            ServiceAdmin.Update(taskRaizUp);
        }
        #endregion

        /// <summary>
        /// Buscar Tarefa Raíz.
        /// </summary>
        /// <param name="idpai"> id da primeira tarefa pai</param>
        /// <returns> Id da Tarefa Raiz </returns>
        public Guid GetParentTask(Guid idpai)
        {
            Guid idParentTask = default(Guid);
            msdyn_projecttask projectTask = new msdyn_projecttask();

            using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
            {
                // Busca tarefas do projeto até encontrar uma que seja raiz
                do
                {
                    projectTask = (from t in crmContext.CreateQuery<msdyn_projecttask>()
                                   where t.Id == idpai
                                   select t).FirstOrDefault();

                    if (projectTask != null && projectTask.msdyn_parenttask != null)
                    {
                        // Se existir a tarefa vai atribuir o id do pai para a variável de retorno do método
                        idpai = projectTask.msdyn_parenttask.Id;
                    }
                    else
                    {
                        idpai = projectTask.Id;
                    }
                } while (projectTask.msdyn_parenttask != null);

                return idpai;
            }
        }

        /// <summary>
        /// Busca as tarefas filhas da mesma raiz que a tarefa do contexto.
        /// </summary>
        /// <param name="preImage"> preImage de tarefa </param>
        /// <param name="idParentTask"> id da Tarefa Raiz da tarefa do contexto</param>
        /// <returns> Lista com as tarefas filhas da mesma raiz </returns>
        public decimal GetChildTasks(msdyn_projecttask preImage, Guid idParentTask)
        {
            using (CrmServiceContext crmContext = new CrmServiceContext(ServiceAdmin))
            {
                decimal tasksfilhas = 0;
                List<msdyn_projecttask> tasks = (from ts in crmContext.CreateQuery<msdyn_projecttask>()
                                                 where ts.smt_statusprojetoEnum == msdyn_projecttask_smt_statusprojeto.Concluido
                                                 && ts.smt_lp_milestone != null && ts.msdyn_project.Id == preImage.msdyn_project.Id
                                                 && ts.Id != preImage.Id && ts.msdyn_parenttask != null
                                                 select ts).ToList();

                if (tasks != null)
                {
                    tasksfilhas = tasks.Where(a => GetParentTask(a.msdyn_parenttask.Id) == idParentTask).Sum(a => a.smt_mn_recognized_revenue.Value);
                }

                return tasksfilhas;

            }
        }
        /// <summary>
        /// Método que retorna os dados do Projeto atrelado a Tarefa do Projeto
        /// </summary>
        /// <param name="task">Dados da tarefa</param>
        /// <returns>Dados do projeto</returns>
        public msdyn_project GetProject(msdyn_projecttask task)
        {
            return ServiceAdmin.Retrieve(msdyn_project.EntityLogicalName, task.msdyn_project.Id, new ColumnSet("msdyn_projecttemplate", "smt_totaldespesasplanejadas")).ToEntity<msdyn_project>();
        }
        /// <summary>
        /// Busca dados da tarefa de projeto de acordo com o modelo.
        /// </summary>
        /// <param name="task">Dados da Tarefa</param>
        /// <param name="projectTemplate">Dados do modelo de projeto</param>
        /// <returns>retorna valor da tarefa do projeto de acordo com o modelo</returns>
        public msdyn_projecttask GetTaskTemplate(msdyn_projecttask task, msdyn_project projectTemplate)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var taskTemplate = (from t in crmService.msdyn_projecttaskSet
                                    where t.msdyn_project.Id == projectTemplate.msdyn_ProjectTemplate.Id
                                    && t.msdyn_WBSID == task.msdyn_WBSID
                                    && t.msdyn_subject == task.msdyn_subject
                                    && t.smt_lp_milestone != null
                                    select new msdyn_projecttask()
                                    {
                                        Id = t.Id,
                                        smt_lp_milestone = t.smt_lp_milestone
                                    }).FirstOrDefault();
                if (taskTemplate == null)
                {
                    taskTemplate = (from t in crmService.msdyn_projecttaskSet
                                    where t.msdyn_project.Id == projectTemplate.msdyn_ProjectTemplate.Id
                                    && t.msdyn_WBSID == task.msdyn_WBSID
                                    && t.msdyn_subject == task.msdyn_subject
                                    && t.smt_lp_milestone == null
                                    select new msdyn_projecttask()
                                    {
                                        Id = t.Id,
                                        smt_lp_milestone = task.smt_lp_milestone
                                    }).FirstOrDefault();

                    if (taskTemplate == null)
                    {
                        taskTemplate = (from t in crmService.msdyn_projecttaskSet
                                        where t.msdyn_project.Id == projectTemplate.msdyn_ProjectTemplate.Id
                                        && t.msdyn_WBSID == task.msdyn_WBSID
                                        && t.msdyn_subject == task.msdyn_subject
                                        select new msdyn_projecttask()
                                        {
                                            Id = t.Id,
                                        }).FirstOrDefault();
                    }
                }

                return taskTemplate;
            }
        }

        /// <summary>
        /// Buscar todas as tarefas que são filhas da mesma tarefa pai que o contexto
        /// </summary>
        /// <param name="mergeTask"> merge de Tarefa do Projeto </param>
        public void GetTasksWithSameParent(msdyn_projecttask mergeTask)
        {
            using (CrmServiceContext crmService = new CrmServiceContext(ServiceAdmin))
            {
                List<msdyn_projecttask> projecttasks = (from task in crmService.CreateQuery<msdyn_projecttask>()
                                                        where task.msdyn_project.Id == mergeTask.msdyn_project.Id
                                                        && task.msdyn_parenttask.Id == mergeTask.msdyn_parenttask.Id
                                                        && task.smt_statusprojetoEnum != msdyn_projecttask_smt_statusprojeto.Concluido
                                                        select task).ToList<msdyn_projecttask>();

                if (projecttasks == null || projecttasks.Count <= 0)
                {
                    msdyn_projecttask parentTask = new msdyn_projecttask();
                    parentTask.Id = mergeTask.msdyn_parenttask.Id;
                    parentTask.smt_statusprojetoEnum = msdyn_projecttask_smt_statusprojeto.Concluido;
                    ServiceAdmin.Update(parentTask);
                }
            }
        }

        /// <summary>
        /// Retrieve como Admin
        /// </summary>
        /// <param name="entityName"> Nome lógico da Entidade </param>
        /// <param name="idRecord"> ID do Registro </param>
        /// <param name="columnSet"> Campos desejados </param>
        /// <returns> Registro encontrado </returns>
        public Entity RetrieveAsAdmin(String entityName, Guid idRecord, ColumnSet columnSet)
        {
            Entity result = new Entity();
            try
            {
                result = ServiceAdmin.Retrieve(entityName, idRecord, columnSet);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"Erro ao tentar buscar uma {entityName} com o id {idRecord}. Mensagem da exceção {ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// Verifica se o marco contém dados e se sim, atualiza o valor de Receita Estimada 
        /// </summary>
        /// <param name="targetTask">Tarefa</param>
        /// <param name="preImageTask">Tarefa - Pre Image</param>
        /// <param name="mergeTask"> merge de tarefa do projeto </param>
        public void SetEstimatedRevenueIfMilestoneIsNotNull(msdyn_projecttask targetTask, msdyn_projecttask preImageTask, msdyn_projecttask mergeTask)
        {
            msdyn_projecttask taskToUpdate = new msdyn_projecttask();
            taskToUpdate.Id = targetTask.Id;

            using (var crmContext = new CrmServiceContext(ServiceAdmin))
            {

                var marco = (from milestone in crmContext.CreateQuery<smt_milestone_project>()
                             where milestone.Id == mergeTask.smt_lp_milestone.Id
                             select milestone).FirstOrDefault();

                var valueProject = (from linha in crmContext.CreateQuery<SalesOrderDetail>()
                                    where linha.msdyn_Project.Id == mergeTask.msdyn_project.Id
                                    select linha.PricePerUnit).FirstOrDefault();

                Money totalRevenue = valueProject != null ? new Money((decimal)(valueProject.Value * (marco.smt_dc_stage / 100))) : null;


                taskToUpdate.smt_dt_estimated = mergeTask.msdyn_finish;
                taskToUpdate.smt_mn_estimated_revenue = totalRevenue;

                UpdateAsAdmin(taskToUpdate);
            }
        }

        /// <summary>
        /// Atualiza a tarefa do projeto como administrador. Caso dê erro, cria um log.
        /// </summary>
        /// <param name="projecttask"> tarefa do projeto a ser atualizada </param>
        public void UpdateAsAdmin(msdyn_projecttask projecttask)
        {
            try
            {
                ServiceAdmin.Update(projecttask);
            }
            catch (Exception ex)
            {
                CreateLog($"Erro ao atualizar a receita estimada da tarefa do projeto {ex}", projecttask.Id, default(Guid));
            }
        }

        /// <summary>
        /// Preencher Nível da WBS.
        /// </summary>
        /// <param name="target"> Tarefa do Projeto </param>
        public void SetWBSLevel(msdyn_projecttask target)
        {
            String strid = target.msdyn_WBSID.Replace(".", string.Empty);
            int level = strid.Length;

            target.smt_int_wbslevel = level;
        }
    }
}