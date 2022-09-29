using GraphicsEditor.Modules;
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
        private System.Drawing.Color _selectedColor;
        private Engine _engine;
        private DispatcherTimer _renderTimer;
        private bool _isStarted;

        public MainWindow()
        {
            InitializeComponent();

            _isStarted = false;

            _engine = new Engine(1, 1);

            _selectedColor = System.Drawing.Color.Black;

            _renderTimer = new DispatcherTimer();
            _renderTimer.Tick += UpdateField;
            _renderTimer.Interval = TimeSpan.FromMilliseconds(20);
            _renderTimer.Start();

            _engine.InitAsyncRender();

            DelayStartAsync();
        }

        private async void DelayStartAsync()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(10);
                ChangeFieldSize();
            });
        }

        private void UpdateField(object? sender, EventArgs e)
        {
            field.Source = _engine.BitmapImage;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(field);
            status_CursorPosition.Text = $"{(int)pos.X};{(int)pos.Y}";

            _engine.AddPointAsync((int)pos.X, (int)pos.Y, 10, _selectedColor);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(field);
            status_CursorPosition.Text = $"{(int)pos.X};{(int)pos.Y}";

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _engine.AddPointAsync((int)pos.X, (int)pos.Y, 10, _selectedColor);
            }
        }

        private void ChangeFieldSize()
        {
            _engine.ChangeFieldSizeAsync((int)uou.ActualWidth, (int)uou.ActualHeight);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ChangeFieldSize();
        }        
    }
}
