using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Dots.Core.ViewModels;

namespace Dots.Core.Models
{
    public abstract class BaseListModel<P> : ObservableCollection<P> where P : INotifyPropertyChanged, IBaseModel
    {
        public delegate void ChildElementPropertyChangedEventHandler(ChildElementPropertyChangedEventArgs e);

        private readonly IMvxTrace trace = Mvx.Resolve<IMvxTrace>();

        public BaseListModel()
        {
        }

        public P this[Guid id]
        {
            get { return this.FirstOrDefault(x => x.Id == id); }
            set
            {
                P result = this.FirstOrDefault(x => x.Id == id);
                if (result != null)
                {
                    this[id] = value;
                }
            }
        }

        public virtual void MoveDown(Guid id)
        {
            trace.Trace(MvxTraceLevel.Diagnostic, "MoveDown", "Moved down");
            for (int i = 0; i < Items.Count; i++)
            {
                var item = (Items[i] as IIdentity);

                if (item != null && item.Id == id)
                {
                    if (i == Items.Count - 1)
                        break;
                    Move(i, ++i);
                }
            }
        }

        public virtual void MoveUp(Guid id)
        {
            trace.Trace(MvxTraceLevel.Diagnostic, "MoveUp", "Moved Up");
            for (int i = 0; i < Items.Count; i++)
            {
                var item = (Items[i] as IIdentity);

                if (item != null && item.Id == id)
                {
                    if (i == 0)
                        break;
                    Move(i, --i);
                }
            }
        }

        protected override void ClearItems()
        {
            foreach (P item in Items)
            {
                item.PropertyChanged -= convertedItem_PropertyChanged;
            }

            base.ClearItems();
        }

        public virtual event ChildElementPropertyChangedEventHandler ChildElementPropertyChanged;

        private void OnChildElementPropertyChanged(object childelement)
        {
            if (ChildElementPropertyChanged != null)
            {
                ChildElementPropertyChanged(new ChildElementPropertyChangedEventArgs(childelement));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (P item in e.NewItems)
                {
                    var convertedItem = item as INotifyPropertyChanged;
                    convertedItem.PropertyChanged += convertedItem_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (P item in e.OldItems)
                {
                    var convertedItem = item as INotifyPropertyChanged;
                    convertedItem.PropertyChanged -= convertedItem_PropertyChanged;
                }
            }
        }

        private void convertedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChildElementPropertyChanged(sender);
        }

        #region This handles property change notifications.

        public class ChildElementPropertyChangedEventArgs : EventArgs
        {
            public ChildElementPropertyChangedEventArgs(object item)
            {
                ChildElement = item;
            }

            public object ChildElement { get; set; }
        }
    }

    #endregion
}