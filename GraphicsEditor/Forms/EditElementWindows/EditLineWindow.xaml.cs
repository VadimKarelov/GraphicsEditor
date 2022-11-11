using GraphicsEditor.Modules;
using GraphicsEditor.Modules.Elements;
using GraphicsEditor.Modules.Tools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GraphicsEditor.Forms.Styles.EditElementWindows
{
    /// <summary>
    /// Логика взаимодействия для EditLineWindow.xaml
    /// </summary>
    public partial class EditLineWindow : Window
    {
        public VLine? ResultLine { get; set; }

        private Brush _tbBackground;
        private bool _isReady;
        private bool _isAnglesReady;
        private bool _isScaleReady;

        private Engine _engine;

        public EditLineWindow(VLine line)
        {
            InitializeComponent();
            _tbBackground = tb_x1.Background;

            ResultLine = line.Clone() as VLine;

            _engine = new(500, 500);
            _engine.AddElementAsync(ResultLine);
            _engine.EditingElements = new();
            _engine.EditingElements.Add(ResultLine);

            SetFields(line);

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

        private void SetFields(VLine ln)
        {
            tb_x1.Text = ln.Point1.X.ToString();
            tb_y1.Text = ln.Point1.Y.ToString();
            tb_z1.Text = ln.Point1.Z.ToString();

            tb_x2.Text = ln.Point2.X.ToString();
            tb_y2.Text = ln.Point2.Y.ToString();
            tb_z2.Text = ln.Point2.Z.ToString();

            tb_size.Text = ln.Size.ToString();

            bt_color.Background = new SolidColorBrush(Color.FromArgb(ln.Color.A, ln.Color.R, ln.Color.G, ln.Color.B));
        }

        private void SetValueToLine(string tag, int value)
        {
            switch (tag)
            {
                case "x1": ResultLine.Point1.X = value; break;
                case "y1": ResultLine.Point1.Y = value; break;
                case "z1": ResultLine.Point1.Z = value; break;
                case "x2": ResultLine.Point2.X = value; break;
                case "y2": ResultLine.Point2.Y = value; break;
                case "z2": ResultLine.Point2.Z = value; break;
                case "size": ResultLine.Size = value; break;
            }
        }

        private void UpdatePreview(object? sender, EventArgs e)
        {
            img_preview.Source = _engine.BitmapImage;
        }

        #region text box events
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (int.TryParse(tb.Text, out int res))
                {
                    tb.Background = _tbBackground;
                    _isReady = true;
                    SetValueToLine(tb.Tag.ToString(), res);
                }
                else
                {
                    tb.Background = new SolidColorBrush(Colors.Orange);
                    _isReady = false;
                }
            }
        }

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
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ResultLine.Color = colorDialog.Color;

                System.Drawing.SolidBrush sb = new System.Drawing.SolidBrush(colorDialog.Color);
                SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                ((Button)sender).Background = solidColorBrush;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_isReady)
            {
                DialogResult = true;
                this.Close();
                _engine.StopEngine();
            }
            else
            {
                MessageBox.Show("Данные введены неправильно");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            ResultLine = null;
            _isReady = true;
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

                int cx = (ResultLine.Point1.X + ResultLine.Point2.X) / 2;
                int cy = (ResultLine.Point1.Y + ResultLine.Point2.Y) / 2;
                int cz = (ResultLine.Point1.Z + ResultLine.Point2.Z) / 2;

                M matr = new M(ResultLine);
                matr.Transition(-cx, -cy, -cz); // move to center
                matr.Rotation(ax, ay, az);      // rotate
                matr.Transition(cx, cy, cz);    // move back
                ResultLine = matr.GetLine();

                SetFields(ResultLine);
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

                int cx = (ResultLine.Point1.X + ResultLine.Point2.X) / 2;
                int cy = (ResultLine.Point1.Y + ResultLine.Point2.Y) / 2;
                int cz = (ResultLine.Point1.Z + ResultLine.Point2.Z) / 2;

                M matr = new M(ResultLine);
                matr.Transition(-cx, -cy, -cz); // move to center
                matr.Scaling(sx, sy, sz);       // scale
                matr.Transition(cx, cy, cz);    // move back
                ResultLine = matr.GetLine();

                SetFields(ResultLine);
            }
            else
            {
                MessageBox.Show("Данные введены неправильно");
            }
        }

        private void Reflect_Click(object sender, RoutedEventArgs e)
        {
            int cx = (ResultLine.Point1.X + ResultLine.Point2.X) / 2;
            int cy = (ResultLine.Point1.Y + ResultLine.Point2.Y) / 2;
            int cz = (ResultLine.Point1.Z + ResultLine.Point2.Z) / 2;

            string tag = ((Button)sender).Tag.ToString();
            bool rx = tag == "reflectionX";
            bool ry = tag == "reflectionY";
            bool rz = tag == "reflectionZ";

            M matr = new M(ResultLine);
            matr.Transition(-cx, -cy, -cz);     // move to center
            matr.Reflection(rx, ry, rz);        // reflect
            matr.Transition(cx, cy, cz);        // move back
            ResultLine = matr.GetLine();

            SetFields(ResultLine);
        }
        #endregion
    }
}
