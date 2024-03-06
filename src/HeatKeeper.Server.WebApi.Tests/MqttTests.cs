using System.Threading.Tasks;
using CQRS.AspNet.Testing;
using FluentAssertions;
using HeatKeeper.Server.Mqtt;
using Moq;
using Xunit;
namespace HeatKeeper.Server.WebApi.Tests;


public class MqttTests : TestBase
{
    [Fact]
    public async Task ShouldSendMqttMessageWithOnPayLoadThroughTestEndpoint()
    {
        var publishMqttMessageHandlerMock = Factory.MockCommandHandler<PublishMqttMessageCommand>();
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        await client.PublishMqttMessage(TestData.Mqtt.TestPublishMqttMessageCommand(TestData.Mqtt.OnPayload), testLocation.Token);
        publishMqttMessageHandlerMock.VerifyCommandHandler<PublishMqttMessageCommand>(c => c.Payload == TestData.Mqtt.OnPayload, Times.Once());
    }

    [Fact]
    public async Task ShouldSendMqttMessageWithOffPayLoadThroughTestEndpoint()
    {
        var publishMqttMessageHandlerMock = Factory.MockCommandHandler<PublishMqttMessageCommand>();
        var client = Factory.CreateClient();
        var testLocation = await Factory.CreateTestLocation();
        await client.PublishMqttMessage(TestData.Mqtt.TestPublishMqttMessageCommand(TestData.Mqtt.OffPayload), testLocation.Token);
        publishMqttMessageHandlerMock.VerifyCommandHandler<PublishMqttMessageCommand>(c => c.Payload == TestData.Mqtt.OffPayload, Times.Once());
    }
}