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
    /// Métodos em Holiday Request
    /// </summary>
    public class BusinessVacationRequest : BaseBusiness
    {
        /// <summary>
        /// Método construtor da Business
        /// </summary>
        /// <param name="service"> service InitialUser</param>
        /// <param name="serviceAdmin"> service Admin</param>
        /// <param name="tracingService"> tracing </param>
        /// <param name="messages"> mensagens resx</param>
        public BusinessVacationRequest(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        /// <summary>
        /// Atualizar dias utilizados em Período aquisitivo, quando a solicitação de férias for aprovada.
        /// </summary>
        /// <param name="vacationRequestPreImage"> Solicitação de Férias Image</param>
        /// <param name="messages"> messages</param>
        public void UpdateUsedDaysInApproval(smt_holiday_request vacationRequestPreImage, List<Resx> messages)
        {

            // Método retorna a lista de períodos aquisitivos adquiridos daquele recurso.
            List<smt_purchasing_period> purchasingperiod = ReturnAcquiredPurchasingPeriod(vacationRequestPreImage);

            CrmServiceContext context = new CrmServiceContext(Service);

            // Se a solicitação for do tipo antecipação
            if (vacationRequestPreImage.smt_bt_antecipation == true)
            {


                // Calculando a quantidade de dias solicitados.
                DateTime? dtInicio = vacationRequestPreImage.smt_dt_start;
                DateTime? dtFim = vacationRequestPreImage.smt_dt_end;
                int diffDays = (int)(((DateTime)dtFim - (DateTime)dtInicio).TotalDays + 1);

                if (purchasingperiod.Count == 1)
                {
                    // Não permitir se está solicitando menos dias do que já tem disponível como "adquirido".
                    if (diffDays <= purchasingperiod.FirstOrDefault().smt_int_dayBalance_calc)
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP02));
                    else
                    {
                        // Método que deduz os dias utilizados em período aquisitivo em aquisição
                        DeductDaysInPurchasingPeriodInAcquisition(context, vacationRequestPreImage, messages);
                    }
                }
                else if (purchasingperiod.Count == 2)
                {
                    if (diffDays <= (purchasingperiod.FirstOrDefault().smt_int_dayBalance_calc + purchasingperiod[1].smt_int_dayBalance_calc))
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP02));
                    else
                    {
                        // Método que deduz os dias utilizados em período aquisitivo em aquisição
                        DeductDaysInPurchasingPeriodInAcquisition(context, vacationRequestPreImage, messages);
                    }
                }
                else
                {
                    // Método que deduz os dias utilizados em período aquisitivo em aquisição
                    DeductDaysInPurchasingPeriodInAcquisition(context, vacationRequestPreImage, messages);
                }
            }
            else
            {
                if (purchasingperiod.Count == 0)
                {
                    if (vacationRequestPreImage.smt_smt_pl_typeEnum == smt_holiday_request_smt_smt_pl_type.Coletivas) { }

                    else
                    {
                        // Se não houverem períodos aquisitivos, não é possível solicitar férias
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP01));
                    }
                }
                else if (purchasingperiod.Count == 1)
                {
                    smt_purchasing_period purchasingPeriod = purchasingperiod.FirstOrDefault();
                    if (vacationRequestPreImage.smt_smt_pl_typeEnum == smt_holiday_request_smt_smt_pl_type.Coletivas)
                    {
                        PPonHolidayClosure(purchasingPeriod, vacationRequestPreImage, messages);
                    }
                    else
                    {
                        // Método que atualiza os dias utilizados em um período aquisitivo apenas.
                        DeductDaysFromOnePurchasingPeriod(purchasingPeriod, vacationRequestPreImage, messages);
                    }
                }
                else
                {
                    // Método que atualiza os dias utilizados com mais de um período aquisitivo.
                    DeductDaysFromTwoPurchasingPeriod(purchasingperiod, vacationRequestPreImage, context, messages);
                }
            }
        }

        /// <summary>
        /// Método para atualizar dias utilizados em períodos aquisitivos, caso a solicitação de férias esteja sendo cancelada.
        /// </summary>
        /// <param name="vacationRequestPreImage"> PreImage HolidayRequest</param>
        /// <param name="messages"> messages</param>
        public void UpdateUsedDaysInCancel(smt_holiday_request vacationRequestPreImage, List<Resx> messages)
        {

            // Método retorna a lista de períodos aquisitivos adquiridos daquele recurso.
            var purchasingperiod = this.ReturnAcquiredPurchasingPeriod(vacationRequestPreImage);
            CrmServiceContext context = new CrmServiceContext(Service);

            // Se não tem períodos adquiridos, verificar se é antecipação de férias.
            if (vacationRequestPreImage.GetAttributeValue<bool>("smt_bt_antecipation") == true)
            {
                // Método que atualiza os dias utilizados quando é antecipação.
                AddDaysInPurchasingPeriodAcquisition(context, vacationRequestPreImage);
            }
            else if (purchasingperiod.Count == 0)
            {

            }
            else if (purchasingperiod.Count == 1)
            {
                // Método para atualizar dias utilizados caso não seja antecipação, e haja apenas um período como "adquirido"
                AddDaystoOnePurchasingPeriodInCancel(purchasingperiod, vacationRequestPreImage);
            }
            else
            {
                // Método para atualizar dias utilizados caso não seja antecipação, e haja mais de um período como "adquirido"
                AddDaystoTwoPurchasingPeriodInCancel(purchasingperiod, vacationRequestPreImage);
            }

        }

        /// <summary>
        /// Este método tem por função retornar os periodos aquisitivos de um determinado usuário. 
        /// </summary>
        /// <param name="vacationRequestPreImage">Pre Image da entidade de solicitação de férias</param>
        /// <returns>a</returns>
        public List<smt_purchasing_period> ReturnAcquiredPurchasingPeriod(smt_holiday_request vacationRequestPreImage)
        {
            List<smt_purchasing_period> purchasingPeriod = new List<smt_purchasing_period>();

            using (var context = new CrmServiceContext(Service))
            {
                purchasingPeriod = (from x in context.CreateQuery<smt_purchasing_period>()
                                    where x.smt_lp_resource.Id == vacationRequestPreImage.smt_lp_resource.Id
                                    && x.StatusCode.Value == 100000000
                                    orderby x.smt_dt_start ascending
                                    select x).ToList();
            }

            return purchasingPeriod;
        }

        /// <summary>
        /// Deduz os dias do periodo aquisitivo onde o usuário só tenha um periodo aquisitivo. 
        /// </summary>
        /// <param name="purchasingPeriod">Periodo aquisitivo</param>
        /// <param name="vacationRequestPreImage">Pre Image da solicitação de férias.</param>
        /// <param name="messages"> messages </param>
        public void DeductDaysFromOnePurchasingPeriod(smt_purchasing_period purchasingPeriod, smt_holiday_request vacationRequestPreImage, List<Resx> messages)
        {
            if (vacationRequestPreImage.smt_int_days_quantity > purchasingPeriod.smt_int_dayBalance_calc)
            {
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP04));
            }
            else
            {
                // Atualizando dias utilizados em período aquisitivo somando a quantidade de dias da solicitação de férias.
                smt_purchasing_period purchasing = new smt_purchasing_period();
                purchasing.Id = purchasingPeriod.Id;
                int? days = purchasingPeriod.smt_int_useddays_period;
                purchasing.smt_int_useddays_period = days + vacationRequestPreImage.smt_int_days_quantity;
                ServiceAdmin.Update(purchasing);
            }
        }

        /// <summary>
        /// Teste. 
        /// </summary>
        /// <param name="purchasingPeriod">Doc.</param>
        /// <param name="vacationRequestPreImage">Doc.</param>
        /// <param name="messages">Doc.</param>
        public void PPonHolidayClosure(smt_purchasing_period purchasingPeriod, smt_holiday_request vacationRequestPreImage, List<Resx> messages)
        {
            // Atualizando dias utilizados em período aquisitivo somando a quantidade de dias da solicitação de férias.
            smt_purchasing_period purchasing = new smt_purchasing_period();
            purchasing.Id = purchasingPeriod.Id;
            int? days = purchasingPeriod.smt_int_useddays_period;
            purchasing.smt_int_useddays_period = days + vacationRequestPreImage.smt_int_days_quantity;
            ServiceAdmin.Update(purchasing);
        }

        /// <summary>
        /// Deduz caso o usuário possua dois periodos aquisitivos. 
        /// </summary>
        /// <param name="purchasingPeriod">Periodo Aquisitivo</param>
        /// <param name="vacationRequestPreImage">Pre Image da solicitação de férias</param>
        /// <param name="context">context </param>
        /// <param name="messages"> messages</param>
        public void DeductDaysFromTwoPurchasingPeriod(List<smt_purchasing_period> purchasingPeriod, smt_holiday_request vacationRequestPreImage, CrmServiceContext context, List<Resx> messages)
        {
            // Quantidade de dias solicitados
            var requestedDays = vacationRequestPreImage.smt_int_days_quantity;
            smt_purchasing_period leastestPurchasingPeriod = new smt_purchasing_period();
            leastestPurchasingPeriod.Id = purchasingPeriod.FirstOrDefault().Id;

            // Dias disponíveis no período aquisitivo mais antigo
            var quantityDaysInPurchasing1 = purchasingPeriod.FirstOrDefault().smt_int_dayBalance_calc.Value;

            // Dias disponíveis no período aquisitivo mais novo
            var quantityDaysInPurchasing2 = purchasingPeriod[1].smt_int_dayBalance_calc.Value;

            // Dias adquiridos do período aquisitivo
            int? daysAcquired = purchasingPeriod.FirstOrDefault().smt_int_quantity;

            leastestPurchasingPeriod.smt_int_useddays_period = purchasingPeriod.FirstOrDefault().smt_int_useddays_period + vacationRequestPreImage.smt_int_days_quantity;

            if (vacationRequestPreImage.smt_int_days_quantity > (quantityDaysInPurchasing1 + quantityDaysInPurchasing2))
            {
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP04));
            }
            else if (leastestPurchasingPeriod.smt_int_useddays_period > daysAcquired)
            {
                // Se os dias utilizados superarem os dias adquiridos, guardar esse valor para atualizar o outro período, e atualizar esse com a quantidade de dias adquiridos.
                requestedDays = leastestPurchasingPeriod.smt_int_useddays_period - daysAcquired;
                leastestPurchasingPeriod.smt_int_useddays_period = daysAcquired;
                ServiceAdmin.Update(leastestPurchasingPeriod);

                // Atualizar o outro período aquisitivo somando o restante dos dias que excederam os 30.
                var purchasing = new smt_purchasing_period();
                purchasing.Id = purchasingPeriod[1].Id;
                purchasing.smt_int_useddays_period = purchasingPeriod[1].smt_int_useddays_period + requestedDays;
                ServiceAdmin.Update(purchasing);
            }
            else
            {
                ServiceAdmin.Update(leastestPurchasingPeriod);
            }

        }

        /// <summary>
        /// Valida a licença do Smart PSA. 
        /// </summary>
        public void ValidateLicense()
        {
            WhoAmIRequest request = new WhoAmIRequest();
            WhoAmIResponse response = (WhoAmIResponse)Service.Execute(request);
            Guid OrgId = response.OrganizationId;

            var ActionRequest = new OrganizationRequest("smt_ActionValidateLicense")
            {
                ["OrgID"] = OrgId.ToString(),
                ["ProductID"] = "EXPSA",
            };
            Service.Execute(ActionRequest);
        }

        /// <summary>
        /// Adiciona dias no periodo aquisitivo ao cancelar a solicitação de férias.
        /// </summary>
        /// <param name="purchasingPeriod">Periodo aquisitivo</param>
        /// <param name="vacationRequestPreImage">Pre Image da solicitação de férias.</param>
        public void AddDaystoOnePurchasingPeriodInCancel(List<smt_purchasing_period> purchasingPeriod, smt_holiday_request vacationRequestPreImage)
        {
            smt_purchasing_period purchasingCurrent = new smt_purchasing_period();
            purchasingCurrent.Id = purchasingPeriod.FirstOrDefault().Id;
            int? quantDays = purchasingPeriod.FirstOrDefault().smt_int_useddays_period.Value - vacationRequestPreImage.smt_int_days_quantity.Value;

            purchasingCurrent.smt_int_useddays_period = quantDays;

            ServiceAdmin.Update(purchasingCurrent);
        }

        /// <summary>
        /// Adiciona dias nos periodos aquisitivos, 2 neste caso, ao cancelar a solicitação de férias.
        /// </summary>
        /// <param name="purchasingPeriod">Periodo Aquisito</param>
        /// <param name="vacationRequestPreImage">Pre Image da solicitação de férias.</param>
        public void AddDaystoTwoPurchasingPeriodInCancel(List<smt_purchasing_period> purchasingPeriod, smt_holiday_request vacationRequestPreImage)
        {
            CrmServiceContext context = new CrmServiceContext(Service);

            smt_purchasing_period purchasingCurrent = new smt_purchasing_period();
            purchasingCurrent.Id = purchasingPeriod[1].Id;
            int? days = vacationRequestPreImage.smt_int_days_quantity.Value;

            purchasingCurrent.smt_int_useddays_period = purchasingPeriod[1].smt_int_useddays_period - days;
            if (purchasingCurrent.smt_int_useddays_period < 0)
            {
                // Se os dias utilizados forem menos que 0, atualizar eles como 0 e guardar o valor "days" para atualizar o outro período aquisitivo.
                days = purchasingPeriod[1].smt_int_useddays_period - days;
                purchasingCurrent.smt_int_useddays_period = 0;
                ServiceAdmin.Update(purchasingCurrent);

                // Atualiza o outro período aquisitivo deduzindo dos dias que restaram do primeiro.
                smt_purchasing_period purchasingPeriodCancel2 = new smt_purchasing_period();
                purchasingPeriodCancel2.Id = purchasingPeriod.FirstOrDefault().Id;
                purchasingPeriodCancel2.smt_int_useddays_period = purchasingPeriod.FirstOrDefault().smt_int_useddays_period.Value + days;
                ServiceAdmin.Update(purchasingPeriodCancel2);
            }
            else
                ServiceAdmin.Update(purchasingCurrent);

        }
        /// <summary>
        /// Atualiza os dias utilizados no período aquisitivo quando as férias forem do tipo antecipação.
        /// </summary>
        /// <param name="context">contexto </param>
        /// <param name="vacationRequestPreImage"> Image de Solicitação de Férias </param>
        /// <param name="messages"> messages</param>
        public void DeductDaysInPurchasingPeriodInAcquisition(CrmServiceContext context, smt_holiday_request vacationRequestPreImage, List<Resx> messages)
        {

            // Método que retorna períodos em Aquisição.
            smt_purchasing_period purchasingPeriodInAcquisition = ReturnPurchasingPeriodInAcquisition(context, vacationRequestPreImage);
            BookableResource resource = ServiceAdmin.Retrieve(BookableResource.EntityLogicalName, vacationRequestPreImage.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract)).ToEntity<BookableResource>();
            smt_model_contract model = RetrieveContractModel(resource);

            if (purchasingPeriodInAcquisition == null)
            {
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP03));
            }
            else if (purchasingPeriodInAcquisition.smt_dc_quantity_days != null)
            {
                if (vacationRequestPreImage.smt_int_days_quantity.Value > (model.smt_int_total_vacation.Value + purchasingPeriodInAcquisition.smt_int_useddays_period.Value))
                {
                    // Se a quantidade de dias solicitados for maior do que o saldo do que a contagem dos dias do período em aquisição, não poderá criar o registro.
                    throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP02));
                }
                else
                {
                    // Atualizando os dias utilizados do Período aquisitivo.
                    var purchasing = new smt_purchasing_period();
                    purchasing.Id = purchasingPeriodInAcquisition.Id;
                    purchasing.smt_int_useddays_period = purchasingPeriodInAcquisition.smt_int_useddays_period.Value + vacationRequestPreImage.smt_int_days_quantity.Value;
                    ServiceAdmin.Update(purchasing);

                }
            }
            else
                throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP03));
        }

        /// <summary>
        /// Buscar e retornar período aquisitivo "em aquisição" de um recurso
        /// </summary>
        /// <param name="context"> context</param>
        /// <param name="vacationRequestPreImage"> solicitação de férias Image </param>
        /// <returns> período aquisitivo </returns>
        public smt_purchasing_period ReturnPurchasingPeriodInAcquisition(CrmServiceContext context, smt_holiday_request vacationRequestPreImage)
        {

            var purchasingPeriodInAcquisition = (from period in context.CreateQuery<smt_purchasing_period>()
                                                 where period.smt_lp_resource.Id == vacationRequestPreImage.smt_lp_resource.Id
                                                 && period.StatusCode.Value == 1
                                                 orderby period.smt_dt_start ascending
                                                 select period).FirstOrDefault();

            return purchasingPeriodInAcquisition;
        }

        /// <summary>
        /// Atualizar dias utilizados do período Aquisitivo em Aquisição no cancelamento.
        /// </summary>
        /// <param name="context"> context</param>
        /// <param name="vacationRequestPreImage"> solicitação de férias img</param>
        public void AddDaysInPurchasingPeriodAcquisition(CrmServiceContext context, smt_holiday_request vacationRequestPreImage)
        {
            smt_purchasing_period purchasingPeriod = ReturnPurchasingPeriodInAcquisition(context, vacationRequestPreImage);

            smt_purchasing_period purchasingPeriodAcquisition = new smt_purchasing_period();
            purchasingPeriodAcquisition.Id = purchasingPeriod.Id;

            purchasingPeriodAcquisition.smt_int_useddays_period = purchasingPeriod.smt_int_useddays_period - vacationRequestPreImage.smt_int_days_quantity;
            ServiceAdmin.Update(purchasingPeriodAcquisition);
        }

        /// <summary>
        /// Verifica se o usuário está solicitando uma antecipação de férias quando já tem uma quantidade de dias suficiente adquiridos.
        /// </summary>
        /// <param name="vacationRequestImg"> Solicitação de Férias - Image </param>
        /// <param name="messages"> mensagens resx </param>
        /// <param name="vacationRequest"> Solicitação de Férias </param>
        public void VerifyIfIsAntecipation(smt_holiday_request vacationRequestImg, List<Resx> messages, smt_holiday_request vacationRequest)
        {
            if (vacationRequestImg.StatusCode.Value == 1)
            {
                // vacationRequest.
                CrmServiceContext context = new CrmServiceContext(Service);
                List<smt_purchasing_period> purchasing_Periods = ReturnAcquiredPurchasingPeriod(vacationRequestImg);
                DateTime? dtInicio = null;
                DateTime? dtFim = null;
                bool? antecipation = null;

                // Verificando se antecipação foi alterado.
                if (vacationRequest.smt_bt_antecipation is null)
                    antecipation = vacationRequestImg.smt_bt_antecipation;
                else
                    antecipation = vacationRequest.smt_bt_antecipation;

                // Verificando se data de ínicio foi alterada.
                if (vacationRequest.smt_dt_start != null)
                    dtInicio = vacationRequest.smt_dt_start;
                else
                    dtInicio = vacationRequestImg.smt_dt_start;

                // Verificando se data de término foi alterada.
                if (vacationRequest.smt_dt_end != null)
                    dtFim = vacationRequest.smt_dt_end;
                else
                    dtFim = vacationRequestImg.smt_dt_end;

                // Cálculo da quantidade de dias solicitado.
                int diffDays = (int)(((DateTime)dtFim - (DateTime)dtInicio).TotalDays + 1);

                // Se for antecipação
                if (antecipation == true)
                {
                    smt_purchasing_period purchasingAcquisition = ReturnPurchasingPeriodInAcquisition(context, vacationRequestImg);

                    if (purchasingAcquisition != null)
                    {
                        BookableResource resource = ServiceAdmin.Retrieve(BookableResource.EntityLogicalName, purchasingAcquisition.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract)).ToEntity<BookableResource>();
                        smt_model_contract model = RetrieveContractModel(resource);

                        // Se os dias solicitados for maior do que "contagem dos dias - dias utilizados" no período aquisição, impedir a criação.
                        if (diffDays > (model.smt_int_total_vacation.Value + purchasingAcquisition.smt_int_useddays_period.Value))
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP03));
                    }
                    else
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP03));
                }
                else
                {
                    if (purchasing_Periods.Count == 0)
                    {
                        throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP01));
                    }
                    else if (purchasing_Periods.Count == 1)
                    {
                        // Se a quantidade de dias solicitados for maior do que o saldo disponível na solicitação de férias, impedir o envio.
                        if (diffDays > purchasing_Periods.FirstOrDefault().smt_int_dayBalance_calc)
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP01));
                    }
                    else if (purchasing_Periods.Count > 1)
                    {
                        // Se a quantidade de dias solicitados for maior do que a soma dos saldos dos períodos aquisitivos do recurso, impedir o envio.
                        if (diffDays > (purchasing_Periods.FirstOrDefault().smt_int_dayBalance_calc + purchasing_Periods[1].smt_int_dayBalance_calc))
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.USRPP01));
                    }
                }
            }
        }

        /// <summary>
        /// Método utilizado para validar e criar todos os registros após a aprovação da solicitação de férias.
        /// </summary>
        /// <param name="managerId">Id do gerente que aprovou a solicitação </param>
        /// <param name="factory"> Service para o Impersonate</param>
        /// <param name="vacationPreImg"> pre Image de Solicitação de férias</param>
        public void CreateHolidayEntry(/*Entity entity,*/ Guid managerId, IOrganizationServiceFactory factory, smt_holiday_request vacationPreImg)
        {
            List<DateTime> dates = new List<DateTime>();
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            EntityReference resource = (EntityReference)vacationPreImg.Attributes["smt_lp_resource"];
            IOrganizationService serviceResource = (IOrganizationService)GetService(resource.Id, factory);
            DateTime dtStart = vacationPreImg.GetAttributeValue<DateTime>("smt_dt_start");
            // dtStart = TimeZoneInfo.ConvertTime(dtStart, timeZone);// new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 00, 00, 00);
            DateTime dtEnd =  vacationPreImg.GetAttributeValue<DateTime>("smt_dt_end");
            // dtEnd = TimeZoneInfo.ConvertTime(dtEnd, timeZone);// new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 00, 00, 00);
            BookableResource resourceContract = ServiceAdmin.Retrieve(BookableResource.EntityLogicalName, vacationPreImg.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract)).ToEntity<BookableResource>();
            smt_model_contract modelContract = RetrieveContractModel(resourceContract);
            List<smt_holiday> holidayList = RetrieveHolidays();
            OptionSetValueCollection workdays = modelContract.GetAttributeValue<OptionSetValueCollection>("smt_mc_work_days");
           
            // dtStart = vacationPreImg.GetAttributeValue<DateTime>("smt_dt_inicio2");
            // dtEnd = vacationPreImg.GetAttributeValue<DateTime>("smt_dt_terimino2");

            for (DateTime date = dtStart; date <= dtEnd; date = date.AddDays(1))
            {
                for (int i = 0; i < workdays.Count; i++)
                {
                    // Verifica se o dia é um dia útil definido pelo modelo de contrato do recurso
                    if ((int)date.DayOfWeek == workdays[i].Value)
                    {
                        // Verifica se o dia não é feriado
                        smt_holiday dt = holidayList.Where(x => x.smt_dt_holiday_date == date).FirstOrDefault();

                        if (dt == null)
                        {
                            dates.Add(TimeZoneInfo.ConvertTime(date, timeZone));
                        }
                    }
                }
            }

            List<Guid> guids = CreateHolidayRecord(dates, /*entity,*/ resource.Id, managerId, serviceResource, vacationPreImg);
            SendEntry(guids, serviceResource, vacationPreImg, managerId);
            IOrganizationService serviceManager = (IOrganizationService)GetService(managerId, factory);
            SendToApproval(resource.Id, serviceManager, vacationPreImg);
        }

        /// <summary>
        /// Retorna os feriados contidos na entidade Feriados Smart. 
        /// </summary>
        /// <returns>Lista de feriados cadastrados.</returns>
        public List<smt_holiday> RetrieveHolidays()
        {
            try
            {
                using (CrmServiceContext smtContext = new CrmServiceContext(Service))
                {
                    return smtContext.smtholiday.Where(day => day.Id != null).ToList();
                }
            }
            catch (ArgumentNullException argumentNullException)
            {
                throw argumentNullException;
            }
            catch (InvalidPluginExecutionException invalidPluginExecutionException)
            {
                throw invalidPluginExecutionException;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        /// <summary>
        /// Método utilizado para criar um Service Impersonate.
        /// </summary>
        /// <param name="userId">Id do usuário que irá ser utilizado para criar o impersonate </param>
        /// <param name="factory"> IOrganizationServiceFactory para realizar o Impersonate</param>
        /// <returns>Retorno é um novo Iorganization Service</returns>
        public IOrganizationService GetService(Guid userId, IOrganizationServiceFactory factory)
        {
            return (IOrganizationService)factory.CreateOrganizationService(userId);
        }

        /// <summary>
        /// Método utilizado para buscar as horas de trabalho configurada para o recurso
        /// </summary>
        /// <param name="resourceContract">ID do usuário que solicitou as férias</param>
        /// <returns>Retorno é uma variavel inteira, com o numero de horas configuradas convertidas para minutos.</returns>
        public smt_model_contract RetrieveContractModel(BookableResource resourceContract)
        {
            smt_model_contract contract = ServiceAdmin.Retrieve(smt_model_contract.EntityLogicalName, resourceContract.smt_lp_model_contract.Id, new ColumnSet(smt_model_contract.Fields.smt_mc_work_days, smt_model_contract.Fields.smt_dc_time_load, smt_model_contract.Fields.smt_int_total_vacation)).ToEntity<smt_model_contract>();

            return contract;
        }

        /// <summary>
        /// Método utilizado para criar as entradas de horas com base nas datas passadas na solicitação de férias.
        /// </summary>
        /// <param name="dates">Lista com todas as datas das férias solicitadas</param>
        /// <param name="entity"> Contexto com os dados da solicitaçao de férias</param>
        /// <param name="userId"> ID do usuário que solicitou as férias</param>
        /// <param name="gerente">ID do gerente que aprovou as férias</param>
        /// <param name="serviceResource">Service Impersonate</param>
        /// <param name="vacationPreImg">PreImage de Solicitação de Férias</param>
        /// <returns>Retorna uma lista com os ID's dos registros criados</returns>
        public List<Guid> CreateHolidayRecord(List<DateTime> dates,/* Entity entity,*/ Guid userId, Guid gerente, IOrganizationService serviceResource, smt_holiday_request vacationPreImg)
        {
            try
            {
                List<Guid> entryCreated = new List<Guid>();
                BookableResource resourceContract = ServiceAdmin.Retrieve(BookableResource.EntityLogicalName, vacationPreImg.smt_lp_resource.Id, new ColumnSet(BookableResource.Fields.smt_lp_model_contract, BookableResource.Fields.UserId)).ToEntity<BookableResource>();
                smt_model_contract contractModel = RetrieveContractModel(resourceContract);
                int hours = Decimal.ToInt32(contractModel.smt_dc_time_load.Value * 60);
                if (hours == 0)
                {
                    OptionSetValue eventType = new OptionSetValue(100000000);
                    String recordid = vacationPreImg.Id.ToString();
                    String message = "Recurso não possui horas de trabalho cadastradas.";
                    String name = "Erro ao buscar horas de trabalho em Modelo de Contrato";
                    CreateLogs(eventType, recordid, message, name, userId);

                    throw new InvalidPluginExecutionException("Recurso não possui horas de trabalho cadastradas.");

                }
                for (int i = 0; i < dates.Count; i++)
                {
                    msdyn_timeentry timeentry = new msdyn_timeentry();
                    timeentry.msdyn_duration = hours;
                    timeentry.msdyn_date = dates[i];
                    timeentry.msdyn_manager = new EntityReference("systemuser", gerente);
                    timeentry["msdyn_description"] = "Férias solicitadas";
                    timeentry.smt_lp_holiday_request = new EntityReference("smt_holiday_request", vacationPreImg.Id);
                    timeentry.msdyn_type = new OptionSetValue(192350002);
                    // timeentry.CreatedBy = new EntityReference(SystemUser.EntityLogicalName, resourceContract.UserId.Id);
                    timeentry.msdyn_bookableresource = new EntityReference(BookableResource.EntityLogicalName, userId);
                    timeentry.OwnerId = new EntityReference(SystemUser.EntityLogicalName, resourceContract.UserId.Id);
                    // throw new InvalidPluginExecutionException("Início entrada de Hora -> " + timeentry.msdyn_date);
                    // entryCreated.Add(serviceResource.Create(timeentry));
                    // entryCreated.Add(ServiceAdmin.Create(timeentry));
                    entryCreated.Add(Service.Create(timeentry));                    
                }
                return entryCreated;
            }
            catch (Exception ex)
            {                
                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = vacationPreImg.Id.ToString();
                String message = "Erro ao criar registro de entrada de horas para a solicitação de férias." + ex.Message;
                String name = "Erro ao criar registro de entrada de horas.";
                CreateLogs(eventType, recordid, message, name, userId);

                throw new InvalidPluginExecutionException("Erro ao criar registro de entrada de horas. \nLOG: " + ex.Message);


            }
        }

        /// <summary>
        /// Criar Logs
        /// </summary>
        /// <param name="eventType"> tipo de Evento </param>
        /// <param name="recordid">id do registro </param>
        /// <param name="message"> mensagem do Throw </param>
        /// <param name="name"> nome </param>
        /// <param name="userId"> id do Usuário </param>
        public void CreateLogs(OptionSetValue eventType, String recordid, String message, String name, Guid userId)
        {
            smt_log log = new smt_log();
            log.smt_name = name;
            log.smt_st_entityname = msdyn_timeentry.EntityLogicalName;
            log.smt_pl_eventtype = eventType;
            log.smt_st_eventorigin = "s";
            log.smt_dt_eventdate = DateTime.Now;
            log.smt_lp_executinguser = new EntityReference("systemuser", userId);
            log.smt_st_recordname = name;
            log.smt_st_recordid = recordid;
            log.smt_tx_message = message;
            Service.Create(log);
        }

        /// <summary>
        /// Método utilizado para enviar as entradas de horas criadas.
        /// </summary>
        /// <param name="entries">Lista com todas as entradas de horas.</param>
        /// <param name="serviceResource">Service Impersonate</param>
        /// <param name="vacationPreImg"> Img de Solicitação de Férias</param>
        /// <param name="userId"> id do Usuário</param>
        public void SendEntry(List<Guid> entries, IOrganizationService serviceResource, smt_holiday_request vacationPreImg, Guid userId)
        {
            try
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    OrganizationRequest orgReq = new OrganizationRequest("msdyn_TimeEntriesSubmit");
                    orgReq["CorrelationId"] = entries[i].ToString();
                    orgReq["TimeEntryIds"] = entries[i].ToString();
                    Service.Execute(orgReq);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Erro ao enviar entrada de horas para aprovação. \nLOG: " + ex.Message);

                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = vacationPreImg.Id.ToString();
                String message = "Erro ao criar registro de entrada de horas." + ex.Message;
                String name = "Erro ao criar registro de entrada de horas.";
                CreateLogs(eventType, recordid, message, name, userId);
            }
        }

        /// <summary>
        /// Método utilizado para aprovar as entradas enviadas anteriomente pelo metodo SendEntry
        /// </summary>
        /// <param name="resourceId">Id do recurso que enviou as entradas de horas.</param>
        /// <param name="serviceManager">Service com o Gerente do recurso para efetuar o request de aprovação das horas. </param>
        /// <param name="vacationPreImg"> Image de Solicitação de Férias</param>
        public void SendToApproval(Guid resourceId, IOrganizationService serviceManager, smt_holiday_request vacationPreImg)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='msdyn_projectapproval'>
                                    <attribute name='msdyn_projectapprovalid' />
                                    <attribute name='msdyn_name' />
                                    <attribute name='createdon' />
                                    <order attribute='msdyn_name' descending='false' />
                                    <filter type='and'>
                                      <condition attribute='createdon' operator='today' />
                                      <condition attribute='msdyn_bookableresource' operator='eq' uitype='bookableresource' value='" + resourceId + @"' />
                                      <condition attribute='msdyn_recordstage' operator='eq' value='0' />
                                    </filter>
                                    <link-entity name='bookableresource' from='bookableresourceid' to='msdyn_bookableresource' visible='false' link-type='outer' alias='als'>
                                      <attribute name='userid' />
                                    </link-entity>
                                    <link-entity name='msdyn_timeentry' from='msdyn_timeentryid' to='msdyn_timeentry' link-type='inner' alias='al'>
                                      <attribute name='msdyn_date' />
                                      <attribute name='msdyn_timeentryid' />
                                    </link-entity>
                                  </entity>
                                </fetch>";

            EntityCollection returnEntity = Service.RetrieveMultiple(new FetchExpression(query));

            try
            {
                foreach (var item in returnEntity.Entities)
                {
                    var entryId = ((AliasedValue)item.Attributes["al.msdyn_timeentryid"]).Value;
                    OrganizationRequest orgReq = new OrganizationRequest("msdyn_TimeEntriesApprove");
                    orgReq["Target"] = new EntityReference("msdyn_projectapproval", item.Id);
                    orgReq["CorrelationId"] = entryId.ToString();
                    orgReq["TimeEntryIds"] = entryId.ToString();
                    serviceManager.Execute(orgReq);
                }
            }
            catch (Exception ex)
            {
                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = vacationPreImg.Id.ToString();
                String message = "Erro ao criar registro de entrada de horas." + ex.Message;
                String name = "Erro ao criar registro de entrada de horas.";
                BookableResource resource = serviceManager.Retrieve(BookableResource.EntityLogicalName, resourceId, new ColumnSet(BookableResource.Fields.UserId)).ToEntity<BookableResource>();
                CreateLogs(eventType, recordid, message, name, resource.UserId.Id);

                throw new InvalidPluginExecutionException("Erro ao aprovar entrada de hora. \nLOG: " + ex.Message);
            }
        }

        /// <summary>
        /// Cancela a requisição de férias ja aprovada (se houver) e rejeita as entradas de horas relacionadas a ela e deleta seus respectivos registros
        /// </summary>
        /// <param name="entity">Registro relacionado ao cancelamento</param>
        /// <param name="vacationPreImg"> Img solicitação de férias </param>
        /// <param name="userId"> id do Usuário </param>
        /// <param name="factory"> factory </param>
        public void CancelHolidayEntry(/*Entity entity,*/ smt_holiday_request vacationPreImg, Guid userId, IOrganizationServiceFactory factory)
        {
            IOrganizationService serviceManager = (IOrganizationService)GetService(userId, factory);

            EntityCollection projectapproval = EntryRecords(vacationPreImg.Id);

            if (projectapproval != null && projectapproval.Entities.Count > 0)
            {
                bool response = CancelApproval(projectapproval);
                if (response)
                {
                    TimeEntriesReject(projectapproval, vacationPreImg, userId, serviceManager);

                    foreach (var item in projectapproval.Entities)
                    {
                        ServiceAdmin.Delete("msdyn_projectapproval", item.Id);
                        var entryId = ((AliasedValue)item.Attributes["ac.msdyn_timeentryid"]).Value;
                        ServiceAdmin.Delete("msdyn_timeentry", new Guid(entryId.ToString()));
                    }
                }
            }
            else
            {
                List<msdyn_timeentry> Senttimeentries = SentEntryRecords(vacationPreImg.Id);

                if (Senttimeentries != null && Senttimeentries.Count > 0)
                {
                    RecoverEntryRecords(Senttimeentries, userId);
                }
            }
        }

        /// <summary>
        /// Retorna todas as entradas de hora da solicitação de férias que estão com status "enviado"
        /// </summary>
        /// <param name="holidayId"> Id da Solicitação de Férias </param>
        /// <returns>Retorna uma EntityCollection com os registros relacionados</returns>
        public List<msdyn_timeentry> SentEntryRecords(Guid holidayId)
        {

            using (CrmServiceContext orgContext = new CrmServiceContext(Service))
            {

                return orgContext.CreateQuery<msdyn_timeentry>().Where(a => a.smt_lp_holiday_request.Id == holidayId).ToList();
            }

        }

        /// <summary>
        /// Recupera as Entradas de horas enviadas.
        /// </summary>
        /// <param name="timeentries"> Entradas de hora enviadas </param>
        /// <param name="userId"> id do usuário </param>
        public void RecoverEntryRecords(List<msdyn_timeentry> timeentries, Guid userId)
        {
            foreach (msdyn_timeentry timeentry in timeentries)
            {
                msdyn_timeentry entry = new msdyn_timeentry();
                entry.Id = timeentry.Id;
                entry.msdyn_entryStatusEnum = msdyn_timeentrystatus.Draft;
                try
                {
                    Service.Update(entry);
                }
                catch (Exception ex)
                {
                    // Criar log
                    OptionSetValue eventType = new OptionSetValue(100000000);
                    String recordid = timeentry.Id.ToString();
                    String message = "Erro ao recuperar registro de entrada de horas." + ex.Message;
                    String name = "Erro ao recuperar registro de entrada de horas.";
                    CreateLogs(eventType, recordid, message, name, userId);
                }
                ExcludeEntryRecords(entry, userId);
            }

        }

        /// <summary>
        /// Exclui as entradas de horas
        /// </summary>
        /// <param name="entry"> Entrada de hora em rascunho </param>
        /// <param name="userId"> Id do Usuário </param>
        public void ExcludeEntryRecords(msdyn_timeentry entry, Guid userId)
        {
            try
            {
                Service.Delete(msdyn_timeentry.EntityLogicalName, entry.Id);
            }
            catch (Exception ex)
            {
                // Criar log
                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = entry.Id.ToString();
                String message = "Erro ao excluir registro de entrada de horas." + ex.Message;
                String name = "Erro ao excluir registro de entrada de horas.";
                CreateLogs(eventType, recordid, message, name, userId);
            }
        }

        /// <summary>
        /// Retorna todos os registros de Aprovação de Projeto relacionados ao ID da Holiday Request passada
        /// </summary>
        /// <param name="holidayRequest">ID do registro de Holiday Request para a pesquisa</param>
        /// <returns>Retorna uma EntityCollection com os registros relacionados</returns>
        public EntityCollection EntryRecords(Guid holidayRequest)
        {
            string query = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='msdyn_projectapproval'>
                                    <attribute name='msdyn_projectapprovalid' />
                                    <attribute name='msdyn_name' />
                                    <attribute name='createdon' />
                                    <order attribute='msdyn_name' descending='false' />
                                    <link-entity name='msdyn_timeentry' from='msdyn_timeentryid' to='msdyn_timeentry' link-type='inner' alias='ac'>
                                        <attribute name='msdyn_timeentryid' />
                                      <filter type='and'>
                                        <condition attribute='smt_lp_holiday_request' operator='eq' uitype='smt_holiday_request' value='" + holidayRequest + @"' />
                                      </filter>
                                    </link-entity>
                                  </entity>
                                </fetch>";

            return (EntityCollection)Service.RetrieveMultiple(new FetchExpression(query));
        }

        /// <summary>
        /// Cancela registros de entrada de horas a serem aprovados ou ja aprovados
        /// </summary>
        /// <param name="entries">Registro relacionado ao cancelamento</param>
        /// <returns>Retorna true se for cancelado com sucesso do contrario false</returns>
        public bool CancelApproval(EntityCollection entries)
        {
            try
            {
                foreach (var item in entries.Entities)
                {
                    OrganizationRequest orgReq = new OrganizationRequest("msdyn_ValidateApprovalCancellation");
                    orgReq["ApprovalIds"] = item.Id.ToString();
                    Service.Execute(orgReq);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            try
            {
                foreach (var item in entries.Entities)
                {
                    OrganizationRequest orgReq = new OrganizationRequest("msdyn_CancelApproval");
                    orgReq["ApprovalIds"] = item.Id.ToString();
                    Service.Execute(orgReq);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Rejeita as entradas de horas relacionadas aos registros passados
        /// </summary>
        /// <param name="entries">Registro relacionado a rejeição</param>
        /// <param name="vacationPreImg"> Image da entrada de férias </param>
        /// <param name="userId"> id do usuário </param>
        /// <param name="serviceManager"> Impersonate do Gerente </param>
        public void TimeEntriesReject(EntityCollection entries, smt_holiday_request vacationPreImg, Guid userId, IOrganizationService serviceManager)
        {
            try
            {
                foreach (var item in entries.Entities)
                {
                    var entryId = ((AliasedValue)item.Attributes["ac.msdyn_timeentryid"]).Value;
                    OrganizationRequest orgReq = new OrganizationRequest("msdyn_TimeEntriesReject");
                    orgReq["CorrelationId"] = item.Id.ToString();
                    orgReq["TimeEntryIds"] = entryId.ToString();
                    serviceManager.Execute(orgReq);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Erro ao aprovar entrada de hora. \nLOG: " + ex.Message);

                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = vacationPreImg.Id.ToString();
                String message = "Erro ao criar registro de entrada de horas." + ex.Message;
                String name = "Erro ao criar registro de entrada de horas.";
                CreateLogs(eventType, recordid, message, name, userId);
            }

        }

        /// <summary>
        /// Método que retorna se o usuário não precisa lançar as horas do tipo Férias.
        /// </summary>
        /// <param name="idResource">id do Recurso</param>
        /// <returns>Parâmetro com aquele recurso que não necessita de entrada de horas.</returns>
        public smt_smartparameter WithoutTimeEntry(Guid idResource)
        {
            CrmServiceContext localContext = new CrmServiceContext(ServiceAdmin);

            EntityReference orgResource = OrgResource(idResource);

            smt_smartparameter parameterSmart = (from parametro in localContext.CreateQuery<smt_smartparameter>()
                                                 where parametro.smt_name == "ORGANIZATIONAL UNIT WITHOUT TIME ENTRY" &&
                                                 parametro.smt_organizationalunit == orgResource &&
                                                 parametro.smt_value == "True"
                                                 select parametro).FirstOrDefault();

            if (parameterSmart != null)
                return parameterSmart;
            else
                return null;

        }

        /// <summary>
        /// Buscar Unidade Organizacional do Recurso Reservável
        /// </summary>
        /// <param name="idResource">id do Recurso </param>
        /// <returns> unidade Organizacional do Recurso</returns>
        public EntityReference OrgResource(Guid idResource)
        {
            BookableResource bookable = Service.Retrieve(BookableResource.EntityLogicalName, idResource, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit)).ToEntity<BookableResource>();

            EntityReference orgResource = bookable.msdyn_organizationalunit;

            return orgResource;
        }

        /// <summary>
        /// "GetParameter"
        /// </summary>
        /// <param name="idUnitOrg">"Id da unidade organizacional"</param>
        /// <param name="nameParameter">"Nome do Parametro"</param>
        /// <param name="global">"global"</param>
        /// <returns>"smt_smartparameter"</returns>
        public smt_smartparameter GetParameter(BookableResource idUnitOrg, String nameParameter = "", bool global = false)
        {
            using (var context = new CrmServiceContext(ServiceAdmin))
            {
                if (!global)
                {
                    return (from parametros in context.CreateQuery<smt_smartparameter>()
                            where parametros.smt_organizationalunit.Id == idUnitOrg.msdyn_organizationalunit.Id &&
                            parametros.smt_name == nameParameter
                            select parametros).FirstOrDefault();
                }
                else
                {
                    return (from parametros in context.CreateQuery<smt_smartparameter>()
                            where parametros.smt_name == nameParameter &&
                            parametros.smt_organizationalunit == null
                            select parametros).FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Esse metodo retorna os campos que serão atualizados conforme a entidade de contexto
        /// </summary>    
        /// <param name="entity">_</param>
        /// <returns>"retorno"</returns>
        public List<String> GetFields(String entity)
        {
            List<String> retorno = new List<String>();

            retorno.Add("smt_dc_days_purchase");
            retorno.Add("smt_dc_useddays");
            return retorno;
        }

        /// <summary>
        /// Força a atualização em tempo real de um campo calculado 
        /// </summary>
        /// <param name="entity">_</param>
        /// <param name="field">_</param>
        public void CalculateRollup(EntityReference entity, String field)
        {
            CalculateRollupFieldRequest request = new CalculateRollupFieldRequest
            {

                Target = entity,
                FieldName = field

            };
            Service.Execute(request);
        }

    }
}