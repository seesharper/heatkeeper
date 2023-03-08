using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server;

[RequireUserRole]
public record LastInsertedRowIdQuery() : IQuery<long>;

public class LastInsertedRowIdQueryHandler : IQueryHandler<LastInsertedRowIdQuery, long>
{
    private readonly IDbConnection _dbConnection;
    private readonly ISqlProvider _sqlProvider;

    public LastInsertedRowIdQueryHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
    {
        _dbConnection = dbConnection;
        _sqlProvider = sqlProvider;
    }

    public async Task<long> HandleAsync(LastInsertedRowIdQuery query, CancellationToken cancellationToken = default)
        => await _dbConnection.ExecuteScalarAsync<long>(_sqlProvider.GetLastInsertedRowId);
}
