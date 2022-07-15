namespace Sebastion.Core
{
    public enum Env
    {
        Dev, 
        QA,
        UAT,
        Prod
    }

    public static class Constants
    {
        public const string RealmKey = "realm";
        public const string MsadPath = "MSAD.MS.COM";

        public static Env[] Envs { get; } = { Env.Dev, Env.QA, Env.UAT };

        public static string ToStr(this Env env) => env.ToString().ToLower();
    }

}
