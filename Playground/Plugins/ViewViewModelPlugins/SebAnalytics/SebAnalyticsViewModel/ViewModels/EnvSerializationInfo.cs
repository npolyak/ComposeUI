using System.Xml.Serialization;

namespace Sebastion.Core.ViewModels
{
    public class EnvSerializationInfo
    {
        [XmlAttribute]
        public Env EnvKind { get; set; }

        [XmlAttribute]
        public bool IsActive { get; set; }

        [XmlAttribute]
        public string AppsAccessInfoFilePath { get; set; }
    }
}
