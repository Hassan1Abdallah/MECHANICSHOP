

using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler ()
        : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        public Task<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
