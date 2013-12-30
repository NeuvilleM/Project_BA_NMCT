using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectV3.ViewModel
{
    // Dit is de viewmodelklasse dat hoort bij de view 'HomePage'
    class HomePageVM : ObservableObject, IPage
    {

        public string Name
        {
            get { return "Startpagina"; } // UNIEKE naam!
        }
    }
}
