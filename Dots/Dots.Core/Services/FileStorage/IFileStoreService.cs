using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Plugins.File;

namespace Dots.Core.Services.FileStorage
{
    public interface IFileStoreService : IMvxFileStore
    {
        string CopyFileToModelStorageLocation(ICopyParams copyParams, Guid id, Type imageDataStoreModelType,
            bool overwrite = false, bool deleteOld = false);

        string CopyFileToModelStorageLocation<T>(ICopyParams copyParams, Guid id, bool overwrite = false,
            bool deleteOld = false);

        List<string> GetFoldersIn(string Path);
    }
}