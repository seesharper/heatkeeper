using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Authentication.RefreshTokens;

public class UpdateRefreshToken : ICommandHandler<RefreshToken>
{
    private readonly IDbConnection _connection;
    private readonly ISqlProvider _sqlProvider;

    public UpdateRefreshToken(IDbConnection connection, ISqlProvider sqlProvider)
    {
        _connection = connection;
        _sqlProvider = sqlProvider;
    }

    public async Task HandleAsync(RefreshToken command, CancellationToken cancellationToken = default) =>
        await _connection.ExecuteAsync(_sqlProvider.UpdateRefreshToken, command);
}