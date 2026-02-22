namespace BusinessLogic.Models
{
    /// <summary>
    /// Модель ответа на загрузку CSV файла (МЕТОД 1 ИЗ ТЗ)
    /// </summary>
    public class UploadCsvResponse
    {
        public bool Success { get; set; } = true;      // true - файл обработан успешно, false - ошибка валидации
        public string Message { get; set; } = string.Empty; // Сообщение об успехе или описание ошибки
        public string? FileName { get; set; }          // Имя загруженного файла
    }
}