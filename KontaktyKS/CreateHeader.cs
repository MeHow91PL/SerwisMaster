using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Xml;
using KontaktyKS.Klasy_połączenia;

namespace KontaktyKS
{
    class CreateHeader
    {

        public static WrapPanel createItemHeader( object obj  )
        {
            string s = obj.GetType().ToString();
            string nazwa = "",
                path = @"C:\kontaktyKS\KontaktyKS\KontaktyKS\Images\";
            switch( s )
            {
                case "KontaktyKS.TeamViewer":
                    path += "images.jpg";
                    nazwa = ( obj as TeamViewer ).nazwa;
                    break;
                case "KontaktyKS.Klasy_połączenia.Rdp":
                    path += "pobrane.jpg";
                    nazwa = ( obj as KontaktyKS.Klasy_połączenia.Rdp ).nazwa;
                    break;
                case "KontaktyKS.Klient":
                    path += "userIcon.png";
                    nazwa = ( obj as Klient).nazwa;
                    break;
                case "KontaktyKS.Folder":
                    path += "Blank Folder.png";
                    nazwa = ( obj as Folder).nazwa;
                    break;
            }

            WrapPanel wrap = new WrapPanel();
            wrap.Children.Add( new System.Windows.Controls.Image
            {
                Source = new BitmapImage( new Uri( path ) ),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 20,
                Height = 25
            } );

            if (obj is TeamViewer || obj is Rdp || (obj as Folder).parent == null)
            {
                TextBlock tb = new TextBlock { Text = nazwa, VerticalAlignment = System.Windows.VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };

                wrap.Children.Add(tb);

            }
            else
            {
                MyTextBox tb = new MyTextBox(obj as Folder);
                wrap.Children.Add(tb);
            }
            return wrap;
        }

      
    }
}    

