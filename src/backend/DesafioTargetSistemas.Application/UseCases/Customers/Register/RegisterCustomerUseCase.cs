using DesafioTargetSistemas.Communication.Requests;
using DesafioTargetSistemas.Communication.Responses;
using DesafioTargetSistemas.Domain.Entities;
using DesafioTargetSistemas.Domain.Repositories;
using DesafioTargetSistemas.Exception;
using DesafioTargetSistemas.Exception.ExceptionsBase;

namespace DesafioTargetSistemas.Application.UseCases.Customers.Register;

public class RegisterCustomerUseCase : IRegisterCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISellerRepository _sellerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCustomerUseCase(ICustomerRepository customerRepository, ISellerRepository sellerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _sellerRepository = sellerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseRegisteredCustomerJson> Execute(RequestRegisterCustomerJson request)
    {
        Validate(request);

        if (await _sellerRepository.GetById(request.SellerId) is null)
            throw new NotFoundException(ResourceMessageException.SELLER_NOT_FOUND);

        var document = request.Document.ToUpperInvariant();

        if (await _customerRepository.ExistsByDocument(document))
            throw new ConflictException(ResourceMessageException.CUSTOMER_DOCUMENT_ALREADY_REGISTERED);

        var customer = new Customer
        {
            PersonType = request.PersonType,
            Document = document,
            Name = request.Name.Trim(),
            TradeName = NullIfEmpty(request.TradeName),
            StateRegistration = NullIfEmpty(request.StateRegistration),
            Email = NullIfEmpty(request.Email),
            Phone = NullIfEmpty(request.Phone),
            SellerId = request.SellerId,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            Addresses =
            [
                new CustomerAddress
                    {
                        ZipCode = request.Address.ZipCode,
                        Street = request.Address.Street.Trim(),
                        Number = NullIfEmpty(request.Address.Number),
                        Complement = NullIfEmpty(request.Address.Complement),
                        District = request.Address.District.Trim(),
                        City = request.Address.City.Trim(),
                        State = request.Address.State.ToUpperInvariant(),
                        IsDefault = true
                    }
            ]
        };

        await _customerRepository.Add(customer);
        await _unitOfWork.Commit();

        return new ResponseRegisteredCustomerJson
        {
            Id = customer.Id
        };
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void Validate(RequestRegisterCustomerJson request)
    {
        var result = new RegisterCustomerValidator().Validate(request);

        if (!result.IsValid)
            throw new ErrorOnValidationException([.. result.Errors.Select(e => e.ErrorMessage)]);
    }
}
