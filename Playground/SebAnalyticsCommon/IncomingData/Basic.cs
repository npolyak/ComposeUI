using NP.Utilities;
using System;

namespace ConsoleTester.IncomingData
{
    public class Basic
	{
		public string Name { get; set; }
		public string Host { get; set; }
		public string Department { get; set; }
		public string CastleVersion { get; set; }
		public string InfraVersion { get; set; }
		public string DeploymentEnv { get; set; }
		public string EonId { get; set; }
		public string Proid { get; set; }
		public DateTime? StartTime { get; set; }

        public override string ToString()
        {
			return string.Format("Name: {0}, Env: {1}, CastleVersion: {2}, StartTime: {3}", Name, DeploymentEnv, CastleVersion, StartTime);
		}
    }

}
