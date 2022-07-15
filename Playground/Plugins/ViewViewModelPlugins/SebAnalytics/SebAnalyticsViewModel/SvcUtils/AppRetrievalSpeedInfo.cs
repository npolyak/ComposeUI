using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sebastion.Core.SvcUtils
{
    public class AppRetrievalSpeedInfo : AppAccessInfo
    {
        #region Status Property
        private AppStatus _status = AppStatus.Unavailable;
        [XmlAttribute]
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


        #region LastTimeUpdated Property
        private DateTime _lastTimeUpdated;
        [XmlAttribute]
        public DateTime LastTimeUpdated
        {
            get
            {
                return this._lastTimeUpdated;
            }
            set
            {
                if (this._lastTimeUpdated == value)
                {
                    return;
                }

                this._lastTimeUpdated = value;
                this.OnPropertyChanged(nameof(LastTimeUpdated));
            }
        }
        #endregion LastTimeUpdated Property


        #region EnvName Property
        private string _envName;
        public string EnvName
        {
            get
            {
                return this._envName;
            }
            set
            {
                if (this._envName == value)
                {
                    return;
                }

                this._envName = value;
                this.OnPropertyChanged(nameof(EnvName));
            }
        }
        #endregion EnvName Property


        #region RetrievalTime Property
        private TimeSpan _retrievalTime;
        [XmlAttribute]
        public TimeSpan RetrievalTime
        {
            get
            {
                return this._retrievalTime;
            }
            set
            {
                if (this._retrievalTime == value)
                {
                    return;
                }

                this._retrievalTime = value;
                this.OnPropertyChanged(nameof(RetrievalTime));
            }
        }
        #endregion RetrievalTime Property


        #region ExcludeFromRetrieval Property
        private bool _excludeFromRetrieval = false;
        [XmlAttribute]
        public bool ExcludeFromRetrieval
        {
            get
            {
                return this._excludeFromRetrieval;
            }
            set
            {
                if (this._excludeFromRetrieval == value)
                {
                    return;
                }

                this._excludeFromRetrieval = value;
                this.OnPropertyChanged(nameof(ExcludeFromRetrieval));
            }
        }
        #endregion ExcludeFromRetrieval Property

        public override string ToString()
        {
            return $"{base.ToString()}\nStatus: {Status}\nLastTimeUpdated: {LastTimeUpdated}\nEnvName: {EnvName}\nRetrievalTime: {RetrievalTime}\nExcludeFromRetrieval: {ExcludeFromRetrieval}";
        }

        public bool Matches(AppAccessInfo appAccessInfo)
        {
            return ((AppAccessInfo)this).Equals(appAccessInfo);
        }

        public void CopyAccessInfoFrom(AppAccessInfo appAccessInfo)
        {
            base.CopyFromAccessInfo(appAccessInfo);
        }

        public void Update(AppStatus status, DateTime lastTimeUpdated, TimeSpan retrievalTime)
        {
            this.Status = status;
            this.LastTimeUpdated = lastTimeUpdated;
            RetrievalTime = retrievalTime;
        }

        public void CopyChangingData(AppRetrievalSpeedInfo appRetrievalSpeedInfo)
        {
            this.RetrievalTime = appRetrievalSpeedInfo.RetrievalTime;
            this.Status = appRetrievalSpeedInfo.Status;
            this.LastTimeUpdated = appRetrievalSpeedInfo.LastTimeUpdated;
        }
    }
}
