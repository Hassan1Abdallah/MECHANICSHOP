

using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer
{
    public sealed class CreateCustomerCommandHandler (IAppDbContext context,
                                                      ILogger<CreateCustomerCommandHandler> logger,
                                                      HybridCache cache)
        : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand command, CancellationToken ct)
        {
            var email = command.Email.Trim().ToLower();

            var exists = await _context.Customers.AnyAsync(c => c.Email!.ToLower()
                                                                == email, ct);

            if (exists)
            {
                _logger.LogWarning("Customer creation aborted. Email already exists");
                return CustomerErrors.CustomerExists;
            }

            List<Vehicle> vehicles = [];

            foreach(var v in command.Vehicles)
            {
                var VehicleResult = Vehicle.Create(Guid.NewGuid(), v.Make, v.Model, v.Year, v.LicensePlate);

                if (VehicleResult.IsError)
                {
                    return VehicleResult.Errors;
                }

                vehicles.Add(VehicleResult.Value);
            }

            var createCustomerResult = Customer.Create(Guid.NewGuid(), command.Name.Trim(), command.PhoneNumber.Trim(), command.Email.Trim(), vehicles);

            if (createCustomerResult.IsError)
            {
                return createCustomerResult.Errors;
            }

            _context.Customers.Add(createCustomerResult.Value);

            await _context.SaveChangesAsync(ct);

            var customer = createCustomerResult.Value;

            _logger.LogInformation("Customer created successfully Id: {CustomerId}", createCustomerResult.Value.Id);

            await _cache.RemoveByTagAsync("Customer", ct);

            return customer.ToDto();


        }
    }
}
