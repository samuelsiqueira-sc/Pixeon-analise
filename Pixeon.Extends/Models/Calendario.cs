using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smart.ExtendedPSA.Extends.Models
{
    /// <summary>
    /// Calendario para controle do dia no SLA
    /// </summary>
    public class Calendario
    {
        /// <summary>
        /// PSA_day
        /// </summary>
        public DateTime PSA_Day;
        /// <summary>
        /// PSA_Duration
        /// </summary>
        public decimal PSA_Duration;
        /// <summary>
        /// PSA_EndTime
        /// </summary>
        public DateTime PSA_EndTime;
        /// <summary>
        /// PSA_EndTimeUtc
        /// </summary>
        public DateTime PSA_EndTimeUtc;
        /// <summary>
        /// PSA_Month
        /// </summary>
        public DateTime PSA_Month;
        /// <summary>
        /// PSA_Quarter
        /// </summary>
        public DateTime PSA_Quarter;
        /// <summary>
        /// PSA_StartTime
        /// </summary>
        public DateTime PSA_StartTime;
        /// <summary>
        /// PSA_StartTimeUtc
        /// </summary>
        public DateTime PSA_StartTimeUtc;
        /// <summary>
        /// PSA_Week
        /// </summary>
        public DateTime PSA_Week;
        /// <summary>
        /// ResourceId
        /// </summary>
        public string ResourceId;
        /// <summary>
        /// ResourceSubCode
        /// </summary>
        public int ResourceSubCode;
        /// <summary>
        /// ResourceTimeCode
        /// </summary>
        public int ResourceTimeCode;
    }
}
