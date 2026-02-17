namespace BusinessLogic.Models
{
    public class UploadCsvResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FileName { get; set; }       
        public int RowsSaved { get; set; }     
        public List<string> Errors { get; set; } = new();
    }
}