using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.Concurrency;

public static class ConcurrencyRetry
{
    private const int MAX_ATTEMPTS = 3;

    public static async Task<T> Execute<T>(IUnitOfWork unitOfWork, Func<Task<T>> operation)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (ConcurrencyConflictException) when (attempt < MAX_ATTEMPTS)
            {
                unitOfWork.Discard();
            }
        }
    }
}
