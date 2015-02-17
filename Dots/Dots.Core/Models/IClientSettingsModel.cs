using System.Xml.Serialization;

namespace GamePlan.Core.Models
{
    public interface IClientSettingsModel : IBaseModel
    {
        [XmlElement("ttl")]
        string Title { get; set; }

        [XmlElement("vsn")]
        string Version { get; set; }

        [XmlElement("ld")]
        string LogoDark { get; set; }

        [XmlElement("ll")]
        string LogoLight { get; set; }

        [XmlElement("yli")]
        string YearLogoImagePathAndFile { get; set; }

        [XmlElement("ti")]
        string TitleImagePathAndFile { get; set; }

        [XmlElement("yp")]
        string YearPartImagePathAndFile { get; set; }
    }
}