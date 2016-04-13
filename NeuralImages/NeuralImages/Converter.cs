using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace NeuralImages
{
    /// <summary>
    /// Класс конвертирует любое изображение в список double,
    /// в котором построчно записаны яркости пикселей.
    /// При этом изображение предварительно приводится к размеру (если размер не совпадает),
    /// задаваемому в конструкторе, с помощью класса Compression.
    /// Т.е. для подготовки произвольного .bmp изображения
    /// к передаче в сеть достаточно вызвать метод Convert данного класса.
    /// </summary>
    class Converter
    {
        /// <summary>
        /// Ширина изображения, к которой мы будем приводить исходное изображение.
        /// </summary>
        public int Size_X { get; set; }
        /// <summary>
        /// Высота изображения, к которой мы будем приводить исходное изображение
        /// </summary>
        public int Size_Y { get; set; }

        public Converter(int size_x=20, int size_y=20)
        {
            Size_X = size_x;
            Size_Y = size_y;
        }

        /// <summary>
        /// Конвертирует файл изображения в список double и возвращает список.
        /// Если размер изображения не совпадает, метод предварительно приводит изображение 
        /// к заданным в класе размерам (Size_X, Size_Y).
        /// Изображение переводится построчно.
        /// </summary>
        /// <param name="Path">Путь к файлу исходного изображения</param>
        /// <returns>Список яркостей пикселей построчно</returns>
        public List<double> Convert(string Path)
        {
            List<double> Res = new List<double>();
            Bitmap source = new Bitmap(Path);
            if (source.Size.Height != Size_Y || source.Size.Width != Size_X)
            {
                source.Dispose();
                Compression.Compress(Path, Size_X, Size_Y);
                source = new Bitmap(Path);
            }
            for (int x = 0; x < Size_X; x++)
                for (int y = 0; y < Size_Y; y++)
                    Res.Add(source.GetPixel(x, y).GetBrightness());
            return Res;
        }
    }
}
