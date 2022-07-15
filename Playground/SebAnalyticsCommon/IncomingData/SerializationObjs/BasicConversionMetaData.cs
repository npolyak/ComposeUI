using ConsoleTester.IncomingData;

namespace Sebastion.Core.IncomingData.SerializationObjs
{
    public class BasicConversionMetaData
    {
        private IBasicPropConversionMetaData[] PropsConversionMetaData { get; }

        public BasicConversionMetaData()
        {
            PropsConversionMetaData =
                new IBasicPropConversionMetaData[]
                {
                    new BasicPropConversionMetaData
                    (
                        nameof(Basic.Name), 
                        "asserted appid", 
                        (name) =>
                        {
                            if (name == string.Empty)
                            {
                                return string.Empty;
                            }

                            string[] nameList = name.Split(new[] { "\\" }, StringSplitOptions.None);
                            var result = (nameList.Length > 1) ? nameList[1] : nameList[0];

                            return result;
                        }
                    ),
                    new BasicPropConversionMetaData
                    (
                        nameof(Basic.DeploymentEnv),
                        "config mode"
                    ),
                    new BasicPropConversionMetaData
                    (
                        nameof(Basic.EonId),
                        "eonid"
                    ),
                    new BasicPropConversionMetaData
                    (
                        nameof(Basic.CastleVersion), 
                        "castle version"
                    ),
                    new BasicPropConversionMetaData
                    (
                        nameof(Basic.InfraVersion),
                        "infrastructure version"
                    ),
                    new BasicPropConversionMetaData<DateTime?>
                    (
                        nameof(Basic.StartTime),
                        "host start time", 
                        (startTimeStr) => DateTime.Parse(startTimeStr)
                    ),
                    new BasicPropConversionMetaData
                    (
                        nameof(Basic.Proid),
                        "service identity",
                        str =>
                        {
                            var start = str.IndexOf("\\") + 1;
                            var end = str.IndexOf("(");

                            if (end < 0 || start < 0)
                                return null;

                            return str.Substring(start, end - start);
                        }
                    ),
                    //new BasicPropConversionMetaData
                    //(
                    //    nameof(Basic.Department),
                    //    null,
                    //    str =>
                    //    {
                    //        try
                    //        {
                    //            var found = Directories.ProIds.FirstOrDefault(p => p.UserId == (str ?? "=="));

                    //            return found != null ? found.Department : "Unknown";
                    //        }
                    //        catch
                    //        {
                    //            return "Unknown";
                    //        }
                    //    },
                    //    basic => basic.Proid
                    //)
                };
        }

        public Basic Convert(BasicResult result)
        {
            Basic basic = new Basic();

            basic.Host = result.Host;

            foreach(IBasicPropConversionMetaData propConversionMetaData in PropsConversionMetaData)
            {
                propConversionMetaData.SetValue(basic, result.Infos);
            }

            return basic;
        }
    }
}
