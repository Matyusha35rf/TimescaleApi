namespace BusinessLogic.Models
{
    public class UploadCsvResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public string? FileName { get; set; }
        public int RowsSaved { get; set; }
        public DateTime FirstOperationDate { get; set; }
        public TimeSpan TimeDelta { get; set; }
        public double AverageExecutionTime { get; set; }
        public double AverageValue { get; set; }
        public double MedianValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
    }
}