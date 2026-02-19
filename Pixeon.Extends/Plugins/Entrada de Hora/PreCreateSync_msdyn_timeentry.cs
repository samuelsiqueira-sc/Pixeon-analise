using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Earlybound;
using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins.Entrada_de_Hora
{
    public class PreCreateSync_msdyn_timeentry : PluginBase
    {
        public PreCreateSync_msdyn_timeentry() : base(typeof(PreCreateSync_msdyn_timeentry)) { }

        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            msdyn_timeentry target = localcontext.GetTarget<msdyn_timeentry>();
            TimeEntryBusiness business = new TimeEntryBusiness(localcontext.OrganizationService, localcontext.OrganizationServiceAdmin, localcontext.TracingService, null);
            ParentProjectTask(target, business);
            VerifyNewTimeEntry(target, business);
        }

        public void ParentProjectTask(msdyn_timeentry target, TimeEntryBusiness business)
        {
            if (target.msdyn_projectTask != null)
            {
                business.IfProjectTaskIsParent(target);
            }
        }
        /// <summary>
        /// Verifica se o recurso reservável pode criar um entrada de hora para uma determinada tarefa de projeto projeto.
        /// </summary>
        /// <param name="timeEntry">Entrada de hora</param>
        /// <param name="business">.</param>
        public void VerifyNewTimeEntry(msdyn_timeentry timeEntry, TimeEntryBusiness business )
        {
            if (timeEntry.msdyn_projectTask != null)            
            {
                if (timeEntry.smt_dc_task_percentage != null && timeEntry.smt_dc_task_percentage.Value == 100)
                {
                    var user = business.FindUser(timeEntry.msdyn_bookableresource.Id);
                    var project = business.FindProject(timeEntry.msdyn_project.Id);
                    var task = business.FindProjectTask(timeEntry.msdyn_projectTask.Id);
                    var verify = true;
                    
                    if (project.smt_lp_temporary_manager != null )
                    {
                        verify = project.smt_lp_temporary_manager.Id != user.Id;
                    }

                    if (user != null && project != null && project.msdyn_projectmanager.Id != user.Id && verify) 
                    {                       
                        if (task.smt_lp_milestone != null)
                        {
                            throw new InvalidPluginExecutionException("Você não pode finalizar uma tarefa que contém marcos, por favor contate seu gerente.");
                        }
                    }
                }
            }
        }
    }
}
