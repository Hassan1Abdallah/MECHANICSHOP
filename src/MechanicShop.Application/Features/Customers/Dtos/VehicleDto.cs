

namespace MechanicShop.Application.Features.Customers.Dtos
{
    public sealed record VehicleDto(Guid VehicleId,
                                    string Makem,
                                    string Model,
                                    int Year,
                                    string LicensePlate);
}
