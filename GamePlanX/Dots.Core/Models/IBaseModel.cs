using System.ComponentModel;
using System.Xml.Serialization;
using Dots.Core.ViewModels;

namespace Dots.Core.Models
{
    public interface IBaseModel : INotifyPropertyChanged, IIdentity
    {
        [XmlAttribute("ip")]
        int Index { get; set; }
    }
}