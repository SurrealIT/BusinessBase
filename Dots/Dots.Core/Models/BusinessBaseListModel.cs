#region

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

#endregion

namespace Dots.Core.Models
{
    public class BusinessBaseListModel<P, C> : BaseListModel<C>
        where P : BusinessBaseListModel<P, C>, new()
        where C : BusinessBaseModel<C>, new()
    {
        protected static BusinessBaseListModel<P, C> instance;

        private bool autoSave;

        public BusinessBaseListModel()
        {
        }

        public new event NotifyCollectionChangedEventHandler CollectionChanged;

        public static P Instance(bool autoSaveList = false)
        {
            if (instance == null)
            {
                instance = new P();
            }
            instance.Autosave(autoSaveList);
            if (instance.Count == 0)
            {
                instance.LoadItems();
            }
            return (P) instance;
        }

        protected virtual void LoadItems()
        {
            if (Count > 0)
                return;
            List<C> newItems = instance.DeserializeAndLoad();
            if (newItems != null)
            {
                foreach (C tSource in newItems.OrderBy(s => s.Index))
                {
                    Add(tSource);
                }
            }
        }

        public void Save()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].Save();
            }
        }

        public void Reset()
        {
            instance.ClearItems();
        }

        #region INotifyPropertyChanged implementation

        protected void Autosave(bool autoSaveList = false)
        {
            autoSave = autoSaveList;
            if (autoSave)
            {
                CollectionChanged += SaveOnChanged;
            }
            else
            {
                if (CollectionChanged != null)
                    CollectionChanged -= SaveOnChanged;
            }
        }

        private void SaveOnChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Save();
        }

        #endregion
    }
}