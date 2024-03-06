// using System.Threading;
// using System.Threading.Tasks;
// using CQRS.Command.Abstractions;
// using HeatKeeper.Server.Authorization;
// using HeatKeeper.Server.Mqtt;

// namespace HeatKeeper.Server.Programs;

// [RequireBackgroundRole]
// public record TasmotaCommand(string Topic, string PayLoad);

// public class TasmotaCommandHandler : ICommandHandler<TasmotaCommand>
// {
//     private readonly ITasmotaClient _tasmotaClient;

//     public TasmotaCommandHandler(ITasmotaClient tasmotaClient)
//         => _tasmotaClient = tasmotaClient;

//     public async Task HandleAsync(TasmotaCommand command, CancellationToken cancellationToken = default)
//         => await _tasmotaClient.PublishCommand(command.Topic, command.PayLoad);
// }