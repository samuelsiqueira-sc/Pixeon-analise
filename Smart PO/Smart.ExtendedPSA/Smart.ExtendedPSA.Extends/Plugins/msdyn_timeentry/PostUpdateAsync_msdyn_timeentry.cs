using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Microsoft.Xrm.Sdk;
using Smart.ExtendedPSA.Extends.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Plugins
{
    // TODO: Andre - Corrigido / ******FALTA TESTAR**********
    // TODO: Verificar se tem mais de uma regra de negocio dentro do metodo Create_Latest_Project_Worked e separar por metodo...
    /// <summary>
    /// Plugin Responsável por criar  um registro de Ultimos Projetos Trabalhado, com base no horario mínimo de projeto
    /// </summary>
    public class PostUpdateAsync_msdyn_timeentry : PluginBase
    {
        /// <summary>
        /// Chamada  padrão da classe
        /// </summary>       
        public PostUpdateAsync_msdyn_timeentry() : base(typeof(PostUpdateAsync_msdyn_timeentry)) { }

        /// <summary>
        /// Execução do plugin
        /// </summary>
        /// <param name="localcontext">Local Context</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            #region Licença
            // Chamada da Action de Licenças
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)localcontext.OrganizationService.Execute(request);

            Guid OrgId = response.OrganizationId;

            // Request da action de validação de licenças
            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };

            localcontext.OrganizationService.Execute(ActionRequest);
            #endregion

            #region ResX

            // chamada do arquivo ResX
            List<Resx> messages;
            string resxFileName = ResxExtension.webResourceName;
            messages = localcontext.LoadResxMessages(resxFileName);
            #endregion

            // variáveis utilizadas
            msdyn_timeentry target = localcontext.GetTarget<msdyn_timeentry>();
            msdyn_timeentry preImg = localcontext.GetPreImage<msdyn_timeentry>();

            // Executions
            // TODO: Verificar se esse metodo ira ser desabilitado - ANDRE
            TimeBankCalculation(target, preImg, localcontext, messages);
            Create_Latest_Project_Worked(target, localcontext, messages);

        }

        /// <summary>
        /// CreatLatestProjectWorked
        /// </summary>
        /// <param name="target">target</param>
        /// <param name="localcontext">localcontext</param>
        /// <param name="messages">messages</param>
        private void Create_Latest_Project_Worked(CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry target, LocalPluginContext localcontext, List<Resx> messages)
        {
            try
            {
                // Variaveis para guardar valor de minimo de horas, total de horas e para a criação do nome do registro
                String NomeUltimoProjetoTrabalhado = String.Empty;
                Int32 ValorMinimoProjeto = 0;
                Int32 HorasTotaisProjeto = 0;

                #region Entidades e retrieves

                // Chamada da BaseBusiness da entidade de Ultimo Projeto Trabalhado
                var smt_latest_project_worked_Business = new Business.smt_latest_project_worked_Business(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
                var SmartParameterBusiness = new Business.SmartParameterBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
                var msdyn_timeentryBusiness = new Business.Msdyn_timeentryBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);

                // smt_type_hourBusiness quoteBusiness = new smt_type_hourBusiness(context.OrganizationService, context.OrganizationServiceAdmin, context.TracingService, null);

                // TODO: UTILIZAR EARLYBOUND
                // chamada das entidades que serão utilizadas no código
                msdyn_timeentry EntradaHoras = localcontext.GetTarget<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry>();
                smt_smartparameter ParametroSmart = new smt_smartparameter();
                smt_latest_project_worked UltimoProjetotrabalhado = new smt_latest_project_worked();
                smt_programa Programas = new smt_programa();
                msdyn_project Projeto = new msdyn_project();
                BookableResource RecusoReservavel = new BookableResource();
                smt_log Log = new smt_log();            

                // Retrieve de dados  da classe de entrada de horas
                ColumnSet EntradaHora_CS = new ColumnSet(new String[] { "msdyn_project", "msdyn_bookableresource", "msdyn_entrystatus" });
                EntradaHoras = (msdyn_timeentry)localcontext.OrganizationService.Retrieve(EntradaHoras.LogicalName, EntradaHoras.Id, EntradaHora_CS);

                // Retrieve de dados  da classe de projeto
                ColumnSet Projeto_CS = new ColumnSet(new String[] { "smt_programa" });
                Projeto = (msdyn_project)localcontext.OrganizationService.Retrieve(Projeto.LogicalName, ((EntityReference)EntradaHoras.msdyn_project).Id, Projeto_CS);

                // Retrieve de dados  da classe de programas 
                // TODO: Andre - Corrigido
                if (!Projeto.Contains("smt_programa"))
                {
                    return;
                }
                ColumnSet Programas_CS = new ColumnSet(new String[] { "smt_bt_update_curriculum" });
                Programas = (smt_programa)localcontext.OrganizationService.Retrieve(Programas.LogicalName, ((EntityReference)Projeto.Attributes["smt_programa"]).Id, Programas_CS);

                // Retrieve de dados  da classe de recurso reservaveis
                ColumnSet RecusoReservavel_CS = new ColumnSet(new String[] { "userid", "msdyn_organizationalunit" });
                RecusoReservavel = (BookableResource)localcontext.OrganizationService.Retrieve(RecusoReservavel.LogicalName, ((EntityReference)EntradaHoras.msdyn_bookableresource).Id, RecusoReservavel_CS);

                #endregion

                // Verifica se o projeto e o recurso possuem dados
                if (((EntityReference)EntradaHoras.msdyn_project).Id != null && ((EntityReference)EntradaHoras.msdyn_bookableresource).Id != null && ((EntityReference)Projeto.Attributes["smt_programa"]).Id != null)
                {
                    if (Programas.GetAttributeValue<bool>("smt_bt_update_curriculum") == true)
                    {
                        // TODO: Andre - Corrigido                   
                        if (EntradaHoras.msdyn_entryStatus.Value.Equals(msdyn_timeentrystatus.Approved.GetHashCode()))
                        {
                            #region Verifica dados dos fetch da classe business e realiza um foreach

                            // Verifica os registros do Ultimo Projeto trabalhado, desde que esteja relacionado  com o recurso / projeto em questão
                            Entity RegistrosUltimoProjeto = smt_latest_project_worked_Business.RegistrosUltimosProjetosTrabalhados(((EntityReference)EntradaHoras.msdyn_project).Id, ((EntityReference)EntradaHoras.msdyn_bookableresource).Id);
                            String RegistrosSmartParametro = SmartParameterBusiness.ParametroSmartHoraProjeto(RecusoReservavel.GetAttributeValue<EntityReference>("msdyn_organizationalunit").Id);
                            EntityCollection RegistrosHorasLancadas = msdyn_timeentryBusiness.HoraslançadasRecurso(EntradaHoras.GetAttributeValue<EntityReference>("msdyn_bookableresource").Id, EntradaHoras.GetAttributeValue<EntityReference>("msdyn_project").Id);

                            // Caso não seja encontrado nenhum parametro
                            if (RegistrosSmartParametro == null)
                            {
                                Log.smt_name = $"Não foi possível localizar o registro de Parametro, para Atualizar os Projetos trabalhados do Recurso";
                                Log.smt_st_entityname = $"Ultimo Projeto Trabalhado (smt_latest_project_worked)";
                                Log.smt_pl_eventtype = new OptionSetValue(100000000);
                                Log.smt_dt_eventdate = DateTime.Now;
                                Log.smt_st_recordname = "Enviar e-mail para a equipe";
                                Log.smt_st_recordid = ((EntityReference)EntradaHoras.msdyn_project).Id.ToString();
                                Log.smt_tx_message = $"Não foi possível encontrar encontrar um parametro, é necessário cria-lo em: Parametros  Smart (smt_smartparameter)";

                                localcontext.OrganizationService.Create(Log);
                                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.NoParameter_01));
                            }

                            // Pega o valor do Parametro e converte para inteiro
                            ValorMinimoProjeto = Int32.Parse(RegistrosSmartParametro);

                            // Verifica todas entradas de horas do recurso
                            foreach (Entity RegistrosHoras in RegistrosHorasLancadas.Entities)
                            {
                                if (RegistrosHoras.Id != null)
                                {
                                    // HorasTotaisProjeto += RegistrosHoras.GetAttributeValue<Int32>("msdyn_duration") / 60;
                                    HorasTotaisProjeto += ((Int32)((AliasedValue)RegistrosHoras["msdyn_duration_sum"]).Value) / 60;
                                }
                            }

                            #endregion

                            if (HorasTotaisProjeto >= ValorMinimoProjeto)
                            {
                                #region criação de registro da entidade "Ultimos projetos Trabalhados"
                                // Caso não exista um registro relacionado a tarefa e o recurso, ele será criado
                                if (RegistrosUltimoProjeto == null)
                                {
                                    UltimoProjetotrabalhado.smt_lp_resource = (EntityReference)EntradaHoras.msdyn_bookableresource;
                                    UltimoProjetotrabalhado.smt_lp_project = (EntityReference)EntradaHoras.msdyn_project;
                                    UltimoProjetotrabalhado.smt_int_hours_worked = HorasTotaisProjeto;
                                    NomeUltimoProjetoTrabalhado = ((EntityReference)EntradaHoras.msdyn_bookableresource).Name.ToString() + " - " + ((EntityReference)EntradaHoras.msdyn_project).Name.ToString();
                                    UltimoProjetotrabalhado.smt_name = NomeUltimoProjetoTrabalhado;

                                    // Cria o registro
                                    localcontext.OrganizationService.Create(UltimoProjetotrabalhado);
                                }
                                #endregion

                                #region atualização de registro da entidade "Ultimos projetos Trabalhados"

                                // Caso ele exista, será atualizado
                                else
                                {
                                    UltimoProjetotrabalhado.Id = RegistrosUltimoProjeto.Id;
                                    UltimoProjetotrabalhado.smt_int_hours_worked = HorasTotaisProjeto;

                                    // Atualiza o registro
                                    localcontext.OrganizationService.Update(UltimoProjetotrabalhado);
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

            base.ExecuteCrmPlugin(localcontext);
        }

        // toda vez q a hora é aprovada
        private void TimeBankCalculation(msdyn_timeentry timeEntry, msdyn_timeentry preTimeEntry, LocalPluginContext localContext, List<Resx> messages)
        {
            var type = timeEntry.msdyn_typeEnum != null ? timeEntry.msdyn_typeEnum : preTimeEntry.msdyn_typeEnum;

            if (type != msdyn_timeentrytype.Vacation)
            {
                Msdyn_timeentryBusiness timeOps = new Msdyn_timeentryBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
                IOrganizationService orgService = localContext.OrganizationService;
                Decimal? timeValue = null;

                Boolean isAbsence = timeOps.IsAbsence(orgService, preTimeEntry);
                if (isAbsence)
                {
                    Decimal? DurationHours = preTimeEntry.msdyn_duration / 60;

                    if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Approved && preTimeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Submitted) // Entrada Aprovada
                    {
                        timeOps.AbsenceSum(DurationHours, preTimeEntry, orgService);
                    }

                    else if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Returned && preTimeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.RecallRequested) // Entrada Recuperada
                    {
                        timeOps.AbsenceSum(-DurationHours, preTimeEntry, orgService);
                    }
                }

                Boolean isBank = timeOps.IsTimeBank(orgService, preTimeEntry);
                if (isBank == true)
                {
                    timeValue = timeOps.GetTimeValue(preTimeEntry.msdyn_bookableresource, orgService, preTimeEntry);
                    if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Approved && preTimeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Submitted)
                    {
                        timeOps.TimeBankSum(timeValue, preTimeEntry, orgService);
                    }
                    else if (timeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.Returned && preTimeEntry.msdyn_entryStatusEnum == msdyn_timeentrystatus.RecallRequested)
                    {
                        timeOps.TimeBankSubtraction(timeValue, preTimeEntry, orgService);
                    }
                }
            }
        }

    }
}
