using System;
using System.Collections.Generic;

namespace DataAccess.Models
{
    /// <summary>
    /// Модель интегральных результатов по файлу (таблица Results)
    /// Содержит агрегированные метрики для каждого загруженного CSV файла
    /// Связана с ValueRecords через FileName
    /// </summary>
    public class Result
    {
        public Guid Id { get; set; }                          // Первичный ключ
        public required string FileName { get; set; }         // Уникальное имя файла
        public double TimeDeltaSeconds { get; set; }          // Дельта времени (max Date - min Date) в секундах
        public DateTime FirstOperationDate { get; set; }      // Дата первой операции (минимальная Date)
        public double AverageExecutionTime { get; set; }      // Среднее время выполнения
        public double AverageValue { get; set; }               // Среднее значение показателя
        public double MedianValue { get; set; }                // Медиана показателя
        public double MaxValue { get; set; }                   // Максимальное значение показателя
        public double MinValue { get; set; }                   // Минимальное значение показателя
        public DateTime CreatedAt { get; set; }                // Дата создания записи (техническое поле)
        public ICollection<ValueRecord> ValueRecords { get; set; } = []; // Связанные записи из Values
    }
}