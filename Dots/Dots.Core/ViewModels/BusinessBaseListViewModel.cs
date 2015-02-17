#region

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Dots.Core.Models;

#endregion

namespace Dots.Core.ViewModels
{
    public class BusinessBaseListViewModel<VM, P, C> : BaseListViewModel<VM>
        where P : BusinessBaseListModel<P, C>, new()
        where C : BusinessBaseModel<C>, new()
        where VM : INotifyPropertyChanged, IBaseViewModel, new()
    {
        public BusinessBaseListViewModel()
        {
        }

        #region Save command

        private MvxCommand saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                saveCommand = saveCommand ?? new MvxCommand(DoSaveCommand);
                return saveCommand;
            }
        }

        public virtual void DoSaveCommand()
        {
            Debug.WriteLine("DoSaveCommand");
            Model.Save();
        }

        #endregion

        public P Model { get; set; }
    }
}