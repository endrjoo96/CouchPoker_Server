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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CouchPoker_Server
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<UserHandler> users;
        private int currentPlayer = 0;
        private bool warunek = true;

        public MainWindow()
        {
            InitializeComponent();
            users = new List<UserHandler>()
            {
                new UserHandler(UserSlot_1, "xdddde"),
                new UserHandler(UserSlot_2, "jerneLeBoi"),
                new UserHandler(UserSlot_3, "a"),
                new UserHandler(UserSlot_4, "s"),
                new UserHandler(UserSlot_5, "d"),
                new UserHandler(UserSlot_6, "f"),
                new UserHandler(UserSlot_7, "g")
            };
            users[0].Status = STATUS.MY_TURN;
            /*while (warunek)
            {
                users[currentPlayer].Status = STATUS.MY_TURN;

            }*/
        }
    }
}
