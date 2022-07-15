using ConsoleTester.IncomingData;
using MorganStanley.Castle.Http;
using Sebastion.Core.IncomingData.SerializationObjs;
using NP.Utilities;

namespace Sebastion.Core.SvcUtils
{
    public static class EndpointInfoGetter
    {
        private static BasicConversionMetaData _conversionMetaData = new BasicConversionMetaData();

        private static async Task<string> GetStr(this string friendlyName, string suffix)
        {
            var str = await GwmHttpClient.GetAsync<string>(friendlyName, suffix);

            return str;
        }

        public static async Task<Basic> GetBasicInfo(string friendlyName)
        {
            var str = await friendlyName.GetStr("?deploy-check/xml/basic-anon");

            BasicResult basicResult =
                XmlSerializationUtils.Deserialize<BasicResult>(str, true);

            Basic basic = _conversionMetaData.Convert(basicResult);

            return basic;
        }

        public static async Task<Health> GetHealthInfo(string friendlyName)
        {
            var str = await friendlyName.GetStr("?deploy-check/xml/health-anon?sla=0");

            HealthResult healthCheck =
                XmlSerializationUtils.Deserialize<HealthResult>(str, true);

            return healthCheck.TheHealth;
        }

        public static async Task<ErrorCheck> GetErrorInfo(string friendlyName)
        {
            var str = await friendlyName.GetStr("?deploy-check/xml/errors-anon");

            ErrorCheck errorCheck =
                XmlSerializationUtils.Deserialize<ErrorCheck>(str, true);

            return errorCheck;
        }
    }
}
