using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Models
{
    public class Result
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public double TimeDeltaSeconds { get; set; }        // max Date - min Date
        public DateTime FirstOperationDate { get; set; }
        public double AverageExecutionTime { get; set; }
        public double AverageValue { get; set; }
        public double MedianValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
