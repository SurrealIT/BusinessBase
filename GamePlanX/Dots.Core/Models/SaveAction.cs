namespace Dots.Core.Models
{
    /// <summary>
    ///     The action performed by the save event.
    /// </summary>
    public enum SaveAction
    {
        /// <summary>
        ///     Default. Nothing happened.
        /// </summary>
        None,

        /// <summary>
        ///     It's a new object that has been inserted.
        /// </summary>
        Insert,

        /// <summary>
        ///     It's an old object that has been updated.
        /// </summary>
        Update,

        /// <summary>
        ///     The object was deleted.
        /// </summary>
        Delete
    }
}