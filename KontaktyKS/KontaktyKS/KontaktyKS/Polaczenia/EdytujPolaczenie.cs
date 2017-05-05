using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerwisMaster
{
    class EdytujPolaczenie : OknoPolaczenia
    {
        public EdytujPolaczenie(string nazwa,string idOrLogin, string haslo, string adresRDP ="")
        {
            base.rodzajComboBox.IsEnabled = false;
            base.nazwaTextBox.Text = nazwa;
            base.loginTextBox.Text = idOrLogin;
            base.hasloTextBox.Text = haslo;
            base.adresTextBox.Text = adresRDP;
        }


    }
}
