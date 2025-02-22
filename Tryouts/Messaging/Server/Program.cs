﻿// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

using Microsoft.Extensions.FileProviders;
using MorganStanley.ComposeUI.Tryouts.Messaging.Server.Transport.WebSocket;

namespace MorganStanley.ComposeUI.Tryouts.Messaging.Server;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddMessageRouterServer();

        var app = builder.Build();

        app.UseWebSockets();
        app.UseStaticFiles();
        app.UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        builder.Environment.ContentRootPath,
                        path2: "..",
                        path3: "messaging-web-client",
                        path4: "output")),
                RequestPath = "/messaging-web-client"
            });


        app.MapMessageRouterWebSocketEndpoint("/ws");

        app.Run();
    }
}