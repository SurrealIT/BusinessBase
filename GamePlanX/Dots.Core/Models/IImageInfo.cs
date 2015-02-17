using System.Xml.Serialization;

namespace Dots.Core.Models
{
    public interface IImageInfo
    {
        [XmlIgnore]
        string NativeImagePath { get; }

        [XmlElement("ip")]
        string ImagePath { get; set; }
    }
}