using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace EfCoreMockHelpers;

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}
public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }
}
public class TestAsyncQueryProvider<T> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }
    public IQueryable CreateQuery(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        return new TestAsyncEnumerable<T>(expression);
    }
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        return new TestAsyncEnumerable<TElement>(expression);
    }
    public object Execute(Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        var result = _inner.Execute(expression);
        return result ?? throw new InvalidOperationException("Execution returned null.");
    }
    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }
     public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
 {
     cancellationToken.ThrowIfCancellationRequested();

     // Handle EF Core async extension methods
     if (expression is MethodCallExpression methodCall &&
         methodCall.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions))
     {
         var methodName = methodCall.Method.Name;
         if (methodName.EndsWith("Async"))
         {
             var syncMethodName = methodName[..^"Async".Length];
             var syncMethod = typeof(Queryable).GetMethods()
                 .FirstOrDefault(m => m.Name == syncMethodName &&
                     m.GetParameters().Length == methodCall.Method.GetParameters().Length - 1);

             if (syncMethod != null)
             {
                 var genericArgs = methodCall.Method.GetGenericArguments();
                 var syncMethodGeneric = syncMethod.MakeGenericMethod(genericArgs);
                 var args = methodCall.Arguments.Take(methodCall.Arguments.Count - 1).ToList();
                 var syncExpression = Expression.Call(null, syncMethodGeneric, args);

                 var result = _inner.Execute(syncExpression);

                 // Handle Task<T> return type using reflection
                 var taskType = typeof(Task<>).MakeGenericType(result?.GetType() ?? typeof(object));
                 return (TResult)Activator.CreateInstance(taskType, result)!;
             }
         }
     }

     // Fallback for other cases
     var rawResult = _inner.Execute(expression);

     // Handle Task<T> return type
     if (typeof(TResult).IsGenericType &&
         typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
     {
         Type resultType = typeof(TResult).GetGenericArguments()[0];
         object castResult = Convert.ChangeType(rawResult, resultType)!;

         // Get Task.FromResult method using reflection
         var fromResultMethod = typeof(Task).GetMethod(nameof(Task.FromResult))
             ?.MakeGenericMethod(resultType);

         return (TResult)fromResultMethod?.Invoke(null, new[] { castResult })!;
     }

     return (TResult)rawResult!;
 }
}