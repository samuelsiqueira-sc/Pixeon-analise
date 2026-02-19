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

namespace Smart.ExtendedPSA.Extends.Business
{

    /// <summary>
    /// kguhi
    /// </summary>
    public class Msdyn_timeentryBusiness : BaseBusiness
    {
        /// <summary>
        /// asd
        /// </summary>
        /// <param name="service">sd</param>
        /// <param name="serviceAdmin">asd</param>
        /// <param name="tracingService">ads</param>
        /// <param name="messages">da</param>
        public Msdyn_timeentryBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        #region DAO
        /// <summary>
        /// Data de inicio de entrada de horas
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>retornoDataInicio</returns>  
        public msdyn_timeentry DataDeInicio(Guid guid)
        {
            String dataDeInicio = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                          <entity name='msdyn_timeentry'>
                                            <attribute name='msdyn_timeentryid' />
                                            <attribute name='msdyn_projecttask' />
                                            <attribute name='msdyn_project' />
                                            <attribute name='msdyn_date' />
                                            <order attribute='msdyn_date' descending='false' />
                                            <filter type='and'>
                                              <condition attribute='msdyn_projecttask' operator='eq' uitype='msdyn_projecttask' value='{" + guid + @"}' />
                                              <condition attribute='msdyn_entrystatus' operator='eq' value='192350002' />
                                            </filter>
                                          </entity>
                                        </fetch>";

            // msdyn_timeentry retornoDataInicio = Service.RetrieveMultiple(new FetchExpression(dataDeInicio)).Entities.FirstOrDefault().ToEntity<msdyn_timeentry>();

            List<Entity> lista = Service.RetrieveMultiple(new FetchExpression(dataDeInicio)).Entities.ToList();

            if (lista.Count > 0)
            {
                msdyn_timeentry retornoDataInicio = lista.FirstOrDefault().ToEntity<msdyn_timeentry>();

                foreach (msdyn_timeentry timeentry in lista)
                {
                    if (timeentry.msdyn_date < retornoDataInicio.msdyn_date)
                        retornoDataInicio = timeentry;
                }

                if (retornoDataInicio != null)
                    return retornoDataInicio;
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Data de termino da entrada de horas
        /// </summary>
        /// <param name="guid">guid</param>
        /// <returns>retornoDataFim</returns>
        public msdyn_timeentry DataDeTermino(Guid guid)
        {
            // Responsavel por verificar se a entrada de horas pertence ao mesmo id do projeto

            // Verifica se a entrada de horas foi aprovada

            String dataDeFim = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                      <entity name='msdyn_timeentry'>
                                        <attribute name='msdyn_timeentryid' />
                                        <attribute name='msdyn_projecttask' />
                                        <attribute name='msdyn_project' />
                                        <attribute name='msdyn_date' />
                                        <order attribute='msdyn_date' descending='true' />
                                        <filter type='and'>
                                          <condition attribute='msdyn_projecttask' operator='eq' uitype='msdyn_projecttask' value='{" + guid + @"}' />
                                          <condition attribute='msdyn_entrystatus' operator='eq' value='192350002' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            // msdyn_timeentry retornoDataFim = Service.RetrieveMultiple(new FetchExpression(dataDeFim)).Entities.FirstOrDefault().ToEntity<msdyn_timeentry>();

            List<Entity> lista = Service.RetrieveMultiple(new FetchExpression(dataDeFim)).Entities.ToList();
            if (lista.Count > 0)
            {
                msdyn_timeentry retornoDataFim = lista.FirstOrDefault().ToEntity<msdyn_timeentry>();

                foreach (msdyn_timeentry timeentry in lista)
                {
                    if (timeentry.msdyn_date > retornoDataFim.msdyn_date)
                        retornoDataFim = timeentry;
                }
                // Retorna a entidade caso ela possua dados
                // Caso negativo retorna nulo
                if (retornoDataFim != null)
                    return retornoDataFim;
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Retorna todas entradas de horas de um usuário e projeto em específico
        /// </summary>
        /// <param name="Recurso">Recurso</param>
        /// <param name="Projeto">Projeto</param>
        /// <returns>Retorno</returns>
        public EntityCollection HoraslançadasRecurso(Guid Recurso, Guid Projeto)
        {
            String dataDeFim = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'>
                                      <entity name='msdyn_timeentry'>
                                        <attribute name='msdyn_duration' alias='msdyn_duration_sum' aggregate='sum'/>
                                        <filter type='and'>
                                          <condition attribute='msdyn_bookableresource' operator='eq' value='{" + Recurso + @"}' />
                                          <condition attribute='msdyn_project' operator='eq' value='{" + Projeto + @"}' />
                                        </filter>
                                      </entity>
                                    </fetch>";

            EntityCollection RetornaHorasLançadas = Service.RetrieveMultiple(new FetchExpression(dataDeFim));

            if (RetornaHorasLançadas != null)
            {
                return RetornaHorasLançadas;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Método que calcula quantas horas o recurso ja lançou no dia
        /// </summary>
        /// <param name="context">Traz o contexto do Formulário</param>
        /// <param name="lpResource">Referência ao Recurso Reservável</param>
        /// <param name="timeEntry">Entidade vinda do Target</param>
        /// <returns>Reorna a quantidade de horas que o recurso gastou em um dia</returns>
        public Decimal GetHoursSpent(CrmServiceContext context, EntityReference lpResource, msdyn_timeentry timeEntry)
        {
            List<msdyn_timeentry> regList;
            var timeStart = timeEntry.msdyn_date.Value.Date;
            var timeEnd = timeEntry.msdyn_date.Value.AddDays(1).Date;
            using (context)
            {
                List<msdyn_timeentry> result = (from time in context.CreateQuery<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry>()
                                                                                         where time.msdyn_bookableresource == lpResource
                                                                                         && (time.msdyn_date.Value >= timeStart && time.msdyn_date.Value < timeEnd)
                                                                                         && (time.smt_lp_type_hours.Id == timeEntry.smt_lp_type_hours.Id
                                                                                         || (time.msdyn_typeEnum == msdyn_timeentrytype.Absence || time.msdyn_typeEnum == msdyn_timeentrytype.Vacation))
                                                                                         && time.msdyn_timeentryId != timeEntry.msdyn_timeentryId
                                                                                         select new CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry
                                                                                         {
                                                                                             msdyn_duration = time.msdyn_duration
                                                                                         }).ToList();
                regList = result;
            }
            Decimal tdWorkedHours = 0;
            foreach (msdyn_timeentry timeEn in regList)
            {
                tdWorkedHours += timeEn.msdyn_duration.Value;
            }
            return tdWorkedHours;
        }

        /// <summary>
        /// Método que calcula quantas horas o recurso ja lançou no dia
        /// </summary>
        /// <param name="context">Traz o contexto do Formulário</param>
        /// <param name="lpResource">Referência ao Recurso Reservável</param>
        /// <param name="timeEntry">Entidade vinda do Target</param>
        /// <returns>Reorna a quantidade de horas que o recurso gastou em um dia</returns>
        public Decimal GetTotalHoursSpent(CrmServiceContext context, EntityReference lpResource, msdyn_timeentry timeEntry)
        {
            List<msdyn_timeentry> regList;
            var timeStart = timeEntry.msdyn_date.Value.Date;
            var timeEnd = timeEntry.msdyn_date.Value.AddDays(1).Date;

            smt_type_hours typeHour = (from type in context.CreateQuery<smt_type_hours>()
                                       where type.smt_pl_type_hours.Value == 100000000 // Tipo Normal
                                       select new smt_type_hours
                                       {
                                           smt_type_hoursId = type.smt_type_hoursId
                                       }).FirstOrDefault();

            using (context)
            {
                List<msdyn_timeentry> result = (from time in context.CreateQuery<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry>()
                                                where time.msdyn_bookableresource == lpResource
                                                && (time.msdyn_date.Value >= timeStart && time.msdyn_date.Value < timeEnd)
                                                && (time.msdyn_typeEnum == msdyn_timeentrytype.Work || time.msdyn_typeEnum == msdyn_timeentrytype.Absence || time.msdyn_typeEnum == msdyn_timeentrytype.Vacation)
                                                && time.msdyn_timeentryId != timeEntry.msdyn_timeentryId
                                                select new CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry
                                                {
                                                    msdyn_duration = time.msdyn_duration
                                                }).ToList();
                regList = result;
            }
            Decimal tdWorkedHours = 0;
            foreach (msdyn_timeentry timeEn in regList)
            {
                tdWorkedHours += timeEn.msdyn_duration.Value;
            }
            return tdWorkedHours;
        }
        /// <summary>
        /// Método que calcula quantas horas o recurso ja lançou no dia
        /// </summary>
        /// <param name="context">Traz o contexto do Formulário</param>
        /// <param name="lpResource">Referência ao Recurso Reservável</param>
        /// <param name="timeEntry">Entidade vinda do Target</param>
        /// <returns>Reorna a quantidade de horas que o recurso gastou em um dia</returns>
        public Decimal GetNormalHoursSpent(CrmServiceContext context, EntityReference lpResource, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry)
        {
            List<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry> regList;
            using (context)
            {
                var timeStart = timeEntry.msdyn_date.Value.Date;
                var timeEnd = timeEntry.msdyn_date.Value.AddDays(1).Date;

                smt_type_hours typeHour = (from type in context.CreateQuery<smt_type_hours>()
                                           where type.smt_pl_type_hours.Value == 100000000
                                           select new smt_type_hours
                                           {
                                               smt_type_hoursId = type.smt_type_hoursId
                                           }).FirstOrDefault();

                List<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry> result = (from time in context.CreateQuery<CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry>()
                                                                                         where time.msdyn_bookableresource == lpResource
                                                                                         && (time.msdyn_date.Value >= timeStart && time.msdyn_date.Value < timeEnd)
                                                                                         && (time.smt_lp_type_hours.Id == typeHour.smt_type_hoursId 
                                                                                         || (time.msdyn_typeEnum == msdyn_timeentrytype.Absence 
                                                                                         && time.smt_pl_type_absenceEnum == smt_pl_type_absenceEnum.Justificada))
                                                                                         select new CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry
                                                                                         {
                                                                                             msdyn_duration = time.msdyn_duration
                                                                                         }).ToList();
                regList = result;
            }
            Decimal tdWorkedHours = 0;
            foreach (CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEn in regList)
            {
                tdWorkedHours += timeEn.msdyn_duration.Value;
            }
            tdWorkedHours = tdWorkedHours / 60;
            return tdWorkedHours;
        }
        /// <summary>
        /// Método que irá dar retrieve na Categoria do recurso relativa ao recurso
        /// </summary>
        /// <param name="orgsSrvice">Variável de serviço que irá fazer a requisição dos dados</param>
        /// <param name="lpResource">Referência ao Recurso Reservável</param>
        /// <returns>Retorna o campo smt_work_hours</returns>
        public smt_model_contract GetCategoryWorkedHours(EntityReference lpResource)
        {
            BookableResource res = Service.Retrieve(lpResource.LogicalName, lpResource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract)).ToEntity<BookableResource>();
            if (res.smt_lp_model_contract == null)
            {
                // TODO: Wesley - RESX
                throw new InvalidPluginExecutionException("Recurso sem modelo contrato registrado");
            }
            smt_model_contract workHours = Service.Retrieve(res.smt_lp_model_contract.LogicalName, res.smt_lp_model_contract.Id, new ColumnSet(smt_model_contract.Fields.smt_dc_time_load, smt_model_contract.Fields.smt_mc_work_days)).ToEntity<smt_model_contract>();
            return workHours;
        }
        /// <summary>
        /// Método que irá dar retrieve no Tipo de Horas relativa ao recurso
        /// </summary>
        /// <param name="orgsSrvice">Variável de serviço que irá fazer a requisição dos dados</param>
        /// <param name="lpTypeHours">Variável que contem o campo lookup para Tipo de Horas</param>
        /// <returns>Retorna a entridade Tipo de Horas</returns>
        public smt_type_hours GetTypeOfHours(EntityReference lpTypeHours)
        {
            smt_type_hours tpHours = Service.Retrieve(lpTypeHours.LogicalName, lpTypeHours.Id, new ColumnSet(smt_type_hours.Fields.smt_pl_type_hours)).ToEntity<smt_type_hours>();
            return tpHours;
        }
        /// <summary>
        /// Método que irá buscar um feriado de acordo com a data do lançamento da entrada
        /// </summary>
        /// <param name="context">Traz o contexto do Formulário</param>
        /// <param name="timeEntry">Entidade vinda do Target</param>
        /// <returns>Uma lista com os feriados</returns>
        public List<smt_holiday> GetHolydays(CrmServiceContext context, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry)
        {
            List<smt_holiday> holyday;
            using (context)
            {
                List<smt_holiday> result = (from holy in context.CreateQuery<smt_holiday>()
                                            where holy.smt_dt_holiday_date.Value == timeEntry.msdyn_date.Value
                                            select new smt_holiday
                                            {
                                            }).ToList();
                holyday = result;
            }
            return holyday;
        }

        /// <summary>
        /// Traz o registro completo do recurso reservável
        /// </summary>
        /// <param name="lpResource">Referência a entidade Recurso Reservável</param>
        /// <param name="orgService">Variavel de serviço geralente utilizada para Retrieves</param>
        /// <returns>Retorna um registro do tipo BookableResource</returns>
        public BookableResource GetResource(EntityReference lpResource, IOrganizationService orgService)
        {
            BookableResource resource = orgService.Retrieve(lpResource.LogicalName, lpResource.Id, new ColumnSet(BookableResource.Fields.smt_dc_bank_hours, BookableResource.Fields.smt_dc_absence_hours)).ToEntity<BookableResource>();
            return resource;
        }

        /// <summary>
        /// Método que irá retornar o valor do Banco de Horas de acordo com o Tipo de Banco de Horas selecionado no Tipo de Horas
        /// </summary>
        /// <param name="lpResource">Referência ao recurso reservável</param>
        /// <param name="orgService">Variavel de serviço geralente utilizada para Retrieves</param>
        /// <param name="timeEntry">Variavel que contém o target</param>
        /// <returns>Valor do campo Banco de Horas</returns>
        public Decimal? GetTimeValue(EntityReference lpResource, IOrganizationService orgService, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry)
        {
            Decimal? timeValue = null;
            BookableResource res = orgService.Retrieve(lpResource.LogicalName, lpResource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract)).ToEntity<BookableResource>();
            if (timeEntry.smt_lp_type_hours != null)
            {
                smt_type_hours type = orgService.Retrieve(timeEntry.smt_lp_type_hours.LogicalName, timeEntry.smt_lp_type_hours.Id, new ColumnSet(smt_type_hours.Fields.smt_pl_type_hours)).ToEntity<smt_type_hours>();
                switch (type.smt_pl_type_hours.Value)
                {
                    case 100000003:
                        smt_model_contract contract1 = orgService.Retrieve(res.smt_lp_model_contract.LogicalName, res.smt_lp_model_contract.Id, new ColumnSet(smt_model_contract.Fields.smt_smt_dc_sunday_hours_bank)).ToEntity<smt_model_contract>();
                        if (contract1.smt_smt_dc_sunday_hours_bank != null)
                        {
                            timeValue = contract1.smt_smt_dc_sunday_hours_bank.Value / 100;
                        }
                        break;
                    case 100000004:
                        smt_model_contract contract2 = orgService.Retrieve(res.smt_lp_model_contract.LogicalName, res.smt_lp_model_contract.Id, new ColumnSet(smt_model_contract.Fields.smt_dc_holidays_hours_bank)).ToEntity<smt_model_contract>();
                        if (contract2.smt_dc_holidays_hours_bank != null)
                        {
                            timeValue = contract2.smt_dc_holidays_hours_bank.Value / 100;
                        }
                        break;
                    default:
                        break;
                }
            }
            return timeValue;
        }

        /// <summary>
        /// Metodo que checa se a entrada de horas é do tipo Banco de Horas
        /// </summary>
        /// <param name="orgService">Variavel de serviço geralente utilizada para Retrieves</param>
        /// <param name="timeEntry">Variavel que contém o target</param>
        /// <returns>Retorna verdadeiro se for Banco de Horas se não retorna falso</returns>
        public Boolean IsTimeBank(IOrganizationService orgService, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry)
        {
            Boolean checkBank;
            smt_type_hours type = orgService.Retrieve(timeEntry.smt_lp_type_hours.LogicalName, timeEntry.smt_lp_type_hours.Id, new ColumnSet(smt_type_hours.Fields.smt_pl_type_hours)).ToEntity<smt_type_hours>();
            if (type.smt_pl_type_hours.Value == 100000001 || type.smt_pl_type_hours.Value == 100000003 || type.smt_pl_type_hours.Value == 100000004)
            {
                checkBank = true;
            }
            else
            {
                checkBank = false;
            }
            return checkBank;
        }

        /// <summary>
        /// Metodo que verifica se a entrada de horas é do tipo Ausência, Compensação de Horas
        /// </summary>
        /// <param name="orgService">Variavel de serviço geralente utilizada para Retrieves</param>
        /// <param name="timeEntry">Variavel que contém o target</param>
        /// <returns>Retorna verdadeiro se for Ausência se não retorna falso</returns>
        public Boolean IsAbsence(IOrganizationService orgService, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry)
        {
            Boolean checkAbsence = false;
            ColumnSet cs = new ColumnSet(nameof(msdyn_timeentry.Fields.msdyn_type), nameof(msdyn_timeentry.Fields.smt_pl_type_absence));
            msdyn_timeentry timeEntryData = orgService.Retrieve(timeEntry.LogicalName, timeEntry.Id, cs).ToEntity<msdyn_timeentry>();

            OptionSetValue typeHours = timeEntryData.msdyn_type;
            OptionSetValue typeAbsence = timeEntryData.smt_pl_type_absence;

            if (typeHours.Value == 192350001)
            {
                if (typeAbsence.Value == 100000001)
                {
                    checkAbsence = true;
                }
            }
            return checkAbsence;
        }

        /// <summary>
        /// Valida se a entrada de horas está sendo lançada em uma tarefa pai.
        /// </summary>
        /// <param name="target"> entrada de horas </param>
        public void IfProjectTaskIsParent(msdyn_timeentry target)
        {
            using (CrmServiceContext context = new CrmServiceContext(ServiceAdmin))
            {
                List<msdyn_projecttask> childTasks = context.CreateQuery<msdyn_projecttask>().Where(p => p.msdyn_parenttask.Id == target.msdyn_projectTask.Id).ToList();

                if (childTasks.Count > 0)
                {
                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.IPTP01));
                }
            }
        }
        #endregion

        #region BO
        /// <summary>
        /// Método responsável por checar se o usuario está tentando criar uma entrada de horas do tipo normal maior que o seu expediente
        /// </summary>
        /// <param name="workedHours">Entidade que contêm o campo com as horas de expediente da categoria</param>
        /// <param name="launchingHours">Entidade vinda do Target</param>
        /// <param name="context">Traz o contexto do Formulário</param>
        /// <param name="lpResource">Referência ao Recurso Reservável</param>
        public void CheckHours(smt_model_contract workedHours, msdyn_timeentry launchingHours, CrmServiceContext context, EntityReference lpResource)
        {
            // O Dynamics traz o campo duração em minutos, por tanto se eu lançar 3hrs de trabalho ele vai me trazer 180
            if (launchingHours.msdyn_typeEnum == msdyn_timeentrytype.Work)
            {
                // Horas Normais
                smt_type_hours tipo = GetTypeOfHours(launchingHours.smt_lp_type_hours);
                if (tipo.smt_pl_type_hours.Value == 100000000)
                {
                    // TODO: Wesley Corrigido. (Mensagens Resx)

                   Boolean workingDays = false;

                    for (int i = 0; i < workedHours.smt_mc_work_days.Count; i++)
                    {
                        if ((int)launchingHours.msdyn_date.Value.DayOfWeek == workedHours.smt_mc_work_days[i].Value)
                        {
                            workingDays = true;
                        }
                    }
                    if (workingDays == true)
                    {
                        Decimal duration = launchingHours.msdyn_duration.Value / 60;

                        // Horas trabalhadas é um campo inteiro, por tanto se eu registrar 8 no formulario o dynamics me devolve esse numero, por isso a converção de inteiro pra minuto
                        Decimal worHours = workedHours.smt_dc_time_load.Value;
                        List<smt_holiday> holy = GetHolydays(context, launchingHours);
                        if (holy.Count > 0)
                        {
                            // 01
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR01));
                        }
                        else
                        {
                            if (duration <= worHours)
                            {
                                Decimal hoursSpent = GetHoursSpent(context, lpResource, launchingHours);
                                Decimal totalHours = hoursSpent + launchingHours.msdyn_duration.Value;
                                totalHours = totalHours / 60;
                                if (totalHours > worHours)
                                {
                                    throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR02));
                                }
                            }
                            else
                            {
                                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR02));
                            }
                        }
                    }
                    else
                    {
                        List<smt_holiday> holy = GetHolydays(context, launchingHours);
                        if (holy.Count > 0)
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR03));
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR04));
                        }
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000001) // Banco de Horas - Dia Útil
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR05));
                    }
                    else
                    {
                        Boolean workingDays = false;
                        for (int i = 0; i < workedHours.smt_mc_work_days.Count; i++)
                        {
                            if ((int)launchingHours.msdyn_date.Value.DayOfWeek != DayOfWeek.Sunday.GetHashCode())
                            {
                                workingDays = true;
                            }
                        }
                        if (workingDays == true)
                        {
                            Decimal duration = launchingHours.msdyn_duration.Value / 60;
                            Decimal hoursSpent = GetNormalHoursSpent(context, lpResource, launchingHours);
                            Decimal worHours = workedHours.smt_dc_time_load.Value;
                            if (hoursSpent < worHours & (int)launchingHours.msdyn_date.Value.DayOfWeek != 6)
                            {
                                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR18));
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR06));
                        }
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000003) // Banco de Horas - Domingo
                {
                    // (int)DateTime.Value.DayOfWeek retorna um numero equivalente ao dia da semana. Sendo que a semana começa no Domingo(0) e vai até Sabado(6)
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR07));
                    }
                    else
                    {
                        if ((int)launchingHours.msdyn_date.Value.DayOfWeek != 0)
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR08));
                        }
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000004) // Banco de Horas - Feriados
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count == 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR09));
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000002) // Horas Extras - Dias Úteis
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {

                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR10));
                    }
                    else
                    {
                        Boolean workingDays = false;
                        for (int i = 0; i < workedHours.smt_mc_work_days.Count; i++)
                        {
                            if ((int)launchingHours.msdyn_date.Value.DayOfWeek == workedHours.smt_mc_work_days[i].Value)
                            {
                                workingDays = true;
                            }
                        }
                        if (workingDays == true)
                        {
                            Decimal duration = launchingHours.msdyn_duration.Value / 60;
                            Decimal hoursSpent = GetNormalHoursSpent(context, lpResource, launchingHours);
                            Decimal worHours = workedHours.smt_dc_time_load.Value;
                            if (hoursSpent < worHours)
                            {
                                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR11));
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR12));

                        }
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000005) // Horas Extras - Feriados
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count == 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR13));
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000006) // Horas Extras - Domingos
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR14));
                    }
                    else
                    {
                        if ((int)launchingHours.msdyn_date.Value.DayOfWeek != 0)
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR15));
                        }
                    }
                }
                else if (tipo.smt_pl_type_hours.Value == 100000007) // Horas Extras - Sábados
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR16));
                    }
                    else
                    {
                        if ((int)launchingHours.msdyn_date.Value.DayOfWeek != 6)
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR17));
                        }
                    }
                }
            }
            else if (launchingHours.msdyn_typeEnum == msdyn_timeentrytype.Absence || launchingHours.msdyn_typeEnum == msdyn_timeentrytype.Vacation)
            {
                Boolean workingDays = false;

                for (int i = 0; i < workedHours.smt_mc_work_days.Count; i++)
                {
                    if ((int)launchingHours.msdyn_date.Value.DayOfWeek == workedHours.smt_mc_work_days[i].Value)
                    {
                        workingDays = true;
                    }
                }
                if (workingDays == true)
                {
                    Decimal duration = launchingHours.msdyn_duration.Value / 60;

                    // Horas trabalhadas é um campo inteiro, por tanto se eu registrar 8 no formulario o dynamics me devolve esse numero, por isso a converção de inteiro pra minuto
                    Decimal worHours = workedHours.smt_dc_time_load.Value;
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {
                        // 01
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR20));
                    }
                    else
                    {
                        if (duration <= worHours)
                        {
                            Decimal hoursSpent = GetTotalHoursSpent(context, lpResource, launchingHours);
                            Decimal totalHours = hoursSpent + launchingHours.msdyn_duration.Value;
                            totalHours = totalHours / 60;
                            if (totalHours > worHours)
                            {
                                throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR19));
                            }
                        }
                        else
                        {
                            throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR19));
                        }
                    }
                }
                else
                {
                    List<smt_holiday> holy = GetHolydays(context, launchingHours);
                    if (holy.Count > 0)
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR20));
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException(Messages.GetMessageById(ResxExtension.MAXHOUR21));
                    }
                }
            }
        }

        /// <summary>
        /// Método responsável por subtrair o Banco de Horas do Recurso levando em conta o tipo de Banco de Horas escolhido 
        /// </summary>
        /// <param name="timeBankValue">Valor do Bando de Horas escolhido</param>
        /// <param name="timeEntry">Entidade do target usada para ter ecesso ao campo Duração</param>
        /// <param name="orgService">Variavel de serviço geralente utilizada para Retrieves</param>
        public void TimeBankSubtraction(Decimal? timeBankValue, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry, IOrganizationService orgService)
        {
            Decimal? balance = 0;
            Decimal? duration = (Decimal?)timeEntry.msdyn_duration.Value;
            BookableResource resource = GetResource(timeEntry.msdyn_bookableresource, orgService);
            if (timeBankValue != null && timeBankValue != 0)
            {
                balance = (duration + (timeBankValue * duration)) / 60;
                if (resource.smt_dc_bank_hours != null)
                {
                    BookableResource res = new BookableResource();
                    res.smt_dc_bank_hours = resource.smt_dc_bank_hours.Value - balance;
                    res.BookableResourceId = resource.BookableResourceId;
                    orgService.Update(res);
                }
            }
            else if (resource.smt_dc_bank_hours != null)
            {
                BookableResource res = new BookableResource();
                res.smt_dc_bank_hours = resource.smt_dc_bank_hours.Value - (duration / 60);
                res.BookableResourceId = resource.BookableResourceId;
                orgService.Update(res);
            }
        }
        /// <summary>
        /// Método responsável por somar o Banco de Horas do Recurso levando em conta o tipo de Banco de Horas escolhido 
        /// </summary>
        /// <param name="timeBankValue">Valor do Bando de Horas escolhido</param>
        /// <param name="timeEntry">Entidade do target usada para ter ecesso ao campo Duração</param>
        /// <param name="orgService">Variavel de serviço geralente utilizada para Retrieves</param>
        public void TimeBankSum(Decimal? timeBankValue, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry, IOrganizationService orgService)
        {
            Decimal? balance = 0;
            Decimal? duration = (Decimal?)timeEntry.msdyn_duration.Value;
            BookableResource resource = GetResource(timeEntry.msdyn_bookableresource, orgService);
            if (timeBankValue != null && timeBankValue != 0)
            {
                balance = (duration + (timeBankValue * duration)) / 60;
                if (resource.smt_dc_bank_hours != null)
                {
                    BookableResource res = new BookableResource();
                    res.smt_dc_bank_hours = resource.smt_dc_bank_hours.Value + balance;
                    res.BookableResourceId = resource.BookableResourceId;
                    orgService.Update(res);
                }
                else
                {
                    BookableResource res = new BookableResource();
                    res.smt_dc_bank_hours = balance;
                    res.BookableResourceId = resource.BookableResourceId;
                    orgService.Update(res);
                }
            }
            else if (resource.smt_dc_bank_hours != null)
            {
                BookableResource res = new BookableResource();
                res.smt_dc_bank_hours = resource.smt_dc_bank_hours.Value + (duration / 60);
                res.BookableResourceId = resource.BookableResourceId;
                orgService.Update(res);
            }
            else
            {
                BookableResource res = new BookableResource();
                res.smt_dc_bank_hours = duration / 60;
                res.BookableResourceId = resource.BookableResourceId;
                orgService.Update(res);
            }
        }

        /// <summary>
        /// Método responsável por somar o saldo de ausência 
        /// </summary>
        /// <param name="absenceValue">Quantidades de hora de ausência da entrada de horas</param>
        /// <param name="timeEntry">Referência do registro de entrada de horas</param>
        /// <param name="orgService">Serviço de Organização</param>
        public void AbsenceSum(Decimal? absenceValue, CRM.Smart.ExtendedPSA.Extends.Earlybound.msdyn_timeentry timeEntry, IOrganizationService orgService)
        {
            BookableResource resource = GetResource(timeEntry.msdyn_bookableresource, orgService);

            if (resource.smt_dc_absence_hours != null)
            {
                resource.smt_dc_absence_hours += absenceValue;
            }
            else
            {
                resource.smt_dc_absence_hours = absenceValue;
            }

            orgService.Update(resource);
        }

        /// <summary>
        /// Método responsável por somar o saldo de ausência 
        /// </summary>
        /// <param name="absenceValue">Quantidades de hora de ausência da entrada de horas</param>
        /// <param name="timeEntry">Referência do registro de entrada de horas</param>
        /// <param name="orgService">Serviço de Organização</param>
        public smt_type_hours GetTypeHours()
        {
          using(var crmContext = new CrmServiceContext(ServiceAdmin))
            {
                var type = (from hours in crmContext.smt_type_hoursSet
                            where hours.smt_pl_type_hoursEnum == smt_type_hours_smt_pl_type_hours.Normal
                            select hours).FirstOrDefault();

                return type;
            }
        }

        #endregion

    }
}
