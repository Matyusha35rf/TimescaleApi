using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interfaces
{
    public interface IResultRepository
    {
        Task CreateAsync(Result result, CancellationToken cancellationToken = default);
    }
}
