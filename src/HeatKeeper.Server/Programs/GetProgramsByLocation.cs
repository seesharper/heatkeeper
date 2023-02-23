using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireUserRole]
public record ProgramsByLocationQuery(long LocationId) : IQuery<Program[]>;


public class GetProgramsByLocation : IQueryHandler<ProgramsByLocationQuery, Program[]>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetProgramsByLocation(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<Program[]> HandleAsync(ProgramsByLocationQuery query, CancellationToken cancellationToken = default)
        => (await _dbConnection.ReadAsync<Program>(_sqlProvider.GetProgramsByLocation, query)).ToArray();
}

public record Program(long Id, string Name, long? ActiveScheduleId);