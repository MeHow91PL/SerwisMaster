using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SerwisMaster
{


    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IDisposable
    {

        string login, pass;
        HtmlElement raportyStatystyczneButton = null;
        Queue<int> raportyDoPobrania = new Queue<int>();
        int iloscRaportowDoPobrania = 0;
        System.Windows.Forms.WebBrowser webBrowser1 = null;
            string sciezkaZapisu = "";

        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                DateTime dt = Convert.ToDateTime(dataOd.Text);
            }
            catch (FormatException)
            {
                dataOd.Text = DateTime.Today.Date.ToString();
            }

            try
            {
                Convert.ToDateTime(dataDo.Text);
            }
            catch (FormatException)
            {
                dataDo.Text = DateTime.Today.Date.ToString();
            }

            try
            {
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    sciezkaZapisu = folderBrowser.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MyMessageBox.Show("Treść błędu:\n" + ex.Message, "Błąd!", MyMessageBoxButtons.Ok);
            }

            if (string.IsNullOrWhiteSpace(sciezkaZapisu))
            {
                MyMessageBox.Show("Nie wybrono folderu docelowego!","Błąd", MyMessageBoxButtons.Ok);
            }
            else
            {
                webBrowser1 = new System.Windows.Forms.WebBrowser();
                progressBar.Value = 0;

                webBrowser1.ScriptErrorsSuppressed = true;
                hostt.Child = webBrowser1;


                login = loginTextBox.Text;
                pass = passTextBox.Text;

                webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
                webBrowser1.Navigate(new Uri("https://szoi.nfz.poznan.pl/ap-mzwi/servlet/mzwiauth/flogin?"));

            }
        }
        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            progressLabel.Content = "Trwa logowanie do SZOI...";

            System.Windows.Forms.WebBrowser bro = sender as System.Windows.Forms.WebBrowser;
            bro.DocumentCompleted -= webBrowser1_DocumentCompleted;
            bro.DocumentCompleted += webBrowser1_DocumentCompleted2;

            if (bro.Document.GetElementById("FFFRAB0520login") != null &&
                bro.Document.GetElementById("FFFRAB0520pasw") != null)
            {
                bro.Document.GetElementById("FFFRAB0520login").SetAttribute("value", loginTextBox.Text);
                bro.Document.GetElementById("FFFRAB0520pasw").SetAttribute("value", passTextBox.Text);

                HtmlElementCollection htmlColl = bro.Document.GetElementsByTagName("input");

                foreach (HtmlElement h in htmlColl)
                {
                    if (h.GetAttribute("classname").ToString() == "p32button_mid")
                    {
                        h.InvokeMember("Click");
                        break;
                    }
                }
            }

        }

        private void webBrowser1_DocumentCompleted2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            System.Windows.Forms.WebBrowser bro = sender as System.Windows.Forms.WebBrowser;


            if (bro.ReadyState == WebBrowserReadyState.Complete)
            {
                bro.Navigate(new Uri("https://szoi.nfz.poznan.pl/ap-mzwi/servlet/mzwiuser/komun"));

                bro.DocumentCompleted -= webBrowser1_DocumentCompleted2;
                bro.DocumentCompleted += webBrowser1_DocumentCompleted3;
            }
        }


        private void webBrowser1_DocumentCompleted3(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            System.Windows.Forms.WebBrowser bro = sender as System.Windows.Forms.WebBrowser;

            HtmlDocument doc = bro.Document;
            bool zalogowany = false, bladLogowania = false;

            HtmlElementCollection coll = doc.GetElementsByTagName("a");

            foreach (HtmlElement h in coll)
            {
                if (h.InnerText == "Raporty statystyczne medyczne")
                {
                    raportyStatystyczneButton = h;
                    h.InvokeMember("Click");
                    zalogowany = true;
                    bro.DocumentCompleted -= webBrowser1_DocumentCompleted3;
                    bro.DocumentCompleted += webBrowser1_DocumentCompleted4;
                    return;
                }
            }

            if (bro.ReadyState == WebBrowserReadyState.Complete && zalogowany == false)
            {
                coll = null;
                coll = doc.GetElementsByTagName("td");
                foreach (HtmlElement h in coll)
                {
                    if (h.GetAttribute("classname").ToString() == "msg")
                    {
                        if (h.InnerText.Contains("Sesja wygasła lub została przerwana"))
                        {
                            MyMessageBox.Show("Wprowadzono błędny PIN lub błędne hasło " +
                                "lub podany operator nie posiada uprawnień do wybranej opcji.");
                            bro.DocumentCompleted -= webBrowser1_DocumentCompleted3;

                            progressLabel.Content = "";

                            bladLogowania = true;
                            return;
                        }
                    }
                }
            }

            if (bro.ReadyState == WebBrowserReadyState.Complete && zalogowany == false && bladLogowania == false)
            {
                webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted3;

                MyMessageBox.Show("Nie można przejść do zakładki raportów statystycznych. " +
                    "Prawdopodobne przyczyny: u świadczeniodawcy pojawił się nowy komunikat, wymagana jest zmiana hasła lub " +
                    "trwają prace serwisowe w serwisie.");

                            progressLabel.Content = "";
                return;
            }

        }

        private void webBrowser1_DocumentCompleted4(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete)
            {
                progressLabel.Content = "Trwa wyszukiwanie raportów do porbrania...";

                HtmlDocument doc = webBrowser1.Document;
                HtmlElementCollection coll = doc.GetElementsByTagName("a");

                HtmlElement mainTable = null;

                foreach (HtmlElement h in coll)
                {
                    if (h.InnerText == "pobierz raport xml")
                    {
                        mainTable = h.Parent.Parent.Parent;
                        break;
                    }
                }


                
                for (int i = 1; i < mainTable.Children.Count; i++)
                {
                    string data = mainTable.Children[i].Children[1].InnerText.Substring(0, 10);
                    try
                    {
                        HtmlElement czyNowyRaport = mainTable.Children[i].Children[4].Children[2];
                    
                    if (Convert.ToDateTime(dataOd.Text) <= Convert.ToDateTime(data)
                        && Convert.ToDateTime(dataDo.Text) >= Convert.ToDateTime(data))
                    {
                        if (czyNowyRaport.TagName == "IMG")//sprawdza czy to nowa zwrotka
                        {
                            raportyDoPobrania.Enqueue(Convert.ToInt32(mainTable.Children[i].Children[0].InnerText.TrimEnd('.')));
                        }
                        else if (!tylkoNoweCheckBox.IsChecked.Value)//sprawdza czy pobrane mają być tylko nowe zwrotki
                        {
                            raportyDoPobrania.Enqueue(Convert.ToInt32(mainTable.Children[i].Children[0].InnerText.TrimEnd('.')));
                        }
                    }
                    }
                    catch
                    { }

                    if (mainTable.Children.Count > 1 && i == mainTable.Children.Count - 1)
                    {
                        if (Convert.ToDateTime(dataOd.Text) <= Convert.ToDateTime(data))
                        {
                            HtmlElement f = mainTable.Parent.Parent.Parent.NextSibling;
                            HtmlElementCollection col = f.GetElementsByTagName("input");

                            foreach (HtmlElement h in col)
                            {
                                if (h.GetAttribute("value").Contains("20 >>"))
                                    h.InvokeMember("Click");
                            }
                        }
                        else
                        {
                            iloscRaportowDoPobrania = raportyDoPobrania.Count();
                            progressBar.Maximum = iloscRaportowDoPobrania * 2;
                            

                            webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted4;
                            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted5;
                        
                            HtmlElementCollection linki = doc.GetElementsByTagName("a");

                            foreach (HtmlElement h in linki)
                            {
                                if (h.InnerText == "Raporty statystyczne medyczne")
                                {
                                    raportyStatystyczneButton = h;
                                    h.InvokeMember("Click");
                                }
                            }
                            //pobierzRaporty();
                        }
                    }
                }

            }
        }


        private void webBrowser1_DocumentCompleted5(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument doc = webBrowser1.Document;
            HtmlElementCollection coll = doc.GetElementsByTagName("a");
            bool pierwszaStrona = false;

            HtmlElement mainTable = null;

            foreach (HtmlElement h in coll)
            {
                if (h.InnerText == "pobierz raport xml")
                {
                    mainTable = h.Parent.Parent.Parent;
                    break;
                }
            }

            HtmlElement f = mainTable.Parent.Parent.Parent.NextSibling;
            HtmlElementCollection col = f.GetElementsByTagName("input");

            foreach (HtmlElement h in col)
            {
                if (h.GetAttribute("value").Contains("<< 20"))
                {
                    h.InvokeMember("Click");
                    pierwszaStrona = false;
                    break;
                }
                else
                    pierwszaStrona = true;
            }

            if(pierwszaStrona == true)
            {
                rozpocznijPobieranie();                
            }



        }

        private void rozpocznijPobieranie()
        {
            


            HtmlElementCollection coll = webBrowser1.Document.GetElementsByTagName("a");

            HtmlElement mainTable = null;

            foreach (HtmlElement h in coll)
           {
                if (h.InnerText == "pobierz raport xml")
                {
                    mainTable = h.Parent.Parent.Parent;
                    break;
                }
            }

            bool znalezionoRaport = false;
            bool rozpoczeto = false;

            for (int i = 1; i < mainTable.Children.Count; i++)
            {
                string lp = mainTable.Children[i].Children[0].InnerText;

                if (lp == raportyDoPobrania.Peek().ToString() + ".")
                {
                    raportyDoPobrania.Dequeue();
                    znalezionoRaport = true;

                    HtmlElementCollection linki = mainTable.Children[i].GetElementsByTagName("a");

                    foreach (HtmlElement pobierzButton in linki)
                    {
                        if (pobierzButton.InnerText == "pobierz raport xml")
                        {
                            progressLabel.Content = "Trwa pobieranie " + (iloscRaportowDoPobrania - raportyDoPobrania.Count) +" z " + iloscRaportowDoPobrania + " raportów...";
                            webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted5;

                            webBrowser1.DocumentCompleted += pobieranieWebBrowser_DocumentCompleted5;
                            webBrowser1.Navigate(pobierzButton.GetAttribute("href"));
                            rozpoczeto = true;
                            break;
                        }
                    }
                }
                if(rozpoczeto == true)
                {
                    break;
                }
            }

            if(znalezionoRaport == false)
            {
                HtmlElementCollection inputs = webBrowser1.Document.GetElementsByTagName("input");

                foreach(HtmlElement button in inputs)
                {
                    if(button.GetAttribute("value").Contains("20 >>"))
                    {
                        webBrowser1.DocumentCompleted += zmienStrone;
                        button.InvokeMember("Click");
                        break;
                    }
                }
            }
        }

        private void zmienStrone(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= zmienStrone;

            rozpocznijPobieranie();
        }

        private Cookie getCookie(string cookie)
        {
            string name, value;
            string[] splits = cookie.Split('=');
            name = splits[0];
            value = splits[1];

            return new Cookie(name.Trim(), value.Trim(), "/ap-mzwi", "szoi.nfz.poznan.pl");
        }

        //##############################################################################################################


        private void pobierzRaporty()
        {
            

        }



        private void pobieranieWebBrowser_DocumentCompleted5(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            System.Windows.Forms.WebBrowser bro = sender as System.Windows.Forms.WebBrowser;

            HtmlElementCollection coll = null;
            coll = bro.Document.GetElementsByTagName("input");
            foreach (HtmlElement h in coll)
            {
                if (h.GetAttribute("value") == "Dalej →")
                {
                    progressBar.Value++;
                    bro.DocumentCompleted -= pobieranieWebBrowser_DocumentCompleted5;
                    h.InvokeMember("Click");
                    Thread w = new Thread(new ThreadStart(przejscie));
                    w.IsBackground = true;
                    w.Start();
                    w.Join();
                    //bro.DocumentCompleted += pobieranieWebBrowser_DocumentCompleted6;
                    break;
                }
            }
        }

        Thread watekPobierania = null;
        private void przejscie()
        {
            watekPobierania = new Thread(new ParameterizedThreadStart(pobierz));
            watekPobierania.IsBackground = true;
            watekPobierania.Start(webBrowser1);
        }




        private void pobierz(object browser)
        {
            System.Windows.Forms.WebBrowser bro = browser as System.Windows.Forms.WebBrowser; 

            HtmlElementCollection coll = null;
            bool gotowy = false;
        loop:
            try
            {
                bro.Invoke(new Action(() => { coll = bro.Document.GetElementsByTagName("span"); }));
                foreach (HtmlElement h in coll)
                {
                    if (h.InnerText == "DOKUMENT WYGENEROWANY")
                    {
                        gotowy = true;
                    }
                }
            }
            catch { goto loop; }
            if (gotowy == false)
                goto loop;

            HtmlElementCollection colls = null;


            bro.Invoke(new Action(() =>
            {
                try
                {
                    colls = bro.Document.GetElementsByTagName("a");
                    HtmlElementCollection szukajNazwy = bro.Document.GetElementsByTagName("span");
                    string nazwa = null;
                    foreach (HtmlElement span in szukajNazwy)
                    {
                        if (span.InnerText.ToString().Contains(".SWX"))
                        {
                            nazwa = span.InnerText.Substring(0, span.InnerText.Length - 4);
                        }
                    }

                    foreach (HtmlElement h in colls)
                    {
                        if (h.InnerText == "pobierz plik")
                        {

                            progressBar.Value++;

                            Uri url = new Uri(h.GetAttribute("href").ToString());

                            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                            wr.Credentials = CredentialCache.DefaultCredentials;
                            wr.CookieContainer = new CookieContainer();
                            Cookie cookie = getCookie(bro.Document.Cookie);
                            wr.CookieContainer.Add(cookie);


                            HttpWebResponse ws = (HttpWebResponse)wr.GetResponse();

                            Stream stream = ws.GetResponseStream();

                            Stream output = new FileStream(sciezkaZapisu + "\\" + nazwa + ".SWZ", FileMode.OpenOrCreate);

                            stream.CopyTo(output);

                            stream.Close();
                            output.Close();


                            HtmlElementCollection inputs = bro.Document.GetElementsByTagName("input");

                            foreach (HtmlElement ht in inputs)
                            {
                                if (ht.GetAttribute("value").ToString() == "Zakończ →")
                                {
                                    webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted6;
                                    ht.InvokeMember("Click");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyMessageBox.Show("Treść błędu:\n" + ex.Message, "Błąd", MyMessageBoxButtons.Ok);
                    progressLabel.Content = "";
                    return;
                }
            }
            ));
        }

        private void webBrowser1_DocumentCompleted6(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted6;

            if (raportyDoPobrania.Count > 0)
                rozpocznijPobieranie();
            else
            {
                webBrowser1 = null;
                progressLabel.Content = "Pobieranie zakończone!";
            }
        }

        private void minimalizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dockPanel1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void loginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            progressBar.Value = 0;
            progressLabel.Content = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                webBrowser1 = null;
            }
            catch
            {
                
            }
        }

        public void Dispose()
        {
            webBrowser1 = null;
            this.watekPobierania.Abort();
            this.watekPobierania = null;
            this.raportyDoPobrania.Clear();
            this.raportyDoPobrania = null;
            this.raportyStatystyczneButton = null;
        }
    }
}
