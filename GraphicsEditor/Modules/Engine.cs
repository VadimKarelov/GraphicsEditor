using GraphicsEditor.Modules.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using System.Threading;
using Point = System.Drawing.Point;
using System.Windows.Media;
using Pen = System.Drawing.Pen;

namespace GraphicsEditor.Modules
{
    internal class Engine
    {
        public List<IElement> Elements => _elements;
        public List<string> StringElements => _stringElements;
        public BitmapImage BitmapImage => _bitmapImg;
        public Camera Camera => _camera;
        public IElement? EditingElement { get; set; }

        private List<IElement> _elements = new List<IElement>();

        private List<string> _stringElements = new();

        private BitmapImage _bitmapImg;

        private Camera _camera;

        private int _width;
        private int _height;
        private object _sizeLocker;

        private int _renderCounter;
        private bool _isRenderRequired;
        private object _renderCounterLocker;

        public Engine(int width, int height)
        {
            _width = width;
            _height = height;
            _sizeLocker = new object();

            _renderCounterLocker = new();

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
                SendSignalToRender();
            });
        }

        public async void ChangeCamera(ProjectionPlane plane)
        {
            await Task.Run(() =>
            {
                lock (_camera)
                {
                    _camera.Plane = plane;
                }
                ComputeRenderParameters();
                SendSignalToRender();
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
                    else if (cpEl[i] is VCurve cl)
                    {
                        bool found = false;
                        Point[] pts = cl.RenderPoints;
                        for (int j = 0; j < pts.Length && !found; j++)
                        {
                            if (Math.Abs(pts[j].X - x) < 5 && Math.Abs(pts[j].Y - y) < 5)
                            {
                                found = true;
                                toRemove.Add(cl);
                            }
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

                SendSignalToRender();
            });
        }

        public async void AddElementAsync(IElement element)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Add(element);
                }
                SendSignalToRender();
            });
        }
        #endregion

        #region async render and background processes
        public async void InitAsyncRender()
        {
            await Task.Run(() =>
            {
                SendSignalToRender();

                while (true)
                {
                    if (_renderCounter >= 20 && (_isRenderRequired || EditingElement != null))
                    {
                        lock (_renderCounterLocker)
                        {
                            _isRenderRequired = false;
                            _renderCounter = 0;
                        }

                        Render();
                        UpdateElementsList();
                    }
                    else if (_renderCounter % 5000 == 0)
                    {
                        Optimization();
                    }

                    Thread.Sleep(20);

                    lock (_renderCounterLocker)
                    {
                        _renderCounter += 20;
                    }
                }
            });
        }

        private void SendSignalToRender()
        {
            lock (_renderCounterLocker)
            {
                _isRenderRequired = true;
            }
        }

        public async void RenderAsync()
        {
            await Task.Run(() =>
            {
                Render();
            });
        }

        public async void DelayRenderAsync(int milliseconds)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(milliseconds);
                //Render();
                SendSignalToRender();
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

                for (int i = cpEl.Count - 1; i >= 0; i--)
                {
                    if (i > 0)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (cpEl[i].Equals(cpEl[j]))
                            {
                                toRemove.Add(cpEl[j]);
                            }
                        }
                    }
                    if (cpEl[i] is VCurve cl)
                    {
                        if (!cl.Points.Any())
                        {
                            toRemove.Add(cpEl[i]);
                        }
                        else
                        {
                            /*
                            bool changed = false;
                            for (int j = cl.Points.Count - 1; j > 0; j--)
                            {
                                if (cl.Points[j].Equals(cl.Points[j - 1]))
                                {
                                    lock (cl)
                                    {
                                        cl.Points.RemoveAt(j);
                                    }
                                    changed = true;
                                }
                                // 5 or more elements
                                
                                else if (j >= 4)
                                {
                                    if (cl.Points[j].X == cl.Points[j - 1].X && cl.Points[j].X == cl.Points[j - 2].X 
                                    && cl.Points[j].X == cl.Points[j - 3].X && cl.Points[j].X == cl.Points[j - 4].X)
                                    {
                                        cl.Points.RemoveAt(j - 2);
                                        changed = true;
                                    }
                                    else if (cl.Points[j].Y == cl.Points[j - 1].Y && cl.Points[j].Y == cl.Points[j - 2].Y
                                    && cl.Points[j].Y == cl.Points[j - 3].Y && cl.Points[j].Y == cl.Points[j - 4].Y)
                                    {
                                        cl.Points.RemoveAt(j - 2);
                                        changed = true;
                                    }                                    
                                }
                                //
                            }
                            if (changed)
                                cl.ChangeProjection();
                            */
                        }
                    }
                    if (cpEl[i] is VLine ln)
                    {
                        if (ln.Point1.Equals(ln.Point2))
                        {
                            toRemove.Add(cpEl[i]);
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

                SendSignalToRender();
            });
        }
        #endregion

        #region render
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
                    gr.FillEllipse(new SolidBrush(pt.Color), pt.Point.RenderX, pt.Point.RenderY, pt.Size, pt.Size);
                }
                else if (el is VLine ln)
                {
                    gr.DrawLine(new Pen(ln.Color, ln.Size), ln.Point1.RenderX, ln.Point1.RenderY, ln.Point2.RenderX, ln.Point2.RenderY);
                }
                else if (el is VCurve cl)
                {
                    Point[] pts = cl.RenderPoints;
                    if (pts.Length == 1)
                    {
                        gr.FillEllipse(new SolidBrush(cl.Color), pts[0].X, pts[0].Y, cl.Size, cl.Size);
                    }
                    else if (pts.Length > 1)
                    {
                        Pen pen = new Pen(cl.Color, cl.Size);
                        pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        gr.DrawCurve(pen, pts);
                    }
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
        #endregion

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
                                    el.ChangeProjection();
                                }
                            }                            
                        });
                    }
                }

                SendSignalToRender();
            });
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
                        elem.ChangeProjection();
                    }
                });
            }

            SendSignalToRender();
        }        

        private bool IsPointOnLine(int x, int y, VLine ln)
        {
            double eps = 10;

            if (x + eps < Math.Min(ln.Point1.RenderX, ln.Point2.RenderX))
                return false;
            if (y + eps < Math.Min(ln.Point1.RenderY, ln.Point2.RenderY))
                return false;

            if (x - eps > Math.Max(ln.Point1.RenderX, ln.Point2.RenderX))
                return false;
            if (y - eps > Math.Max(ln.Point1.RenderY, ln.Point2.RenderY))
                return false;

            return (x - ln.Point1.RenderX) * (ln.Point2.RenderY - ln.Point1.RenderY) -
                (y - ln.Point1.RenderY) * (ln.Point2.RenderX - ln.Point1.RenderX) <= eps;
        }
    }
}
