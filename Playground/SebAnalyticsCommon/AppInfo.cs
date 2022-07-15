using ConsoleTester.IncomingData;
using NP.Utilities.BasicInterfaces;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sebastion.Core.SvcUtils
{
    public enum AppStatus
    {
        Good,
        Warning,
        Error,
        Unavailable,
        NonCastle,
        MswmHost
    }

    public class AppInfo : AppAccessInfo, ICopyable<AppInfo>
    {
        [XmlElement]

        #region BasicData Property
        private Basic _basicData;
        public Basic BasicData
        {
            get
            {
                return this._basicData;
            }
            set
            {
                if (this._basicData == value)
                {
                    return;
                }

                this._basicData = value;
                this.OnPropertyChanged(nameof(BasicData));

                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(CastleVersion));
                OnPropertyChanged(nameof(DeploymentEnv));
                OnPropertyChanged(nameof(EonId));
            }
        }
        #endregion BasicData Property

        [XmlIgnore]
        public string Name => BasicData?.Name;

        [XmlIgnore]
        public string CastleVersion => BasicData?.CastleVersion;

        [XmlIgnore]
        public string InfraVersion => BasicData?.InfraVersion;

        [XmlIgnore]
        public string DeploymentEnv => BasicData?.DeploymentEnv;

        [XmlIgnore]
        public string EonId => BasicData?.EonId;


        [XmlElement]

        #region HealthData Property
        private Health _healthData;
        public Health HealthData
        {
            get
            {
                return this._healthData;
            }
            set
            {
                if (this._healthData == value)
                {
                    return;
                }

                this._healthData = value;
                this.OnPropertyChanged(nameof(HealthData));
            }
        }
        #endregion HealthData Property


        [XmlElement]
        #region ErrorData Property
        private ErrorCheck _errorData;
        public ErrorCheck ErrorData
        {
            get
            {
                return this._errorData;
            }
            set
            {
                if (this._errorData == value)
                {
                    return;
                }

                this._errorData = value;
                this.OnPropertyChanged(nameof(ErrorData));
            }
        }
        #endregion ErrorData Property


        [XmlAttribute]

        #region Status Property
        private AppStatus _status;
        public AppStatus Status
        {
            get
            {
                return this._status;
            }
            set
            {
                if (this._status == value)
                {
                    return;
                }

                this._status = value;
                this.OnPropertyChanged(nameof(Status));
            }
        }
        #endregion Status Property


        [XmlAttribute]
        #region UpdateTime Property
        private DateTime _updateTime;
        public DateTime UpdateTime
        {
            get
            {
                return this._updateTime;
            }
            set
            {
                if (this._updateTime == value)
                {
                    return;
                }

                this._updateTime = value;
                this.OnPropertyChanged(nameof(UpdateTime));
            }
        }
        #endregion UpdateTime Property

        [XmlIgnore]
        public string Host => this.BasicData?.Host;


        public AppInfo()
        {

        }

        public AppInfo(Basic basic, Health health, ErrorCheck error, AppAccessInfo appAccessInfo) : this(appAccessInfo)
        {
            CopyFrom(basic, health, error, appAccessInfo);
        }

        public AppInfo(AppAccessInfo appAccessInfo) : base(appAccessInfo)
        {

        }

        public bool AllNull => BasicData == null && HealthData == null && ErrorData == null;

        public override bool Equals(object obj)
        {
            if (obj is not AppInfo target)
            {
                return false;
            }

            return base.Equals(target) && (this.Host == target?.Host);
            //return this.BasicData?.Name == target.BasicData?.Name &&
            //       this.BasicData?.DeploymentEnv == target.BasicData?.DeploymentEnv;
        }

        public void VisitPage()
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = this.FullPath + "?deploy-check",
                UseShellExecute = true
            });
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BasicData?.Name, BasicData?.DeploymentEnv);
        }

        private void CopyFromAppAccessInfo(IAppAccessInfo appAccessInfo)
        {
            this.CopyFromAccessInfo(appAccessInfo);
        }

        private void CopyFrom(Basic basic, Health health, ErrorCheck error, AppAccessInfo appAccessInfo)
        {
            CopyFromAppAccessInfo(appAccessInfo);

            this.BasicData = basic;
            this.HealthData = health;
            this.ErrorData = error;

            if (HealthData != null)
            {
                Status = HealthData.Status == HealthStatus.Okay ? AppStatus.Good : AppStatus.Error;
            }
            else
            {
                Status = AppStatus.Unavailable;
            }

            // --TODO-- change to UtcNow
            this.UpdateTime = DateTime.Now;
        }

        public void CopyFrom(AppInfo source)
        {
            this.CopyFrom(source.BasicData, source.HealthData, source.ErrorData, source);

            this.UpdateTime = source.UpdateTime;

            OnPropertyChanged(null);
        }

        public override string ToString()
        {
            string result = $"{base.ToString()}\nName: {Name}\nStatus: {Status}\nCastleVersion: {CastleVersion}\nInfraVersion: {InfraVersion}\nEonId: {EonId}\nHost: {Host}\nStartTime: {BasicData?.StartTime}\nUpdateTime: {UpdateTime}";

            return result;
        }
    }
}
