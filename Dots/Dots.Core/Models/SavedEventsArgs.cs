#region

using System;

#endregion

namespace Dots.Core.Models
{
    /// <summary>
    ///     The saved event args.
    /// </summary>
    public class SavedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SavedEventArgs" /> class.
        /// </summary>
        /// <param name="action">
        ///     The action.
        /// </param>
        public SavedEventArgs(SaveAction action)
        {
            Action = action;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the action that occured when the object was saved.
        /// </summary>
        public SaveAction Action { get; set; }

        #endregion
    }
}