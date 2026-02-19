using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRM.Pixeon.Extends.Plugins;
using CrmEarlyBound;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Pixeon.Extends.Business;

namespace Pixeon.Extends.Plugins.Issue
{
    /// <summary>
    /// Gerar corpo da requisição JIRA
    /// </summary>
    public class PostUpdateSync : PluginBase
    {
        /// <summary>
        /// .
        /// </summary>
        public PostUpdateSync() : base(typeof(PostUpdateSync)) { }

        /// <summary>
        /// Doc. 
        /// </summary>
        /// <param name="localContext">Contexto de execução local.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            Smt_issueBusiness issueBusiness = new Smt_issueBusiness(localContext.OrganizationService, localContext.OrganizationServiceAdmin, localContext.TracingService, null);
            smt_issue target = localContext.GetTarget<smt_issue>();
            smt_issue postImage = localContext.GetPostImage<smt_issue>("updateIssuePostImage");
            smt_issue preImage = localContext.GetPreImage<smt_issue>();
            if (preImage.smt_bt_existingissue != true)
            {
                GenerateJIRARequestBody(target, postImage, issueBusiness);
            }
        }

        /// <summary>
        /// a
        /// </summary>
        /// <param name="target">.</param>
        /// <param name="postImage">.</param>
        /// <param name="issueBusiness">.</param>
        protected void GenerateJIRARequestBody(smt_issue target, smt_issue postImage, Smt_issueBusiness issueBusiness)
        {
            issueBusiness.GenerateJson(target, postImage, issueBusiness);
        }
    }
}
