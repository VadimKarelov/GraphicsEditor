using GraphicsEditor.Modules;
using GraphicsEditor.Modules.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GraphicsEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Media.Brush _defaultButtonBrush;
        private System.Windows.Media.Brush _selectedButtonBrush;

        private System.Drawing.Color _selectedColor;
        private Instrument _selectedInstrument;
        private Plane _selectedPlane;

        private Engine _engine;
        private DispatcherTimer _renderTimer;

        private IElement? _editingElement;

        #region initialization
        public MainWindow()
        {
            InitializeComponent();

            _engine = new Engine(1, 1);

            _selectedColor = System.Drawing.Color.Black;

            _defaultButtonBrush = bt_arrow.Background;
            _selectedButtonBrush = new SolidColorBrush(Colors.Orange);

            _renderTimer = new DispatcherTimer();
            _renderTimer.Tick += UpdateField;
            _renderTimer.Interval = TimeSpan.FromMilliseconds(20);
            _renderTimer.Start();

            _engine.InitAsyncRender();

            ChangeInstrument_Click(bt_arrow, new RoutedEventArgs());
            ChangePlane_Click(bt_XY, new RoutedEventArgs());

            DelayStartAsync();
        }

        private async void DelayStartAsync()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(50);
                ChangeFieldSize();
            });
        }
        #endregion

        #region background proccesses
        private void UpdateField(object? sender, EventArgs e)
        {
            field.Source = _engine.BitmapImage;
        }
        #endregion

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(field);

            switch (_selectedInstrument)
            {
                case Instrument.Pen:
                    {
                        _engine.AddPointAsync((int)pos.X, (int)pos.Y, (int)sl_Size.Value, _selectedColor);
                        break;
                    }
                case Instrument.Line:
                    {
                        VLine line = new VLine(_selectedColor, (int)pos.X, (int)pos.Y, (int)pos.X, (int)pos.Y, (int)sl_Size.Value);
                        _editingElement = line;
                        _engine.AddLineAsync(line);
                        break;
                    }
            }            
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(field);
            DrawCursorPosition(pos);

            switch (_selectedInstrument)
            {
                case Instrument.Pen:
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            _engine.AddPointAsync((int)pos.X, (int)pos.Y, (int)sl_Size.Value, _selectedColor);
                        }
                        break;
                    }
                case Instrument.Line:
                    {
                        if (_editingElement != null && _editingElement is VLine ln)
                        {
                            ln.X2 = (int)pos.X;
                            ln.Y2 = (int)pos.Y;
                        }
                        break;
                    }
            }            
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(field);

            switch (_selectedInstrument)
            {
                case Instrument.Pen:
                    {
                        break;
                    }
                case Instrument.Line:
                    {
                        if (_editingElement != null && _editingElement is VLine ln)
                        {
                            ln.X2 = (int)pos.X;
                            ln.Y2 = (int)pos.Y;
                            _editingElement = null;
                        }
                        break;
                    }
            }
        }

        #region planes
        private async void DrawCursorPosition(Point pos)
        {
            int x = -1, y = -1, z = -1;
            await Task.Run(() =>
            {
                GetCoordsFromMousePosition(pos, ref x, ref y, ref z);
            });
            status_CursorPosition.Text = $"{x};{y};{z}";
        }

        private void GetCoordsFromMousePosition(Point pos, ref int x, ref int y, ref int z)
        {
            x = -1; y = -1; z = -1;
            switch (_selectedPlane)
            {
                case Plane.XY: x = (int)pos.X; y = (int)pos.Y; z = 0; break;
                case Plane.XZ: x = (int)pos.X; y = 0; z = (int)pos.Y; break;
                case Plane.YZ: x = 0; y = (int)pos.X; z = x = (int)pos.X; break;
            }
        }

        enum Plane
        {
            XY,
            XZ,
            YZ
        }

        private void ChangePlane_Click(object sender, RoutedEventArgs e)
        {
            string t = ((Button)sender).Tag.ToString();

            switch (t)
            {
                case "XY": _selectedInstrument = Instrument.Nothing; break;
                case "XZ": _selectedInstrument = Instrument.Pen; break;
                case "YZ": _selectedInstrument = Instrument.Eraser; break;
            }

            SetPlanesButtonsInactive();
            ((Button)sender).Background = _selectedButtonBrush;
        }

        private void SetPlanesButtonsInactive()
        {
            bt_XY.Background = _defaultButtonBrush;
            bt_XZ.Background = _defaultButtonBrush;
            bt_YZ.Background = _defaultButtonBrush;
        }
        #endregion

        #region window size settings
        private void ChangeFieldSize()
        {
            _engine.ChangeFieldSizeAsync((int)uou.ActualWidth, (int)uou.ActualHeight);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeFieldSize();
        }
        #endregion

        private void SelectColor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _selectedColor = colorDialog.Color;

                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color);
                SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                ((Button)sender).Background = solidColorBrush;
            }
        }

        #region instrument selection
        enum Instrument
        {
            Nothing,
            Pen,
            Eraser,
            Line
        }

        private void ChangeInstrument_Click(object sender, RoutedEventArgs e)
        {
            string t = ((Button)sender).Tag.ToString();

            switch (t)
            {
                case "nothing": _selectedInstrument = Instrument.Nothing; break;
                case "pen": _selectedInstrument = Instrument.Pen; break;
                case "eraser": _selectedInstrument = Instrument.Eraser; break;
                case "line": _selectedInstrument = Instrument.Line; break;
            }

            SetButtonsInactive();
            ((Button)sender).Background = _selectedButtonBrush;
        }

        private void SetButtonsInactive()
        {
            bt_arrow.Background = _defaultButtonBrush;
            bt_pen.Background = _defaultButtonBrush;
            bt_eraser.Background = _defaultButtonBrush;
            bt_line.Background = _defaultButtonBrush;
        }
        #endregion
    }
}
