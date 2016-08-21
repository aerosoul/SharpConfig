// Copyright (c) 2013-2016 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig

using System;

namespace SharpConfig
{
    [Serializable]
    public sealed class SettingValueCastException : Exception
    {
        private SettingValueCastException(string message, Exception innerException)
            : base(message, innerException)
        { }

        internal static SettingValueCastException Create(string stringValue, Type dstType, Exception innerException)
        {
            string msg = string.Format("Failed to convert value '{0}' to type {1}.", stringValue, dstType.FullName);
            return new SettingValueCastException(msg, innerException);
        }

        internal static SettingValueCastException CreateBecauseConverterMissing(string stringValue, Type dstType)
        {
            string msg = string.Format(
                "Failed to convert value '{0}' to type {1}; no converter for this type is registered.",
                stringValue, dstType.FullName);

            var innerException = new NotImplementedException("no converter for this type is registered.");

            return new SettingValueCastException(msg, innerException);
        }
    }
}