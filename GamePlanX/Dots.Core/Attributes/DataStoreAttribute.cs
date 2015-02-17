#region

using System;

#endregion

namespace Dots.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataStoreAttribute : Attribute
    {
        public string Path { get; set; }
    }
}