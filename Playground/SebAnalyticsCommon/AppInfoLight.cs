using NP.Utilities;
using NP.Utilities.BasicInterfaces;
using System;

namespace Sebastion.Core.SvcUtils
{
    /// <summary>
    /// minimal number of properties used for snapshots
    /// </summary>
    public class AppInfoLight : 
        AppAccessInfo, 
        ICopyable<AppInfoLight>, 
        ICopyable<AppInfo>
    {
        public AppStatus Status { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Name { get; set; }

        public string Host { get; set; }

        public AppInfoLight()
        {

        }

        public AppInfoLight(AppInfoLight source)
        {
            CopyFrom(source);
        }

        public AppInfoLight(AppInfo source)
        {
            CopyFrom(source);
        }

        public void CopyFrom(AppInfoLight source)
        {
            this.CopyFromAccessInfo(source);

            this.Status = source.Status;
            this.UpdateTime = source.UpdateTime;
            this.Name = source.Name;
            this.Host = source.Host;
        }

        public void CopyFrom(AppInfo source)
        {
            this.CopyFromAccessInfo(source);

            this.Status = source.Status;
            this.UpdateTime = source.UpdateTime;
            this.Name = source.BasicData?.Name;
            this.Host = source.Host;
        }

        public override bool Equals(object obj)
        {
            if (obj is AppInfoLight info)
            {
                return this.FullPath.ObjEquals(info.FullPath) && this.Host == info.Host;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.FullPath?.GetHashCode() ?? 0 ;
        }

        public override string ToString()
        {
            return $"{base.ToString()}\nName: {Name}\nHost: {Host}\nStatus: {Status}\nUpdateTime: {UpdateTime}";
        }
    }
}
