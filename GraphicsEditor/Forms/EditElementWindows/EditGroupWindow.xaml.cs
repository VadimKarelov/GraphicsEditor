using GraphicsEditor.Modules.Tools;
using GraphicsEditor.Modules;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GraphicsEditor.Modules.Elements;
using System.Windows.Threading;

namespace GraphicsEditor.Forms.EditElementWindows
{
    /// <summary>
    /// Логика взаимодействия для EditGroupWindow.xaml
    /// </summary>
    public partial class EditGroupWindow : Window
    {
        public VGroup? ResultGroup { get; set; }

        private Brush _tbBackground;
        private bool _isAnglesReady;
        private bool _isScaleReady;

        private Engine _engine;

        public EditGroupWindow(VGroup group)
        {
            InitializeComponent();
            _tbBackground = new SolidColorBrush(Colors.White);

            ResultGroup = group.Clone() as VGroup;

            _engine = new(500, 500);
            _engine.AddElementAsync(ResultGroup);
            _engine.EditingElements = new();
            _engine.EditingElements.Add(ResultGroup);

            tb_ax.Background = _tbBackground;
            tb_ay.Background = _tbBackground;
            tb_az.Background = _tbBackground;

            tb_sx.Background = _tbBackground;
            tb_sy.Background = _tbBackground;
            tb_sz.Background = _tbBackground;

            _engine.InitAsyncRender();

            DispatcherTimer timer = new DispatcherTimer();
            timer = new DispatcherTimer();
            timer.Tick += UpdatePreview;
            timer.Interval = TimeSpan.FromMilliseconds(33);
            timer.Start();
        }

        private void UpdatePreview(object? sender, EventArgs e)
        {
            img_preview.Source = _engine.BitmapImage;
            lb_elements.ItemsSource = _engine.StringElements;
        }

        #region text box events
        private void AngleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (double.TryParse(tb.Text, out double res))
                {
                    tb.Background = _tbBackground;
                    _isAnglesReady = true;
                }
                else
                {
                    tb.Background = new SolidColorBrush(Colors.Orange);
                    _isAnglesReady = false;
                }
            }
        }

        private void ScaleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (double.TryParse(tb.Text, out double res))
                {
                    tb.Background = _tbBackground;
                    _isScaleReady = true;
                }
                else
                {
                    tb.Background = new SolidColorBrush(Colors.Orange);
                    _isScaleReady = false;
                }
            }
        }
        #endregion

        #region button events
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
            _engine.StopEngine();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            ResultGroup = null;
            _engine.StopEngine();
            this.Close();
        }

        private void Rotate_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnglesReady)
            {
                double ax = double.Parse(tb_ax.Text);
                double ay = double.Parse(tb_ay.Text);
                double az = double.Parse(tb_az.Text);

                FindCenter(ResultGroup, out int cx, out int cy, out int cz);

                MGroup matr = new MGroup(ResultGroup);
                matr.Transition(-cx, -cy, -cz); // move to center
                matr.Rotation(ax, ay, az);      // rotate
                matr.Transition(cx, cy, cz);    // move back
                ResultGroup = matr.GetGroup();
            }
            else
            {
                MessageBox.Show("Данные введены неправильно");
            }
        }

        private void Scale_Click(object sender, RoutedEventArgs e)
        {
            if (_isScaleReady)
            {
                double sx = double.Parse(tb_sx.Text);
                double sy = double.Parse(tb_sy.Text);
                double sz = double.Parse(tb_sz.Text);

                FindCenter(ResultGroup, out int cx, out int cy, out int cz);

                MGroup matr = new MGroup(ResultGroup);
                matr.Transition(-cx, -cy, -cz); // move to center
                matr.Scaling(sx, sy, sz);       // scale
                matr.Transition(cx, cy, cz);    // move back
                ResultGroup = matr.GetGroup();
            }
            else
            {
                MessageBox.Show("Данные введены неправильно");
            }
        }

        private void Reflect_Click(object sender, RoutedEventArgs e)
        {
            FindCenter(ResultGroup, out int cx, out int cy, out int cz);

            string tag = ((Button)sender).Tag.ToString();
            bool rx = tag == "reflectionX";
            bool ry = tag == "reflectionY";
            bool rz = tag == "reflectionZ";

            MGroup matr = new MGroup(ResultGroup);
            matr.Transition(-cx, -cy, -cz);     // move to center
            matr.Reflection(rx, ry, rz);        // reflect
            matr.Transition(cx, cy, cz);        // move back
            ResultGroup = matr.GetGroup();
        }
        #endregion

        private void FindCenter(VGroup grp, out int cx, out int cy, out int cz)
        {
            int maxX = -1000, maxY = -1000, maxZ = -1000;
            int minX = 10000, minY = 10000, minZ = 10000;

            List<IElement> elements = new List<IElement>(grp.Elements);

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is VGroup group)
                {
                    elements.AddRange(group.Elements);
                }
                else if (elements[i] is VLine ln)
                {
                    More(ln.Point1, ref maxX, ref maxY, ref maxZ);
                    More(ln.Point2, ref maxX, ref maxY, ref maxZ);
                    Less(ln.Point1, ref minX, ref minY, ref minZ);
                    Less(ln.Point2, ref minX, ref minY, ref minZ);
                }
            }

            cx = (minX + maxX) / 2;
            cy = (minY + maxY) / 2;
            cz = (minZ + maxZ) / 2;
        }

        private void More(TDPoint pt, ref int maxX, ref int maxY, ref int maxZ)
        {
            if (pt.X > maxX) maxX = pt.X;
            if (pt.Y > maxY) maxY = pt.Y;
            if (pt.Z > maxZ) maxZ = pt.Z;
        }

        private void Less(TDPoint pt, ref int minX, ref int minY, ref int minZ)
        {
            if (pt.X < minX) minX = pt.X;
            if (pt.Y < minY) minY = pt.Y;
            if (pt.Z < minZ) minZ = pt.Z;
        }

        private void FindMaxMin_R(List<IElement> elements, ref int maxX, ref int minX,
            ref int maxY, ref int minY, ref int maxZ, ref int minZ)
        {
            foreach (IElement element in elements)
            {
                if (element is VLine ln)
                {

                }
            }
        }
    }
}
