using Microsoft.Win32;
using SerwisMaster.BL;
using SerwisMaster.DAL;
using SerwisMaster.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace SerwisMaster
{


    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static IBazaDanych bazaDanych = new BazaLocalDb();
        public static TreeView listOfClients;
        bool isMaximalize = false;
        List<Folder> allItems = null;
        private static bool pierwszeLadowanieListy = true;
        public static Queue<Element> allElements { get; private set; }
        //Thread watekSprawdzaniaZmianBazyNaSerwerze;

        public MainWindow()
        {
            InitializeComponent();
            listaKlientowTreeView.ItemsSource = bazaDanych.PobierzWszystkieElementy();
        }

        

        
        //private void UruchomAplikacje()
        //{

        //    try
        //    {
        //        sprawdzenieBazyDanych();

        //        listOfClients = this.listaKlientowTreeView;

        //        createContextMenu();

        //        if (Properties.Settings.Default.previouslyWindowWidth > 100)
        //            this.Width = Properties.Settings.Default.previouslyWindowWidth;
        //        if (Properties.Settings.Default.previouslyWindowHeight > 100)
        //            this.Height = Properties.Settings.Default.previouslyWindowHeight;
        //        this.Left = Properties.Settings.Default.MinimalizeWindowPosition.X;
        //        this.Top = Properties.Settings.Default.MinimalizeWindowPosition.Y;

        //        dropShadow(1);
        //        allElements = new Queue<Element>();
        //        aktualizujTreeView(listOfClients);
        //        allItems = getAllItemsOfTreeView();
        //        watekSprawdzaniaZmianBazyNaSerwerze = new Thread(new ParameterizedThreadStart(SprawdzaniaZmianBazyNaSerwerze));
        //        watekSprawdzaniaZmianBazyNaSerwerze.Start(Properties.Settings.Default.baseXmlPath);
        //    }
        //    catch (Exception ex)
        //    {
        //        MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
        //    }
        //}

        private void SprawdzaniaZmianBazyNaSerwerze(object obj)
        {
            try
            {
                string databasePath = obj as string;
                FileInfo databaseFile = new FileInfo(databasePath);
                while (true)
                {
                    FileInfo newDatabaseFile = new FileInfo(databasePath);

                    if (databaseFile.LastWriteTime < newDatabaseFile.LastWriteTime || databaseFile.Length != newDatabaseFile.Length)
                    {
                        Thread.Sleep(1000);
                        while (true)
                        {
                            try
                            {
                                FileStream temp = databaseFile.Open(FileMode.Open);
                                temp.Close();
                                databaseFile = newDatabaseFile;
                                listaKlientowTreeView.Dispatcher.Invoke(new Action(() => { aktualizujTreeView(listOfClients); }));
                                break;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        public static void sprawdzenieBazyDanych()
        {
            try
            {
                string sciezkaInstalacji = System.AppDomain.CurrentDomain.BaseDirectory;
                //if (sciezkaInstalacji.Contains("Progam Files(x86)"))
                //  sciezkaInstalacji.Replace("Progam Files(x86)",)

                Properties.Settings.Default.sciezkaInstalacji = sciezkaInstalacji;

                if (!Directory.Exists(sciezkaInstalacji + "xml"))
                {
                    Directory.CreateDirectory(sciezkaInstalacji + "xml");
                }

                while (true)
                {
                    if (string.IsNullOrWhiteSpace(Properties.Settings.Default.baseXmlPath))
                    {
                        Properties.Settings.Default.baseXmlPath = sciezkaInstalacji + @"xml\db.xml";
                        if (!File.Exists(Properties.Settings.Default.baseXmlPath))
                            createDataBaseFile(Properties.Settings.Default.baseXmlPath);
                        if (!sprawdzKluczBazyDanych(Properties.Settings.Default.baseXmlPath))
                            createDataBaseFile(Properties.Settings.Default.baseXmlPath);
                    }
                    if (!File.Exists(Properties.Settings.Default.baseXmlPath) || !sprawdzKluczBazyDanych(Properties.Settings.Default.baseXmlPath))
                    {
                        MyMessageBox.Show("Nie można uzyskać dostępu do pliku: \n" + Properties.Settings.Default.baseXmlPath, "Brak pliku bazy danych");
                        new DatabasePath().ShowDialog();
                        continue;
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message + "\nSzczegóły: " + ex.Source, "Błąd", MyMessageBoxButtons.Ok);
                App.Current.Shutdown();
            }
        }

        public static bool sprawdzKluczBazyDanych(string databasePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(databasePath))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(databasePath);

                    if (xml["Connections"].Attributes["DatabaseKey"].InnerText != "Kam$oft1")
                        throw new Exception();
                }
            }
            catch (NullReferenceException)
            {
                MyMessageBox.Show("Wskazany plik nie jest poprawnym plikiem bazy danych", "Błędny plik bazy");
                return false;
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd");
                return false;
            }
            return true;
        }

        public static void createDataBaseFile(string connectionsPath)
        {
            try
            {
                XmlDocument newXml = new XmlDocument();
                XmlNode root = newXml.CreateNode(XmlNodeType.Element, "Connections", "Main");
                root.Attributes.Append(newXml.CreateAttribute("DatabaseKey"));
                root.Attributes["DatabaseKey"].InnerText = "Kam$oft1";
                newXml.AppendChild(root);
                newXml.Save(connectionsPath);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message + "\n" + connectionsPath, "Błąd", MyMessageBoxButtons.Ok);
            }

        }

        private void createContextMenu()
        {
            try
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
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        public static void aktualizujTreeView(TreeView listaKlientow)
        {
            try
            {
                Queue<Element> queueBeforeRefresh = new Queue<Element>();
                Queue<Element> queueAfterRefresh = new Queue<Element>();

                //zapisuje klientów, którzy mają rozwiniętą listę połaczeń
                List<string> expandedItems = new List<string>();
                List<string> visibleElements = new List<string>();
                string selectedItemId = "";
                foreach (Element f in listaKlientow.Items)
                {
                    queueBeforeRefresh.Enqueue(f);
                }

                while (queueBeforeRefresh.Count > 0)
                {
                    Element tempFolder = queueBeforeRefresh.Dequeue();
                    if (tempFolder.IsExpanded)
                        expandedItems.Add(tempFolder.Id);

                    if (tempFolder.Visibility == Visibility.Visible)
                        visibleElements.Add(tempFolder.Id);

                    if (tempFolder.IsSelected)
                        selectedItemId = tempFolder.Id;

                    if (tempFolder.HasItems)
                    {
                        foreach (object f in tempFolder.Items)
                        {
                            if (f is Folder)
                                queueBeforeRefresh.Enqueue(f as Folder);
                        }
                    }
                }

                listaKlientow.ItemsSource = null;
                //listaKlientow.ItemsSource = Serializator.deserializuj(Properties.Settings.Default.baseXmlPath);

                listaKlientow.ItemsSource = bazaDanych.PobierzWszystkieElementy();

                //odczytuje klientów, którzy mają rozwiniętą listę połaczeń i są widoczne
                foreach (Element f in listaKlientow.Items)
                {
                    queueAfterRefresh.Enqueue(f);
                }


                while (queueAfterRefresh.Count > 0)
                {
                    Element tempElement = queueAfterRefresh.Dequeue();

                    if (expandedItems.Exists(f => f == tempElement.Id))
                        tempElement.IsExpanded = true;

                    if (pierwszeLadowanieListy == false)//sprawdza czy jest to pierwsze załadowanie elementów na liste.
                    {
                        if (visibleElements.Exists(e => e == tempElement.Id))
                            tempElement.Visibility = Visibility.Visible;
                        else
                            tempElement.Visibility = Visibility.Collapsed;
                    }

                    if (tempElement.Id == selectedItemId)
                        tempElement.IsSelected = true;

                    if (tempElement.HasItems)
                    {
                        foreach (object f in tempElement.Items)
                        {

                            if (f is Folder)
                                queueAfterRefresh.Enqueue(f as Folder);
                        }
                    }

                }
                pierwszeLadowanieListy = false;


                Queue<Element> tempElements = new Queue<Element>();
                allElements.Clear();
                foreach (Element item in listaKlientow.Items)
                {
                    tempElements.Enqueue(item);
                }


                while (tempElements.Count > 0)
                {
                    Element tempElement = tempElements.Dequeue();
                    allElements.Enqueue(tempElement);

                    if (tempElement.HasItems)
                    {
                        foreach (Element child in tempElement.Items)
                        {
                            tempElements.Enqueue(child);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }

        }

        private void dockPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MyMessageBox.Show((sender as Element).Parent.ToString());
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
            try
            {
                Element selectedItem = e.NewValue as Element;

                if (selectedItem != null)
                {
                    opisRichTextBox.Document.Blocks.Clear();
                    opisRichTextBox.Document.Blocks.Add(new Paragraph(new Run(selectedItem.Opis)) { Margin = new Thickness(0) });
                }

                if (selectedItem is Klient)
                {
                    rozwinPanele(selectedItem as Klient);

                    dodajKlientaButtonDown.Visibility = Visibility.Collapsed;
                    edytujKlientaButtonDown.Visibility = Visibility.Visible;
                    usunKlientaButtonDown.Visibility = Visibility.Visible;

                    dodajFolderButtonDown.Visibility = Visibility.Visible;
                    edytujFolderButtonDown.Visibility = Visibility.Collapsed;
                    usunFolderButtonDown.Visibility = Visibility.Collapsed;

                    dodajPolaczenieButtonDown.Visibility = Visibility.Visible;
                    edytujPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                    usunPolaczenieButtonDown.Visibility = Visibility.Collapsed;

                }
                else if (selectedItem is Element)
                {

                    if (selectedItem.Parent != null)
                    {
                        while (!(selectedItem.Parent is Klient))
                        {
                            if (selectedItem.Parent != null)
                                selectedItem = selectedItem.Parent as Element;
                            else
                                goto parentIsNull;
                        }
                        rozwinPanele(selectedItem.Parent as Klient);

                    }


                    parentIsNull:

                    dodajKlientaButtonDown.Visibility = Visibility.Visible;
                    edytujKlientaButtonDown.Visibility = Visibility.Collapsed;
                    usunKlientaButtonDown.Visibility = Visibility.Collapsed;

                    dodajFolderButtonDown.Visibility = Visibility.Visible;
                    edytujFolderButtonDown.Visibility = Visibility.Visible;
                    usunFolderButtonDown.Visibility = Visibility.Visible;

                    dodajPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                    edytujPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                    usunPolaczenieButtonDown.Visibility = Visibility.Collapsed;

                }
                else if (selectedItem is Polaczenie)
                {
                    dodajKlientaButtonDown.IsEnabled = false;
                    //rozwinPanele(selectedItem.Parent as Klient);

                    dodajKlientaButtonDown.Visibility = Visibility.Collapsed;
                    edytujKlientaButtonDown.Visibility = Visibility.Collapsed;
                    usunKlientaButtonDown.Visibility = Visibility.Collapsed;

                    dodajFolderButtonDown.Visibility = Visibility.Collapsed;
                    edytujFolderButtonDown.Visibility = Visibility.Collapsed;
                    usunFolderButtonDown.Visibility = Visibility.Collapsed;

                    dodajPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                    edytujPolaczenieButtonDown.Visibility = Visibility.Visible;
                    usunPolaczenieButtonDown.Visibility = Visibility.Visible;

                }
                else
                {
                    maileView.ItemsSource = telefonyView.ItemsSource = haslaView.ItemsSource = null;
                    checkListViews();

                    dodajKlientaButtonDown.Visibility = Visibility.Visible;
                    edytujKlientaButtonDown.Visibility = Visibility.Collapsed;
                    usunKlientaButtonDown.Visibility = Visibility.Collapsed;

                    dodajFolderButtonDown.Visibility = Visibility.Visible;
                    edytujFolderButtonDown.Visibility = Visibility.Collapsed;
                    usunFolderButtonDown.Visibility = Visibility.Collapsed;

                    dodajPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                    edytujPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                    usunPolaczenieButtonDown.Visibility = Visibility.Collapsed;
                }


                string databasePath = Properties.Settings.Default.baseXmlPath;
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }


        private void rozwinPanele(Klient k)
        {
            try
            {
                maileView.ItemsSource = k.emailList;
                telefonyView.ItemsSource = k.telefonList;
                haslaView.ItemsSource = k.daneLogowaniaList;

                checkListViews();
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void checkListViews()
        {
            try
            {
                if (maileView.Items.Count > 0) emailsExpander.IsExpanded = true;
                else emailsExpander.IsExpanded = false;

                if (telefonyView.Items.Count > 0) phonesExpander.IsExpanded = true;
                else phonesExpander.IsExpanded = false;

                if (haslaView.Items.Count > 0) credentialsExpander.IsExpanded = true;
                else credentialsExpander.IsExpanded = false;
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void databasePathMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabasePath databasePath = new DatabasePath();
                databasePath.ShowDialog();
                listOfClients.ItemsSource = Serializator.deserializuj(Properties.Settings.Default.baseXmlPath);
                aktualizujTreeView(listOfClients);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void minimalizeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }


        private void maileView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            try
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Clipboard.SetText((maileView.SelectedItem as Email).adresEmail);
                    MyMessageBox.Show("Adres: " + Clipboard.GetText() + " został skopiowany do schowka", "Skopiowano", MyMessageBoxButtons.Ok);
                }
                else
                {
                    Process.Start("mailto:" + (maileView.SelectedItem as Email).adresEmail);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                return;
            }
        }

        private void telefonyView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Clipboard.SetText((telefonyView.SelectedItem as Telefon).numer);
                MyMessageBox.Show("Numer: " + Clipboard.GetText() + " został skopiowany do schowka", "Skopiowano", MyMessageBoxButtons.Ok);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                return;
            }
        }

        private void haslaView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Clipboard.SetText((haslaView.SelectedItem as DaneLogowania).login + " " + (haslaView.SelectedItem as DaneLogowania).haslo);
                MyMessageBox.Show(Clipboard.GetText() + " skopiowano do schowka", "Skopiowano", MyMessageBoxButtons.Ok);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                return;
            }
            //https://szoi.nfz.poznan.pl/ap-mzwi/servlet/mzwiuser/komun
            //Process.Start("https://szoi.nfz.poznan.pl/ap-mzwi/servlet/mzwiuser/komun");
        }

        private void maximalizeButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.isMaximalize)// minimalizowanie okna
                {
                    this.Width = Properties.Settings.Default.previouslyWindowWidth;
                    this.Height = Properties.Settings.Default.previouslyWindowHeight;
                    this.Left = Properties.Settings.Default.MinimalizeWindowPosition.X;
                    this.Top = Properties.Settings.Default.MinimalizeWindowPosition.Y;
                    BitmapImage icon = new BitmapImage(new Uri("Images/maximalize.png", UriKind.Relative));
                    this.resizeButtonImage.Source = icon;

                    this.isMaximalize = !this.isMaximalize;
                }
                else
                {
                    Properties.Settings.Default.MinimalizeWindowPosition = new Point(App.Current.MainWindow.Left, App.Current.MainWindow.Top);
                    Properties.Settings.Default.previouslyWindowHeight = this.Height;
                    Properties.Settings.Default.previouslyWindowWidth = this.Width;
                    this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
                    this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                    this.Left = 0;
                    this.Top = 0;
                    BitmapImage icon = new BitmapImage(new Uri("Images/minimalize.png", UriKind.Relative));
                    this.resizeButtonImage.Source = icon;


                    this.isMaximalize = !this.isMaximalize;
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 1);

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Tick += (s, args) =>
                {
                    MainWindow.wyszukajElement(tb.Text, listaKlientowTreeView);
                    timer.Stop();
                };

                timer.Start();
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }





        /// <summary>
        /// Wyszukuje elementów na liście z kontaktami i połączeniami
        /// </summary>
        /// <param name="szukanyString">Wyszukiwany element</param>
        /// <param name="listaElementów">Lista do przeszukania</param>
        public static void wyszukajElement(string szukanyString, TreeView listaElementów)
        {
            try
            {
                if (szukanyString != "      szukaj...")
                {
                    if (!string.IsNullOrWhiteSpace(szukanyString))
                    {
                        Queue<Element> visibleElements = new Queue<Element>();
                        visibleElements.Clear();
                        Queue<Element> queue = new Queue<Element>();
                        allElements.Clear();

                        foreach (Element f in listaElementów.Items)
                        {
                            allElements.Enqueue(f);
                            queue.Enqueue(f);
                        }
                        while (queue.Count > 0)
                        {
                            Element temp = queue.Dequeue();
                            if (temp.HasItems)
                            {
                                foreach (Element i in temp.Items)
                                {
                                    allElements.Enqueue(i);
                                    if (i is Folder)
                                        queue.Enqueue(i as Element);
                                }
                            }
                        }

                        Element elem = null;
                        foreach (Element element in allElements)
                        {
                            if (element is Element)
                                elem = element as Element;
                            if (elem != null)
                            {
                                bool a = elem.Nazwa.ToUpper().Contains(szukanyString.ToUpper());
                                if (a == true)
                                {
                                    if (elem.HasItems)
                                    {
                                        for (int i = 0; i < elem.Items.Count; i++)
                                        {
                                            visibleElements.Enqueue(elem.Items[i] as Element);
                                        }
                                    }

                                    elem.Visibility = Visibility.Visible;
                                    elem.IsExpanded = true;
                                    elem.IsSelected = true;

                                    while (true)
                                    {
                                        if (elem.Parent is Folder)
                                        {
                                            (elem.Parent as Folder).Visibility = Visibility.Visible;
                                            (elem.Parent as Folder).IsExpanded = true;
                                            elem = elem.Parent as Folder;
                                        }
                                        else
                                            break;
                                    }
                                }
                                else
                                {
                                    elem.Visibility = Visibility.Collapsed;
                                }
                            }
                        }
                        Queue<Element> tempQueue = new Queue<Element>(visibleElements);
                        while (tempQueue.Count > 0)
                        {
                            Element tempElement = tempQueue.Dequeue();
                            if (tempElement.HasItems)
                            {
                                foreach (Element item in tempElement.Items)
                                {
                                    visibleElements.Enqueue(item);
                                    tempQueue.Enqueue(item);
                                }
                            }
                        }
                        foreach (Element element in visibleElements)
                        {
                            element.Visibility = Visibility.Visible;
                        }
                    }
                    else
                        listaElementów.ItemsSource = bazaDanych.PobierzWszystkieElementy();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                return;
            }
        }

        private List<Folder> getAllItemsOfTreeView()
        {

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
            try
            {

                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Plik bazy danych RDM|*.rdm|Plik XML|*.xml";
                if (fileDialog.ShowDialog() == true)
                {
                    string rdmDatabasePath = fileDialog.FileName;
                    ImportRemoteDesktopManager.ImportRdmToLocalDb(rdmDatabasePath);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                return;
            }
        }

        private void searchFolderTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;


            if (tb.Text == "      szukaj...")
            {
                tb.Text = "";
            }
        }

        private void searchFolderTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "      szukaj...";
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {

        }

        private void dodajKlienta(RoutedEventArgs e)
        {
            try
            {
                if (!(listaKlientowTreeView.SelectedItem is Klient) && listaKlientowTreeView.SelectedItem is Folder)
                    Klient.DodajKlienta(this.listaKlientowTreeView.SelectedItem, e);
                else if (listaKlientowTreeView.SelectedItem == null)
                    Klient.DodajKlienta(this.listaKlientowTreeView, e);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show(ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                return;
            }
        }

        private void edytujElement(RoutedEventArgs e)
        {
            try
            {
                var selectedItem = listaKlientowTreeView.SelectedItem;

                if (selectedItem is Klient)
                {
                    Klient klient = selectedItem as Klient;
                    klient.Klient_Edytuj_Click(selectedItem, e);
                }
                else if (selectedItem is Folder)
                {
                    Folder folder = selectedItem as Folder;
                    folder.ZmienNazwe(folder, e);
                }
                else if (selectedItem is Polaczenie)
                {
                    Polaczenie polaczenie = selectedItem as Polaczenie;
                    polaczenie.edytujPolaczenieClick(polaczenie, e);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void usunElement(RoutedEventArgs e)
        {
            try
            {
                if ((listaKlientowTreeView.SelectedItem is Klient) || listaKlientowTreeView.SelectedItem is Folder)
                {
                    Folder folder = this.listaKlientowTreeView.SelectedItem as Folder;
                    folder.UsunElement(this.listaKlientowTreeView.SelectedItem);
                }
                else if (listaKlientowTreeView.SelectedItem is Polaczenie)
                {
                    Polaczenie polaczenie = listaKlientowTreeView.SelectedItem as Polaczenie;
                    polaczenie.usunPolaczenie_Click(polaczenie, e);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void dodajFolder(RoutedEventArgs e)
        {
            try
            {
                Folder folder = new Folder();
                if (listaKlientowTreeView.SelectedItem is Folder)
                {
                    folder.DodajFolder(listaKlientowTreeView.SelectedItem, e);
                }
                else if (listaKlientowTreeView.SelectedItem == null)
                {
                    folder.DodajFolder(listaKlientowTreeView, e);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void dodajPolaczenie(RoutedEventArgs e)
        {
            try
            {
                if (listaKlientowTreeView.SelectedItem is Klient)
                {
                    Klient klient = listaKlientowTreeView.SelectedItem as Klient;
                    klient.Klient__DodajPolaczenie_Click(klient, e);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void listaKlientowTreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }


        private void dodajKlientaButtonDown_Click(object sender, RoutedEventArgs e)
        {
            dodajKlienta(e);
        }

        private void edytujKlientaButtonDown_Click(object sender, RoutedEventArgs e)
        {
            edytujElement(e);
        }

        private void usunKlientaButtonDown_Click(object sender, RoutedEventArgs e)
        {
            usunElement(e);
        }

        private void dodajFolderButtonDown_Click(object sender, RoutedEventArgs e)
        {
            dodajFolder(e);
        }

        private void edytujFolderButtonDown_Click(object sender, RoutedEventArgs e)
        {
            edytujElement(e);
        }

        private void usunFolderButtonDown_Click(object sender, RoutedEventArgs e)
        {
            usunElement(e);
        }

        private void dodajPolaczenieButtonDown_Click(object sender, RoutedEventArgs e)
        {
            dodajPolaczenie(e);
        }

        private void edytujPolaczenieButtonDown_Click(object sender, RoutedEventArgs e)
        {
            edytujElement(e);
        }

        private void usunPolaczenieButtonDown_Click(object sender, RoutedEventArgs e)
        {
            usunElement(e);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    zamknijAplikacje();
                }

                switch (e.Key)
                {
                    case Key.F2:
                        if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            dodajFolder(e);
                        else if (Keyboard.IsKeyDown(Key.LeftShift))
                            dodajPolaczenie(e);
                        else
                            dodajKlienta(e);
                        break;
                    case Key.F3:
                        dodajFolder(e);
                        break;
                    case Key.F4:
                        edytujElement(e);
                        break;
                    case Key.F8:
                        usunElement(e);
                        break;
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }



        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            zamknijAplikacje();
        }

        private void searchFolderTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                listaKlientowTreeView.Focus();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && listaKlientowTreeView.SelectedItem != null)
                (listaKlientowTreeView.SelectedItem as Element).IsSelected = false;
        }

        private void defaultPasswords_Click(object sender, RoutedEventArgs e)
        {
            new DefaultPasswordsWindow().Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OknoWebBrowser wb = new OknoWebBrowser();
            wb.Show();
        }


        Point _lastMouseDown;
        Element draggedItem, _target;

        private void listaKlientowTreeView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Usuwa zaznaczenie z wszystkich items gdy user kliknie w puste pole tree view
            try
            {
                var t = sender as TreeView;

                t.Focus();

                if (t.IsMouseOver == true && e.Source is TreeView && t.SelectedItem != null)
                {
                    (t.SelectedItem as Element).IsSelected = false;
                }


                //ustawia pierwotną pozycję myszy przy przenoszeniu elementów poprzez drag and drop
                if (e.ChangedButton == MouseButton.Left)
                {
                    _lastMouseDown = e.GetPosition(listaKlientowTreeView);
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }

        }

        private void listaKlientowTreeView_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && (e.OriginalSource is TextBlock || e.OriginalSource is Image))
                {
                    Point currentPosition = e.GetPosition(listaKlientowTreeView);


                    if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                        (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                    {
                        draggedItem = (Element)listaKlientowTreeView.SelectedItem;
                        if (draggedItem != null)
                        {
                            DragDropEffects finalDropEffect = DragDrop.DoDragDrop(listaKlientowTreeView, listaKlientowTreeView.SelectedValue,
                                DragDropEffects.Move);
                            //Checking target is not null and item is dragging(moving)
                            if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                            {
                                // A Move drop was accepted
                                if (!draggedItem.Id.ToString().Equals(_target.Id.ToString()))
                                {
                                    CopyItem(draggedItem, _target);
                                    _target = null;
                                    draggedItem = null;
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        private void listaKlientowTreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {

                Point currentPosition = e.GetPosition(listaKlientowTreeView);


                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) ||
                    (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    // Verify that this is a valid drop and then store the drop target
                    Element item = GetNearestContainer(e.OriginalSource as UIElement);
                    if (item is Polaczenie)
                    {
                        e.Effects = DragDropEffects.None;
                    }
                    else if (CheckDropTarget(draggedItem, item))
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.None;
                    }
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void listaKlientowTreeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                Element TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;

                }
            }
            catch (Exception)
            {
            }
        }

        private bool CheckDropTarget(Element _sourceItem, Element _targetItem)
        {
            //Check whether the target item is meeting your condition
            try
            {
                bool _isEqual = false;
                if (!_sourceItem.Id.ToString().Equals(_targetItem.Id.ToString()))
                {
                    _isEqual = true;
                }
                return _isEqual;
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
                return false;
            }

        }
        private void CopyItem(Element _sourceItem, Element _targetItem)
        {

            //Asking user wether he want to drop the dragged Element here or not
            if (MyMessageBox.Show("Czy na pewno chcesz przenieść element " + _sourceItem.Nazwa.ToString() + " do " + _targetItem.Nazwa.ToString(), "Przenoszenie", MyMessageBoxButtons.OkAnuluj) == MessageBoxResult.OK)
            {
                try
                {

                    List<string> listaIdWykluczonychElementow = new List<string>();

                    //pobiera listę id elementów, do których nie można przenieść elementu ponieważ element jest ich rodzicem
                    if (listaKlientowTreeView.Items != null)
                    {
                        foreach (Element item in allElements)
                        {
                            if (item.Id == _sourceItem.Id)
                            {
                                listaIdWykluczonychElementow.Add(item.Id);
                                Queue<Element> kolejkaPrzegladaniaDzieciWezlaItem = new Queue<Element>();
                                kolejkaPrzegladaniaDzieciWezlaItem.Enqueue(item);

                                if (item.HasItems)
                                {
                                    while (kolejkaPrzegladaniaDzieciWezlaItem.Count > 0)
                                    {
                                        Element temp = kolejkaPrzegladaniaDzieciWezlaItem.Dequeue();

                                        if (temp.HasItems)
                                        {
                                            foreach (Element child in temp.Items)
                                            {
                                                kolejkaPrzegladaniaDzieciWezlaItem.Enqueue(child);
                                                listaIdWykluczonychElementow.Add(child.Id);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }

                    if (listaIdWykluczonychElementow != null && listaIdWykluczonychElementow.Count > 0 && listaIdWykluczonychElementow.Contains(_targetItem.Id))
                    {
                        throw new ProbaDodaniaRodzicaElementuDoDzieckaException("Próba dodania węzłu rodzica do elementu podległego (dziecka)");
                    }


                    addChild(_sourceItem, _targetItem);

                    //finding Parent Element of dragged Element 
                }
                catch (ProbaDodaniaRodzicaElementuDoDzieckaException ex)
                {
                    MyMessageBox.Show("Nie można przenieść elementu nadrzędnego do elementu podrzędnego!\n\nTreść błędu: " + ex.Message, "Błąd przy przenoszeniu elementu", MyMessageBoxButtons.Ok);
                }
            }

        }


        public void addChild(Element _sourceItem, Element _targetItem)
        {
            // add item in target Element 
            //Folder item1 = new Folder();
            //item1.Header = _sourceItem.id;
            //_targetItem.Items.Add(item1);
            //foreach (Element item in _sourceItem.Items)
            //{
            //    addChild(item, item1);
            //}

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(Properties.Settings.Default.baseXmlPath);

                XmlNodeList nodeList = xml["Connections"].ChildNodes;
                XmlNode sourceItem = null;
                //XmlNode targetItem = null;

                foreach (XmlNode item in nodeList)
                {
                    if (item.Attributes["Id"].InnerText == _sourceItem.Id)
                    {
                        sourceItem = item;
                        break;
                    }
                    //if (item.Attributes["Id"].InnerText == _targetItem.id)
                    //{
                    //    targetItem = item;
                    //}
                    //if (targetItem != null && sourceItem != null)
                    //    break;
                }

                sourceItem.Attributes["Group"].InnerText = _targetItem.Id;
                xml.Save(Properties.Settings.Default.baseXmlPath);
                MainWindow.aktualizujTreeView(MainWindow.listOfClients);
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            try
            {
                if (child == null)
                {
                    return null;
                }

                UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

                while (parent != null)
                {
                    TObject found = parent as TObject;
                    if (found != null)
                    {
                        return found;
                    }
                    else
                    {
                        parent = VisualTreeHelper.GetParent(parent) as UIElement;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
                return null;
            }
        }

        private Element GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            try
            {
                Element container = element as Element;
                while ((container == null) && (element != null))
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                    container = element as Element;
                }
                return container;
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
                return null;
            }
        }

        private void obslugaWizytySomedPoradnikiMedycynaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Properties.Settings.Default.sciezkaInstalacji + "poradniki\\Instrukcja korzystania z modułu Wizyty Lekarskiej (1).pdf");
            }
            catch (Exception ex)
            {

                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }

        private void opisRichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listaKlientowTreeView != null && listaKlientowTreeView.SelectedItem is Element)
                {
                    Element selected = listaKlientowTreeView.SelectedItem as Element;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(Properties.Settings.Default.baseXmlPath);
                    XmlNodeList nodeList = xml["Connections"].ChildNodes;

                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes["Id"].InnerText == ((listaKlientowTreeView.SelectedItem) as Element).Id)
                        {
                            node.Attributes["Description"].InnerText = new TextRange(opisRichTextBox.Document.ContentStart, opisRichTextBox.Document.ContentEnd).Text;
                            xml.Save(Properties.Settings.Default.baseXmlPath);
                            aktualizujTreeView(listOfClients);
                            selected.IsSelected = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błędu: \n" + ex.Message, "Wystąpuł nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }

        }

        private void opisRichTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        private void zamknijAplikacje()
        {
            try
            {
                if (MyMessageBox.Show("Czy na pewno zamknąć aplikację?", "Zamykanie aplikacji", MyMessageBoxButtons.OkAnuluj) == MessageBoxResult.OK)
                {
                    Properties.Settings.Default.previouslyWindowHeight = this.Height;
                    Properties.Settings.Default.previouslyWindowWidth = this.Width;
                    Properties.Settings.Default.Save();

                    //if (watekSprawdzaniaZmianBazyNaSerwerze.ThreadState != System.Threading.ThreadState.Stopped)
                    //{
                    //    watekSprawdzaniaZmianBazyNaSerwerze.Abort();
                    //}
                    App.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błedu: \n" + ex.Message, "Nastąpił nieoczekiwany błąd", MyMessageBoxButtons.Ok);
            }
        }
    }
}