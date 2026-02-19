using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Models
{
    /// <summary>
    /// hhk
    /// </summary>
    [DataContract]
    public class OperationSetResponse
    {
        /// <summary>
        ///  Define OperationSetTd
        /// </summary>
        [DataMember(Name = "operationSetId")]
        public Guid OperationSetId { get; set; }

        /// <summary>
        /// Define OperationSetDetailTd
        /// </summary>
        [DataMember(Name = "operationSetDetailId")]
        public Guid OperationSetDetailId { get; set; }

        /// <summary>
        /// Define OperationType
        /// </summary>
        [DataMember(Name = "operationType")]
        public string OperationType { get; set; }
        /// <summary>
        /// Define RecordId
        /// </summary>
        [DataMember(Name = "recordId")]
        public string RecordId { get; set; }
        /// <summary>
        /// Define CorrelationId
        /// </summary>
        [DataMember(Name = "correlationId")]
        public string CorrelationId { get; set; }
    }
}
