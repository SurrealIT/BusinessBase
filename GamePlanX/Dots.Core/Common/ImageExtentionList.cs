using System.Collections.Generic;

namespace Dots.Core.Common
{
    public class ImageExtentionList
    {
        private static readonly List<string> ImageExtensions = new List<string> {".jpg", ".jpe", ".bmp", ".gif", ".png"};

        public static List<string> KnownImageExtentions()
        {
            return ImageExtensions;
        }
    }
}