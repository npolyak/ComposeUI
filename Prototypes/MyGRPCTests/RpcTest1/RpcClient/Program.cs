using System;
using Grpc.Core;
using Helloworld;

// See https://aka.ms/new-console-template for more information


Channel channel = new Channel("localhost:30051", ChannelCredentials.Insecure);

var client = new Services.ServicesClient(channel);

string user = "Nick Polyak";

var reply = client.Print(new PrintRequest { Name = user }); 

Console.WriteLine(reply.Message );

channel.ShutdownAsync().Wait(); 