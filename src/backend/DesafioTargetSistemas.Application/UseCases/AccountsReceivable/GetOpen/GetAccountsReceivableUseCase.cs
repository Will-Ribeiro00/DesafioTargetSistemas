using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Domain.Services;

namespace DesafioTargetSistemas.Application.UseCases.AccountsReceivable.GetOpen
{
    public class GetAccountsReceivableUseCase(IAccountReceivableRepository accountReceivableRepository) : IGetAccountsReceivableUseCase
    {
        private readonly IAccountReceivableRepository _accountReceivableRepository = accountReceivableRepository;

        public async Task<List<ResponseAccountReceivableJson>> Execute()
        {
            var accounts = await _accountReceivableRepository.GetOpen();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            return [.. accounts.Select(a =>
            {
                var daysOverdue = InterestCalculator.DaysOverdue(a.DueDate, today);
                var interest = InterestCalculator.Calculate(a.Amount, daysOverdue);

                return new ResponseAccountReceivableJson
                {
                    Id = a.Id,
                    SaleId = a.SaleId,
                    Amount = a.Amount,
                    DueDate = a.DueDate,
                    DaysOverdue = daysOverdue,
                    Interest = interest,
                    TotalAmount = a.Amount + interest
                };
            })];
        }
    }
}
