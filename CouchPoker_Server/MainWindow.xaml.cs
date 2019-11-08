using CouchPoker_Server.Management;
using CouchPoker_Server.Networking;
using CouchPoker_Server.Player;
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
        List<UserData> usersHistory;
        private int currentPlayer = 0;
        private bool warunek = true;

        public MainWindow()
        {
            InitializeComponent();
            usersHistory = new List<UserData>()
            {
                new UserData(){ ballance=0, uID="", username="" },
                new UserData(){ ballance=1000, uID="abcd-123f-123da-wesf", username="jerne" },
                new UserData(){ ballance=2900, uID="dsaw-123f-123da-wesf", username="player2" },
                new UserData(){ ballance=31245, uID="123r-123f-123da-wesf", username="xdddde" }
            };

            users = new List<UserHandler>()
            {
                new UserHandler(UserSlot_1, usersHistory[1]),
                new UserHandler(UserSlot_2, usersHistory[2]),
                new UserHandler(UserSlot_3, usersHistory[3]),
                new UserHandler(UserSlot_4, usersHistory[0]),
                new UserHandler(UserSlot_5, usersHistory[0]),
                new UserHandler(UserSlot_6, usersHistory[0]),
                new UserHandler(UserSlot_7, usersHistory[0])
            };
            users[0].Status = STATUS.MY_TURN;

            Worker.Run();
            JoiningManagement.Run(users, usersHistory);

            /*while (warunek)
            {
                users[currentPlayer].Status = STATUS.MY_TURN;

            }*/
        }
    }
}
