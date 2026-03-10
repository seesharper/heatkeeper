using System.ComponentModel;

namespace HeatKeeper.Server.Heaters.Api;

[RequireUserRole]
[Get("api/heaters/states")]
public record HeaterStatesQuery : IQuery<HeaterStateInfo[]>;

public class GetHeaterStates : IQueryHandler<HeaterStatesQuery, HeaterStateInfo[]>
{
    public Task<HeaterStateInfo[]> HandleAsync(HeaterStatesQuery query, CancellationToken cancellationToken = default)
    {
        var states = Enum.GetValues<HeaterState>()
            .Select(state => new HeaterStateInfo(
                (int)state,
                state.GetType()
                    .GetField(state.ToString())!
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .FirstOrDefault()?.Description ?? state.ToString()))
            .ToArray();

        return Task.FromResult(states);
    }
}

public record HeaterStateInfo(int Id, string Name);
