using BusinessLogic.Models;
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
            //Ответ в случае, если файл не пройдёт валидацию
            var response = new UploadCsvResponse
            {
                FileName = file.FileName,
                Success = false,
                Message = "Файл не прошёл валидацию. ",
            };

            try
            {
                var lines = new List<string>();
                using var reader = new StreamReader(file.OpenReadStream());

                // Пропускаем строку с заголовками
                await reader.ReadLineAsync();

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }

                // Проверка количества строк
                if (lines.Count < 1 || lines.Count > 10000)
                {
                    response.Success = false;
                    response.Message += $"Количество строк ({lines.Count}) должно быть от 1 до 10000";
                    return response;
                }

                var validRecords = new List<ValueRecord>();

                for (var i = 0; i < lines.Count; i++)
                {
                    var lineNumber = i + 1; // +1 потому что, i начинается с 0

                    var parts = lines[i].Split(';');

                    // Проверка количества частей
                    if (parts.Length != 3)
                    {
                        response.Success = false;
                        response.Message = $"Строка {lineNumber}: неверный формат. Ожидается 3 поля, получено {parts.Length}";
                        return response;
                    }

                    var strDate = parts[0];
                    var strExecutionTime = parts[1];
                    var strValue = parts[2];

                    // Проверка поля даты
                    var format = "yyyy-MM-ddTHH-mm-ss.ffffZ";

                    if (!DateTime.TryParseExact(
                        strDate,
                        format,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal,
                        out var date))
                    {
                        response.Message = $"Строка {lineNumber}: неверный формат даты '{strDate}'";
                        return response;
                    }

                    // Проверка: дата не позже текущей
                    if (date > DateTime.UtcNow)
                    {
                        response.Message = $"Строка {lineNumber}: дата '{date:yyyy-MM-dd}' не может быть позже текущей";
                        return response;
                    }

                    // Проверка: дата не раньше 01.01.2000
                    if (date < new DateTime(2000, 1, 1))
                    {
                        response.Message = $"Строка {lineNumber}: дата '{date:yyyy-MM-dd}' не может быть раньше 01.01.2000";
                        return response;
                    }

                    // Проверка времени выполнения
                    if (!double.TryParse(parts[1], CultureInfo.InvariantCulture, out var executionTime))
                    {
                        response.Message = $"Строка {lineNumber}: неверный формат времени выполнения '{parts[1]}'";
                        return response;
                    }

                    // Проверка: время выполнения не может быть меньше 0
                    if (executionTime < 0)
                    {
                        response.Message = $"Строка {lineNumber}: время выполнения ({strExecutionTime}) не может быть меньше 0";
                        return response;
                    }

                    // Проверка значения показателя
                    if (!double.TryParse(parts[2], CultureInfo.InvariantCulture, out var value))
                    {
                        response.Message = $"Строка {lineNumber}: неверный формат значения '{parts[2]}'";
                        return response;
                    }

                    // Проверка: значение не может быть меньше 0
                    if (value < 0)
                    {
                        response.Message = $"Строка {lineNumber}: значение ({strValue}) не может быть меньше 0";
                        return response;
                    }

                    // Если все проверки пройдены - добавляем запись
                    validRecords.Add(new ValueRecord
                    {
                        FileName = file.FileName,
                        Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                        ExecutionTime = executionTime,
                        Value = value
                    });
                }

                // Если все записи прошли тест на валидность, то сохраняем их в базу
                await valuesRepository.ReplaceByFileNameAsync(validRecords, validRecords[0].FileName, cancellationToken);

                // Расчёт интегральных результатов
                var result = CalculateResults(validRecords, file.FileName);

                // Сохранение результатов в бд
                await resultsRepository.ReplaceByFileNameAsync(result, file.FileName, cancellationToken);

                // Изменяем и отправляем ответ после успешной обработки файла
                response.Success = true;
                response.Message = "Файл успешно обработан";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Ошибка обработки файла: {ex.Message}";
                return response;
            }
        }

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

        private Result CalculateResults(List<ValueRecord> records, string fileName)
        {
            var dates = records.Select(r => r.Date);
            var values = records.Select(r => r.Value);
            var executionTimes = records.Select(r => r.ExecutionTime);

            return new Result
            {
                Id = Guid.NewGuid(),
                FileName = fileName,

                // Дельта времени (max Date - min Date)
                TimeDelta = dates.Max() - dates.Min(),

                // Минимальная дата (первая операция)
                FirstOperationDate = dates.Min(),

                // Среднее время выполнения
                AverageExecutionTime = executionTimes.Average(),

                // Среднее значение
                AverageValue = values.Average(),

                // Медиана
                MedianValue = CalculateMedian(values),

                // Максимальное значение
                MaxValue = values.Max(),

                // Минимальное значение
                MinValue = values.Min()
            };
        }
    }
}