using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SerwisMaster
{
    class wspolneOpcjeOkienek
    {
        public static void zamknijOkienko(Window window)
        {
            if (MyMessageBox.Show("Czy na pewno chcesz opuścić okno bez zapisywania?", "Zamykanie okna", MyMessageBoxButtons.OkAnuluj) == MyResult.OK)
            {
                window.Close();
            }
        }
    }
}
