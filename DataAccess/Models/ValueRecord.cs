namespace DataAccess.Models
{
    /// <summary>
    /// Модель данных из CSV файла (таблица Values)
    /// Содержит все записи из загруженных файлов
    /// </summary>
    public class ValueRecord
    {
        public Guid Id { get; set; }                          // Первичный ключ
        public required string FileName { get; set; }         // Имя файла-источника
        public DateTime Date { get; set; }                    // Время начала операции (из CSV)
        public double ExecutionTime { get; set; }             // Время выполнения в секундах
        public double Value { get; set; }                     // Показатель
        public DateTime CreatedAt { get; set; }               // Дата создания записи (техническое поле)
        public Result? Result { get; set; }                   // Навигационное свойство: результат файла
    }
}