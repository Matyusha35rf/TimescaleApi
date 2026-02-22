namespace BusinessLogic.Models.DTOs
{
    /// <summary>
    /// DTO для передачи результатов клиенту (МЕТОД 2 ИЗ ТЗ)
    /// </summary>
    public class ResultDto
    {
        public Guid Id { get; set; }                          // Идентификатор записи
        public required string FileName { get; set; }         // Имя файла
        public double TimeDeltaSeconds { get; set; }          // Дельта времени в секундах (max Date - min Date)
        public DateTime FirstOperationDate { get; set; }      // Дата первой операции (минимальная дата)
        public double AverageExecutionTime { get; set; }      // Среднее время выполнения
        public double AverageValue { get; set; }               // Среднее значение показателя
        public double MedianValue { get; set; }                // Медиана показателя
        public double MaxValue { get; set; }                   // Максимальное значение
        public double MinValue { get; set; }                   // Минимальное значение
    }
}