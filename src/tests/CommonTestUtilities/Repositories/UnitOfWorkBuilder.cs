using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception.ExceptionsBase;
using Moq;

namespace CommonTestUtilities.Repositories;

public class UnitOfWorkBuilder
{
    public static IUnitOfWork Build() => new Mock<IUnitOfWork>().Object;

    public static Mock<IUnitOfWork> BuildWithConflictOnFirstCommit()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork
            .SetupSequence(u => u.Commit())
            .ThrowsAsync(new ConcurrencyConflictException())
            .Returns(Task.CompletedTask);

        return unitOfWork;
    }
}
