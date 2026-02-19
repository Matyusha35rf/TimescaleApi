using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interfaces
{
    public interface IValuesRepository
    {
        // Массовое добавление
        Task AddRangeAsync(IEnumerable<ValueRecord> records, CancellationToken cancellationToken = default);

        // Массовое удаление по имени файла
        Task DeleteByFileNameAsync(string fileName, CancellationToken cancellationToken = default);

        // Замена (перезапись) - комбинация удаления и добавления
        Task ReplaceByFileNameAsync(IEnumerable<ValueRecord> records, string fileName, CancellationToken cancellationToken = default);
        
        // Получения списка последних 10 значений, отсортированных по начальному времени запуска Date по имени заданного файла..
        Task<List<ValueRecord>> GetLastTenByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
    }
}
