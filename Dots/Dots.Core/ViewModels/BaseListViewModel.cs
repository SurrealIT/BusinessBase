#region

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

#endregion

namespace Dots.Core.ViewModels
{
    public class BaseListViewModel<VM> : ObservableCollection<VM>
        where VM : INotifyPropertyChanged, IBaseViewModel, new()
    {
        private int selectedIndex;

        public BaseListViewModel()
        {
        }

        public VM this[Guid id]
        {
            get { return this.FirstOrDefault(x => x.Id == id); }
            set
            {
                VM result = this.FirstOrDefault(x => x.Id == id);
                if (null != result)
                {
                    this[id] = value;
                }
            }
        }

        public VM CurrentSelectedItem
        {
            get
            {
                if (selectedIndex == -1 || selectedIndex >= Count) selectedIndex = 0;
                return Count == 0 ? new VM() : this[selectedIndex];
            }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentSelectedItem"));
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedIndex"));
            }
        }

        public virtual void LoadItems()
        {
        }

        public virtual void MoveDown(Guid id)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = (Items[i] as IBaseViewModel);

                if (item != null && item.Id == id)
                {
                    if (i == Items.Count - 1) break;
                    Move(i, i + 1);
                    ((IBaseViewModel) Items[i + 1]).Index = i + 1;
                    SelectedIndex = i + 1;
                    break;
                }
            }
        }

        public virtual void MoveUp(Guid id)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = (Items[i] as IBaseViewModel);

                if (item != null && item.Id == id)
                {
                    if (i == 0) break;
                    Move(i, i - 1);
                    ((IBaseViewModel) Items[i - 1]).Index = i - 1;
                    SelectedIndex = i - 1;
                    break;
                }
            }
        }

        #region SetSelectedIndexCommand

        private ICommand selectIndex;

        public ICommand SetSelectedIndexCommand
        {
            get
            {
                selectIndex = selectIndex ??
                              new MvxCommand<Guid>(SelectIndexFromGuid);
                return selectIndex;
            }
        }

        public virtual void SelectIndexFromGuid(Guid id)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = (Items[i] as IIdentity);
                if (item == null || item.Id != id) continue;
                SelectedIndex = i;
                return;
            }
        }

        #endregion

        #region RemoveItem command

        private MvxCommand<Guid> removeItemCommand;

        public ICommand RemoveItemCommand
        {
            get
            {
                removeItemCommand = removeItemCommand ?? new MvxCommand<Guid>(DoRemoveItemCommand);
                return removeItemCommand;
            }
        }

        public virtual void DoRemoveItemCommand(Guid id)
        {
            for (int index = 0; index < Items.Count; index++)
            {
                VM item = Items[index];
                if (item.Id == id)
                {
                    RemoveAt(index);
                    SelectedIndex = 0;
                    //convertedItem_PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
                    break;
                }
            }
        }

        #endregion

        #region MoveUpCommand command

        private MvxCommand<Guid> moveUpCommand;

        public ICommand MoveUpCommand
        {
            get
            {
                moveUpCommand = moveUpCommand ?? new MvxCommand<Guid>(MoveUp);
                return moveUpCommand;
            }
        }

        #endregion

        #region MoveUpCommand command

        private MvxCommand<Guid> moveDownCommand;

        public ICommand MoveDownCommand
        {
            get
            {
                moveDownCommand = moveDownCommand ?? new MvxCommand<Guid>(MoveDown);
                return moveDownCommand;
            }
        }

        #endregion

        #region This handles property change notifications.

        public delegate void ChildElementPropertyChangedEventHandler(ChildElementPropertyChangedEventArgs e);

        protected override void ClearItems()
        {
            foreach (VM item in Items)
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
                foreach (VM item in e.NewItems)
                {
                    var convertedItem = item as INotifyPropertyChanged;
                    convertedItem.PropertyChanged += convertedItem_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (VM item in e.OldItems)
                {
                    var convertedItem = item as INotifyPropertyChanged;
                    convertedItem.PropertyChanged -= convertedItem_PropertyChanged;
                }
            }
            for (int i = 0; i < Count; i++)
            {
                ((IBaseViewModel) this[i]).Index = i;
            }
        }

        private void convertedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChildElementPropertyChanged(sender);
        }

        public class ChildElementPropertyChangedEventArgs : EventArgs
        {
            public ChildElementPropertyChangedEventArgs(object item)
            {
                ChildElement = item;
            }

            public object ChildElement { get; set; }
        }

        #endregion
    }
}