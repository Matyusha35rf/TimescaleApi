using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services.Interfaces
{
    public interface IValueQueryService
    {
        Task<List<ValueRecord>> GetLastTenValuesAsync(string fileName, CancellationToken cancellationToken);
    }
}
