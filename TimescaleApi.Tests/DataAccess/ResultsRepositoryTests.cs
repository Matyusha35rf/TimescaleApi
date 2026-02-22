using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TimescaleApi.Tests.DataAccess;

public class ResultsRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IResultsRepository _repository;

    public ResultsRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ResultsRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ShouldAddResultToDatabase()
    {
        // Arrange
        var result = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test.csv",
            TimeDeltaSeconds = 3600,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 2.5,
            AverageValue = 150,
            MedianValue = 150,
            MaxValue = 200,
            MinValue = 100
        };

        // Act
        await _repository.AddAsync(result);

        // Assert
        var dbResult = await _context.Results.FirstOrDefaultAsync();
        Assert.NotNull(dbResult);
        Assert.Equal("test.csv", dbResult.FileName);
        Assert.Equal(3600, dbResult.TimeDeltaSeconds);
    }

    [Fact]
    public async Task AddAsync_ShouldSetCreatedAt()
    {
        // Arrange
        var result = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test.csv",
            TimeDeltaSeconds = 3600,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 2.5,
            AverageValue = 150,
            MedianValue = 150,
            MaxValue = 200,
            MinValue = 100
        };

        // Act
        await _repository.AddAsync(result);

        // Assert
        var dbResult = await _context.Results.FirstOrDefaultAsync();
        Assert.NotNull(dbResult);
        Assert.NotEqual(default, dbResult.CreatedAt);
    }

    [Fact]
    public async Task GetByFileNameAsync_ShouldReturnCorrectResult()
    {
        // Arrange
        var result1 = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test1.csv",
            TimeDeltaSeconds = 3600,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 2.5,
            AverageValue = 150,
            MedianValue = 150,
            MaxValue = 200,
            MinValue = 100
        };

        var result2 = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test2.csv",
            TimeDeltaSeconds = 7200,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 3.5,
            AverageValue = 250,
            MedianValue = 250,
            MaxValue = 300,
            MinValue = 200
        };

        await _context.Results.AddRangeAsync(result1, result2);
        await _context.SaveChangesAsync();

        // Act
        var found = await _repository.GetByFileNameAsync("test2.csv");

        // Assert
        Assert.NotNull(found);
        Assert.Equal("test2.csv", found.FileName);
        Assert.Equal(7200, found.TimeDeltaSeconds);
    }

    [Fact]
    public async Task GetByFileNameAsync_WhenNotFound_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByFileNameAsync("nonexistent.csv");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteByFileNameAsync_ShouldRemoveResult()
    {
        // Arrange
        var result = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test.csv",
            TimeDeltaSeconds = 3600,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 2.5,
            AverageValue = 150,
            MedianValue = 150,
            MaxValue = 200,
            MinValue = 100
        };
        await _context.Results.AddAsync(result);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteByFileNameAsync("test.csv");

        // Assert
        var dbResult = await _context.Results.FirstOrDefaultAsync();
        Assert.Null(dbResult);
    }

    [Fact]
    public async Task DeleteByFileNameAsync_WhenNotFound_ShouldNotThrow()
    {
        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            _repository.DeleteByFileNameAsync("nonexistent.csv"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task ReplaceByFileNameAsync_ShouldReplaceOldWithNew()
    {
        // Arrange
        var oldResult = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test.csv",
            TimeDeltaSeconds = 3600,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 2.5,
            AverageValue = 150,
            MedianValue = 150,
            MaxValue = 200,
            MinValue = 100
        };
        await _context.Results.AddAsync(oldResult);
        await _context.SaveChangesAsync();

        var newResult = new Result
        {
            Id = Guid.NewGuid(),
            FileName = "test.csv",
            TimeDeltaSeconds = 7200,
            FirstOperationDate = DateTime.UtcNow,
            AverageExecutionTime = 3.5,
            AverageValue = 250,
            MedianValue = 250,
            MaxValue = 300,
            MinValue = 200
        };

        // Act
        await _repository.ReplaceByFileNameAsync(newResult, "test.csv");

        // Assert
        var dbResult = await _context.Results.FirstOrDefaultAsync();
        Assert.NotNull(dbResult);
        Assert.Equal(7200, dbResult.TimeDeltaSeconds);
        Assert.Equal(3.5, dbResult.AverageExecutionTime);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldFilterByMultipleCriteria()
    {
        // Arrange
        var baseDate = new DateTime(2024, 1, 15);

        var results = new List<Result>
    {
        new() {
            Id = Guid.NewGuid(),
            FileName = "test1.csv",
            TimeDeltaSeconds = 3600,
            FirstOperationDate = baseDate,
            AverageExecutionTime = 2.5,
            AverageValue = 150,
            MedianValue = 150,
            MaxValue = 200,
            MinValue = 100
        },
        new() {
            Id = Guid.NewGuid(),
            FileName = "test2.csv",
            TimeDeltaSeconds = 7200,
            FirstOperationDate = baseDate.AddDays(1),
            AverageExecutionTime = 3.5,
            AverageValue = 250,
            MedianValue = 250,
            MaxValue = 300,
            MinValue = 200
        }
    };

        _context.Results.RemoveRange(_context.Results);
        await _context.SaveChangesAsync();

        await _context.Results.AddRangeAsync(results);
        await _context.SaveChangesAsync();

        var filter = new ResultFilter
        {
            FileName = "test2.csv",
            FirstOperationDateFrom = baseDate,
            FirstOperationDateTo = baseDate.AddDays(2)
        };

        // Act
        var filtered = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.Single(filtered);
        Assert.Equal("test2.csv", filtered[0].FileName);
    }

    [Fact]
    public async Task GetFilteredAsync_ShouldFilterByDateRange()
    {
        // Arrange
        var results = new List<Result>
        {
            new() { Id = Guid.NewGuid(), FileName = "test1.csv", TimeDeltaSeconds = 3600, FirstOperationDate = new DateTime(2024, 1, 1), AverageExecutionTime = 2.5, AverageValue = 150, MedianValue = 150, MaxValue = 200, MinValue = 100 },
            new() { Id = Guid.NewGuid(), FileName = "test2.csv", TimeDeltaSeconds = 7200, FirstOperationDate = new DateTime(2024, 2, 1), AverageExecutionTime = 3.5, AverageValue = 250, MedianValue = 250, MaxValue = 300, MinValue = 200 },
            new() { Id = Guid.NewGuid(), FileName = "test3.csv", TimeDeltaSeconds = 4800, FirstOperationDate = new DateTime(2024, 3, 1), AverageExecutionTime = 4.5, AverageValue = 350, MedianValue = 350, MaxValue = 400, MinValue = 300 }
        };
        await _context.Results.AddRangeAsync(results);
        await _context.SaveChangesAsync();

        var filter = new ResultFilter
        {
            FirstOperationDateFrom = new DateTime(2024, 1, 15),
            FirstOperationDateTo = new DateTime(2024, 2, 15)
        };

        // Act
        var filtered = await _repository.GetFilteredAsync(filter);

        // Assert
        Assert.Single(filtered);
        Assert.Equal("test2.csv", filtered[0].FileName);
    }
}