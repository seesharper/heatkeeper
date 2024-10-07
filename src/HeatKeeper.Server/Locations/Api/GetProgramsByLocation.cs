namespace HeatKeeper.Server.Locations.Api;

[RequireUserRole]
[Get("api/locations/{locationId}/programs")]
public record ProgramsByLocationQuery(long LocationId) : IQuery<ProgramInfo[]>;


public class GetProgramsByLocation(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<ProgramsByLocationQuery, ProgramInfo[]>
{
    public async Task<ProgramInfo[]> HandleAsync(ProgramsByLocationQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ProgramInfo>(sqlProvider.GetProgramsByLocation, query)).ToArray();
}

public record ProgramInfo(long Id, string Name, long? ActiveScheduleId);