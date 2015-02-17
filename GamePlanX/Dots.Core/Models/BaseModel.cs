using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Dots.Core.Annotations;

namespace Dots.Core.Models
{
    public class BaseModel : IBaseModel
    {
        /// <summary>
        ///     The changed properties.
        /// </summary>
        protected readonly HashSet<string> changedProperties = new HashSet<string>();

        protected IMvxTrace TraceService;

        private int index;

        public BaseModel()
        {
            Id = Guid.NewGuid();
            TraceService = Mvx.Resolve<IMvxTrace>();
        }

        /// <summary>
        ///     Gets a value indicating whether if this object's data has been changed.
        /// </summary>
        [XmlIgnore]
        public virtual bool IsChanged { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether if this is a new object, False if it is a pre-existing object.
        /// </summary>
        [XmlIgnore]
        public virtual bool New { get; protected set; }

        /// <summary>
        ///     Gets a collection of the properties that have
        ///     been marked as being dirty.
        /// </summary>
        [XmlIgnore]
        protected virtual HashSet<string> ChangedProperties
        {
            get { return changedProperties; }
        }

        [XmlAttribute("id")]
        public Guid Id { get; set; }

        [XmlAttribute("ip")]
        public int Index
        {
            get { return index; }
            set { SetValue("Index", value, ref index); }
        }

        /// <summary>
        ///     Occurs when this instance is marked dirty.
        ///     It means the instance has been changed but not saved.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises the PropertyChanged event safely.
        /// </summary>
        /// <param name="propertyName">
        ///     The property Name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Marks an object as being dirty, or changed.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property to mark dirty.
        /// </param>
        public virtual void MarkChanged(string propertyName)
        {
            IsChanged = true;

            // No need to check for duplicates since changedProperties 
            // is just a HashSet.
            changedProperties.Add(propertyName);
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        ///     Use this method to change values of properties that participate in INotifyPropertyChanged event notification.
        /// </summary>
        /// <typeparam name="TValueType">The object type for both the new value and old value.</typeparam>
        /// <param name="propertyName">The name of the property that should be used when raising the PropertyChanged event.</param>
        /// <param name="newValue">The new value to be set on the property if it's different from oldValue</param>
        /// <param name="oldValue">The current value of the property.</param>
        /// <returns>True if the the property value has been changed, false otherwise.</returns>
        /// <remarks>
        ///     This is left as virtual so users can override this if they have their own validation needs.
        /// </remarks>
        protected virtual bool SetValue<TValueType>(string propertyName, TValueType newValue, ref TValueType oldValue)
        {
            bool isChanged = (!Equals(newValue, oldValue));

            if (isChanged)
            {
                oldValue = newValue;
                MarkChanged(propertyName);
            }
            return isChanged;
        }

        /// <summary>
        ///     Marks the object as being an clean,
        ///     which means not dirty.
        /// </summary>
        public virtual void MarkOld()
        {
            IsChanged = false;
            New = false;
            changedProperties.Clear();
        }
    }
}