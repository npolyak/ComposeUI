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

using Avalonia.Threading;
using MorganStanley.ComposeUI.Common.Interfaces;
using NP.Utilities.Attributes;


namespace MorganStanley.ComposeUI.Common.VisualUtils
{
    /// <summary>
    /// 
    /// </summary>
    [Implements(typeof(ISyncContext), isSingleton:true)]
    public class DispatcherAdapter : ISyncContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckAccess()
        {
            return Dispatcher.UIThread.CheckAccess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="priority"></param>
        public void Post(Action action, SyncPriority priority = SyncPriority.Normal)
        {
            if (!CheckAccess())
            {
                Dispatcher.UIThread.Post(action, (DispatcherPriority) priority);
            }
            else
            {
                action?.Invoke();
            }
        }
    }
}
