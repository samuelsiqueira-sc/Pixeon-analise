using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using CRM.Smart.ExtendedPSA.Extends.Earlybound;
using CRM.Smart.ExtendedPSA.Extends.Plugins;
using Smart.ExtendedPSA.Extends.Business;
using Smart.ExtendedPSA.Extends.MultiLanguage;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Smart.ExtendedPSA.Extends.Business
{
    // TODO: Remover esta Classe Após Teste de TimeBankCalculation no TimeEntry
    /// <summary>
    /// Classe responsável por realizar os retireves necessários para o calculo do banco de horas e por fim calculalo
    /// </summary>
    public class TimeBankOperations
    {
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
    }
}
