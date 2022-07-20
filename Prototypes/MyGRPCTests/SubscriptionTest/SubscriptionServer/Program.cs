using CommonLib;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Subscriptions;
using System.Collections.Concurrent;

namespace SubscriptionsServer;

public class SubscriptionsServiceImpl : SubscriptionsService.SubscriptionsServiceBase
{
    private ConcurrentDictionary<string, SubscriptionData> _topicsDictionary =
        new ConcurrentDictionary<string, SubscriptionData>();


    public SubscriptionsServiceImpl(params (string Topic, System.Type Type)[] topicToTypeMaps)
    {
        foreach(var topicAndType in topicToTypeMaps)
        {
            _topicsDictionary[topicAndType.Topic] = 
                new SubscriptionData(topicAndType.Topic, topicAndType.Type);
        }
    }

    private (List<SingleSubscription>, SingleSubscription) FindSubscription(string topic, string pluginId)
    {

        SubscriptionData topicSubscriptions;
        if (!_topicsDictionary.TryGetValue(topic, out topicSubscriptions!))
        {
            // exception or errror - unexisting topic has been requested by the client
        }

        var existingSubscription =
            topicSubscriptions.TopicsSubscriptions.FirstOrDefault(topicSubscription => topicSubscription.PluginId == pluginId);

        return (topicSubscriptions.TopicsSubscriptions, existingSubscription!);
    }

    public override async Task Subscribe
    (
        SubscriptionRequest request,
        IServerStreamWriter<ReturnedSubscriptionItem> responseStream,
        ServerCallContext context)
    {
        string topic = request.Topic;
        string pluginId = request.PluginId;

        (List<SingleSubscription> topicSubscriptions, SingleSubscription existingSubscription) = 
            FindSubscription(topic, pluginId);  

        if (existingSubscription != null)
        {
            // exception or error - there should be only one subscription per plugin
        }

        SingleSubscription singleSubscription = 
            new SingleSubscription(topic, pluginId, context.CancellationToken);

        topicSubscriptions.Add(singleSubscription);

        while (!context.CancellationToken.IsCancellationRequested)
        {
            Any message = 
                singleSubscription.GetMessage();

            ReturnedSubscriptionItem returningSubscriptionItem = new ReturnedSubscriptionItem
            {
                DateTimeStamp = Timestamp.FromDateTime(DateTime.UtcNow),
                Topic = topic,
                Message = message
            };

            await responseStream.WriteAsync(returningSubscriptionItem);
        }

        // cleanup after cancellation
        topicSubscriptions.Remove(singleSubscription);
    }

    public override async Task<PublishReply> Publish(PublishRequest request, ServerCallContext context)
    {
        string topic = request.Topic;

        SubscriptionData? topicSubscriptionData;

        if (!_topicsDictionary.TryGetValue(topic, out topicSubscriptionData))
        {
            // exception or error
        }

        foreach (SingleSubscription singleSubscription in topicSubscriptionData.TopicsSubscriptions)
        {
            singleSubscription.Publish(request.Message);
        }

        return new PublishReply { DateTimeStamp = Timestamp.FromDateTime(DateTime.UtcNow), Topic = topic };
    }

    private class SubscriptionData
    {
        public string Topic { get; }
        public System.Type ReturnedItemType { get; }

        internal List<SingleSubscription> TopicsSubscriptions { get; } = new List<SingleSubscription>();

        public SubscriptionData(string topic, System.Type returnedItemType)
        {
            Topic = topic;
            ReturnedItemType = returnedItemType;

            if (!typeof(IMessage).IsAssignableFrom(returnedItemType))
                throw new Exception("Only messages are allowed as topic item types");
        }
    }

    private class SingleSubscription
    {
        BlockingCollection<Any> _subscriptionMessageQueue = new BlockingCollection<Any>();

        public string Topic { get; }

        public string PluginId { get; }

        public CancellationToken CancellationToken { get; }

        public SingleSubscription(string topic, string pluginId, CancellationToken cancellationToken)
        {
            Topic = topic;
            PluginId = pluginId;
            CancellationToken = cancellationToken;
        }

        public void Publish(Any message)
        {
            _subscriptionMessageQueue.Add(message);
        }

        public Any GetMessage()
        {
            Any message = _subscriptionMessageQueue.Take(CancellationToken);

            return message;
        }
    }

}


public class Program
{ 
    public static void Main(string[] args)
    {
        //TestTopicMessage testTopicMessage = new TestTopicMessage { DateTimeStamp = Timestamp.FromDateTime(DateTime.UtcNow), TheStr = "Hello" };

        //Any msg = Any.Pack(testTopicMessage);

        //TestTopicMessage testTopicMessage1 = msg.Unpack<TestTopicMessage>();

        Server server = new Server
        {
            Services = 
            { 
                SubscriptionsService.BindService
                (
                    new SubscriptionsServiceImpl(("MyFavoriteTopic", typeof(TestTopicMessage)))
                )
            }
        };

        server.Ports.Add(new ServerPort("localhost", Constants.Port, ServerCredentials.Insecure));

        server.Start();

        Console.ReadLine();

        server.ShutdownAsync().Wait();
    }
}