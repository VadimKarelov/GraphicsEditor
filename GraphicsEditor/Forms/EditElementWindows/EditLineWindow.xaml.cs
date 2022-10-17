using GraphicsEditor.Modules.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GraphicsEditor.Forms.Styles.EditElementWindows
{
    /// <summary>
    /// Логика взаимодействия для EditLineWindow.xaml
    /// </summary>
    public partial class EditLineWindow : Window
    {
        public VLine? ResultLine { get; set; }

        private Brush tbBackground;

        public EditLineWindow(VLine line)
        {
            InitializeComponent();

            tbBackground = tb_A.Background;

            SetFields(line);
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

        private void CountCoefficients(VLine ln)
        {

        }

        private void CountCoordinates()
        {

        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (int.TryParse(tb.Text, out int res))
                {
                    tb.Background = tbBackground;
                }
                else
                {
                    tb.Background = new SolidColorBrush(Colors.Orange);
                }
            }
        }
    }
}
