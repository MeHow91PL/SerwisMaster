using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KontaktyKS
{
    public static class EdycjaTreeViewItemDoubleClick
    {
        public static void editDoubleClick(TreeViewItem obj,string nazwa)
        {
            TextBox textBox = new TextBox();
            textBox.Text = nazwa;
            obj.MouseDoubleClick += ( o, e ) =>
            {
                obj.Header = textBox;
                textBox.Text = nazwa;
            };
            textBox.LostFocus += ( o, e ) =>
            {
                obj.Header = CreateHeader.createItemHeader( obj );
            };
            textBox.KeyDown += ( o, e ) =>
            {
                if( e.Key == System.Windows.Input.Key.Enter )
                {
                    obj.Header = CreateHeader.createItemHeader( obj );
                }
            };
        }
    }
}
