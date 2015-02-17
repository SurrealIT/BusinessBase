namespace Dots.Core.Services.FileStorage
{
    public interface ICopyParams
    {
        string FolderAndFileName { get; set; }
        string FileName { get; set; }
        string Folder { get; }

        string OldPathAndFileName { get; set; }
    }
}