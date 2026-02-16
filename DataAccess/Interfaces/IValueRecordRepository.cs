using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Interfaces
{
    public interface IValueRecordRepository
    {
        Task CreateAsync(ValueRecord valueRecord, CancellationToken cancellationToken = default);
    }
}
