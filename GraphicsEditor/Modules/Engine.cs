﻿using GraphicsEditor.Modules.Elements;
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
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;
using System.Threading;

namespace GraphicsEditor.Modules
{
    internal class Engine
    {
        public List<IElement> Elements => _elements;
        public List<string> StringElements => _stringElements;
        public BitmapImage BitmapImage => _bitmapImg;
        public Camera Camera => _camera;

        private List<IElement> _elements = new List<IElement>();

        private List<string> _stringElements = new();

        private BitmapImage _bitmapImg;

        private Camera _camera;

        private int _width;
        private int _height;
        private object _sizeLocker;

        private bool _isCameraChanged;

        public Engine(int width, int height)
        {
            _width = width;
            _height = height;
            _sizeLocker = new object();

            _bitmapImg = new BitmapImage();

            _camera = new Camera();
        }

        #region change engine parameters
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

        public async void ChangeCamera(ProjectionPlane plane)
        {
            await Task.Run(() =>
            {
                lock (_camera)
                {
                    _camera.Plane = plane;
                    _isCameraChanged = true;
                }
            });
        }
        #endregion

        #region add elements
        public async void RemoveElementAtPointAsync(int x, int y, int eps)
        {
            await Task.Run(() =>
            {
                List<IElement> cpEl;
                List<IElement> toRemove = new();

                // copy elements
                lock (_elements)
                {
                    cpEl = new List<IElement>(_elements);
                }

                for (int i = cpEl.Count - 1; i >= 0; i--)
                {
                    if (cpEl[i] is VPoint pt)
                    {
                        if (Math.Abs(pt.Point.RenderX - x) <= eps && Math.Abs(pt.Point.RenderY - y) <= eps)
                        {
                            toRemove.Add(pt);
                        }
                    }
                    else if (cpEl[i] is VLine ln)
                    {
                        if (IsPointOnLine(x, y, ln))
                        {
                            toRemove.Add(ln);
                        }
                    }
                }

                lock (_elements)
                {
                    foreach (var item in toRemove)
                    {
                        _elements.Remove(item);
                    }
                }
            });
        }
        public async void AddPointAsync(int x, int y, int z, int size, Color cl)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(new VPoint(_camera, x, y, z, size, cl));
                }
            });
        }

        public async void AddPointAsync(VPoint point)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(point);
                }
            });
        }

        public async void AddLineAsync(int x1, int y1, int z1, int x2, int y2, int z2, int size, Color cl)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(new VLine(_camera, x1, y1, z1, x2, y2, z2, size, cl));
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
        #endregion

        #region async render and background processes
        public async void InitAsyncRender()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    Render();
                    UpdateElementsList();
                    Optimization();
                    //Task.Delay(10);
                    Thread.Sleep(10);
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

        private async void UpdateElementsList()
        {
            await Task.Run(() =>
            {
                List<string> lines = new();

                List<IElement> cpEl;

                // copy elements
                lock (_elements)
                {
                    cpEl = new List<IElement>(_elements);
                }

                foreach (var el in cpEl)
                {
                    lines.Add(el.ToString());
                }

                lock (_stringElements)
                {
                    _stringElements = lines;
                }
            });
        }

        private async void Optimization()
        {
            await Task.Run(() =>
            {
                List<IElement> cpEl;
                List<IElement> toRemove = new();

                // copy elements
                lock (_elements)
                {
                    cpEl = new List<IElement>(_elements);
                }

                for (int i = cpEl.Count - 1; i > 0; i--)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (cpEl[i].Equals(cpEl[j]))
                        {
                            toRemove.Add(cpEl[j]);
                        }
                    }
                }

                lock (_elements)
                {
                    foreach (var item in toRemove)
                    {
                        _elements.Remove(item);
                    }
                }
            });
        }
        #endregion

        private void Render()
        {
            ComputeRenderParameters();

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
                    gr.FillEllipse(new SolidBrush(pt.Color), pt.Point.RenderX, pt.Point.RenderY, pt.Size, pt.Size);
                }
                else if (el is VLine ln)
                {
                    gr.DrawLine(new System.Drawing.Pen(ln.Color, ln.Size), ln.Point1.RenderX, ln.Point1.RenderY, ln.Point2.RenderX, ln.Point2.RenderY);
                }
            }

            BitmapImage t = ToBitmapImage(btmp);

            lock (_bitmapImg)
            {
                _bitmapImg = t;
            }
        }

        private async void ChangeCameraAsync()
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    foreach (var el in _elements)
                    {
                        Task.Run(() =>
                        {
                            lock (el)
                            {
                                lock (_camera)
                                {
                                    el.ChangeProjection(_camera);
                                }
                            }                            
                        });
                    }
                }
            });
            _isCameraChanged = false;
        }

        private void ComputeRenderParameters()
        {
            List<IElement> cpEl;

            // copy elements
            lock (_elements)
            {
                cpEl = new List<IElement>(_elements);
            }

            foreach (IElement elem in cpEl)
            {
                Task.Run(() =>
                {
                    lock (elem)
                    {
                        elem.ChangeProjection(_camera);
                    }
                });
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

        private bool IsPointOnLine(int x, int y, VLine ln)
        {
            return false;
            //double tmpX = 0, tmpY = 0;
            //if (ln.RenderX2 - ln.RenderX1 == 0)
            //{
            //    tmpX = ln.RenderX2;
            //}
            //else
            //{
            //    tmpX = ln.RenderX2 - ln.RenderX1;
            //}
            //if (ln.RenderY2 -ln.RenderY1 == 0)
            //{
            //    tmpY = ln.RenderY2;
            //}
            //else
            //{
            //    tmpY = ln.RenderY2 - ln.RenderY1;
            //}
            //if (Math.Abs((x - ln.RenderX1) / tmpX - (y - ln.RenderY1) / tmpY) <= 0.05)
            //{
            //    return true;
            //}
            //return false;
        }
    }
}
