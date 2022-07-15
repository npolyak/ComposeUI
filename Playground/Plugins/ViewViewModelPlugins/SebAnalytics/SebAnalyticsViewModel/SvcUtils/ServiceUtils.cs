using ConsoleTester.IncomingData;
using MorganStanley.Castle.Core;
using MorganStanley.Castle.Runtime;
using MorganStanley.Castle.Wcf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sebastion.Core.SvcUtils
{
    using static Constants;

    public static class ServiceUtils
    {
        public static string GetEnvUrl(this Env env) => $"wm-*?#@env={env.ToStr()}";

        public static async Task<IEnumerable<ServiceUri>> GetServices(this Env env) =>
            //(await ServiceUri.ResolveAsync(env.GetEnvUrl())).Where(x => x[RealmKey] == MsadPath).Take(limit);
            await Task.Run(() => ServiceUri.Resolve(env.GetEnvUrl()).Where(x => x[RealmKey] == MsadPath));

        public static async Task<IEnumerable<AppAccessInfo>> GetEnvAppAccessInfos(this Env envKind)
        {
            try
            {
                Log.Info($"Calling to get all services for Env='{envKind}'");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var services = await envKind.GetServices();
                stopwatch.Stop();
                Log.Info($"Done getting all services for Env='{envKind}' after {stopwatch.Elapsed.TotalSeconds} seconds");
                return services.Select(service => new AppAccessInfo(service.FriendlyName, envKind, service["env"], service.ToString())).Distinct();
            }
            catch(Exception exception)
            {
                Log.Error($"Error getting services for Env='{envKind}': {exception.Message}");
                return null;
            }
        }

        public static async Task<HashSet<AppAccessInfo>> GetAppAccessInfosFromEnvs(this IEnumerable<Env> envs)
        {
            IEnumerable<AppAccessInfo>[] completedTasks = await Task.WhenAll(envs.Select(env => env.GetEnvAppAccessInfos()));

            HashSet<AppAccessInfo> result = new HashSet<AppAccessInfo>();

            completedTasks.MixCollectionsRoundRobin(100000, result);

            return result;
        }

    }
}
