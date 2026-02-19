using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Models
{
    /// <summary>
    /// 0
    /// </summary>
    [DataContract]
    public class UpdatedContour
    {
        /// <summary>
        /// Define Start
        /// </summary>
        [DataMember(Name = "start")]
        public DateTime Start { get; set; }
        /// <summary>
        /// Define End
        /// </summary>
        [DataMember(Name = "end")]
        public DateTime End { get; set; }
        /// <summary>
        /// Define Minutes
        /// </summary>
        [DataMember(Name = "minutes")]
        public decimal Minutes { get; set; }
    }
}
