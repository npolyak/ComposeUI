using ConsoleTester.IncomingData;
using MorganStanley.Castle.Core;
using MorganStanley.Castle.Runtime;
using Sebastion.Core.SvcUtils;
using NP.Concepts.Behaviors;
using NP.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NP.Utilities.PluginUtils;
using NP.Utilities.Attributes;
using MorganStanley.ComposeUI.Common.Interfaces;

namespace Sebastion.Core.ViewModels
{
    [Implements(typeof(IPlugin), partKey:"SebMainViewModel", isSingleton:true)]
    public class EnvAppInfosViewModel : VMBase, IAppInfoContainer, IPlugin
    {
        public Env EnvKind { get; }

        private ISyncContext _syncContext;

        private readonly IDisposable _envBehaviorDisposable;

        private List<AppAccessInfo> _appAccessList = null;

        #region IsActive Property
        private bool _isActive = false;
        public bool IsActive
        {
            get
            {
                return this._isActive;
            }
            set
            {
                if (this._isActive == value)
                {
                    return;
                }

                this._isActive = value;
                this.OnPropertyChanged(nameof(IsActive));
            }
        }
        #endregion IsActive Property

        public int Limit { get; set; }


        #region BoundByTheLimit Property
        private bool _boundByTheLimit;
        public bool BoundByTheLimit
        {
            get
            {
                return this._boundByTheLimit;
            }
            set
            {
                if (this._boundByTheLimit == value)
                {
                    return;
                }

                this._boundByTheLimit = value;
                this.OnPropertyChanged(nameof(BoundByTheLimit));
            }
        }
        #endregion BoundByTheLimit Pro Property


        public ObservableCollection<AppInfo> AppInfos { get; } =
            new ObservableCollection<AppInfo>();

        public ObservableCollection<AppInfo> Errors { get; } =
            new ObservableCollection<AppInfo>();

        public ObservableCollection<AppRetrievalSpeedInfo> AppRetrievalSpeedInfos { get; }

        public int NumberAppInfos => AppInfos.Count;

        public int NumberOkItems => AppInfos.Count(item => item.Status == AppStatus.Good);

        public int NumberErrorItems => AppInfos.Count(item => item.Status == AppStatus.Error);

        public int NumberUnavailableItems => AppInfos.Count(item => item.Status == AppStatus.Unavailable);

        public double Ratio => NumberAppInfos == 0 ? 0 : ((double)NumberOkItems) / ((double)NumberAppInfos);

        public bool CanCancel =>
            !IsCancelled && IsRefreshing;


        #region IsCancelled Property
        private bool _isCanceled;
        public bool IsCancelled
        {
            get
            {
                return this._isCanceled;
            }
            set
            {
                if (this._isCanceled == value)
                {
                    return;
                }

                this._isCanceled = value;
                this.OnPropertyChanged(nameof(IsCancelled));
                this.OnPropertyChanged(nameof(CanCancel));
            }
        }
        #endregion IsCancelled Property


        public void CancelRefreshProcess()
        {
            IsCancelled = true;
        }

        #region DoneLoadingAccessInfo Property
        private bool _doneLoadingAccessInfo = false;
        public bool DoneLoadingAccessInfo
        {
            get
            {
                return this._doneLoadingAccessInfo;
            }
            private set
            {
                if (this._doneLoadingAccessInfo == value)
                {
                    return;
                }

                this._doneLoadingAccessInfo = value;
                this.OnPropertyChanged(nameof(DoneLoadingAccessInfo));
            }
        }
        #endregion DoneLoadingAccessInfo Property

        public string UrlExclusionListFileName => $"URLExclusionList.{EnvKind}.xml";

        public string FullRetrievalInfoFileName => $"FullRetrievalInfo.{EnvKind}.xml";

        public string AppInfosFileName => $"AppInfos.{EnvKind}.xml";


        #region AppsAccessInfoFilePath Property
        private string _appsAccessInfoFilePath;
        public string AppsAccessInfoFilePath
        {
            get
            {
                return this._appsAccessInfoFilePath;
            }
            set
            {
                if (this._appsAccessInfoFilePath == value)
                {
                    return;
                }

                this._appsAccessInfoFilePath = value;
                this.OnPropertyChanged(nameof(AppsAccessInfoFilePath));
            }
        }
        #endregion AppsAccessInfoFilePath Property


        public string AppsAccessInfoFileNameToSaveTo => $"Apps.AccessInfo.{EnvKind}.xml";

        public EnvAppInfosViewModel(Env envKind, ISyncContext syncContext)
        {
            this.EnvKind = envKind;
            _syncContext = syncContext;

            _envBehaviorDisposable =
                AppInfos.AddBehavior(OnAppInfoItemAdded, OnAppInfoItemRemoved);

            AppRetrievalSpeedInfos =
                File.Exists(UrlExclusionListFileName) ?
                    XmlSerializationUtils.DeserializeFromFile<ObservableCollection<AppRetrievalSpeedInfo>>(UrlExclusionListFileName, true) :
                    new ObservableCollection<AppRetrievalSpeedInfo>();
        }


        [CompositeConstructor]
        public EnvAppInfosViewModel([Part]ISyncContext syncContext) : this(Env.Dev, syncContext)
        {
            this.AppsAccessInfoFilePath =
                Directory.GetParent(this.GetType().Assembly.Location) + @"\DevAccessInfo.xml";
            this.Limit = 100;
        }

        public void SaveAccessList()
        {
            XmlSerializationUtils.SerializeToFile(_appAccessList, AppsAccessInfoFileNameToSaveTo);
        }

        public void ResetAccessList()
        {
            this.AppsAccessInfoFilePath = null;

            _appAccessList = null;

            BoundByTheLimit = false;
        }

        public void SaveUrlExclusionList()
        {
            var itemsToSave = AppRetrievalSpeedInfos.Where(item => item.ExcludeFromRetrieval).ToList();

            if (itemsToSave.IsNullOrEmpty())
            {
                if (File.Exists(UrlExclusionListFileName))
                {
                    File.Delete(UrlExclusionListFileName);
                }
            }
            else
            {
                itemsToSave.SerializeToFile(UrlExclusionListFileName);
            }
        }

        public void SaveFullRetrievalInfosToFile()
        {
            XmlSerializationUtils.SerializeToFile(AppRetrievalSpeedInfos, FullRetrievalInfoFileName);
        }

        public void SaveAppInfosToFile()
        {
            XmlSerializationUtils.SerializeToFile(AppInfos, AppInfosFileName);
        }

        private void OnAppInfoItemAdded(AppInfo item)
        {
            item.PropertyChanged += Item_PropertyChanged;

            if (item.Status == AppStatus.Error)
            {
                Errors.Add(item);
            }

            FirePropsChanged();
        }


        private void OnAppInfoItemRemoved(AppInfo item)
        {
            FirePropsChanged();

            if (item.Status == AppStatus.Error)
            {
                Errors.Remove(item);
            }

            item.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppInfo.Status))
            {
                FirePropsChanged();

                AppInfo appInfo = (AppInfo)sender;

                if (appInfo.Status == AppStatus.Error)
                {
                    Errors.Add(appInfo);
                }
                else
                {
                    Errors.Remove(appInfo);
                }
            }
        }

        public async Task SetAppAccessListIfNeeded()
        {
            if (!_appAccessList.IsNullOrEmpty())
            {
                return;
            }

            BoundByTheLimit = false;

            if (File.Exists(AppsAccessInfoFilePath))
            {
                _appAccessList =
                    XmlSerializationUtils.DeserializeFromFile<List<AppAccessInfo>>(AppsAccessInfoFilePath);
            }

            if (_appAccessList.IsNullOrEmpty())
            {
                _appAccessList = (await EnvKind.GetEnvAppAccessInfos()).ToList();
            }

            if (_appAccessList.Count > Limit)
            {
                _appAccessList = _appAccessList.Take(Limit).ToList();
                BoundByTheLimit = true;
            }

            DoneLoadingAccessInfo = true;
        }

        private void FirePropsChanged()
        {
            OnPropertyChanged(nameof(NumberAppInfos));
            OnPropertyChanged(nameof(Ratio));
            OnPropertyChanged(nameof(NumberOkItems));
            OnPropertyChanged(nameof(NumberErrorItems));
            OnPropertyChanged(nameof(NumberUnavailableItems));
        }

        private void AddOrUpdateItem
        (
            AppInfo appInfo,
            AppAccessInfo accessInfo,
            AppStatus? appStatus,
            TimeSpan retrievalInterval)
        {
            AppRetrievalSpeedInfo appRetrievalSpeedInfo = new AppRetrievalSpeedInfo();
            appRetrievalSpeedInfo.CopyAccessInfoFrom(accessInfo);
            DateTime lastUpdatedDateTime = DateTime.Now;

            if (appInfo == null)
            {
                if (appStatus == null)
                {
                    appStatus = AppStatus.Unavailable;
                }
            }
            else
            {
                lastUpdatedDateTime = appInfo.UpdateTime;
                if (appInfo.BasicData?.Name == "MSWM-Host")
                {
                    appStatus = AppStatus.MswmHost;
                }
                else
                {
                    appStatus = appInfo.Status;
                    AppInfo existingItem =
                        AppInfos.FirstOrDefault(existingAppInfo => existingAppInfo.FullPath.Equals(appInfo.FullPath));

                    if ( (existingItem?.Host != null) && 
                         (appInfo.Host != null) && 
                         (existingItem.Host != appInfo.Host)) 
                    {
                        // this case where the URLs are the same, but the
                        // hosts are different is an indication of a load balancing.
                        // Such case should produce a new row since the behavior and performance of 
                        // two different nodes within a load balancer can be different
                        existingItem = null;
                    }

                    if (existingItem != null)
                    {
                        existingItem.CopyFrom(appInfo);
                    }
                    else
                    {
                        AppInfos.Add(appInfo);
                        OnPropertyChanged(nameof(NumberAppInfos));
                    }
                }
            }

            appRetrievalSpeedInfo.Update(appStatus.Value, lastUpdatedDateTime, retrievalInterval);

            appRetrievalSpeedInfo.EnvName = appInfo?.DeploymentEnv ?? accessInfo.EnvKind.ToString();

            AppRetrievalSpeedInfo matchingInfo =
                AppRetrievalSpeedInfos.FirstOrDefault(existingItem => existingItem.Matches(appRetrievalSpeedInfo));

            if (matchingInfo != null)
            {
                matchingInfo.CopyChangingData(appRetrievalSpeedInfo);
            }
            else
            {
                AppRetrievalSpeedInfos.Add(appRetrievalSpeedInfo);
            }
        }


        public async Task<AppInfo> LoadAppInfo(AppAccessInfo appAccessInfo)
        {
            if (!IsActive)
            {
                return null;
            }

            string appUri = appAccessInfo.FullPath;


            AppRetrievalSpeedInfo retrievalSpeedInfo =
                AppRetrievalSpeedInfos.FirstOrDefault(existingItem => existingItem.Matches(appAccessInfo));

            // if excluded, do not try to retrieve.
            if (retrievalSpeedInfo?.ExcludeFromRetrieval == true)
            {
                return null;
            }

            AppInfo info = null;

            Basic basic = null;
            Health health = null;
            ErrorCheck error = null;

            AppStatus? appStatus = null;
            SvcUtils.Timer retrievalTimer = new SvcUtils.Timer();

            if (IsCancelled)
            {
                return null;
            }

            try
            {
                basic = await EndpointInfoGetter.GetBasicInfo(appUri);
                health = await EndpointInfoGetter.GetHealthInfo(appUri);
                error = await EndpointInfoGetter.GetErrorInfo(appUri);

                info = new AppInfo(basic, health, error, appAccessInfo);
                if (info.AllNull)
                {
                    info = null;
                }
            }
            catch (Exception exception)
            {
                if (exception is HttpRequestException httpException)
                {
                    info = new AppInfo(basic, health, error, appAccessInfo);
                    Log.Error($"Cannot get Application Info for Url='{appUri}'. Error Message: '{httpException.Message}'.");
                }
                //java apps that have fsns, shouldn't be monitered
                else if (exception.GetType() == typeof(InvalidOperationException) ||
                         exception.GetType() == typeof(Win32Exception))
                {
                    info = null;

                    appStatus = AppStatus.NonCastle;
                }

                Log.Error($"Cannot get Application Info for Url='{appUri}'. Error Message: '{exception.Message}'.");
            }

            retrievalTimer.SetInterval();
            
            _syncContext.Post(() => AddOrUpdateItem(info, appAccessInfo, appStatus, retrievalTimer.Interval), SyncPriority.Input);
            return info;
        }


        public async Task RefreshData()
        {
            if ( (!IsActive) || IsRefreshing)
                return;

            IsRefreshing = true;

            await SetAppAccessListIfNeeded();

            int count = _appAccessList.Select(ai => ai.FullPath).Distinct().Count();

            await LoadAppInfosFromAccessInfos(_appAccessList);
        }

        public void LoadAppInfosDataFromFile()
        {
            if (!File.Exists(AppInfosFileName))
            {
                return;
            }

            var appInfos = XmlSerializationUtils.DeserializeFromFile<List<AppInfo>>(AppInfosFileName, true);

            appInfos.DoForEach(appInfo => AppInfos.Add(appInfo));
        }

        public EnvSerializationInfo ToSerializationInfo()
        {
            return new EnvSerializationInfo
            {
                EnvKind = this.EnvKind,
                IsActive = this.IsActive,
                AppsAccessInfoFilePath = this.AppsAccessInfoFilePath
            };
        }

        public void SetFromSerializationInfo(EnvSerializationInfo serializationInfo)
        {
            this.IsActive = serializationInfo.IsActive;
            this.AppsAccessInfoFilePath = serializationInfo.AppsAccessInfoFilePath;
        }

        public void LoadFullRetrievalInfosDataFromFile()
        {
            if (!File.Exists(FullRetrievalInfoFileName))
            {
                return;
            }

            var appInfos = XmlSerializationUtils.DeserializeFromFile<List<AppRetrievalSpeedInfo>>(FullRetrievalInfoFileName, true);

            appInfos.DoForEach(appInfo => AppRetrievalSpeedInfos.Add(appInfo));
        }


        #region IsRefreshing Property
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get
            {
                return this._isRefreshing;
            }
            private set
            {
                if (this._isRefreshing == value)
                {
                    return;
                }

                this._isRefreshing = value;
                this.OnPropertyChanged(nameof(IsRefreshing));
                this.OnPropertyChanged(nameof(CanCancel));
            }
        }
        #endregion IsRefreshing Property

        private class FullPathComparer : IEqualityComparer<AppAccessInfo>
        {
            public static FullPathComparer Instance { get; } = new FullPathComparer();

            public bool Equals(AppAccessInfo x, AppAccessInfo y)
            {
                return x?.FullPath == y?.FullPath;
            }

            public int GetHashCode([DisallowNull] AppAccessInfo obj)
            {
                return obj.FullPath.GetHashCode();
            }
        }

        public async Task LoadAppInfosFromAccessInfos(IEnumerable<AppAccessInfo> appAccessInfos)
        {
            Log.Info($"Getting application infos for Env='{EnvKind}'");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ThreadPriority oldPriority = Thread.CurrentThread.Priority; 
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            try
            {
                int ct = appAccessInfos.Select(i => i.FullPath).Distinct().Count();

                var groupedAppAccessInfos = 
                    appAccessInfos.GroupBy(item => item.FullPath)
                                  .Select(g => g.FirstOrDefault())
                                  .Where(g => g != null).ToList();

                AppInfo[] result =
                    await groupedAppAccessInfos.Where(accessInfo => !IsCancelled).Select(accessInfo => this.LoadAppInfo(accessInfo)).WhenAll(maxConcurrent: 20);

            }
            catch(Exception exception)
            {
                Log.Error($"Got exception while getting application infos for Env='{EnvKind}': '{exception.Message}'");

                return;
            }
            finally
            {
                IsRefreshing = false;
                stopwatch.Stop();
                Log.Info($"Got all application infos for Env='{EnvKind}', time taken is {stopwatch.Elapsed.TotalSeconds} seconds");

                IsCancelled = false;
                Thread.CurrentThread.Priority = oldPriority;
            }
        }


        #region CurrentlySelectedAppInfo Property
        private AppInfo _currentlySelectedAppInfo;
        public AppInfo CurrentlySelectedAppInfo
        {
            get
            {
                return this._currentlySelectedAppInfo;
            }
            private set
            {
                if (this._currentlySelectedAppInfo == value)
                {
                    return;
                }

                this._currentlySelectedAppInfo = value;
                this.OnPropertyChanged(nameof(CurrentlySelectedAppInfo));
            }
        }
        #endregion CurrentlySelectedAppInfo Property

        public void SelectAppInfo(object selectedItem)
        {
            CurrentlySelectedAppInfo = selectedItem as AppInfo;
        }
    }
}
