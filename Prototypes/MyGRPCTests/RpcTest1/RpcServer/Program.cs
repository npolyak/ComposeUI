using Grpc.Core;
using Helloworld;

namespace RpcServer;

class ServicesImpl : Services.ServicesBase
{
    public override Task<PrintReply> Print(PrintRequest request, ServerCallContext context)
    {
        Console.WriteLine(request.Name);
        return Task.FromResult(new PrintReply { Message = "Hello " + request.Name });
    }
}

public class Program
{
    const int Port = 30051;

    public static void Main(string[] args)
    {
        Server server = new Server
        {
            Services = { Services.BindService(new ServicesImpl()) }
        };

        server.Ports.Add(new ServerPort("localhost", Port, ServerCredentials.Insecure));

        server.Start();

        Console.ReadLine();

        server.ShutdownAsync().Wait();
    }
}