using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TimescaleApi.Tests.DataAccess;

public class ValuesRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly IValuesRepository _repository;

    public ValuesRepositoryTests()
    {
        // Создаем InMemory базу данных для тестов
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ValuesRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddRecordsToDatabase()
    {
        // Arrange
        var records = new List<ValueRecord>
        {
            new() { Id = Guid.NewGuid(), FileName = "test1.csv", Date = DateTime.UtcNow, ExecutionTime = 1.5, Value = 100 },
            new() { Id = Guid.NewGuid(), FileName = "test1.csv", Date = DateTime.UtcNow, ExecutionTime = 2.5, Value = 200 }
        };

        // Act
        await _repository.AddRangeAsync(records);

        // Assert
        var dbRecords = await _context.Values.ToListAsync();
        Assert.Equal(2, dbRecords.Count);
        Assert.Contains(dbRecords, r => r.Value == 100);
        Assert.Contains(dbRecords, r => r.Value == 200);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldSetCreatedAt()
    {
        // Arrange
        var records = new List<ValueRecord>
        {
            new() { Id = Guid.NewGuid(), FileName = "test1.csv", Date = DateTime.UtcNow, ExecutionTime = 1.5, Value = 100 }
        };

        // Act
        await _repository.AddRangeAsync(records);

        // Assert
        var dbRecord = await _context.Values.FirstOrDefaultAsync();
        Assert.NotNull(dbRecord);
        Assert.NotEqual(default, dbRecord.CreatedAt);
    }

    [Fact]
    public async Task DeleteByFileNameAsync_ShouldRemoveAllRecordsForFile()
    {
        // Arrange
        var records = new List<ValueRecord>
        {
            new() { Id = Guid.NewGuid(), FileName = "test1.csv", Date = DateTime.UtcNow, ExecutionTime = 1.5, Value = 100 },
            new() { Id = Guid.NewGuid(), FileName = "test1.csv", Date = DateTime.UtcNow, ExecutionTime = 2.5, Value = 200 },
            new() { Id = Guid.NewGuid(), FileName = "test2.csv", Date = DateTime.UtcNow, ExecutionTime = 3.5, Value = 300 }
        };
        await _context.Values.AddRangeAsync(records);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteByFileNameAsync("test1.csv");

        // Assert
        var remainingRecords = await _context.Values.ToListAsync();
        Assert.Single(remainingRecords);
        Assert.Equal("test2.csv", remainingRecords[0].FileName);
    }

    [Fact]
    public async Task DeleteByFileNameAsync_WhenNoRecords_ShouldNotThrow()
    {
        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            _repository.DeleteByFileNameAsync("nonexistent.csv"));
        Assert.Null(exception);
    }

    [Fact]
    public async Task GetLastTenByFileNameAsync_ShouldReturnLastTenRecords()
    {
        // Arrange
        var fileName = "test.csv";
        var records = new List<ValueRecord>();

        for (int i = 0; i < 15; i++)
        {
            records.Add(new ValueRecord
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                Date = DateTime.UtcNow.AddHours(-i),
                ExecutionTime = i,
                Value = i * 10
            });
        }
        await _context.Values.AddRangeAsync(records);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLastTenByFileNameAsync(fileName);

        // Assert
        Assert.Equal(10, result.Count);
        Assert.True(result[0].Date > result[^1].Date);
    }

    [Fact]
    public async Task GetLastTenByFileNameAsync_WhenLessThanTen_ShouldReturnAll()
    {
        // Arrange
        var fileName = "test.csv";
        var records = new List<ValueRecord>();

        for (int i = 0; i < 3; i++)
        {
            records.Add(new ValueRecord
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                Date = DateTime.UtcNow,
                ExecutionTime = i,
                Value = i * 10
            });
        }
        await _context.Values.AddRangeAsync(records);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLastTenByFileNameAsync(fileName);

        // Assert
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task ReplaceByFileNameAsync_ShouldReplaceOldRecordsWithNew()
    {
        // Arrange
        var fileName = "test.csv";

        var oldRecords = new List<ValueRecord>
        {
            new() { Id = Guid.NewGuid(), FileName = fileName, Date = DateTime.UtcNow, ExecutionTime = 1, Value = 100 },
            new() { Id = Guid.NewGuid(), FileName = fileName, Date = DateTime.UtcNow, ExecutionTime = 2, Value = 200 }
        };
        await _context.Values.AddRangeAsync(oldRecords);
        await _context.SaveChangesAsync();

        var newRecords = new List<ValueRecord>
        {
            new() { Id = Guid.NewGuid(), FileName = fileName, Date = DateTime.UtcNow, ExecutionTime = 3, Value = 300 },
            new() { Id = Guid.NewGuid(), FileName = fileName, Date = DateTime.UtcNow, ExecutionTime = 4, Value = 400 },
            new() { Id = Guid.NewGuid(), FileName = fileName, Date = DateTime.UtcNow, ExecutionTime = 5, Value = 500 }
        };

        // Act
        await _repository.ReplaceByFileNameAsync(newRecords, fileName);

        // Assert
        var dbRecords = await _context.Values.Where(x => x.FileName == fileName).ToListAsync();
        Assert.Equal(3, dbRecords.Count);
        Assert.Contains(dbRecords, r => r.Value == 300);
        Assert.Contains(dbRecords, r => r.Value == 400);
        Assert.Contains(dbRecords, r => r.Value == 500);
    }
}