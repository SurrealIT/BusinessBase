using System;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Dots.Core.Services.Analytics;

namespace Dots.Core.ViewModels
{
    public class BaseViewModel : MvxViewModel, IBaseViewModel
    {
        //public readonly IFileStoreService FileStoreService = Mvx.Resolve<IFileStoreService>();
        //public readonly ISecurityService SecurityService = Mvx.Resolve<ISecurityService>();
        //protected IMvxTrace TraceService = Mvx.Resolve<IMvxTrace>();
        //protected IAnalyticTrackingService TrackingService = Mvx.Resolve<IAnalyticTrackingService>();
        private Guid id = Guid.Empty;
        private int index;

        public BaseViewModel()
        {
        }

        protected IAnalyticTrackingService TrackingService
        {
            get { return Mvx.Resolve<IAnalyticTrackingService>(); }
        }

        public virtual Guid Id
        {
            get { return id; }
            set
            {
                if (id == value) return;
                id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        public virtual int Index
        {
            get { return index; }
            set
            {
                if (index == value) return;
                index = value;
                RaisePropertyChanged(() => Index);
            }
        }

        /// <summary>
        ///     Use this method to change values of properties that participate in INotifyPropertyChanged event notification.
        /// </summary>
        /// <typeparam name="T_VALUE_TYPE">The object type for both the new value and old value.</typeparam>
        /// <param name="propertyName">The name of the property that should be used when raising the PropertyChanged event.</param>
        /// <param name="newValue">The new value to be set on the property if it's different from oldValue</param>
        /// <param name="oldValue">The current value of the property.</param>
        /// <returns>True if the the property value has been changed, false otherwise.</returns>
        /// <remarks>
        ///     This is left as virtual so users can override this if they have their own validation needs.
        /// </remarks>
        protected virtual bool SetValue<T_VALUE_TYPE>(string propertyName, T_VALUE_TYPE newValue,
            ref T_VALUE_TYPE oldValue)
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
        ///     Marks an object as being dirty, or changed.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property to mark dirty.
        /// </param>
        public virtual void MarkChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
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
        }

        #endregion

        #region MarkForDelete command

        private MvxCommand deleteCommand;

        public ICommand DeleteCommand
        {
            get
            {
                deleteCommand = deleteCommand ?? new MvxCommand(DoDeleteCommand);
                return deleteCommand;
            }
        }

        public virtual void DoDeleteCommand()
        {
        }

        #endregion

        #region OpenAFile command

        private MvxCommand<string> openAFileCommand;

        public ICommand OpenAFileCommand
        {
            get
            {
                openAFileCommand = openAFileCommand ?? new MvxCommand<string>(DoOpenAFileCommand);
                return openAFileCommand;
            }
        }

        public virtual void DoOpenAFileCommand(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            var browserTask = Mvx.Resolve<IMvxWebBrowserTask>();
            browserTask.ShowWebPage(path);
        }

        #endregion
    }
}