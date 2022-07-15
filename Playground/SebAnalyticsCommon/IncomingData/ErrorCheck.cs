using System.Xml.Serialization;

namespace ConsoleTester.IncomingData
{
    [XmlRoot(ElementName = "result")]
    public class ErrorCheck
    {
        [XmlArray(ElementName = "errors")]
        [XmlArrayItem("error")]
        public ErrorCheckData[] ErrorDataList { get; set; }
    }
}
