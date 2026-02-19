using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services.Interfaces
{
    public interface IResultQueryService
    {
        Task<List<Result>> GetFilteredResultsAsync(ResultFilter filter, CancellationToken cancellationToken = default);
    }
}
