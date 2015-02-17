#region

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cirrious.CrossCore;
using Dots.Core.Models;
using Dots.Core.Services.Security;

#endregion

namespace Dots.Core.ViewModels
{
    public class BaseViewModel<VM, M> : BaseViewModel
        where VM : IBaseViewModel
        where M : IBaseModel, new()
    {
        private M model;
        private bool showModal;

        public BaseViewModel()
        {
        }

        #region Properties

        public override Guid Id
        {
            get { return Model.Id; }
            set
            {
                if (Model.Id == value) return;
                Model.Id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        public override int Index
        {
            get { return Model.Index; }
            set
            {
                if (Model.Index == value) return;
                Model.Index = value;
                RaisePropertyChanged(() => Index);
            }
        }

        public virtual M Model
        {
            get { return EqualityComparer<M>.Default.Equals(model, default(M)) ? new M() : model; }
            set { model = value; }
        }

        [XmlIgnore]
        public bool IsAdmin
        {
            get { return Mvx.Resolve<ISecurityService>().IsAdministrator; }
        }

        public bool ShowModal
        {
            get { return showModal; }
            set
            {
                showModal = value;
                RaisePropertyChanged(() => ShowModal);
            }
        }

        #endregion
    }
}