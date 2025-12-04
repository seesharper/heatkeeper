using System.ComponentModel;

namespace HeatKeeper.Server.Heaters.Api;

[RequireUserRole]
[Get("api/heaters/disabled-reasons")]
public record HeaterDisabledReasonsQuery : IQuery<HeaterDisabledReasonInfo[]>;

public class GetHeaterDisabledReasons : IQueryHandler<HeaterDisabledReasonsQuery, HeaterDisabledReasonInfo[]>
{
    public Task<HeaterDisabledReasonInfo[]> HandleAsync(HeaterDisabledReasonsQuery query, CancellationToken cancellationToken = default)
    {
        var reasons = Enum.GetValues<HeaterDisabledReason>()
            .Select(reason => new HeaterDisabledReasonInfo(
                (int)reason,
                reason.GetType()
                    .GetField(reason.ToString())!
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .FirstOrDefault()?.Description ?? reason.ToString()))
            .ToArray();

        return Task.FromResult(reasons);
    }
}

public record HeaterDisabledReasonInfo(int Id, string Name);
