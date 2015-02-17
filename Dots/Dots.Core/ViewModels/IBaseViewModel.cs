#region

using System.Windows.Input;

#endregion

namespace Dots.Core.ViewModels
{
    public interface IBaseViewModel : IIdentity
    {
        ICommand DeleteCommand { get; }
        ICommand SaveCommand { get; }
        int Index { get; set; }
    }
}