using BusinessLogic.Models;
using BusinessLogic.Services.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace BusinessLogic.Services
{
    public class FileProcessingService(
        IValuesRepository valuesRepository,
        IResultsRepository resultsRepository) : IFileProcessingService
    {
        public async Task<UploadCsvResponse> ProcessCsvFileAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1. Чтение строк из файла
                var lines = await ReadLinesFromFileAsync(file, cancellationToken);

                // 2. Базовая валидация количества строк
                ValidateLineCount(lines);

                // 3. Парсинг и валидация каждой строки
                var validRecords = await ParseAndValidateRecordsAsync(lines, file.FileName);

                // 4. Сохранение в БД
                await valuesRepository.ReplaceByFileNameAsync(validRecords, file.FileName, cancellationToken);

                // 5. Расчет и сохранение результатов
                var result = CalculateResults(validRecords, file.FileName);
                await resultsRepository.ReplaceByFileNameAsync(result, file.FileName, cancellationToken);

                // 6. Успешный ответ
                return CreateSuccessResponse(file.FileName, validRecords.Count);
            }
            catch (ValidationException ex)
            {
                return CreateErrorResponse(file.FileName, ex.Message);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(file.FileName, $"Ошибка обработки файла: {ex.Message}");
            }
        }

        private async Task<List<string>> ReadLinesFromFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            var lines = new List<string>();
            using var reader = new StreamReader(file.OpenReadStream());

            // Пропускаем заголовок
            await reader.ReadLineAsync();

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }

            return lines;
        }

        private void ValidateLineCount(List<string> lines)
        {
            if (lines.Count < 1 || lines.Count > 10000)
            {
                throw new ValidationException($"Количество строк ({lines.Count}) должно быть от 1 до 10000");
            }
        }

        private async Task<List<ValueRecord>> ParseAndValidateRecordsAsync(List<string> lines, string fileName)
        {
            var validRecords = new List<ValueRecord>();

            for (var i = 0; i < lines.Count; i++)
            {
                var lineNumber = i + 1;
                var record = await ParseAndValidateLineAsync(lines[i], lineNumber, fileName);
                validRecords.Add(record);
            }

            return validRecords;
        }

        private async Task<ValueRecord> ParseAndValidateLineAsync(string line, int lineNumber, string fileName)
        {
            var parts = line.Split(';');

            ValidateColumnCount(parts, lineNumber);

            var date = ValidateAndParseDate(parts[0], lineNumber);
            var executionTime = ValidateAndParseExecutionTime(parts[1], lineNumber);
            var value = ValidateAndParseValue(parts[2], lineNumber);

            return new ValueRecord
            {
                FileName = fileName,
                Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                ExecutionTime = executionTime,
                Value = value
            };
        }

        private void ValidateColumnCount(string[] parts, int lineNumber)
        {
            if (parts.Length != 3)
            {
                throw new ValidationException($"Строка {lineNumber}: неверный формат. Ожидается 3 поля, получено {parts.Length}");
            }
        }

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
                throw new ValidationException($"Строка {lineNumber}: неверный формат даты '{dateStr}'");
            }

            if (date > DateTime.UtcNow)
            {
                throw new ValidationException($"Строка {lineNumber}: дата '{date:yyyy-MM-dd}' не может быть позже текущей");
            }

            if (date < new DateTime(2000, 1, 1))
            {
                throw new ValidationException($"Строка {lineNumber}: дата '{date:yyyy-MM-dd}' не может быть раньше 01.01.2000");
            }

            return date;
        }

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

        private UploadCsvResponse CreateSuccessResponse(string fileName, int rowsSaved)
        {
            return new UploadCsvResponse
            {
                FileName = fileName,
                Success = true,
                Message = "Файл успешно обработан"
            };
        }

        private UploadCsvResponse CreateErrorResponse(string fileName, string errorMessage)
        {
            return new UploadCsvResponse
            {
                FileName = fileName,
                Success = false,
                Message = errorMessage
            };
        }

        private double CalculateMedian(IEnumerable<double> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int count = sorted.Count;

            if (count == 0) return 0;

            if (count % 2 == 1)
                return sorted[count / 2];
            else
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }

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
                TimeDeltaSeconds = timeDelta.TotalSeconds,
                FirstOperationDate = dates.Min(),
                AverageExecutionTime = executionTimes.Average(),
                AverageValue = values.Average(),
                MedianValue = CalculateMedian(values),
                MaxValue = values.Max(),
                MinValue = values.Min()
            };
        }
    }

    // Кастомное исключение для валидации
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}