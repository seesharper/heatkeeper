namespace HeatKeeper.Server.VATRates;

[RequireAdminRole]
[Post("api/vat-rates")]
public record PostVATRateCommand(string Name, decimal Rate) : PostCommand;

public class PostVATRate(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PostVATRateCommand>
{
    public async Task HandleAsync(PostVATRateCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.InsertVATRate, command);
}