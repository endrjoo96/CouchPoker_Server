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
using System.Text.RegularExpressions;


namespace CouchPoker_Server
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public string ServerName {
            get { return Servername_TextBox.Text; }
        }
        public string Gamemode {
            get { return Gamemode_ComboBox.Text; }
        }

        public int SmallBlind {
            get { return Int32.Parse(Small_blind_TextBox.Text); }
        }

        public int StartupTokens {
            get { return Int32.Parse(StartupBallance_TextBox.Text); }
        }

        public SettingsWindow()
        {
            InitializeComponent();
            Gamemode_ComboBox.ItemsSource = new List<string>() { "Texas Hold'em" };
            Gamemode_ComboBox.SelectedIndex = 0;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
