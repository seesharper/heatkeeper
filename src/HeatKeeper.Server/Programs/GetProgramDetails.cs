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

public record ProgramDetails(long Id, string Name, string Description, long LocationId, long? ActiveScheduleId);

public class GetProgram(IDbConnection dbConnection, ISqlProvider sqlProvider) : IQueryHandler<GetProgramDetailsQuery, ProgramDetails>
{
    public async Task<ProgramDetails> HandleAsync(GetProgramDetailsQuery query, CancellationToken cancellationToken = default)
        => (await dbConnection.ReadAsync<ProgramDetails>(sqlProvider.GetProgramDetails, new { id = query.ProgramId })).Single();
}