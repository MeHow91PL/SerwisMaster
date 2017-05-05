using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Input;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace KontaktyKS.Klasy_połączenia
{
    class Rdp : Polaczenie
    {
        public string adresRDP = string.Empty;
        public string login = string.Empty;

        public Rdp( string id, string nazwa, string adresRDP, string login, string haslo, string typ )
            : base( id, nazwa, haslo, typ )
        {
            this.adresRDP = adresRDP;
            this.login = login;

            Header = CreateHeader.createItemHeader(this);
        }
    
        private void stworzRdp(string nazwa, string adresRDP, string login, string haslo = "")
        {
            
            bool rdpJuzIstnieje = false;
            DirectoryInfo dir = new DirectoryInfo( @"C:\kontaktyKS\rdp" );
            FileInfo[] files = dir.GetFiles();
            foreach( FileInfo s in files )
            {
                if( s.Name == nazwa + ".rdp" )
                    rdpJuzIstnieje = true;
            }

            if( rdpJuzIstnieje == false )
            {
                StreamWriter writer = File.CreateText( @"C:\kontaktyKS\rdp\" + nazwa + ".rdp" );

                Process process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = "cmdkey";
                process.StartInfo.Arguments = "/generic:TERMSRV/" + adresRDP + " /user:" + login + " /pass:" + haslo;
                process.Start();

                writer.WriteLine( "screen mode id:i:2" );
                writer.WriteLine( "desktopwidth:i:1280" );
                writer.WriteLine( "desktopheight:i:800" );
                writer.WriteLine( "session bpp:i:32" );
                writer.WriteLine( "winposstr:s:0,1,237,52,1037,652" );
                writer.WriteLine( "full address:s:" + adresRDP );
                writer.WriteLine( "compression:i:1" );
                writer.WriteLine( "keyboardhook:i:2" );
                writer.WriteLine( "audiomode:i:0" );
                writer.WriteLine( "redirectprinters:i:1" );
                writer.WriteLine( "redirectcomports:i:0" );
                writer.WriteLine( "redirectsmartcards:i:1" );
                writer.WriteLine( "redirectclipboard:i:1" );
                writer.WriteLine( "redirectposdevices:i:0 " );
                writer.WriteLine( "displayconnectionbar:i:1 " );
                writer.WriteLine( "autoreconnection enabled:i:1 " );
                writer.WriteLine( "authentication level:i:2 " );
                writer.WriteLine( "prompt for credentials:i:0 " );
                writer.WriteLine( "negotiate security layer:i:1 " );
                writer.WriteLine( "remoteapplicationmode:i:0 " );
                writer.WriteLine( "alternate shell:s: " );
                writer.WriteLine( "shell working directory:s: " );
                writer.WriteLine( "disable wallpaper:i:0 " );
                writer.WriteLine( "disable full window drag:i:0 " );
                writer.WriteLine( "allow desktop composition:i:1 " );
                writer.WriteLine( "allow font smoothing:i:1 " );
                writer.WriteLine( "disable menu anims:i:0 " );
                writer.WriteLine( "disable themes:i:0 " );
                writer.WriteLine( "disable cursor setting:i:0 " );
                writer.WriteLine( "bitmapcachepersistenable:i:1 " );
                writer.WriteLine( "gatewayhostname:s: " );
                writer.WriteLine( "gatewayusagemethod:i:0 " );
                writer.WriteLine( "gatewaycredentialssource:i:4 " );
                writer.WriteLine( "gatewayprofileusagemethod:i:0 " );
                writer.WriteLine( "redirectdrives:i:0 " );
                writer.WriteLine( "username:s:" + login );
                writer.WriteLine( "promptcredentialonce:i:1 " );
                writer.WriteLine( "drivestoredirect:s: " );
                writer.Close();
            }
        }

        public override void Polaczenie_MouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            bool rdpJuzIstnieje = false;
            DirectoryInfo dir = new DirectoryInfo( @"C:\kontaktyKS\rdp" );
            if (!Directory.Exists(dir.FullName))
            {
              dir = createRdpFolder();
            }
            FileInfo[] files = dir.GetFiles();
            foreach( FileInfo s in files )
            {
                if( s.Name == nazwa + ".rdp" )
                    rdpJuzIstnieje = true;
            }

            if( rdpJuzIstnieje == false )
                stworzRdp(base.nazwa, this.adresRDP, this.login, base.haslo);

            Process.Start( @"C:\kontaktyKS\rdp\" + nazwa + ".rdp" );
        }

        private DirectoryInfo createRdpFolder()
        {
            DirectoryInfo dir = Directory.CreateDirectory(@"C:\kontaktyKS\rdp");
            dir.Attributes = FileAttributes.Hidden;

            return dir;
        }

        protected override void usunPolaczenie( object remoteId )
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Properties.Settings.Default.baseXmlPath);

            XmlNodeList remoteNodes = xml.GetElementsByTagName("RDP");

            foreach(XmlNode node in remoteNodes)
            {
                
                if (node.Attributes["Id"].InnerText == remoteId.ToString())
                {
                    node.ParentNode.RemoveChild(node);
                    break;
                }
            }
        }
    }
}
