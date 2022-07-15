using Sebastion.Core.IncomingData.SerializationObjs;
using NP.Utilities.BasicInterfaces;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConsoleTester.IncomingData
{
    public enum HealthStatus
    {
        [XmlEnum("200")]
        Okay = 200,
        [XmlEnum("520")]
        SelfCheckFailed = 520,
        [XmlEnum("521")]
        DependencyCheckFailed = 521,
        [XmlEnum("0")]
        Unavailable = 0,
        [XmlEnum("1")]
        Undefined = 1
    }

    [XmlType("healthChecks")]
    public class Health : IPastRestorable
    {
        [XmlAttribute(AttributeName = "status")]
        public HealthStatus Status { get; set; }

        [XmlElement(ElementName = "healthCheck")]
        public List<HealthTestData> HealthTests { get; set; }

        public void AfterRestore()
        {
            HealthTests?.ForEach(hTest =>
            {
                if (!Enum.IsDefined(typeof(HealthStatus), (int) hTest.ErrorCode))
                {
                    hTest.ErrorCode = HealthStatus.Undefined;
                }
            });
        }
    }

}
