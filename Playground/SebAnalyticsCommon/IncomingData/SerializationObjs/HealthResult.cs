using ConsoleTester.IncomingData;
using System.Xml.Serialization;

namespace Sebastion.Core.IncomingData.SerializationObjs
{
    [XmlRoot(ElementName = "result")]
    public class HealthResult
    {
        [XmlElement("healthChecks")]
        public Health TheHealth { get; set; }
    }
}
