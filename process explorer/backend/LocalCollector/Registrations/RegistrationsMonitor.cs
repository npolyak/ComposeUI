﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

namespace ProcessExplorer.Entities.Registrations
{
    public class RegistrationDto 
    {
        #region Properties
        public string? ImplementationType { get; set; }
        public string? LifeTime { get; set; }
        public string? ServiceType { get; set; }
        #endregion
        public static RegistrationDto FromProperties(string type, string serviceType, string lifeTime)
        {
            return new RegistrationDto()
            {
                ImplementationType = type,
                LifeTime = lifeTime,
                ServiceType = type
            };
        }
    }
    public class RegistrationMonitorDto
    {
        public SynchronizedCollection<RegistrationDto>? Services { get; set; } = new SynchronizedCollection<RegistrationDto>();

        public static RegistrationMonitorDto FromCollection(ICollection<RegistrationDto> services)
        {
            var monitor = new RegistrationMonitorDto();
            foreach(var item in services)
            {
                monitor.Services.Add(item);
            }
            return monitor;
        }
    }
}
