using Sampoerna.EMS.Website.Models.WorkflowHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class MasterModel : BaseModel
    {
        public MasterModel() : base()
        {

        }

        /// <summary>
        /// Property to bind workflow history data displayed on data grid
        /// </summary>
        public List<WorkflowHistoryViewModel> WorkflowHistory
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate whether current user is the creator of a form data
        /// </summary>
        public bool IsCreator
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate whether current entry is already submitted for approval
        /// </summary>
        public bool IsSubmitted
        {
            set; get;
        }

        /// <summary>
        /// Property that indicate whether current entry is already approved
        /// </summary>
        public bool IsApproved
        {
            set; get;
        }

        /// <summary>
        /// Property represents revision data message
        /// </summary>
        public Shared.WorkflowHistory RevisionData
        {
            set; get;
        }

    }
}