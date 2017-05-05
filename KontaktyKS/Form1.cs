using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KontaktyKS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
            webBrowser1.Navigate(new Uri("https://szoi.nfz.poznan.pl/ap-mzwi/servlet/mzwiauth/flogin?"));
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
                    webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted;
                    webBrowser1.DocumentCompleted +=webBrowser1_DocumentCompleted2;
            
                if (webBrowser1.Document.GetElementById("FFFRAB0520login") != null &&
                    webBrowser1.Document.GetElementById("FFFRAB0520pasw") != null)
                {
                    webBrowser1.Document.GetElementById("FFFRAB0520login").SetAttribute("value", "150004540");
                    webBrowser1.Document.GetElementById("FFFRAB0520pasw").SetAttribute("value", "pa7T-4PU");

                    HtmlElementCollection htmlColl = webBrowser1.Document.GetElementsByTagName("input");

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

            webBrowser1.Navigate(new Uri("https://szoi.nfz.poznan.pl/ap-mzwi/servlet/mzwiuser/komun"));


            webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted2;
            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted3;
        }

        private void webBrowser1_DocumentCompleted3(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            HtmlDocument doc = webBrowser1.Document;

            HtmlElementCollection coll = doc.GetElementsByTagName("a");

            foreach (HtmlElement h in coll)
            {
                if (h.InnerText == "Raporty statystyczne medyczne")
                {
                    h.InvokeMember("Click");
                    webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted3;
                    webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted4;

                    break;
                }
            }
           
        }

        private void webBrowser1_DocumentCompleted4(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument doc = webBrowser1.Document;
            HtmlElementCollection coll = doc.GetElementsByTagName("a");

            foreach (HtmlElement h in coll)
            {
                if (h.InnerText == "pobierz raport xml")
                {
                    webBrowser1.Navigate(h.GetAttribute("href").ToString());
                    webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted4;
                    webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted5;
                    break;
                }
            }
        }

        private void webBrowser1_DocumentCompleted5(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlElementCollection coll = webBrowser1.Document.GetElementsByTagName("input");
            foreach (HtmlElement h in coll)
            {
                if (h.GetAttribute("value") == "Dalej →")
                {
                    h.InvokeMember("Click");
                    Thread w = new Thread(pobierz);
                    w.Start(webBrowser1);
                    webBrowser1.DocumentCompleted -= webBrowser1_DocumentCompleted5;
                    break;
                }
            }



        }



       
             private void button5_Click(object sender, EventArgs e)
        {
            HtmlDocument doc = webBrowser1.Document;
            HtmlElementCollection coll = doc.GetElementsByTagName("a");

            foreach (HtmlElement h in coll)
            {
                if (h.InnerText == "pobierz raport xml")
                {
                    webBrowser1.Navigate(h.GetAttribute("href").ToString());
                    break;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HtmlElementCollection coll = webBrowser1.Document.GetElementsByTagName("input");
            foreach (HtmlElement h in coll)
            {
                if (h.GetAttribute("value") == "Dalej →")
                {
                    h.InvokeMember("Click");
                    break;
                }
            }


            Thread w = new Thread(pobierz);
            w.Start(webBrowser1.Document.GetElementsByTagName("span"));



        }

        private void pobierz(object obj)
        {
            HtmlElementCollection coll = null;
            bool gotowy = false;
        loop:
            webBrowser1.Invoke(new Action(() => { coll = webBrowser1.Document.GetElementsByTagName("span"); }));
            try
            {
                foreach (HtmlElement h in coll)
                {
                    if (h.InnerText == "DOKUMENT WYGENEROWANY")
                    {
                        gotowy = true;
                    }
                    richTextBox1.Invoke(new Action(() => {richTextBox1.Text = gotowy.ToString();}));
                }
            }
            catch { goto loop; }
            if (gotowy == false)
                goto loop;

            HtmlElementCollection colls = null;


            webBrowser1.Invoke(new Action(() => { colls = webBrowser1.Document.GetElementsByTagName("a"); 
            
            foreach (HtmlElement h in colls)
            {
                if (h.InnerText == "pobierz plik")
                {

                    Uri url = new Uri(h.GetAttribute("href").ToString());

                    HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                    wr.Credentials = CredentialCache.DefaultCredentials;
                    wr.CookieContainer = new CookieContainer();
                    Cookie cookie = getCookie(webBrowser1.Document.Cookie);
                    wr.CookieContainer.Add(cookie);                    
                    

                    HttpWebResponse ws =(HttpWebResponse) wr.GetResponse();

                    Stream stream = ws.GetResponseStream();

                    Stream output = new FileStream(@"C:\kontaktyks\Musi.swz",FileMode.OpenOrCreate);

                    stream.CopyTo(output);

                    stream.Close();
                    output.Close();
                    


                }
            }
            }));


        }


        private Cookie getCookie(string cookie)
        {
            string name, value;
            string[] splits = cookie.Split('=');
            name = splits[0];
            value = splits[1];

            return new Cookie(name.Trim(), value.Trim(), "/ap-mzwi", "szoi.nfz.poznan.pl");
        }


        public static bool TryAddCookie(WebRequest webRequest, Cookie cookie)
        {
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                return false;
            }

            if (httpRequest.CookieContainer == null)
            {
                httpRequest.CookieContainer = new CookieContainer();
            }

            httpRequest.CookieContainer.Add(cookie);
            return true;
        }


        private void button7_Click(object sender, EventArgs e)
        {

        }
    }
}
