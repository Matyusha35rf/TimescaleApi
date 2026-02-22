using System;

namespace DataAccess.Models
{
    /// <summary>
    /// Модель фильтра для запросов к таблице Results (МЕТОД 2 ИЗ ТЗ)
    /// Все поля опциональны - применяются только указанные фильтры
    /// </summary>
    public class ResultFilter
    {
        public string? FileName { get; set; }                    // Фильтр по имени файла (частичное совпадение)
        public DateTime? FirstOperationDateFrom { get; set; }    // Начало диапазона даты первой операции
        public DateTime? FirstOperationDateTo { get; set; }      // Конец диапазона даты первой операции
        public double? AverageValueFrom { get; set; }            // Минимальное среднее значение
        public double? AverageValueTo { get; set; }              // Максимальное среднее значение
        public double? AverageExecutionTimeFrom { get; set; }    // Минимальное среднее время выполнения
        public double? AverageExecutionTimeTo { get; set; }      // Максимальное среднее время выполнения
    }
}