namespace HeatKeeper.Server.VATRates;

[RequireAdminRole]
[Patch("api/vat-rates/{Id}")]
public record PatchVATRateCommand(long Id, string Name, decimal Rate) : PatchCommand;

public class PatchVATRate(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<PatchVATRateCommand>
{
    public async Task HandleAsync(PatchVATRateCommand command, CancellationToken cancellationToken = default)
        => await dbConnection.ExecuteAsync(sqlProvider.UpdateVATRate, command);
}