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


        using var replies = 
            client.Subscribe(new SubscriptionRequest { PluginId = "1", Topic = "MyFavoriteTopic" }, cancellationToken:cts.Token);
        while( await replies.ResponseStream.MoveNext())
        {
            var msg = replies.ResponseStream.Current;

            TestTopicMessage message = msg.Message.Unpack<TestTopicMessage>();


            Console.WriteLine(message.TheStr);
        }

        Console.ReadLine();
    }
}