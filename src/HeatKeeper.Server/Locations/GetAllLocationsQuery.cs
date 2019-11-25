using CQRS.Query.Abstractions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Locations
{
    [RequireUserRole]
    public class GetAllLocationsQuery : IQuery<LocationQueryResult[]>
    {

    }
}
