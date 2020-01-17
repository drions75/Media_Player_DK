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

namespace Film_Player
{
    /// <summary>
    /// Logika interakcji dla klasy Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info(string info)
        {
            InitializeComponent();
            infoBox.AppendText(info);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
