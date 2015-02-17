using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Dots.Core.ViewModels
{
    public interface IIdentity
    {
        [XmlAttribute("id"), JsonProperty("id")]
        Guid Id { get; set; }
    }
}