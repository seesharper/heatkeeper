namespace HeatKeeper.Server.VATRates;

[RequireAdminRole]
[Delete("api/vat-rates/{Id}")]
public record DeleteVATRateCommand(long Id) : DeleteCommand;

public class DeleteVATRate(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<DeleteVATRateCommand>
{
    public async Task HandleAsync(DeleteVATRateCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.DeleteVATRate, command);
}