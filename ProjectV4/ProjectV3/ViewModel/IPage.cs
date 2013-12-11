using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectV3.ViewModel
{
    // elke viewmodel klasse zal deze interface MOETEN implementeren, zo kan ik later een lijst van objecten
    // van klasse gaan bijhouden waarvan de klasse deze interface implementeert. Deze lijst zit in de klasse
    // ApplicationVM.
    interface IPage
    {
        // 1 property insteken. Elke klasse moet deze property gaan uitwerken.
        // setter is niet nodig omdat het een constante waarde is.
        string Name { get; }

    }
}
