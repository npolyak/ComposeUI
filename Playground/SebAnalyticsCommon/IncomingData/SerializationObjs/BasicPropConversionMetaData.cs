using ConsoleTester.IncomingData;
using NP.Utilities.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sebastion.Core.IncomingData.SerializationObjs
{
    internal interface IBasicPropConversionMetaData
    {
        void SetValue(Basic basic, IEnumerable<BasicInfo> infos);
    }

    internal class BasicPropConversionMetaData<TProp> : IBasicPropConversionMetaData
    {
        public string BasicPropName { get; }

        public Action<Basic, TProp> PropSetter { get; }

        public string XmlFieldName { get; }

        public Func<string, TProp> Converter { get; }

        public Func<Basic, string> StrChooser { get; }

        public BasicPropConversionMetaData
        (
            string basicPropName, 
            string fieldName, 
            Func<string, TProp> converter = null,
            Func<Basic, string> strChooser = null)
        {
            BasicPropName = basicPropName;

            PropSetter = CompiledExpressionUtils.GetFullyTypedCSPropertySetter<Basic, TProp>(BasicPropName);

            XmlFieldName = fieldName?.ToLower();

            Converter = converter;

            StrChooser = strChooser;
        }

        private TProp Convert(IEnumerable<BasicInfo> infos, Basic basic)
        {
            string value = 
                XmlFieldName == null ? StrChooser?.Invoke(basic) : infos.FirstOrDefault(info => info.Name == XmlFieldName)?.Value;

            if (Converter == null)
            {
                if (typeof(TProp) == typeof(string))
                {
                    return (TProp)(object) value;
                }
                else
                {
                    return default;
                }
            }
            return Converter.Invoke(value);
        }

        public void SetValue(Basic basic, IEnumerable<BasicInfo> infos)
        {
            TProp val = Convert(infos, basic);

            PropSetter(basic, val);
        }
    }

    internal class BasicPropConversionMetaData : BasicPropConversionMetaData<string>
    {
        public BasicPropConversionMetaData
        (
            string basicPropName, 
            string fieldName, 
            Func<string, string> converter = null,
            Func<Basic, string> strChooser = null)
            :
            base(basicPropName, fieldName, converter, strChooser)
        {
        }
    }
}
