using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Models
{
    public class ResultFilter
    {
        public string? FileName { get; set; }
        public DateTime? FirstOperationDateFrom { get; set; }
        public DateTime? FirstOperationDateTo { get; set; }
        public double? AverageValueFrom { get; set; }
        public double? AverageValueTo { get; set; }
        public double? AverageExecutionTimeFrom { get; set; }
        public double? AverageExecutionTimeTo { get; set; }
    }
}
