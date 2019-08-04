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
using System.Windows.Forms;
using AutoBuyer.Core.Data;

namespace AutoBuyer.App.Views
{
    /// <summary>
    /// Interaction logic for Calibrator.xaml
    /// </summary>
    public partial class Calibrator : Window
    {
        public Calibrator()
        {
            InitializeComponent();

            var screenBounds = Screen.PrimaryScreen.Bounds;
            Top = screenBounds.Height / 3;
            Left = 0;
            Width = screenBounds.Width / 3;
            Height = screenBounds.Height / 3;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Close any instances of Firefox then press OK.");
            var playerView = new PlayerSelectView(new Logger());
            playerView.Show();
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var position = System.Windows.Forms.Cursor.Position;
                txtCoordinates.Text = $"X = {position.X} | Y = {position.Y}";
            }
        }
    }
}