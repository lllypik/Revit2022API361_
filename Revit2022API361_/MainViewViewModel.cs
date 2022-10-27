using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPILibrary;
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

        public List<DuctType> DuctTypes { get; private set; } = new List<DuctType>();
        public List<Level> Levels { get; private set; }
        public DelegateCommand ApplyCommand { get; private set; }
        public double DuctDisplacement { get; set; }
        public XYZ PointBegin { get; private set; }
        public XYZ PointEnd { get; private set; }
        public List<XYZ> Points { get; private set; } = new List<XYZ>();
        public DuctType SelectedDuctType { get; set; }
        public Level SelectedLevel { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            DuctTypes = DuctData.GetDuctType(commandData);
            Levels = LevelData.GetLevels(commandData);
            ApplyCommand = new DelegateCommand(OnApplyCommand);
            DuctDisplacement = 100;
            PointBegin = SelectionUtils.GetPoint(_commandData, "Введите начальную точку", ObjectSnapTypes.Endpoints);
            PointEnd = SelectionUtils.GetPoint(_commandData, "Введите конечную точку", ObjectSnapTypes.Endpoints);
        }

        private void OnApplyCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uIDocument = uiapp.ActiveUIDocument;
            Document document = uIDocument.Document;

            XYZ pointBeginOffset = new XYZ(PointBegin.X, PointBegin.Y, UnitUtils.ConvertToInternalUnits(PointBegin.X, UnitTypeId.Millimeters));
            XYZ pointEndOffset = new XYZ(PointEnd.X, PointEnd.Y, UnitUtils.ConvertToInternalUnits(PointEnd.X, UnitTypeId.Millimeters));

            MEPSystemType systemType = new FilteredElementCollector(document)
                .OfClass(typeof(MEPSystemType))
                .Cast<MEPSystemType>()
                .FirstOrDefault(m => m.SystemClassification == MEPSystemClassification.SupplyAir);

            using (Transaction tr = new Transaction(document, "Create duct"))
            {
                tr.Start();
                Duct.Create(
                    document,
                    systemType.Id,
                    SelectedDuctType.Id,
                    SelectedLevel.Id,
                    pointBeginOffset,
                    pointEndOffset);
                tr.Commit();
            };

            ReiseCloseReqest();
        }

        public event EventHandler CloseReqest;
        private void ReiseCloseReqest()
        {
            CloseReqest?.Invoke(this, EventArgs.Empty);
        }
    }
}
