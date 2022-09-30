using GraphicsEditor.Modules.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Media;
using Color = System.Drawing.Color;
using System.Security.Policy;

namespace GraphicsEditor.Modules
{
    internal class Engine
    {
        public List<IElement> Elements => _elements;
        public BitmapImage BitmapImage => _bitmapImg;

        private List<IElement> _elements = new List<IElement>();

        private BitmapImage _bitmapImg;

        private int _width;
        private int _height;
        private object _sizeLocker;

        public Engine(int width, int height)
        {
            _width = width;
            _height = height;
            _sizeLocker = new object();

            _bitmapImg = new BitmapImage();
        }

        public async void ChangeFieldSizeAsync(int width, int height)
        {
            await Task.Run(() =>
            {
                lock (_sizeLocker)
                {
                    _width = width;
                    _height = height;
                }
            });
        }

        public async void AddPointAsync(int x, int y, int size, Color cl)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(new VPoint(x, y, size, cl));
                }
            });
        }

        public async void AddLineAsync(int x1, int y1, int x2, int y2, int size, Color cl)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(new VLine(cl, x1, y1, x2, y2, size));
                }
            });
        }

        public async void AddLineAsync(VLine line)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(line);
                }
            });
        }

        public async void InitAsyncRender()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    Render();
                    Task.Delay(10);
                });
            }
        }

        public async void RenderAsync()
        {
            await Task.Run(() =>
            {
                Render();
            });
        }

        private void Render()
        {
            Bitmap btmp;
            lock (_sizeLocker)
            {
                btmp = new Bitmap(_width, _height);
            }
            Graphics gr = Graphics.FromImage(btmp);
            List<IElement> cpEl;

            // copy elements
            lock (_elements)
            {
                cpEl = new List<IElement>(_elements);
            }

            gr.Clear(Color.White);

            foreach (IElement el in cpEl)
            {
                if (el is VPoint pt)
                {
                    gr.FillEllipse(new SolidBrush(pt.Color), pt.RenderX, pt.RenderY, pt.Size, pt.Size);
                }
                else if (el is VLine ln)
                {
                    gr.DrawLine(new System.Drawing.Pen(ln.Color, ln.Size), ln.X1, ln.Y1, ln.X2, ln.Y2);
                }
            }

            BitmapImage t = ToBitmapImage(btmp);

            lock (_bitmapImg)
            {
                _bitmapImg = t;
            }
        }

        private BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
