#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Dots.Core.Attributes;
using Dots.Core.Services.FileStorage;

#endregion

namespace Dots.Core.Models
{
    public static class ModelExtensions
    {

        private static IFileStoreService FileStoreService
        {
            get
            {
                if (Mvx.CanResolve(typeof(IFileStoreService)))
                {
                    return Mvx.Resolve<IFileStoreService>();
                }
                return Mvx.Resolve<IFileStoreService>();
            }
        }

        private static string FilePath(Type type, Guid id = default(Guid))
        {
            var trace = Mvx.Resolve<IMvxTrace>();
            TypeInfo t = type.GetTypeInfo();
            var attribute = t.GetCustomAttribute<DataStoreAttribute>();

            if (attribute != null)
            {
                string storePath = FileStoreService.PathCombine(FileStoreService.NativePath(string.Empty),
                    string.Format(@"{0}", attribute.Path));

                FileStoreService.EnsureFolderExists(storePath);

                if (id != Guid.Empty)
                {
                    string fileName = string.Format("{0}.{1}", id, "xml");
                    storePath = FileStoreService.PathCombine(storePath, fileName);
                }

                trace.Trace(MvxTraceLevel.Diagnostic, "file path computed", storePath);
                return storePath;
            }
            return string.Empty;
        }

        public static void SerializeAndSave<T>(this T item)
            where T : BusinessBaseModel<T>, INotifyPropertyChanged, new()
        {
            var trace = Mvx.Resolve<IMvxTrace>();
            string storeFileName = FilePath(typeof(T), item.Id);
            if (string.IsNullOrEmpty(storeFileName))
                return;
            try
            {
                FileStoreService.WriteFile(storeFileName, stream =>
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stream, item);
                });
            }
            catch (Exception ex)
            {
                trace.Trace(MvxTraceLevel.Error, "SerializeAndSave", ex.Message);
            }
        }

        public static T DeserializeAndLoad<T>(this T item, Guid id = default (Guid))
            where T : BusinessBaseModel<T>, new()
        {
            var trace = Mvx.Resolve<IMvxTrace>();
            string storeFileName = FilePath(typeof(T), id);
            if (string.IsNullOrEmpty(storeFileName))
                return null;
            try
            {
                XDocument loadedData = XDocument.Load(storeFileName);
                if (loadedData.Root == null)
                    return null;

                using (XmlReader reader = loadedData.Root.CreateReader())
                {
                    var result = (T)new XmlSerializer(typeof(T)).Deserialize(reader);
                    result.MarkOld();
                    return result;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(MvxTraceLevel.Error, "DeserializeAndLoad", ex.Message);
                return null;
            }
        }

        public static List<C> DeserializeAndLoad<P, C>(this BusinessBaseListModel<P, C> item)
            where P : BusinessBaseListModel<P, C>, new()
            where C : BusinessBaseModel<C>, new()
        {
            var trace = Mvx.Resolve<IMvxTrace>();
            var result = new List<C>();
            string filePath = FilePath(typeof(C));

            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                foreach (string file in FileStoreService.GetFilesIn(filePath))
                {
                    XDocument loadedData = XDocument.Load(file);
                    if (loadedData.Root != null)
                    {
                        using (XmlReader reader = loadedData.Root.CreateReader())
                        {
                            var loadedItem = new XmlSerializer(typeof(C)).Deserialize(reader) as C;
                            if (loadedItem == null) continue;
                            loadedItem.MarkOld();
                            result.Add(loadedItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                trace.Trace(MvxTraceLevel.Error, "DeserializeAndLoad", ex.Message);
                return null;
            }

            return result;
        }

        public static void DeleteFile<T>(this T item) where T : BusinessBaseModel<T>, INotifyPropertyChanged, new()
        {
            var trace = Mvx.Resolve<IMvxTrace>();
            string storeFileName = FilePath(typeof(T), item.Id);
            if (string.IsNullOrEmpty(storeFileName))
                return;
            try
            {
                FileStoreService.DeleteFile(storeFileName);
            }
            catch (Exception ex)
            {
                trace.Trace(MvxTraceLevel.Error, "DeleteFile", ex.Message);
            }
        }

        public static string NativeImagePath(this string value)
        {
            if (value == null) return string.Empty;

            string file;
            try
            {
                file = FileStoreService.PathCombine(FileStoreService.NativePath(string.Empty),
                    value);
            }
            catch (Exception ex)
            {
                file = string.Empty;
            }
            return file;
        }
    }
}