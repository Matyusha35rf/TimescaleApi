using DataAccess.Models;

namespace BusinessLogic.Models.DTOs
{
    /// <summary>
    /// DTO для передачи данных записи клиенту (без технических полей)
    /// </summary>
    public class ValueRecordDto
    {
        public Guid Id { get; set; }              // Идентификатор записи (для возможных ссылок)
        public required string FileName { get; set; } // Имя файла-источника
        public DateTime Date { get; set; }         // Время начала операции (из CSV)
        public double ExecutionTime { get; set; }  // Время выполнения в секундах
        public double Value { get; set; }           // Показатель
    }
}