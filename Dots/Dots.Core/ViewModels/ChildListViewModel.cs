using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GamePlan.Core.ViewModels
{
    public class ChildListViewModel<P> : ObservableCollection<P> where P : INotifyPropertyChanged
    {
        public ChildListViewModel()
        {
            CollectionChanged += ItemModels_CollectionChanged;
        }

        private void ItemModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object newItem in e.NewItems)
                {
                    ((P) newItem).PropertyChanged += SubChannelNavigationItemModels_PropertyChanged;
                }
            }
        }

        private void SubChannelNavigationItemModels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}