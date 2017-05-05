using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml;
using Microsoft.Win32;
using KontaktyKS.Klasy_połączenia;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Timers;
using System.Threading;

namespace KontaktyKS
{


    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TreeView listOfClients;
        bool isMaximalize = false;
        List<Folder> allItems = null;

        public MainWindow()
        {

            InitializeComponent();

            listOfClients = this.listaKlientowTreeView;

            createContextMenu();

            dropShadow(1);

            //downloadNewClientsFromServer();
            aktualizujTreeView(listOfClients);
            allItems = getAllItemsOfTreeView();

            //cv = (CollectionView)CollectionViewSource.GetDefaultView(listaKlientowTreeView.ItemsSource);
          
            //cv.Filter = filter;

        }

        private void createContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem[] mi = new MenuItem[2];

            for (int i = 0; mi.Length > i; i++)
                mi[i] = new MenuItem();

            mi[0].Header = "Dodaj klienta";
            mi[0].Click += Klient.DodajKlienta;

            mi[1].Header = "Dodaj folder";
            Folder temp = new Folder();
            mi[1].Click += temp.DodajFolder;

            for (int i = 0; mi.Length > i; i++)
                cm.Items.Add(mi[i]);

            listaKlientowTreeView.ContextMenu = cm;
        }

        private void downloadNewClientsFromServer()
        {
            DirectoryInfo dir = new DirectoryInfo(Properties.Settings.Default.DataBasePath);
            FileInfo[] files = dir.GetFiles("*.xml");

            foreach (FileInfo f in files)
                File.Copy(f.FullName, @"C:\kontaktyKS\xml\" + f.Name, true);
        }

        public static void aktualizujTreeView(TreeView listaKlientow)
        {
            Queue<Folder> queueBeforeRefresh = new Queue<Folder>();
            Queue<Folder> queueAfterRefresh = new Queue<Folder>();

            //zapisuje klientów, którzy mają rozwiniętą listę połaczeń
            List<string> expandedItems = new List<string>();
            foreach (Folder f in listaKlientow.Items)
            {
                queueBeforeRefresh.Enqueue(f);
            }

            while (queueBeforeRefresh.Count > 0)
            {
                Folder tempFolder = queueBeforeRefresh.Dequeue();
                if (tempFolder.IsExpanded)
                    expandedItems.Add(tempFolder.id);
                if (tempFolder.HasItems)
                {
                    foreach (object f in tempFolder.Items)
                    {
                        if(f is Folder)
                        queueBeforeRefresh.Enqueue(f as Folder);
                    }
                }
            }

            listaKlientow.ItemsSource = null;

            listaKlientow.ItemsSource = Serializator.deserializuj(Properties.Settings.Default.baseXmlPath);

            

            


            //odczytuje klientów, którzy mają rozwiniętą listę połaczeń
            foreach (Folder f in listaKlientow.Items)
            {
                queueAfterRefresh.Enqueue(f);
            }


            while (queueAfterRefresh.Count > 0)
            {
                Folder tempFolder = queueAfterRefresh.Dequeue();
                
                if (expandedItems.Exists(f => f == tempFolder.id))
                    tempFolder.IsExpanded = true;
                if (tempFolder.HasItems)
                {
                    foreach (object f in tempFolder.Items)
                    {
                        
                        if(f is Folder)
                        queueAfterRefresh.Enqueue(f as Folder);
                    }
                }

            }
        }

        private void dockPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show((sender as TreeViewItem).Parent.ToString());
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("");
        }

        private void dropShadow(double blur)
        {
            tlo.Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                ShadowDepth = 0,
                RenderingBias = System.Windows.Media.Effects.RenderingBias.Performance,
                BlurRadius = blur,
                Direction = 0,
                Opacity = 1
            };
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            dropShadow(2);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            dropShadow(20);
        }

        private void listaKlientowTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (listaKlientowTreeView.SelectedItem is Klient)
            {
                maileView.ItemsSource = (listaKlientowTreeView.SelectedItem as Klient).emailList;
                telefonyView.ItemsSource = (listaKlientowTreeView.SelectedItem as Klient).telefonList;
                haslaView.ItemsSource = (listaKlientowTreeView.SelectedItem as Klient).daneLogowaniaList;
                checkListViews();
                dodajKlientaButtonDown.IsEnabled = false;

            }
            else if (listaKlientowTreeView.SelectedItem is Folder)
            {
                dodajKlientaButtonDown.IsEnabled = true;
            }
            else if (listaKlientowTreeView.SelectedItem is Polaczenie)
            {
                Klient k = (listaKlientowTreeView.SelectedItem as Polaczenie).Parent as Klient;
                maileView.ItemsSource = k.emailList;
                telefonyView.ItemsSource = k.telefonList;
                haslaView.ItemsSource = k.telefonList;
                dodajKlientaButtonDown.IsEnabled = false;


                checkListViews();
            }
            //else
            //{
            //    maileView.ItemsSource = telefonyView.ItemsSource = haslaView.ItemsSource = null;
            //    checkListViews();
            //}

        }

        private void checkListViews()
        {
            if (maileView.Items.Count > 0) emailsExpander.IsExpanded = true;
            else emailsExpander.IsExpanded = false;

            if (telefonyView.Items.Count > 0) phonesExpander.IsExpanded = true;
            else phonesExpander.IsExpanded = false;

            if (haslaView.Items.Count > 0) credentialsExpander.IsExpanded = true;
            else credentialsExpander.IsExpanded = false;
        }

        private void databasePathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Opcje opcje = new Opcje();
            opcje.ShowDialog();
        }

        private void minimalizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }
     

        private void maileView_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (maileView.SelectedItem != null)
                Clipboard.SetText((maileView.SelectedItem as Email).adresEmail);
        }

        private void telefonyView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void haslaView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (haslaView.SelectedItem != null)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                { MyMessageBox.Show(""); }
            }
            //    Clipboard.SetText((maileView.SelectedItem as Email).adresEmail);
        }

        private void maximalizeButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.isMaximalize)
            {
                this.Width = Properties.Settings.Default.primaryWidth;
                this.Height = Properties.Settings.Default.primaryHeight;
                this.Left = Properties.Settings.Default.MinimalizeWindowPosition.X;
                this.Top = Properties.Settings.Default.MinimalizeWindowPosition.Y;
                this.maximalizeButton.Content = new Border()
                {
                    BorderBrush = Brushes.White,
                    BorderThickness = new Thickness(1.5),
                    Width = 13,
                    Height = 13
                };

                this.isMaximalize = !this.isMaximalize;
            }
            else
            {
                Properties.Settings.Default.MinimalizeWindowPosition = new Point(App.Current.MainWindow.Left, App.Current.MainWindow.Top);
                this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
                this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                this.Left = 0;
                this.Top = 0;
                this.maximalizeButton.Content = 'm';

                this.isMaximalize = !this.isMaximalize;
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(listaKlientowTreeView.ItemsSource != null)
            //CollectionViewSource.GetDefaultView(listaKlientowTreeView.ItemsSource).Refresh();
            TextBox tb = sender as TextBox;

            if (allItems != null && tb.Text != "      szukaj...")
            {
                if (!string.IsNullOrWhiteSpace(searchFolderTextBox.Text))
                {
                    listaKlientowTreeView.ItemsSource = allItems;

                    foreach (Folder f in allItems)
                    {
                        if (f.nazwa.ToUpper().Contains(searchFolderTextBox.Text.ToUpper()))
                            f.Visibility = System.Windows.Visibility.Visible;
                        else
                            f.Visibility = System.Windows.Visibility.Collapsed;
                    }

                }
                else
                {
                    listaKlientowTreeView.ItemsSource = Serializator.deserializuj(Properties.Settings.Default.baseXmlPath);

                }
            }
        }

        private List<Folder> getAllItemsOfTreeView()
        {
            Queue<Folder> queue = new Queue<Folder>();
            List<Folder> allItems = new List<Folder>();

            foreach (Folder f in listaKlientowTreeView.Items)
                queue.Enqueue(f);

            while (queue.Count > 0)
            {
                Folder temp = queue.Dequeue();
                if (temp.HasItems)
                {
                    foreach (object i in temp.Items)
                    {
                        if(i is Folder)
                        queue.Enqueue(i as Folder);
                    }

                    temp.Items.Clear();
                }
                allItems.Add(temp);
            }
            MainWindow.aktualizujTreeView(listOfClients);
            return allItems;
        }

        private void searchClearButton_Click(object sender, RoutedEventArgs e)
        {
            searchFolderTextBox.Text = "";
            searchFolderTextBox.Focus();
        }

        private void importSWX_Click(object sender, RoutedEventArgs e)
        {
            Window1 w = new Window1();
            w.Show();
            
        }

        private void importRDM_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Plik bazy danych RDM|*.rdm|Plik XML|*.xml";
            bool? userClickedOK = fileDialog.ShowDialog();
            if (userClickedOK == true)
            {
                string rdmDatabasePath = fileDialog.FileName;
                ImportRemoteDesktopManager.importRDM(rdmDatabasePath);
            }
        }

        private void searchFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;


           if(tb.Text == "      szukaj...")
            {
                tb.Text = "";
                tb.Background.Opacity = 0;
                tb.Foreground.Opacity = 1;
            }
        }

        private void searchFolderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Foreground.Opacity = 0.7;
                tb.Text = "      szukaj...";
                tb.Background.Opacity = 0.5;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {

        }

        private void dodajKlientaButtonDown_Click(object sender, RoutedEventArgs e)
        {
            if ((listaKlientowTreeView.SelectedItem is Klient) && listaKlientowTreeView.SelectedItem is Folder)
                Klient.DodajKlienta(this.listaKlientowTreeView.SelectedItem, e);
            else if (listaKlientowTreeView.SelectedItem == null)
                Klient.DodajKlienta(this.listaKlientowTreeView, e);
        }

        //private bool filter(object obj)
        //{

        //    if(string.IsNullOrEmpty(searchFolderTextBox.Text))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return (obj as Folder).nazwa.ToUpper().Contains(searchFolderTextBox.Text.ToUpper());
        //    }

        //}
    }
}