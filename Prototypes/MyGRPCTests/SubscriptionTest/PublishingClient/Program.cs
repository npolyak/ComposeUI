using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Subscriptions;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Channel channel =
            new Channel($"localhost:30051", ChannelCredentials.Insecure);

        var cts = new CancellationTokenSource();

        var client = new SubscriptionsService.SubscriptionsServiceClient(channel);
        PublishRequest publishingRequest = new PublishRequest { Topic = "MyFavoriteTopic", Message = Any.Pack(new TestTopicMessage { TheStr = "Bla Bla" }) };
        client.Publish(publishingRequest);

        Console.ReadLine();
    }
}