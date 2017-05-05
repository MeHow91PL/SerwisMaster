using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SerwisMaster
{
    /// <summary>
    /// Interaction logic for DefaultPasswordsWindow.xaml
    /// </summary>
    public partial class DefaultPasswordsWindow : Window
    {
        public DefaultPasswordsWindow()
        {
            InitializeComponent();

            teamViewerPasswordTextBox.Text = Properties.Settings.Default.defaultPasswordOfTeamViewer;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save();
            }
            else if (e.Key == Key.Escape)
            {
            }
        }

        private void closeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Exit()
        {
                wspolneOpcjeOkienek.zamknijOkienko(this);
         
        }

        private void Save()
        {
            Properties.Settings.Default.defaultPasswordOfTeamViewer = teamViewerPasswordTextBox.Text;
            Properties.Settings.Default.Save();
            MainWindow.aktualizujTreeView(MainWindow.listOfClients);
            this.Close();
        }

        private void dockPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
