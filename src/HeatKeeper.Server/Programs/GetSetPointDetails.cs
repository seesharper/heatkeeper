using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[RequireAdminRole]
public record GetSetPointDetailsQuery(long SetPointId) : IQuery<SetPointDetails>;

public record SetPointDetails(long Id, double Value, double Hysteresis, string ZoneName, string ScheduleName);


public class GetSetPointDetails : IQueryHandler<GetSetPointDetailsQuery, SetPointDetails>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public GetSetPointDetails(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<SetPointDetails> HandleAsync(GetSetPointDetailsQuery query, CancellationToken cancellationToken = default)
    {
        return (await _dbConnection.ReadAsync<SetPointDetails>(_sqlProvider.GetSetPointDetails, new { id = query.SetPointId })).Single();
    }
}
