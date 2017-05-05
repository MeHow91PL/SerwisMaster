using System;
using System.Windows.Input;
using System.Windows;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KontaktyKS
{
    class TeamViewer : Polaczenie
    {
        public string teamViewerId = "";

        public TeamViewer( string id, string nazwa, string teemViewerId, string haslo, string typ )
            : base(id, nazwa, haslo, typ )
        {
            this.teamViewerId = teemViewerId;
            

            Header = CreateHeader.createItemHeader( this );
        }

        public override void Polaczenie_MouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            string path;

            if( Environment.Is64BitOperatingSystem == true )
                path = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 );
            else
                path = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles );

            znajdzTeamViewer( ref path );
            if( !string.IsNullOrEmpty( path ) )
                System.Diagnostics.Process.Start( path, "-i " + teamViewerId +
                    " --Password " + base.haslo );
        }

        private void znajdzTeamViewer( ref string path )
        {
            string[] TeamViewerDirectory = Directory.GetDirectories( path, "TeamViewer" );
            if( TeamViewerDirectory.Length > 0 )
            {
                string[] lista = Directory.GetDirectories( TeamViewerDirectory[0] );
                foreach( string s in lista )
                {
                    DirectoryInfo dir = new DirectoryInfo( s );
                    FileInfo[] tv = dir.GetFiles( "TeamViewer.exe" );

                    if( tv.Length > 0 )
                    {
                        path = tv[0].FullName;
                        return;
                    }
                }
            }
            path = string.Empty;
        }

        protected override void usunPolaczenie( object idKlienta )
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load( Properties.Settings.Default.baseXmlPath );
            var tv = xmlDoc.GetElementsByTagName( "Teamviewer" );
            for( int i = 0; i < tv.Count; i++ )
            {
                if( tv[i].ParentNode.ParentNode.Attributes["Id"].InnerText == idKlienta.ToString() )
                {
                    tv[i].ParentNode.RemoveChild(tv[i]);
                    break;
                }
            }
            xmlDoc.Save(Properties.Settings.Default.baseXmlPath);
        }


    }
}
