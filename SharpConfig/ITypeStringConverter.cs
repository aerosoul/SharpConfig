using System;
using System.Collections.Generic;
using System.Text;

namespace SharpConfig
{
    public interface ITypeStringConverter
    {
        string ConvertToString(object value);
        object ConvertFromString(string value, Type hint);
        Type ConvertibleType { get; }
    }

    public abstract class TypeStringConverter<T> : ITypeStringConverter
    {
        public abstract string ConvertToString(object value);
        public abstract object ConvertFromString(string value, Type hint);

        public Type ConvertibleType
        {
            get { return typeof(T); }
        }
    }
}
