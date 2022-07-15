using System;
using System.Xml.Serialization;

namespace ConsoleTester.IncomingData
{
    public class ErrorCheckData
	{
		[XmlAttribute("EventId")]
		public string EventId { get; set; }

		[XmlAttribute("UniqueId")]
		public string UniqueId { get; set; }

		[XmlAttribute("Count")]
		public int Count { get; set; }

		[XmlAttribute("Frequency")]
		public double Frequency { get; set; }

		[XmlAttribute("LastUpdate")]
		public string LastUpdate { get; set; }

		[XmlAttribute("Module")]
		public string Module { get; set; }

		[XmlAttribute("Method")]
		public string Method { get; set; }

		[XmlAttribute("Messages")]
		public string Message { get; set; }

		[XmlAttribute("Stack")]
		public string StackTrace { get; set; }

		[XmlAttribute("MeanFrequency")]
		public double MeanFrequency { get; set; }

		public override string ToString()
		{
			return $"EventId: {EventId}\nUniqueID: {UniqueId}\nMethod: {Method}\nMessage: {Message}";
		}
	}
}
