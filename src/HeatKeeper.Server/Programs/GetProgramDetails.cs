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
public record GetProgramDetailsQuery(long ProgramId) : IQuery<ProgramDetails>;

public class GetProgram : IQueryHandler<GetProgramDetailsQuery, ProgramDetails>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetProgram(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<ProgramDetails> HandleAsync(GetProgramDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await _dbConnection.ReadAsync<ProgramDetails>(_sqlProvider.GetProgramDetails, new { id = query.ProgramId })).Single();
    }
}


public record ProgramDetails(long Id, string Name, string Description, long LocationId, long? ActiveScheduleId);