#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

#endregion

namespace Dots.Core.Models
{
    /// <summary>
    ///     This is the base class from which most business objects will be derived.
    ///     To create a business object, inherit from this class.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the derived class.
    /// </typeparam>
    public class BusinessBaseModel<T> : BaseModel, IChangeTracking, IDisposable
        where T : BusinessBaseModel<T>, new()
    {
        #region Constants and Fields

        /// <summary>
        ///     The broken rules.
        /// </summary>
        /// <remarks>
        ///     This has been updated from using the old StringDictionary class from .Net 1.1. The StringDictionary class
        ///     used case-insensitive keys, so this one needs to have StringComparer.OrdinalIgnoreCase passed in the constructor
        ///     for
        ///     backwards compatibility.
        ///     INotifyPropertyChanging is implemented in case down the line someone wants to create a provider that uses
        ///     BusinessBase
        ///     objects as Linq-To-SQL entities(DataContext performance skyrockets when this is used).
        /// </remarks>
        private readonly Dictionary<String, String> brokenRules =
            new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        ///     The date created.
        /// </summary>
        private DateTime dateCreated = DateTime.MinValue;

        /// <summary>
        ///     The date modified.
        /// </summary>
        private DateTime dateModified = DateTime.MinValue;

        private bool deleted;
        private List<string> tags;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BusinessBaseModel{T}" /> class.
        /// </summary>
        protected BusinessBaseModel()
        {
            New = true;
            IsChanged = true;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when the class is Saved
        /// </summary>
        public static event EventHandler<SavedEventArgs> Saved;

        /// <summary>
        ///     Occurs when the class is Saved
        /// </summary>
        public static event EventHandler<SavedEventArgs> Saving;

        #endregion

        #region Properties

        [XmlElement("timezone")]
        [JsonProperty("timezone")]
        public double Timezone
        {
            get { return Convert.ToDouble(TimeZoneInfo.Local.BaseUtcOffset.Hours); }
        }

        /// <summary>
        ///     Gets or sets the date on which the instance was created.
        /// </summary>
        [XmlElement("dc")]
        [JsonProperty("dc")]
        public DateTime DateCreated
        {
            get { return dateCreated == DateTime.MinValue ? dateCreated : dateCreated.AddHours(Timezone); }

            set { SetValue("DateCreated", value, ref dateCreated); }
        }

        /// <summary>
        ///     Gets or sets the date on which the instance was modified.
        /// </summary>
        [XmlElement("dm")]
        [JsonProperty("dm")]
        public DateTime DateModified
        {
            get { return (dateModified == DateTime.MinValue ? dateModified : dateModified.AddHours(Timezone)); }

            set
            {
                SetValue("DateModifier", value, ref dateModified);
                //  dateModified = value;
            }
        }

        [XmlElement("tgs")]
        public List<string> Tags
        {
            get { return tags; }
            set { SetValue("Tags", value, ref tags); }
        }

        //[XmlElement("so")]
        //public int SortOrder
        //{
        //    get { return sortOrder; }
        //    set {
        //        SetValue("SortOrder", value, ref sortOrder);
        //    }
        //}

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        [XmlIgnore]
        [JsonIgnore]
        public string Error
        {
            get { return ValidationMessage; }
        }

        /// <summary>
        ///     Gets a value indicating whether if this object is marked for deletion.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool Deleted
        {
            get { return deleted; }
            private set { SetValue("Deleted", value, ref deleted); }
        }

        /// <summary>
        ///     Gets a value indicating whether the object is valid or not.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public bool Valid
        {
            get
            {
                ValidationRules();
                return brokenRules.Count == 0;
            }
        }

        /// ///
        /// <summary>
        ///     Gets if the object has broken business rules, use this property to get access
        ///     to the different validation messages.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual string ValidationMessage
        {
            get
            {
                if (!Valid)
                {
                    var sb = new StringBuilder();
                    foreach (string messages in brokenRules.Values)
                    {
                        sb.AppendLine(messages);
                    }

                    return sb.ToString();
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets whether or not the current user owns this object.
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool CurrentUserOwns
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets whether the current user can delete this object.
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool CanUserDelete
        {
            //get { return SecurityService.IsAdministrator; }
            get { return true; }
        }

        /// <summary>
        ///     Gets whether the current user can edit this object.
        /// </summary>
        /// <returns></returns>
        [XmlIgnore]
        [JsonIgnore]
        public virtual bool CanUserEdit
        {
            //get { return SecurityService.IsAdministrator; }
            get { return true; }
        }

        /// <summary>
        ///     Gets a value indicating whether the object has been disposed.
        ///     <remarks>
        ///         If the objects is disposed, it must not be disposed a second
        ///         time. The Disposed property is set the first time the object
        ///         is disposed. If the Disposed property is true, then the Dispose()
        ///         method will not dispose again. This help not to prolong the object's
        ///         life if the Garbage Collector.
        ///     </remarks>
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        [JsonIgnore]
        protected bool Disposed { get; private set; }

        #endregion

        #region Indexers

        /// <summary>
        ///     Gets the <see cref="System.String" /> with the specified column name.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        public string this[string columnName]
        {
            get { return brokenRules.ContainsKey(columnName) ? brokenRules[columnName] : string.Empty; }
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Checks to see if two business objects are the same.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BusinessBaseModel<T> first, BusinessBaseModel<T> second)
        {
            return ReferenceEquals(first, second) ||
                   ((object) first != null && (object) second != null && first.GetHashCode() == second.GetHashCode());
        }

        /// <summary>
        ///     Checks to see if two business objects are different.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BusinessBaseModel<T> first, BusinessBaseModel<T> second)
        {
            return !(first == second);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Loads an instance of the object based on the Id.
        /// </summary>
        /// <param name="id">The unique identifier of the object</param>
        /// <returns>The instance of the object based on the Id.</returns>
        public static T Load(Guid id)
        {
            var instance = new T();
            return instance.DeserializeAndLoad(id);
        }

        /// <summary>
        ///     Marks the object for deletion. It will then be
        ///     deleted when the object's Save() method is called.
        /// </summary>
        public virtual void MarkForDelete()
        {
            Deleted = true;
            IsChanged = true;
        }

        /// <summary>
        ///     Comapares this object with another
        /// </summary>
        /// <param name="obj">
        ///     The object to compare
        /// </param>
        /// <returns>
        ///     True if the two objects as equal
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && (obj.GetType() == GetType() && obj.GetHashCode() == GetHashCode());
        }

        /// <summary>
        ///     A uniquely key to identify this particullar instance of the class
        /// </summary>
        /// <returns>
        ///     A unique integer value
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        ///     Saves the object to the data store (inserts, updates or deletes).
        /// </summary>
        /// <returns>The SaveAction.</returns>
        public virtual SaveAction Save()
        {
            if (Deleted && !CanUserDelete)
            {
                throw new SecurityException("You are not authorized to delete the object");
            }

            if (!Valid && !Deleted)
            {
                throw new InvalidOperationException(ValidationMessage);
            }

            if (Disposed && !Deleted)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "You cannot save a disposed {0}", GetType().Name));
            }

            return IsChanged ? Task.Run(async () => await Update()).Result : SaveAction.None;
            //return IsChanged ? Update().Result: SaveAction.None;
        }

        #endregion

        #region Implemented Interfaces

        #region IChangeTracking

        /// <summary>
        ///     Resets the object's state to unchanged by accepting the modifications.
        /// </summary>
        public void AcceptChanges()
        {
            Save();
        }

        #endregion

        #region IDisposable

        /// <summary>
        ///     Disposes the object and frees ressources for the Garbage Collector.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region "INotifyPropertyChanged"

        #endregion

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the Saved event.
        /// </summary>
        /// <param name="businessObject">
        ///     The business Object.
        /// </param>
        /// <param name="action">
        ///     The action.
        /// </param>
        protected static void OnSaved(BusinessBaseModel<T> businessObject, SaveAction action)
        {
            if (Saved != null)
            {
                Saved(businessObject, new SavedEventArgs(action));
            }
        }

        /// <summary>
        ///     Raises the Saving event
        /// </summary>
        /// <param name="businessObject">
        ///     The business Object.
        /// </param>
        /// <param name="action">
        ///     The action.
        /// </param>
        protected static void OnSaving(BusinessBaseModel<T> businessObject, SaveAction action)
        {
            if (Saving != null)
            {
                Saving(businessObject, new SavedEventArgs(action));
            }
        }

        /// <summary>
        ///     Add or remove a broken rule.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property.
        /// </param>
        /// <param name="errorMessage">
        ///     The description of the error
        /// </param>
        /// <param name="isbroken">
        ///     True if the validation rule is broken.
        /// </param>
        protected virtual void AddRule(string propertyName, string errorMessage, bool isbroken)
        {
            if (isbroken)
            {
                brokenRules[propertyName] = errorMessage;
            }
            else
            {
                brokenRules.Remove(propertyName);
            }
        }

        //protected abstract Task DataDelete();
        //protected abstract Task DataCreate();
        /// <summary>
        ///     Deletes the object from the data store.
        /// </summary>
        /// <summary>
        ///     Inserts a new object to the data store.
        /// </summary>
        /// <summary>
        ///     Retrieves the object from the data store and populates it.
        /// </summary>
        /// <param name="id">
        ///     The unique identifier of the object.
        /// </param>
        /// <returns>
        ///     True if the object exists and is being populated successfully
        /// </returns>
        private async Task<T> DataSelect(Guid id)
        {
            return await Task.Run(() =>
            {
                var x = new T();
                x = x.DeserializeAndLoad();
                return x;
            });
        }

        //protected abstract Task DataUpdate();
        /// <summary>
        ///     Updates the object in its data store.
        /// </summary>
        /// <summary>
        ///     Disposes the object and frees ressources for the Garbage Collector.
        /// </summary>
        /// <param name="disposing">
        ///     If true, the object gets disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (Disposed)
                {
                    return;
                }

                if (!disposing)
                {
                    return;
                }

                changedProperties.Clear();
                brokenRules.Clear();
            }
            finally
            {
                Disposed = true;
            }
        }

        /// <summary>
        ///     Reinforces the business rules by adding additional rules to the
        ///     broken rules collection.
        /// </summary>
        protected virtual void ValidationRules()
        {
        }

        /// <summary>
        ///     Is called by the save method when the object is old and dirty.
        /// </summary>
        /// <returns>
        ///     The update.
        /// </returns>
        private async Task<SaveAction> Update()
        {
            var action = SaveAction.None;

            if (Deleted)
            {
                if (!New)
                {
                    action = SaveAction.Delete;
                    OnSaving(this, action);
                    ((T) this).DeleteFile();
                }
            }
            else
            {
                if (New)
                {
                    if (dateCreated == DateTime.MinValue)
                    {
                        dateCreated = DateTime.UtcNow;
                    }

                    dateModified = DateTime.UtcNow;
                    action = SaveAction.Insert;
                    OnSaving(this, action);
                    ((T) this).SerializeAndSave();
                }
                else
                {
                    dateModified = DateTime.UtcNow;

                    action = SaveAction.Update;
                    OnSaving(this, action);
                    ((T) this).SerializeAndSave();
                }

                MarkOld();
            }

            OnSaved(this, action);
            return action;
        }

        #endregion
    }
}