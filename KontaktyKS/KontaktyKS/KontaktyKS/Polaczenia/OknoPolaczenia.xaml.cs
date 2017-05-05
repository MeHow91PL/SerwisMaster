using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Text.RegularExpressions;
using SerwisMaster.Klasy_połączenia;

namespace SerwisMaster
{
    /// <summary>
    /// Interaction logic for Dodaj_polaczenie.xaml
    /// </summary>
    public partial class OknoPolaczenia : Window
    {
        string klientId = null;
        string thisRemoteId = null;
        protected OknoPolaczenia() 
        {
            InitializeComponent();

           

        }

        public OknoPolaczenia(string klientId)
        {
            InitializeComponent();
            this.klientId = klientId;
        }
        public OknoPolaczenia(string klientId, string thisRemoteId)
        {
            InitializeComponent();
            this.klientId = klientId;
            this.thisRemoteId = thisRemoteId;
        }



        private void rodzajComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded == true)
            {
                if (rodzajComboBox.SelectedItem.ToString().Contains("RDP"))
                {
                    rdpLayout();
                }
                else if (rodzajComboBox.SelectedItem.ToString().Contains("TeamViewer"))
                {
                    teamViewerLeyout();
                }
            }
        }

        private void teamViewerLeyout()
        {
            nazwaLabel.Margin = new Thickness(36, 112, 0, 0);
            nazwaTextBox.Margin = new Thickness(93, 113, 0, 0);

            adresLabel.Visibility = System.Windows.Visibility.Hidden;
            adresTextBox.Visibility = System.Windows.Visibility.Hidden;

            loginLabel.Content = "Id";
            loginLabel.Margin = new Thickness(62, 157, 0, 0);
            loginTextBox.Margin = new Thickness(93, 158, 0, 0);

            hasloLabel.Margin = new Thickness(42, 203, 0, 0);
            hasloTextBox.Margin = new Thickness(93, 204, 0, 0);
        }

        private void rdpLayout()
        {
            nazwaLabel.Margin = new Thickness(36, 104, 0, 0);
            nazwaTextBox.Margin = new Thickness(93, 105, 0, 0);

            adresLabel.Visibility = System.Windows.Visibility.Visible;
            adresTextBox.Visibility = System.Windows.Visibility.Visible;

            loginLabel.Content = "Login";
            loginLabel.Margin = new Thickness(41, 184, 0, 0);
            loginTextBox.Margin = new Thickness(93, 185, 0, 0);

            hasloLabel.Margin = new Thickness(42, 224, 0, 0);
            hasloTextBox.Margin = new Thickness(93, 225, 0, 0);
        }

        private void anulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void titlePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void nazwaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapisz();
        }
        private void loginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapisz();
        }

        private void hasloTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapisz();
        }

        private void adresTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                zapisz();
        }

        private void OkBT_Click(object sender, RoutedEventArgs e)
        {
            zapisz();
        }

        private void zapisz()
        {
            Regex reg = new Regex(@"^[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9 ]*$");

            if (string.IsNullOrWhiteSpace(rodzajComboBox.Text) ||
                string.IsNullOrWhiteSpace(nazwaTextBox.Text) ||
                string.IsNullOrWhiteSpace(loginTextBox.Text) ||
                string.IsNullOrWhiteSpace(hasloTextBox.Text))
            {
                MessageBox.Show("Uzupełnij wszystkie pola!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(!reg.IsMatch(nazwaTextBox.Text))
            {
                MyMessageBox.Show("Wprowadzono nieprawidłowe znaki w pole nazwy");
            }
            else
            {
              

                if (thisRemoteId == null)// dodanie nowego połączenia
                {
                    //############# DODAJ TEAMVIEWER ###############################################################################
                    if (rodzajComboBox.Text == "TeamViewer")
                    {
                        Serializator.serializuj(new TeamViewer(nazwaTextBox.Text, klientId, "", hasloTextBox.Text, "TeamViewer", loginTextBox.Text));
                    }

                    //############# DODAJ RDP ###############################################################################
                    else if (rodzajComboBox.Text == "RDP")
                    {
                        if (!string.IsNullOrWhiteSpace(adresTextBox.Text))
                        {
                            Serializator.serializuj(new Rdp(nazwaTextBox.Text, klientId, "", hasloTextBox.Text, "Rdp", adresTextBox.Text, loginTextBox.Text));
                        }
                        else
                        {
                            MessageBox.Show("Uzupełnij wszystkie pola!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }
                else// edycja połączenia
                {
                    XmlDocument xml = new XmlDocument();

                    xml.Load(Properties.Settings.Default.baseXmlPath);

                    XmlNodeList connectionsNodes = null;

                    if (xml.ChildNodes[0] != null)
                    {
                        connectionsNodes = xml.ChildNodes[0].ChildNodes;
                    }
                    else
                    {
                        MyMessageBox.Show("Plik XML jest uszkodzony, brakuje głównego węzła Connections");
                    }

                    XmlNodeList remoteNodes;
                    if (rodzajComboBox.SelectedIndex == 0)
                        remoteNodes = xml.GetElementsByTagName("TeamViewer");
                    else
                        remoteNodes = xml.GetElementsByTagName("Rdp");

                    foreach (XmlNode node in remoteNodes)
                    {
                        if (node.Attributes["Id"].InnerText == thisRemoteId)
                        {
                            if (node.Attributes["Type"].InnerText == "TeamViewer")
                            {
                                node.Attributes["Name"].InnerText = nazwaTextBox.Text;
                                node.Attributes["Password"].InnerText = hasloTextBox.Text;
                                node.Attributes["TeamViewerId"].InnerText = loginTextBox.Text;
                            }
                            else if (node.Attributes["Type"].InnerText == "Rdp")
                            {
                                node.Attributes["Name"].InnerText = nazwaTextBox.Text;
                                node.Attributes["Address"].InnerText = adresTextBox.Text;
                                node.Attributes["Login"].InnerText = loginTextBox.Text;
                                node.Attributes["Password"].InnerText = hasloTextBox.Text;
                            }
                        }
                    }
                 xml.Save(Properties.Settings.Default.baseXmlPath);
                }

                MainWindow.aktualizujTreeView(MainWindow.listOfClients);
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (rodzajComboBox.IsEnabled)
                this.rodzajComboBox.Focus();
            else
                this.nazwaTextBox.Focus();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                wspolneOpcjeOkienek.zamknijOkienko(this);
            }

        }

        private void adresTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (rodzajComboBox.SelectedItem.ToString().Contains("RDP"))
            {
                rdpLayout();
            }
            else if (rodzajComboBox.SelectedItem.ToString().Contains("TeamViewer"))
            {
                teamViewerLeyout();
            }
        }

        private void nazwaTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
