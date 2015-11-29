using System;
using System.Collections.Generic;
using System.Text;

namespace SharpConfig
{
    /// <summary>
    /// Represents an attribute that tells SharpConfig to
    /// ignore the subject this attribute is applied to.
    /// For example, if this attribute is applied to a property
    /// of a type, that property will be ignored when creating
    /// sections from objects and vice versa.
    /// </summary>
    public sealed class IgnoreAttribute : Attribute
    { }
}
