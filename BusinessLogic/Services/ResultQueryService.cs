using BusinessLogic.Services.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Services
{
    public class ResultQueryService(IResultsRepository resultsRepository) : IResultQueryService
    {
        public async Task<List<Result>> GetFilteredResultsAsync(ResultFilter filter, CancellationToken cancellationToken = default)
        {
            return await resultsRepository.GetFilteredAsync(filter, cancellationToken);
        }

        
    }
}
