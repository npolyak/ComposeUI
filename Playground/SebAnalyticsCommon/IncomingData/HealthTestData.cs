using System;
using System.Xml.Serialization;

namespace ConsoleTester.IncomingData
{
    public class HealthTestData
    {
        [XmlAttribute(AttributeName = "testName")]
        public string TestName { get; set; }

        [XmlAttribute(AttributeName = "testType")]
        public string TestType { get; set; }

        [XmlAttribute(AttributeName = "testdesc")]
        public string TestDesc { get; set; }

        [XmlAttribute(AttributeName = "lastRunAt")]
        public DateTime LastRunTime { get; set; }

        [XmlAttribute(AttributeName = "errorCode")]
        public HealthStatus ErrorCode { get; set; } = HealthStatus.Undefined;

        [XmlAttribute(AttributeName = "isRunning")]
        public bool IsRunning { get; set; }

        [XmlText]
        public string ErrorMsg { get; set; }

        public override string ToString()
        {
            return $"Test: {TestName}\nType: {TestType}\nDescription: {TestDesc}\nRuntime: {LastRunTime}\nStatus: {ErrorCode}\nis Running: {IsRunning}";
        }
    }
}
