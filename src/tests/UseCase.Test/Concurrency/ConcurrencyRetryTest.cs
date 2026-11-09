using DesafioTargetSistemas.Application.Concurrency;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Moq;
using Shouldly;

namespace UseCase.Test.Concurrency;

public class ConcurrencyRetryTest
{
    [Fact]
    public async Task SuccessFirstAttempt()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var executions = 0;

        Task<string> Operation()
        {
            executions++;
            return Task.FromResult("ok");
        }

        // Act
        var result = await ConcurrencyRetry.Execute(unitOfWork.Object, Operation);

        // Assert
        result.ShouldBe("ok");
        executions.ShouldBe(1);
        unitOfWork.Verify(u => u.Discard(), Times.Never);
    }

    [Fact]
    public async Task SuccessAfterConflict()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var executions = 0;

        Task<string> Operation()
        {
            executions++;

            if (executions == 1)
                throw new ConcurrencyConflictException();

            return Task.FromResult("ok");
        }

        // Act
        var result = await ConcurrencyRetry.Execute(unitOfWork.Object, Operation);

        // Assert
        result.ShouldBe("ok");
        executions.ShouldBe(2);
        unitOfWork.Verify(u => u.Discard(), Times.Once);
    }

    [Fact]
    public async Task ErrorAfterMaxAttempts()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var executions = 0;

        Task<string> Operation()
        {
            executions++;
            throw new ConcurrencyConflictException();
        }

        // Act
        Func<Task> act = async () => await ConcurrencyRetry.Execute(unitOfWork.Object, Operation);

        // Assert
        await act.ShouldThrowAsync<ConcurrencyConflictException>();
        executions.ShouldBe(3);
        unitOfWork.Verify(u => u.Discard(), Times.Exactly(2));
    }

    [Fact]
    public async Task ErrorOtherExceptionIsNotRetried()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var executions = 0;

        Task<string> Operation()
        {
            executions++;
            throw new InsufficientStockException();
        }

        // Act
        Func<Task> act = async () => await ConcurrencyRetry.Execute(unitOfWork.Object, Operation);

        // Assert
        await act.ShouldThrowAsync<InsufficientStockException>();
        executions.ShouldBe(1);
        unitOfWork.Verify(u => u.Discard(), Times.Never);
    }
}
