using CQRS.Query.Abstractions;

namespace HeatKeeper.Server.Locations
{
    public class GetAllLocationsQuery : IQuery<LocationQueryResult[]>
    {

    }
}
