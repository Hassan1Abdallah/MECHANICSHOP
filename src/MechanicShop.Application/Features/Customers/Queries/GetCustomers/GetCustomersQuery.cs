

using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers
{
    public sealed record GetCustomersQuery() : ICachedQuery<Result<List<CustomerDto>>>
    {
        public string CacheKey => "Customers";

        public string[] Tags => ["Customer"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
