using Sebastion.Core.SvcUtils;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Sebastion.Core
{
    public interface IAppInfoContainer
    {
        ObservableCollection<AppInfo> AppInfos { get; }

        private List<AppInfoLight> ToLight()
        {
            return this.AppInfos.Select(appInfo => new AppInfoLight(appInfo)).ToList();
        }

        public void CreateSnapshot(string snapshotFilePath)
        {
            var appInfosToSerialize = ToLight();

            string jsonStr = JsonSerializer.Serialize(appInfosToSerialize);

            File.WriteAllText(snapshotFilePath, jsonStr);
        }

        public List<AppInfoLight> GetChangedEntries(string snapshotFilePath)
        {
            using StreamReader jsonStreamReader =
                new StreamReader(snapshotFilePath);

            string jsonStr = jsonStreamReader.ReadToEnd();

            if (jsonStr == null)
                return null;

            var snapshot = JsonSerializer.Deserialize<List<AppInfoLight>>(jsonStr);

            List<AppInfoLight> current = ToLight();

            List<AppInfoLight> changed =
                current.Where
                        (app => 
                            snapshot.Any
                            (
                                snap => 
                                    (app.FullPath == snap.FullPath) && 
                                    (app.Host == null || snap.Host == null || app.Host == snap.Host) && 
                                    (app.Status != snap.Status))).ToList();

            var matchingSnapshots = snapshot.Where
                (
                    snap => 
                        current.Any
                        (
                            app => 
                                (app.FullPath == snap.FullPath) && 
                                (app.Host == null || snap.Host == null || app.Host == snap.Host)));

            List<AppInfoLight> offLine =
                snapshot.Except(matchingSnapshots).ToList();

            changed.AddRange(offLine);

            return changed;
        }

        bool IsActive { get; set; }

        Task RefreshData();
    }
}
