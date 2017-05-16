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
    /// Interaction logic for myMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {

        static MessageBoxResult result;
        
        private MyMessageBox()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Display window with a message.
        /// </summary>
        /// <param name="message">Type System.String. Text to display as message</param>
        public static void Show(string message)
        {
            MyMessageBox box = new MyMessageBox();
            ControlTemplate template =(ControlTemplate) box.FindResource("MyMessageButtonControlTemplate");

            box.buttonsStackPanel.Children.Add(createButton( "OK", template ));
            box.buttonsStackPanel.Children[0].Focus();
            box.messageText.Text = message;
            box.ShowDialog();
        }

        public static void Show( string message, string caption )
        {
            MyMessageBox box = new MyMessageBox();
            ControlTemplate template = (ControlTemplate)box.FindResource("MyMessageButtonControlTemplate");
            box.buttonsStackPanel.Children.Add( createButton( "OK", template) );
            box.messageText.Text = message;
            box.messageBoxTitleLabel.Content = caption;
            box.ShowDialog();
        }

        /// <summary>
        /// Display message window with message, title and button (max 3). Returns myResult.
        /// </summary>
        /// <param name="message">Type string. Text to display as massage</param>
        /// <param name="caption">Type string. Text to display as title of window</param>
        /// <param name="buttons">Type array of string. Texts to display in new button controls</param>
        /// <returns></returns>
        public static MessageBoxResult Show( string message, string caption, MyMessageBoxButtons buttons )
        {

            MyMessageBox box = new MyMessageBox();
            ControlTemplate template = (ControlTemplate)box.FindResource("MyMessageButtonControlTemplate");
            box.messageText.Text = message;
            box.messageBoxTitleLabel.Content = caption;


            switch( buttons )
            {
                case MyMessageBoxButtons.Ok:
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "OK", template) );
                    break;
                case MyMessageBoxButtons.OkAnuluj:
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "OK", template) );
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "ANULUJ", template) );
                    box.buttonsStackPanel.Children[1].Focus();
                    break;
                case MyMessageBoxButtons.PominAnuluj:
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "POMIŃ" , template) );
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "ANULUJ", template) );
                    break;
                case MyMessageBoxButtons.Popraw:
                    box.buttonsStackPanel.Children.Add(MyMessageBox.createButton("POPRAW", template));
                    break;
                case MyMessageBoxButtons.PominPopraw:
                    box.buttonsStackPanel.Children.Add(MyMessageBox.createButton("POPRAW", template));
                    box.buttonsStackPanel.Children.Add(MyMessageBox.createButton("POMIŃ", template));
                    break;
                case MyMessageBoxButtons.Usun:
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "USUŃ", template) );
                    break;
                case MyMessageBoxButtons.UsunPopraw:
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "POPRAW", template) );
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "USUŃ", template) );
                    break;
                case MyMessageBoxButtons.UsunAnuluj:
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "ANULUJ", template) );
                    box.buttonsStackPanel.Children.Add( MyMessageBox.createButton( "USUŃ", template) );
                    break;

            }
            box.ShowDialog();
            return result; 
        }


        private static Button createButton(string buttonText, ControlTemplate template)
        {
            Button button = new Button();
            button.Padding = new Thickness( 10, 0, 10, 0 );
            button.Margin = new Thickness( 25, 0, 25, 0 );
            button.Height = 35;
            button.MinWidth = 100;
            button.Background = new SolidColorBrush(Color.FromArgb(0xFF,0x34,0x45,0xb2));
            button.Foreground = Brushes.White;
            button.BorderBrush = Brushes.DodgerBlue;
            button.Content = buttonText;

            button.Template = template;

            button.Click += new RoutedEventHandler( button_Click );

            return button;
        }

        static void button_Click( object sender, RoutedEventArgs e )
        {
            switch( ( sender as Button ).Content.ToString() )
            { 
                case "OK":
                    result = MessageBoxResult.OK;
                    break;
                case "POPRAW":
                    result = MessageBoxResult.POPRAW;
                    break;
                case "ANULUJ":
                    result = MessageBoxResult.ANULUJ;
                    break;
                case "USUŃ":
                    result = MessageBoxResult.USUN;
                    break;
                case "POMIŃ":
                    result = MessageBoxResult.POMIN;
                    break;
            }

            StackPanel tempStackPanel = (StackPanel)( sender as Button ).Parent;
            Grid tempGrid = ( Grid )tempStackPanel.Parent;
            MyMessageBox tempWindowHandler = (MyMessageBox)tempGrid.Parent;
            tempWindowHandler.Close();
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
           
        }
    }



    public enum MessageBoxResult
    {
        OK,
        POPRAW,
        ANULUJ,
        POMIN,
        USUN
    }

    public enum MyMessageBoxButtons
    {
        Ok,
        OkAnuluj,
        PominAnuluj,
        Popraw,
        PominPopraw,
        Usun,
        UsunAnuluj,
        UsunPopraw
    }
        
}
