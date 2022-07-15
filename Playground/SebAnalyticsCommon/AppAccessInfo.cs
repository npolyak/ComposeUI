using NP.Utilities;
using NP.Utilities.BasicInterfaces;
using System;
using System.Xml.Serialization;

namespace Sebastion.Core.SvcUtils
{
    public interface IAppAccessInfo : ICopyable<IAppAccessInfo>
    {
        string Fsn { get; set; }

        Env EnvKind { get; set; }

        string Env { get; set; }

        string FullPath { get; set; }

        bool EqualsImpl(IAppAccessInfo info)
        {
            return Fsn == info.Fsn &&
                   EnvKind == info.EnvKind &&
                   Env == info.Env &&
                   FullPath == info.FullPath;
        }

        int GetHashCodeImpl()
        {
            return HashCode.Combine(Fsn, EnvKind, Env, FullPath);
        }

        void ICopyable<IAppAccessInfo>.CopyFrom(IAppAccessInfo source)
        {
            this.Fsn = source.Fsn;
            this.EnvKind = source.EnvKind;
            this.Env = source.Env;
            this.FullPath = source.FullPath;
        }
    }

    public class AppAccessInfo : VMBase, IAppAccessInfo
    {
        [XmlAttribute]
        public string Fsn { get; set; }

        [XmlAttribute]
        public Env EnvKind { get; set; }

        [XmlAttribute]
        public string Env { get; set; }

        [XmlAttribute]
        public string FullPath { get; set; }

        public AppAccessInfo()
        {
        }

        public AppAccessInfo(AppAccessInfo source) : 
            this(source.Fsn, source.EnvKind, source.Env, source.FullPath)
        {
                
        }

        public AppAccessInfo(string fsn, Env envKind, string env, string full)
        {
            Fsn = fsn;
            EnvKind = envKind;
            Env = env;
            FullPath = full;
        }

        protected void CopyFromAccessInfo(IAppAccessInfo source)
        {
            (this as IAppAccessInfo).CopyFrom(source);
        }

        public override bool Equals(object obj)
        {
            return (obj is AppAccessInfo info) && (this as IAppAccessInfo).EqualsImpl(info);
        }

        public bool UrlMatches(AppAccessInfo info)
        {
            return this.FullPath == info.FullPath;
        }

        public override int GetHashCode()
        {
            return (this as IAppAccessInfo).GetHashCodeImpl();
        }

        public override string ToString()
        {
            return $"FullPath: {FullPath}\nFsn: {Fsn}\n Env: {Env}\nEnvKind: {EnvKind}";
        }
    }
}
