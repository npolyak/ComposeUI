/// ********************************************************************************************************
///
/// Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License").
/// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
/// See the NOTICE file distributed with this work for additional information regarding copyright ownership.
/// Unless required by applicable law or agreed to in writing, software distributed under the License
/// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and limitations under the License.
/// 
/// ********************************************************************************************************
namespace MorganStanley.ComposeUI.Tryouts.Core.Abstractions.Modules;

public sealed class LifecycleEvent
{
    private LifecycleEvent
    (
        ProcessInfo processInfo, 
        string eventType, 
        bool isExpected,
        Guid instanceId)
    {
        ProcessInfo = processInfo;
        EventType = eventType;
        IsExpected = isExpected;
        InstanceId = instanceId;
    }

    public Guid InstanceId { get; }

    public ProcessInfo ProcessInfo { get; }
    public string EventType { get; }
    public bool IsExpected { get; }

    public static LifecycleEvent Stopped(ProcessInfo processInfo, Guid instanceId, bool expected = true) => new LifecycleEvent(processInfo: processInfo, eventType: LifecycleEventType.Stopped, isExpected: expected, instanceId: instanceId);
    public static LifecycleEvent Started(ProcessInfo processInfo, Guid instanceId) => new LifecycleEvent(processInfo: processInfo, eventType: LifecycleEventType.Started, isExpected: true, instanceId: instanceId);
    public static LifecycleEvent StoppingCanceled(ProcessInfo processInfo, Guid instanceId, bool expected) => new LifecycleEvent(processInfo: processInfo, eventType: LifecycleEventType.StoppingCanceled, isExpected: expected, instanceId: instanceId);
    public static LifecycleEvent FailedToStart(ProcessInfo processInfo, Guid instanceId) => new LifecycleEvent(processInfo: processInfo, eventType: LifecycleEventType.FailedToStart, isExpected: false, instanceId: instanceId);
}

public static class LifecycleEventType
{
    public const string Stopped = "stopped";
    public const string Started = "started";
    public const string StoppingCanceled = "stoppingCanceled";
    public const string FailedToStart = "failedToStart";
}