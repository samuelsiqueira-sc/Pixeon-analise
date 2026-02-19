using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRM.Pixeon.Extends.Business;
using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.MultiLanguage;
using Microsoft.Xrm.Sdk.Metadata;
using CRM.Pixeon.Extends.Earlybound;
using CrmEarlyBound;

namespace Pixeon.Extends.Business
{
    public class smt_imposto_parameterBusiness : BaseBusiness
    {

        public smt_imposto_parameterBusiness(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService, List<Resx> messages = null) : base(service, serviceAdmin, tracingService, null) { }

        public void GetImposto(smt_imposto_parameter target)
        {
            using (var crmService = new CrmServiceContext(ServiceAdmin))
            {
                var imposto = (from i in crmService.CreateQuery<smt_imposto_parameter>()
                               where i.smt__year == target.smt__year
                               select i).ToList();

                if (imposto != null && imposto.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Um parâmetro com esse ano já foi criado. Por favor, verifique se os dados estão corretos.");
                }
            }
        }
    }
}
