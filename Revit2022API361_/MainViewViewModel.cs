using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit2022API361_
{
    class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
        }
    }
}
