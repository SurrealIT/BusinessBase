using System;

namespace Dots.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImageDataStoreAttribute : Attribute
    {
        public string Path { get; set; }
    }
}