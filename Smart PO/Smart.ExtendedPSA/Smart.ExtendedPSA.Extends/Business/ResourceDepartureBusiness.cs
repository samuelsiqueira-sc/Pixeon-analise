using CRM.Smart.ExtendedPSA.Extends.Business;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel;
using System.Threading.Tasks;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Smart.ExtendedPSA.Extends.Earlybound;

namespace Smart.ExtendedPSA.Extends.Business
{
    /// <summary>
    /// Business de Afastamento do Recurso.
    /// </summary>
    public class ResourceDepartureBusiness : BaseBusiness
    {
        /// <summary>
        /// .
        /// </summary>
        /// <param name="service">Serviço da Organização.</param>
        /// <param name="serviceAdmin">Serviço Administrador da Organização.</param>
        /// <param name="tracingService">Rota de Serviço da Organização.</param>
        public ResourceDepartureBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService) : base(service, serviceAdmin, tracingService) { }

        /// <summary>
        /// Método para encontrar registro do recurso relacionado ao afastamento.
        /// </summary>
        /// <param name="resourceId">Id do recurso.</param>
        /// <returns>retorna recurso relacionado.</returns>
        public BookableResource SearchResource(Guid resourceId)
        {
            BookableResource resource = new BookableResource();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                resource = (from register in serviceContext.CreateQuery<BookableResource>()
                            where register.Id == resourceId
                            select register).FirstOrDefault();
            }
            return resource;
        }

        /// <summary>
        /// Método para encontrar registro do gerente do recurso relacionado ao afastamento.
        /// </summary>
        /// <param name="userId">Id do recurso.</param>
        /// <returns>usuario do gerente.</returns>
        public SystemUser ManagerId(Guid userId)
        {
            SystemUser manager = new SystemUser();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                // Busca pelo recurso atual que possua um gerente.
                SystemUser currentUser = (from newUser in serviceContext.CreateQuery<SystemUser>()
                                          where newUser.Id == userId
                                          && newUser.ParentSystemUserId.Id != null
                                          select newUser).FirstOrDefault();

                // Se recurso encontrado, então buscar pelo gerente.
                if (currentUser == null)
                {
                    currentUser = (from newUser in serviceContext.CreateQuery<SystemUser>()
                                   where newUser.Id == userId
                                   select newUser).FirstOrDefault();
                    manager = (from newManager in serviceContext.CreateQuery<SystemUser>()
                              where newManager.Id == currentUser.Id
                              select newManager).FirstOrDefault();
                }
                else if (currentUser != null)
                {
                    manager = (from newManager in serviceContext.CreateQuery<SystemUser>()
                               where newManager.Id == currentUser.ParentSystemUserId.Id
                               select newManager).FirstOrDefault();
                }
                else
                {
                    throw new InvalidPluginExecutionException("Erro ao tentar encontrar o gerente do recurso. \nLOG: Dados incorretos.");
                    OptionSetValue eventType = new OptionSetValue(100000000);
                    String recordid = userId.ToString();
                    String message = "Erro ao tentar encontrar o gerente do recurso.";
                    String name = "Erro ao tentar encontrar o gerente do recurso.";
                    CreateLogs(eventType, Service, recordid, message, name, manager.Id);
                }
            }
            return manager;
        }

        /// <summary>
        /// Busca usuários que não precisam lançar horas.
        /// </summary>
        /// <param name="resourceId">ID do recurso.</param>
        /// <returns>retorna parametro do afastamento.</returns>
        public smt_smartparameter WorkWithoutHours(Guid resourceId)
        {
            EntityReference orgservice = GetOrg(resourceId);
            smt_smartparameter parameter = new smt_smartparameter();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                BookableResource resource = (from item in serviceContext.CreateQuery<BookableResource>()
                                             where item.Id == resourceId
                                             select item).FirstOrDefault();
                SystemUser user = (from userid in serviceContext.CreateQuery<SystemUser>()
                                   where userid.Id == resource.UserId.Id
                                   select userid).FirstOrDefault();
                Team team = GetTeam(user);
                parameter = (from smart in serviceContext.CreateQuery<smt_smartparameter>()
                             where smart.smt_name == "GET TEAM RH FOR HOLIDAY REQUEST"
                             && smart.smt_value == team.Name
                             && smart.smt_organizationalunit == orgservice
                             select smart).FirstOrDefault();

                if (parameter != null)
                    return parameter;
                else
                    return null;
            }
        }

        /// <summary>
        /// Busca unidade da organização do recurso.
        /// </summary>
        /// <param name="resourceId">ID do recurso.</param>
        /// <returns>retorna referencia.</returns>
        public EntityReference GetOrg(Guid resourceId)
        {
            EntityReference entity = new EntityReference();
            using (CrmServiceContext serviceContext = new CrmServiceContext(ServiceAdmin))
            {
                BookableResource bookable = Service.Retrieve(BookableResource.EntityLogicalName, resourceId, new ColumnSet(BookableResource.Fields.msdyn_organizationalunit)).ToEntity<BookableResource>();

                entity = bookable.msdyn_organizationalunit;

            }
            return entity;
        }

        /// <summary>
        /// .
        /// </summary>
        /// <param name="resource">.</param>
        /// <returns>.</returns>
        public Team GetTeam(SystemUser resource)
        {
            Team team = new Team();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                team = (from item in serviceContext.CreateQuery<Team>()
                        where item.BusinessUnitId == resource.BusinessUnitId
                        select item).FirstOrDefault();
            }
            return team;
        }

        /// <summary>
        /// Método para encontrar modelo de contrato do recurso relacionado ao afastamento.
        /// </summary>
        /// <param name="contractId">Id do contrato.</param>
        /// <returns>Registro do modelo de contrato.</returns>
        public smt_model_contract FindContractModel(Guid contractId)
        {
            smt_model_contract contract = new smt_model_contract();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                contract = (from contractModel in serviceContext.CreateQuery<smt_model_contract>()
                            where contractModel.Id == contractId
                            select contractModel).FirstOrDefault();

                if (contract == null)
                {
                    throw new InvalidPluginExecutionException("Erro ao encontrar contrato do registro. Recurso precisa ter um contrato associado.");
                }
            }
            return contract;
        }

        /// <summary>
        /// Método para criar quantidade necessária de entrada de horas relacionadas ao afastamento do recurso.
        /// </summary>
        /// <param name="messages">mensagem de erro resx.</param>
        /// <param name="contract">Contrato do recurso.</param>
        /// <param name="dates">lista com datas de trabalho.</param>
        /// <param name="manager">Gerente do recurso.</param>
        /// <param name="factory">factory do impersonate.</param>
        /// <param name="resource">Recurso do afastamento.</param>
        /// <param name="departure">Afastamento do recurso.</param>
        public void SendEntry(List<Resx> messages, smt_model_contract contract, List<DateTime> dates, SystemUser manager, IOrganizationServiceFactory factory, BookableResource resource, smt_resource_departure_eb departure)
        {
            IOrganizationService serviceManager = (IOrganizationService)GetService(manager.Id, factory);

            // Lista que chama o método que cria e retorna IDs de entradas de horas relacionadas ao afastamento de acordo com os dias selecionados.
            List<Guid> timeentryList = TimeEntryRecord(messages, contract, dates, resource, factory, manager, departure);

            // Se a lista obtiver dados, então chamar outro método.
            if (timeentryList != null && timeentryList.Count > 0)
            {
                // Método que cria as aprovações para as entradas de hora criadas.
                ApproveEntries(resource.Id, serviceManager, departure);
            }
            else
            {
                throw new InvalidPluginExecutionException("Erro ao criar registro de aprovação de horas. \nLOG:");
                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = departure.Id.ToString();
                String message = "Erro ao criar registro de aprovação de horas. " + messages.GetMessageById(ResxExtension.CUTDR111);
                String name = "Erro ao criar registro de aprovação de horas.";
                CreateLogs(eventType, serviceManager, recordid, message, name, manager.Id);
            }
        }

        /// <summary>
        /// Busca aprovações relacionadas e aprova as entradas de horas.
        /// </summary>
        /// <param name="resourceId"> id do recurso </param>
        /// <param name="serviceManager"> impersonate do Gerente </param>
        /// <param name="departure"> afastamento do contexto </param>
        public void ApproveEntries(Guid resourceId, IOrganizationService serviceManager, smt_resource_departure_eb departure)
        {
            List<msdyn_projectapproval> list = new List<msdyn_projectapproval>();
            using (CrmServiceContext crmContext = new CrmServiceContext(serviceManager))
            {
                // Busca por aprovações relacionadas a entrada de horas que é relacionada ao afastamento.
                var approvals = (from approval in crmContext.CreateQuery<msdyn_projectapproval>()
                                 join timeentry in crmContext.CreateQuery<msdyn_timeentry>() on approval.msdyn_TimeEntry.Id equals timeentry.Id
                                 where timeentry.smt_lp_resource_departure.Id == departure.Id
                                 && approval.msdyn_recordstageEnum == msdyn_projectapproval_msdyn_recordstage.Sent
                                 select approval).ToList<msdyn_projectapproval>();

                // Se houverem aprovações criadas e enviadas, atualizar registros de aprovação.
                if (approvals != null && approvals.Count > 0)
                {
                    try
                    {
                        foreach (msdyn_projectapproval approval in approvals)
                        {
                            var entryId = approval.msdyn_TimeEntry.Id;
                            OrganizationRequest orgReq = new OrganizationRequest("msdyn_TimeEntriesApprove");
                            orgReq["Target"] = new EntityReference("msdyn_projectapproval", approval.Id);
                            orgReq["CorrelationId"] = entryId.ToString();
                            orgReq["TimeEntryIds"] = entryId.ToString();
                            serviceManager.Execute(orgReq);
                        }
                    }
                    catch (Exception ex)
                    {
                        OptionSetValue eventType = new OptionSetValue(100000000);
                        String recordid = departure.Id.ToString();
                        String message = "Erro ao aprovar registro de entrada de horas." + ex.Message;
                        String name = "Erro ao aprovar registro de entrada de horas.";
                        BookableResource resource = serviceManager.Retrieve(BookableResource.EntityLogicalName, resourceId, new ColumnSet(BookableResource.Fields.UserId)).ToEntity<BookableResource>();
                        CreateLogs(eventType, serviceManager, recordid, message, name, resource.UserId.Id);

                        throw new InvalidPluginExecutionException("Erro ao aprovar entrada de hora. \nLOG: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Busca por aprovação relacionada a entrada de horas.
        /// </summary>
        /// <param name="entry">ID da entrada de horas.</param>
        /// <returns>retorna registro de aprovação.</returns>
        public List<msdyn_projectapproval> ListaAprovacoes(Guid entry)
        {
            List<msdyn_projectapproval> approves = new List<msdyn_projectapproval>();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                approves = (from projects in serviceContext.CreateQuery<msdyn_projectapproval>()
                            where projects.msdyn_TimeEntry.Id == entry
                            select projects).ToList();
            }
            return approves;
        }

        /// <summary>
        /// Método que verifica se as datas escolhidas para o afastamento são válidas.
        /// </summary>
        /// <param name="target">entidade atual.</param>
        /// <param name="resource">recurso do afastamento.</param>
        /// <param name="messages">mensagens de erro resx.</param>
        public void DepartureDates(smt_resource_departure_eb target, BookableResource resource, List<Resx> messages)
        {
            List<smt_resource_departure_eb> departureList = new List<smt_resource_departure_eb>();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                departureList = (from item in serviceContext.CreateQuery<smt_resource_departure_eb>()
                                 where item.smt_lp_resource.Id == resource.Id
                                 && item.Id != target.Id
                                 select item).ToList();

                foreach (var departure in departureList)
                {
                    for (DateTime time = target.smt_dt_start.Value; time <= target.smt_dt_end.Value; time = time.AddDays(1))
                    {
                        if (time.Date == departure.smt_dt_start || time.Date == departure.smt_dt_end)
                        {
                            throw new InvalidPluginExecutionException(messages.GetMessageById(ResxExtension.EHAE1234));
                            OptionSetValue eventType = new OptionSetValue(100000000);
                            String recordid = departure.Id.ToString();
                            String message = messages.GetMessageById(ResxExtension.EHAE1234);
                            String name = "Erro ao criar registro. Entrada de horas deve possuir duração.";
                            CreateLogs(eventType, ServiceAdmin, recordid, message, name, resource.UserId.Id);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Método para cancelamento do pedido de afastamento realizado. 
        /// </summary>
        /// <param name="departure">Registro de afastamento relacionado.</param>
        /// <param name="serviceManager">Organization Service do gerente.</param>
        public void Canceltarget(smt_resource_departure_eb departure, IOrganizationService serviceManager)
        {

            using (CrmServiceContext crmContext = new CrmServiceContext(serviceManager))
            {
                List<msdyn_timeentry> timeentry = (from timeid in crmContext.CreateQuery<msdyn_timeentry>()
                                                   where timeid.smt_lp_resource_departure.Id == departure.Id
                                                   && timeid.msdyn_bookableresource.Id == departure.smt_lp_resource.Id
                                                   select timeid).ToList();

                // Busca por aprovações de projeto relacionados a entrada de horas cujo status igual a cancelado e afastamento igual ao target.
                foreach (var timeentity in timeentry)
                {
                    var approvals = (from approval in crmContext.CreateQuery<msdyn_projectapproval>()
                                     where approval.msdyn_TimeEntry.Id == timeentity.Id
                                     && approval.msdyn_date == timeentity.msdyn_date
                                     select approval).FirstOrDefault();

                    if (approvals != null)
                    {
                        Guid timeId = approvals.msdyn_TimeEntry.Id;
                        timeentity.msdyn_entryStatusEnum = Earlybound.msdyn_timeentrystatus.RecallRequested;
                        crmContext.UpdateObject(timeentity);
                        approvals.msdyn_recordstageEnum = msdyn_projectapproval_msdyn_recordstage.RequestedRetrieve;
                        crmContext.UpdateObject(approvals);
                        approvals.msdyn_recordstageEnum = msdyn_projectapproval_msdyn_recordstage.RetrievalRequestApproved;
                        crmContext.UpdateObject(approvals);
                        timeentity.msdyn_entryStatusEnum = Earlybound.msdyn_timeentrystatus.Draft;
                        crmContext.UpdateObject(timeentity);
                        crmContext.DeleteObject(timeentity);
                    }
                }
            }
        }

        /// <summary>
        /// Método que retorna o Impersonate.
        /// </summary>
        /// <param name="userId">Id do usuario.</param>
        /// <param name="factory">Factory do Impersonate.</param>
        /// <returns>retorna serviço local.</returns>
        public IOrganizationService GetService(Guid userId, IOrganizationServiceFactory factory)
        {
            return (IOrganizationService)factory.CreateOrganizationService(userId);
        }

        /// <summary>
        /// Método que busca a entrada de horas de acordo com o ID coletado na lista.
        /// </summary>
        /// <param name="entryId">Id da entrada de horas selecionada.</param>
        /// <returns>retorna entrada de horas relacionada.</returns>
        public msdyn_timeentry SearchEntry(Guid entryId)
        {
            msdyn_timeentry time = new msdyn_timeentry();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                time = (from entry in serviceContext.CreateQuery<msdyn_timeentry>()
                        where entry.Id == entryId
                        select entry).FirstOrDefault();
            }
            return time;
        }

        /// <summary>
        /// Método para verificar datas do afastamento.
        /// </summary>
        /// <param name="manager">Gerente do recurso.</param>
        /// <param name="messages">mensagem de erro resx.</param>
        /// <param name="factory">factory da organização do afastamento.</param>
        /// <param name="departure">Afastamento do recurso.</param>
        /// <param name="resource">Recurso do afastamento.</param>
        /// <param name="contract">Contrato do recurso.</param>
        public void CreateDepartureForDays(SystemUser manager, List<Resx> messages, IOrganizationServiceFactory factory, smt_resource_departure_eb departure, BookableResource resource, smt_model_contract contract)
        {
            List<DateTime> dates = new List<DateTime>();
            DateTime start = (DateTime)departure.smt_dt_start.Value;
            DateTime end = (DateTime)departure.smt_dt_end.Value;
            OptionSetValueCollection daysToWork = (OptionSetValueCollection)contract.smt_mc_work_days;
            List<smt_holiday> holidayList = RetrieveHolidays();

            // Executar para cada dia entre inicio e termino de afasatamento.
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                // Executar enquanto o dia for menor que a data limite.
                for (int day = 0; day < daysToWork.Count; day++)
                {
                    // Verifica se datas selecionadas são relacionadas aos dias da semana de trabalho permitidos pelo contrato do recurso.
                    if ((int)date.DayOfWeek == daysToWork[day].Value)
                    {
                        smt_holiday dt = holidayList.Where(x => x.smt_dt_holiday_date.Value.Date == date.Date).FirstOrDefault();

                        if (dt == null)
                        {
                            dates.Add(date);
                        }
                    }
                }
            }

            // Chamada do método que cria e valida as entradas de horas relacionadas.
            SendEntry(messages, contract, dates, manager, factory, resource, departure);
        }

        /// <summary>
        /// Método para enviar entrada de horas para a aprovação.
        /// </summary>
        /// <param name="approvalsList">Id da aprovação.</param>
        /// <returns>Retorna aprovação do id da lista.</returns>
        public msdyn_projectapproval SendForApproval(Guid approvalsList)
        {
            msdyn_projectapproval approval = new msdyn_projectapproval();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                approval = (from project in serviceContext.CreateQuery<msdyn_projectapproval>()
                            where project.Id == approvalsList
                            select project).FirstOrDefault();
            }
            return approval;
        }

        /// <summary>
        /// Método para listar os feriados existentes.
        /// </summary>
        /// <returns>Retorna lista com feriados.</returns>
        public List<smt_holiday> RetrieveHolidays()
        {
            List<smt_holiday> holidays = new List<smt_holiday>();
            using (CrmServiceContext serviceContext = new CrmServiceContext(Service))
            {
                return serviceContext.smtholiday.Where(day => day.Id != null).ToList();
            }
        }

        /// <summary>
        /// Método para criar entradas de horas de acordo com os dias do afastamento.
        /// </summary>
        /// <param name="messages">mensagem de erro resx.</param>
        /// <param name="contract">Contrato do recurso.</param>
        /// <param name="timeentryDate">Lista de dias.</param>
        /// <param name="resource">Recurso do afastamento.</param>
        /// <param name="factory">factory do impersonate.</param>
        /// <param name="manager">Gerente do recurso.</param>
        /// <param name="departure">Afastamento do recurso.</param>
        /// <returns>retorna lista com Ids de entradas de hora.</returns>
        public List<Guid> TimeEntryRecord(List<Resx> messages, smt_model_contract contract, List<DateTime> timeentryDate, BookableResource resource, IOrganizationServiceFactory factory, SystemUser manager, smt_resource_departure_eb departure)
        {
            IOrganizationService serviceResource = (IOrganizationService)GetService(resource.UserId.Id, factory);
            List<Guid> list = new List<Guid>();

            // Define a duração da entrada de horas para envio.
            int time = Decimal.ToInt32(contract.smt_dc_time_load.Value * 60);
            if (time == 0)
            {
                throw new InvalidPluginExecutionException("Erro ao criar registro. Entrada de horas deve possuir duração. \nLOG: " + messages.GetMessageById(ResxExtension.CUTDR111));
                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = departure.Id.ToString();
                String message = "Erro ao criar registro. Entrada de horas deve possuir duração. " + messages.GetMessageById(ResxExtension.CUTDR111);
                String name = "Erro ao criar registro. Entrada de horas deve possuir duração.";
                CreateLogs(eventType, serviceResource, recordid, message, name, resource.UserId.Id);
            }

            try
            {
                // Execute para cada data dentro da lista de dias selecionados.
                for (int days = 0; days < timeentryDate.Count; days++)
                {
                    msdyn_timeentry timeEntry = new msdyn_timeentry();
                    timeEntry.Id = Guid.NewGuid();
                    timeEntry.StatusCodeEnum = msdyn_timeentry_StatusCode.Active;
                    timeEntry.msdyn_date = timeentryDate[days].Date;
                    timeEntry.msdyn_duration = time;
                    timeEntry.msdyn_bookableresource = resource.ToEntityReference();
                    timeEntry.msdyn_typeEnum = msdyn_timeentrytype.Absence;
                    timeEntry.smt_pl_type_absence = new OptionSetValue(100000000);
                    timeEntry.msdyn_description = departure.smt_st_justification;
                    timeEntry.smt_lp_resource_departure = departure.ToEntityReference();
                    timeEntry.msdyn_externalDescription = departure.smt_st_justification;
                    timeEntry.OwnerId = new EntityReference(SystemUser.EntityLogicalName, resource.UserId.Id);
                    timeEntry.msdyn_manager = manager.ToEntityReference();
                    list.Add(serviceResource.Create(timeEntry));
                }

                // Execute para cada Id na lista com as entradas de horas já criadas.
                foreach (Guid id in list)
                {
                    msdyn_timeentry timeEntry = SearchEntry(id);
                    timeEntry.msdyn_entryStatusEnum = Earlybound.msdyn_timeentrystatus.Submitted;
                    timeEntry.EntityState = EntityState.Changed;
                    serviceResource.Update(timeEntry);
                }

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Erro ao criar registro de horas. \nLOG: " + ex.Message);
                OptionSetValue eventType = new OptionSetValue(100000000);
                String recordid = departure.Id.ToString();
                String message = "Erro ao criar registro de horas. " + messages.GetMessageById(ResxExtension.CUTDR111);
                String name = "Erro ao criar registro de horas.";
                CreateLogs(eventType, serviceResource, recordid, message, name, resource.UserId.Id);
            }
            return list;
        }

        /// <summary>
        /// Método para criação de logs em caso de erro esperado.
        /// </summary>
        /// <param name="eventType">Tipo de Evento que causou o erro.</param>
        /// <param name="newService">Serviço local relacionado ao usuário.</param>
        /// <param name="recordid">Id da entidade na qual o erro ocorreu.</param>
        /// <param name="message">mensagem de erro determinada para o erro específico.</param>
        /// <param name="name">nome da mensagem.</param>
        /// <param name="userId">usuário relacionado.</param>
        public void CreateLogs(OptionSetValue eventType, IOrganizationService newService, String recordid, String message, String name, Guid userId)
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
            newService.Create(log);
        }
    }
}