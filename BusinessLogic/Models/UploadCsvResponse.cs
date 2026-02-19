namespace BusinessLogic.Models
{
    public class UploadCsvResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }
}