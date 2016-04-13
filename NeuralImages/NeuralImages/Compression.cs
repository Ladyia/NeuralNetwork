using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;

namespace NeuralImages
{
    /// <summary>
    /// Сжимает размер изображения до 40х40 пикселов
    /// </summary>
    static class Compression
    {
        public static void Compress(string Path, int sizeW, int sizeH)
        {
            using (Bitmap image = new Bitmap(@Path))
            {
                Size size = new Size(sizeW, sizeH);
                using (Bitmap newImage = new Bitmap(image, size))
                {
                    image.Dispose();
                    newImage.Save(Path);
                }
            }
        }
    }
}
