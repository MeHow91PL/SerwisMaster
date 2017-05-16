using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Xml;
using SerwisMaster.Klasy_połączenia;

namespace SerwisMaster
{
    class CreateHeader
    {

        public static WrapPanel createItemHeader( object obj  )
        {
            string s = obj.GetType().ToString();
            string nazwa = "";
            Uri obrazek = null;
            switch( s )
            {
                case "SerwisMaster.TeamViewer":
                    obrazek = new Uri("Images/tvIcon.png", UriKind.Relative);
                    nazwa = ( obj as TeamViewer ).Nazwa;
                    break;
                case "SerwisMaster.Klasy_połączenia.Rdp":
                    obrazek = new Uri("Images/zdalnyIcon.png", UriKind.Relative);
                    nazwa = ( obj as SerwisMaster.Klasy_połączenia.Rdp ).Nazwa;
                    break;
                case "SerwisMaster.Klient":
                    obrazek = new Uri("Images/userIcon.png", UriKind.Relative);
                    nazwa = ( obj as Klient).Nazwa;
                    break;
                case "SerwisMaster.Folder":
                    obrazek = new Uri("Images/folderIcon.png", UriKind.Relative);
                    nazwa = ( obj as Folder).Nazwa;
                    break;
            }

            WrapPanel wrap = new WrapPanel();
            wrap.Children.Add( new System.Windows.Controls.Image
            {
                Source = new BitmapImage( obrazek ),
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

