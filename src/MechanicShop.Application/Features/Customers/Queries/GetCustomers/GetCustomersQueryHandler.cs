

using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler(IAppDbContext context) : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
    {
        private readonly IAppDbContext _context = context;

        public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken ct)
        {
            return await _context.Customers.Include(c => c.Vehicles).AsNoTracking().Select(c => c.ToDto()).ToListAsync(ct);
        }
    }
}
