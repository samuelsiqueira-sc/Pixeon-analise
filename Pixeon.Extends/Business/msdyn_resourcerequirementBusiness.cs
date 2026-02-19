using CRM.Pixeon.Extends.Business;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixeon.Extends.Business
{
    public class msdyn_resourcerequirementBusiness : BaseBusiness
    {
        public msdyn_resourcerequirementBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, messages) { }

        public void ValidateUpdate(Guid userId, BookableResourceBooking preImage, Guid bookingStatus, Guid bookable)
        {
            using (CrmServiceContext crmServiceContext = new CrmServiceContext(ServiceAdmin))
            {
                var status = (from b in crmServiceContext.CreateQuery<BookingStatus>()
                              where b.BookingStatusId == bookingStatus
                              select b).FirstOrDefault();

                if (status != null && status.Status.Value == 3)
                {
                    msdyn_project project = crmServiceContext.CreateQuery<msdyn_project>().Where(p => p.Id == preImage.msdyn_projectid.Id).FirstOrDefault();

                    if (project.smt_lp_temporary_manager != null && project.smt_lp_temporary_manager.Id != userId && project.msdyn_projectmanager.Id != userId)
                    {
                        throw new InvalidPluginExecutionException("Não é permitido um usuário que não é gerente alterar o status de um requisito");
                    }
                    else if (project.smt_lp_temporary_manager == null && project.msdyn_projectmanager.Id != userId)
                    {
                        throw new InvalidPluginExecutionException("Não é permitido um usuário que não é gerente alterar o status de um requisito");
                    }
                }
            }
        }

        public void ValidateDelete(Guid userId, BookableResourceBooking preImage)
        {
            using (CrmServiceContext crmServiceContext = new CrmServiceContext(ServiceAdmin))
            {
                msdyn_project project = crmServiceContext.CreateQuery<msdyn_project>().Where(p => p.Id == preImage.msdyn_projectid.Id).FirstOrDefault();

                if (((project.smt_lp_temporary_manager != null && project.smt_lp_temporary_manager.Id != userId) || (project.smt_lp_temporary_manager == null)) && (project.msdyn_projectmanager != null && project.msdyn_projectmanager.Id != userId))
                {
                    throw new InvalidPluginExecutionException("Não é permitido um usuário que não é gerente do projeto excluir a reserva.");
                }
            }
        }

        public void ValidateUpdateCancel(Guid userId, BookableResourceBooking preImage, Guid bookingStatus, Guid bookable)
        {
            using (CrmServiceContext crmServiceContext = new CrmServiceContext(ServiceAdmin))
            {
                var status = (from b in crmServiceContext.CreateQuery<BookingStatus>()
                              where b.BookingStatusId == bookingStatus
                              select b).FirstOrDefault();

                if (status != null && status.Status.Value == 3)
                {
                    ServiceAdmin.Delete(BookableResourceBooking.EntityLogicalName, bookable);
                }
            }
        }

        // public void ValidateIfExistBooking(Guid userId, BookableResourceBooking target, Guid bookingStatus, Guid bookable)
        // {
        //    using (CrmServiceContext crmServiceContext = new CrmServiceContext(ServiceAdmin))
        //    {
        //        var bookables = (from b in crmServiceContext.CreateQuery<BookableResourceBooking>()
        //                         join c in crmServiceContext.CreateQuery<BookingStatus>() on b.BookingStatus.Id equals c.BookingStatusId
        //                         where b.Resource.Id == target.Resource.Id
        //                         && b.StartTime >= target.StartTime && b.EndTime <= target.EndTime
        //                         && c.Status.Value != 3
        //                         select b).ToList();

        // if (bookables.Count > 0)
        //        {
        //            var duration = bookables.Sum(a => a.Duration);

        // duration = (duration + target.Duration) / 60;

        // if ()
        //        }
        //    }
        // }
    }
}
