using BusinessLogic.Models;
using BusinessLogic.Services.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace BusinessLogic.Services
{
    /// <summary>
    /// Сервис для обработки CSV файлов с данными
    /// Отвечает за: чтение, валидацию, сохранение и расчет метрик
    /// </summary>
    public class FileProcessingService(
        IValuesRepository valuesRepository,
        IResultsRepository resultsRepository) : IFileProcessingService
    {
        /// <summary>
        /// Основной метод обработки CSV файла
        /// </summary>
        /// <param name="file">Загруженный CSV файл</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат обработки с деталями</returns>
        public async Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Чтение строк из файла
                var lines = await ReadLinesFromFileAsync(file, cancellationToken);

                // 2. Базовая валидация количества строк (ТЗ: от 1 до 10000)
                ValidateLineCount(lines);

                // 3. Парсинг и валидация каждой строки согласно ТЗ
                var validRecords = await ParseAndValidateRecordsAsync(lines, file.FileName);

                // 4. Сохранение в БД с перезаписью (ТЗ: если файл существует - перезаписать)
                await valuesRepository.ReplaceByFileNameAsync(validRecords, file.FileName, cancellationToken);

                // 5. Расчет и сохранение интегральных результатов (ТЗ: метрики в таблицу Results)
                var result = CalculateResults(validRecords, file.FileName);
                await resultsRepository.ReplaceByFileNameAsync(result, file.FileName, cancellationToken);

                // 6. Успешный ответ
                return CreateSuccessResponse(file.FileName, validRecords.Count);
            }
            catch (ValidationException ex)
            {
                // Ошибка валидации - возвращаем детали пользователю (ТЗ: вернуть соответствующую ошибку)
                return CreateErrorResponse(file.FileName, ex.Message);
            }
            catch (Exception ex)
            {
                // Непредвиденная ошибка
                return CreateErrorResponse(file.FileName, $"Ошибка обработки файла: {ex.Message}");
            }
        }

        /// <summary>
        /// Чтение всех строк из файла, пропуская заголовок
        /// </summary>
        private async Task<List<string>> ReadLinesFromFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var lines = new List<string>();
            using var reader = new StreamReader(file.OpenReadStream());

            // Пропускаем строку с заголовками (Date;ExecutionTime;Value)
            await reader.ReadLineAsync();

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }

            return lines;
        }

        /// <summary>
        /// Проверка количества строк согласно ТЗ (1-10000)
        /// </summary>
        private void ValidateLineCount(List<string> lines)
        {
            if (lines.Count < 1 || lines.Count > 10000)
            {
                throw new ValidationException($"Количество строк ({lines.Count}) должно быть от 1 до 10000");
            }
        }

        /// <summary>
        /// Парсинг и валидация всех строк файла
        /// </summary>
        private async Task<List<ValueRecord>> ParseAndValidateRecordsAsync(List<string> lines, string fileName)
        {
            var validRecords = new List<ValueRecord>();

            for (var i = 0; i < lines.Count; i++)
            {
                var lineNumber = i + 1; // +1 для понятных пользователю сообщений
                var record = await ParseAndValidateLineAsync(lines[i], lineNumber, fileName);
                validRecords.Add(record);
            }

            return validRecords;
        }

        /// <summary>
        /// Парсинг и валидация ОДНОЙ строки CSV
        /// Проверяет все требования ТЗ к отдельной записи
        /// </summary>
        private async Task<ValueRecord> ParseAndValidateLineAsync(string line, int lineNumber, string fileName)
        {
            var parts = line.Split(';');

            // Проверка количества колонок (ТЗ: ровно 3 поля)
            ValidateColumnCount(parts, lineNumber);

            // Валидация каждого поля
            var date = ValidateAndParseDate(parts[0], lineNumber);
            var executionTime = ValidateAndParseExecutionTime(parts[1], lineNumber);
            var value = ValidateAndParseValue(parts[2], lineNumber);

            return new ValueRecord
            {
                FileName = fileName,
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc), // Явно указываем UTC
                ExecutionTime = executionTime,
                Value = value
                // CreatedAt и Id проставляются в репозитории (технические поля)
            };
        }

        /// <summary>
        /// Проверка количества колонок в строке
        /// </summary>
        private void ValidateColumnCount(string[] parts, int lineNumber)
        {
            if (parts.Length != 3)
            {
                throw new ValidationException($"Строка {lineNumber}: неверный формат. Ожидается 3 поля, получено {parts.Length}");
            }
        }

        /// <summary>
        /// Валидация и парсинг даты
        /// Проверяет: формат, диапазон (2000-текущая), соответствие UTC
        /// </summary>
        private DateTime ValidateAndParseDate(string dateStr, int lineNumber)
        {
            var format = "yyyy-MM-ddTHH-mm-ss.ffffZ";

            if (!DateTime.TryParseExact(
                dateStr,
                format,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out var date))
            {
                throw new ValidationException($"Строка {lineNumber}: неверный формат даты '{dateStr}'. Ожидается: ГГГГ-ММ-ДДTчч-мм-сс.ммммZ");
            }

            // ТЗ: дата не может быть позже текущей
            if (date > DateTime.UtcNow)
            {
                throw new ValidationException($"Строка {lineNumber}: дата '{date:yyyy-MM-dd}' не может быть позже текущей");
            }

            // ТЗ: дата не может быть раньше 01.01.2000
            if (date < new DateTime(2000, 1, 1))
            {
                throw new ValidationException($"Строка {lineNumber}: дата '{date:yyyy-MM-dd}' не может быть раньше 01.01.2000");
            }

            return date;
        }

        /// <summary>
        /// Валидация и парсинг времени выполнения
        /// ТЗ: не может быть меньше 0, должно соответствовать типу double
        /// </summary>
        private double ValidateAndParseExecutionTime(string timeStr, int lineNumber)
        {
            if (!double.TryParse(timeStr, CultureInfo.InvariantCulture, out var executionTime))
            {
                throw new ValidationException($"Строка {lineNumber}: неверный формат времени выполнения '{timeStr}'");
            }

            if (executionTime < 0)
            {
                throw new ValidationException($"Строка {lineNumber}: время выполнения ({timeStr}) не может быть меньше 0");
            }

            return executionTime;
        }

        /// <summary>
        /// Валидация и парсинг значения
        /// ТЗ: не может быть меньше 0, должно соответствовать типу double
        /// </summary>
        private double ValidateAndParseValue(string valueStr, int lineNumber)
        {
            if (!double.TryParse(valueStr, CultureInfo.InvariantCulture, out var value))
            {
                throw new ValidationException($"Строка {lineNumber}: неверный формат значения '{valueStr}'");
            }

            if (value < 0)
            {
                throw new ValidationException($"Строка {lineNumber}: значение ({valueStr}) не может быть меньше 0");
            }

            return value;
        }

        /// <summary>
        /// Формирование успешного ответа
        /// </summary>
        private UploadCsvResponse CreateSuccessResponse(string fileName, int rowsSaved)
        {
            return new UploadCsvResponse
            {
                FileName = fileName,
                Success = true,
                Message = "Файл успешно обработан"
            };
        }

        /// <summary>
        /// Формирование ответа с ошибкой
        /// </summary>
        private UploadCsvResponse CreateErrorResponse(string fileName, string errorMessage)
        {
            return new UploadCsvResponse
            {
                FileName = fileName,
                Success = false,
                Message = errorMessage
            };
        }

        /// <summary>
        /// Расчет медианы для набора значений
        /// </summary>
        private double CalculateMedian(IEnumerable<double> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int count = sorted.Count;

            if (count == 0) return 0;

            if (count % 2 == 1) // нечетное количество
                return sorted[count / 2];
            else // четное количество
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }

        /// <summary>
        /// Расчет интегральных результатов согласно ТЗ
        /// </summary>
        private Result CalculateResults(List<ValueRecord> records, string fileName)
        {
            var dates = records.Select(r => r.Date);
            var values = records.Select(r => r.Value);
            var executionTimes = records.Select(r => r.ExecutionTime);

            var timeDelta = dates.Max() - dates.Min();

            return new Result
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                TimeDeltaSeconds = timeDelta.TotalSeconds, // ТЗ: дельта в секундах
                FirstOperationDate = dates.Min(),          // ТЗ: момент запуска первой операции
                AverageExecutionTime = executionTimes.Average(), // ТЗ: среднее время выполнения
                AverageValue = values.Average(),           // ТЗ: среднее значение
                MedianValue = CalculateMedian(values),      // ТЗ: медиана
                MaxValue = values.Max(),                    // ТЗ: максимальное значение
                MinValue = values.Min()                      // ТЗ: минимальное значение
                // CreatedAt проставляется в репозитории
            };
        }
    }

    /// <summary>
    /// Кастомное исключение для ошибок валидации
    /// Позволяет отделить ошибки бизнес-логики от системных
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}