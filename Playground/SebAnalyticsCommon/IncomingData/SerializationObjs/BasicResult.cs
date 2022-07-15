using NP.Utilities.BasicInterfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

public class BasicInfo
{
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("value")]
    public string Value { get; set; }
}

namespace Sebastion.Core.IncomingData.SerializationObjs
{
    [XmlRoot("result")]
    public class BasicResult : IPastRestorable
    {
        [XmlAttribute("host")]
        public string Host { get; set; }

        [XmlArray("basic")]
        [XmlArrayItem("info", Type = typeof(BasicInfo))]
        public BasicInfo[] Infos { get; set; }

        public void AfterRestore()
        {
            foreach(BasicInfo basicInfo in Infos)
            {
                basicInfo.Name = basicInfo.Name?.ToLower();
            }
        }
    }
}
