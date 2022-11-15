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
using System.Diagnostics;
using System.Linq.Expressions;

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

        private Optimizator _optimizator;

        private int _width;
        private int _height;
        private object _sizeLocker;

        private int _renderCounter;
        private bool _isRenderRequired;
        private object _renderCounterLocker;

        private bool _isEngineStop;

        public List<IElement>? EditingElements
        {
            get
            {
                return _editingElements;
            }
            set
            {
                lock (_editingElementsLocker)
                {
                    _editingElements = value;
                }
            }
        }
        private List<IElement>? _editingElements;
        private object _editingElementsLocker;

        public Engine(int width, int height)
        {
            _width = width;
            _height = height;
            _sizeLocker = new object();

            _renderCounterLocker = new();

            _bitmapImg = new BitmapImage();

            _camera = new Camera();

            _optimizator = new Optimizator();

            _editingElementsLocker = new();

            _isEngineStop = false;
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
                //List<IElement> cpEl;

                //// copy elements
                //lock (_elements)
                //{
                    //cpEl = new List<IElement>(_elements);
                //}

                bool isRemoving = RemoveAtPointR(x, y, eps, _elements);

                if (isRemoving)
                {
                    SendSignalToRender();
                }
            });
        }

        private bool RemoveAtPointR(int x, int y, int eps, List<IElement> cpEl)
        {
            bool isRemoving = false;

            List<IElement> toRemove = new();

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
                    if (IsPointOnLine(x, y, eps, ln))
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
                        if (Math.Abs(pts[j].X - x) < eps && Math.Abs(pts[j].Y - y) < eps)
                        {
                            found = true;
                            toRemove.Add(cl);
                        }
                    }
                }
                else if (cpEl[i] is VGroup grp)
                {
                    isRemoving = isRemoving || RemoveAtPointR(x, y, eps, grp.Elements);
                }
            }

            isRemoving = isRemoving || toRemove.Any();

            if (isRemoving)
            {
                lock (cpEl)
                {
                    foreach (var item in toRemove)
                    {
                        cpEl.Remove(item);
                    }
                }
            }

            return isRemoving;
        }

        public async void RemoveElementAsync(IElement element)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    _elements.Remove(element);
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

        public async void AddEditingElementsToGroupAsync()
        {
            await Task.Run(() =>
            {
                if (_editingElements != null)
                {
                    VGroup gr = new(this._camera);
                    lock (_editingElementsLocker)
                    {
                        foreach (var item in _editingElements)
                        {
                            gr.AddElement(item);
                            this.RemoveElementAsync(item);
                        }
                    }
                    _editingElements = null;
                    this.AddElementAsync(gr);
                    SendSignalToRender();
                }
            });
        }

        public async void UngroupingEditingElementsAsync()
        {
            await Task.Run(() =>
            {
                if (_editingElements != null)
                {
                    bool f = false;
                    foreach (var elem in _editingElements)
                    {
                        if (elem is VGroup grp)
                        {
                            foreach (var item in grp.Elements)
                            {
                                this.AddElementAsync(item);
                            }
                            this.RemoveElementAsync(grp);
                            f = true;
                        }
                    }
                    if (f)
                    {
                        SendSignalToRender();
                    }
                }
            });
        }
        #endregion

        #region async render and background processes
        public async void InitAsyncRender()
        {
            await Task.Run(() =>
            {
                SendSignalToRender();

                int minInterval = 33;

                while (!_isEngineStop)
                {
                    if (_renderCounter >= minInterval && (_isRenderRequired || EditingElements != null))
                    {
                        lock (_renderCounterLocker)
                        {
                            _isRenderRequired = false;
                            _renderCounter = 0;
                        }

                        Render();
                        UpdateElementsList();
                    }
                    else if (_renderCounter > 5000)
                    {
                        Optimization();
                    }

                    Thread.Sleep(minInterval);

                    lock (_renderCounterLocker)
                    {
                        _renderCounter += minInterval;
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
                bool isOptimized = _optimizator.Optimization(this._camera, this._elements);

                if (isOptimized)
                {
                    SendSignalToRender();
                }
            });
        }

        public async void StopEngine()
        {
            await Task.Run(() =>
            {
                _isEngineStop = true;
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
            
            List<IElement> cpEl;

            // copy elements
            lock (_elements)
            {
                cpEl = new List<IElement>(_elements);
            }

            btmp = DrawingEngineModule.Draw(btmp, cpEl, EditingElements);

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

        #region element selection
        public void SelectElement(int x, int y, int eps, bool append)
        {
            lock (_elements)
            {
                bool f = false;
                foreach (IElement element in _elements)
                {
                    if (CheckElementsForSelectionR(element, x, y, eps))
                    {
                        if (EditingElements is null || !append)
                        {
                            EditingElements = new();
                            EditingElements.Add(element);
                        }
                        else if (append && EditingElements.IndexOf(element) == -1)
                        {
                            EditingElements.Add(element);
                        }
                        f = true;
                        break;
                    }
                }
                if (!f)
                {
                    EditingElements = null;
                    SendSignalToRender();
                }                
            }
        }

        private bool CheckElementsForSelectionR(IElement element, int x, int y, int eps)
        {
            if (element is VLine ln)
            {
                if (IsPointOnLine(x, y, eps, ln))
                {
                    return true;
                }
            }
            else if (element is VGroup group)
            {
                foreach (var item in group.Elements)
                {
                    bool f = CheckElementsForSelectionR(item, x, y, eps);
                    if (f)
                    {
                        return true;
                    }
                }
            }
            return false;
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

        private bool IsPointOnLine(int x, int y, int eps, VLine ln)
        {
            return Math.Abs(L(x, y, ln.Point1.RenderX, ln.Point1.RenderY) + L(x, y, ln.Point2.RenderX, ln.Point2.RenderY) 
                - L(ln.Point1.RenderX, ln.Point1.RenderY, ln.Point2.RenderX, ln.Point2.RenderY)) <= eps;
        }

        private double L(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        #region
        public async void SaveElementsAsync(string path)
        {
            await Task.Run(() =>
            {
                lock (_elements)
                {
                    ElementsSaver.SavePoints(_elements, path);
                }
            });
        }

        public void LoadElements(string path)
        {
            List<IElement> collection = ElementsSaver.LoadPoints(path);
            lock (_elements)
            {
                _elements = collection;
            }
        }
        #endregion
    }
}
