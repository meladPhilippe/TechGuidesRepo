using Microsoft.EntityFrameworkCore;
using Moq;

namespace EfCoreMockHelpers

public static class MockDbSet
{
    public static Mock<DbSet<T>> Create<T>(IEnumerable<T> data) where T : class
               => SetupMock(new Mock<DbSet<T>>(), data);
    public static Mock<DbSet<T>> Setup<T>(Mock<DbSet<T>> mock, IEnumerable<T> data) where T : class
               => SetupMock(mock, data);
    private static Mock<DbSet<T>> SetupMock<T>(Mock<DbSet<T>> mock, IEnumerable<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        return mock;
    }

    public static Mock<DbSet<T>> Empty<T>() where T : class
               => SetupMock(new Mock<DbSet<T>>(), Enumerable.Empty<T>());
    public static Mock<DbSet<T>> CreateAsyncDbSet<T>(List<T> data) where T : class
    {
        var mockDbSet = new Mock<DbSet<T>>();

        // Mock IAsyncEnumerable<T>
        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

        // Mock IQueryable<T>
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(data.AsQueryable().Provider));

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(data.AsQueryable().Expression);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(data.AsQueryable().ElementType);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(data.GetEnumerator());

        return mockDbSet;
    }

}