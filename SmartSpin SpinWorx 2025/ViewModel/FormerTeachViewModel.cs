using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartSpin.Dialogs;
using SmartSpin.Hardware;
using SmartSpin.Model;
using SmartSpin.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSpin.ViewModel
{
    public partial class FormerTeachViewModel : MyViewModelBase
    {
        private readonly ControllerAxis HAxis;
        private readonly ControllerAxis VAxis;
        private readonly FormerTeach FormerTeach;

        public FormerTeachViewModel(ControllerAxis hAxis, ControllerAxis vAxis, FormerTeach formerTeach)
        {
            HAxis = hAxis;
            VAxis = vAxis;
            FormerTeach = formerTeach;
        }

        [ObservableProperty]
        private int formSelected;

        [RelayCommand]
        public void SelectForm(int x)
        {
            FormSelected = x;
            FormerTeach.ChangeSelectedForm();
        }

        [RelayCommand]
        public void ChangeHPosition(FormerMove x)
        {
            double d = x.HPosition;

            if (Calculator.ShowCalculator(ref d, "Enter Value"))
            {
                double minusLimit = (HAxis as ServoDrive).Parameters.Find("MinusLimit").ValueAsDouble;
                double plusLimit = (HAxis as ServoDrive).Parameters.Find("PlusLimit").ValueAsDouble;

                if (d < minusLimit)
                {
                    d = minusLimit;
                }
                if (d > plusLimit)
                {
                    d = plusLimit;
                }
                x.HPosition = d;
                FormerTeach.UpdateDataGrid();
            }
        }

        [RelayCommand]
        public void ChangeHSpeed(FormerMove x)
        {
            double d = x.HSpeed;

            if (Calculator.ShowCalculator(ref d, "Enter Value"))
            {
                if (d < 0.01)
                {
                    d = 0.01;
                }
                if (d > 100)
                {
                    d = 100;
                }
                x.HSpeed = d;
                FormerTeach.UpdateDataGrid();
            }
        }

        [RelayCommand]
        public void ChangeVPosition(FormerMove x)
        {
            double d = x.VPosition;

            if (Calculator.ShowCalculator(ref d, "Enter Value"))
            {
                double minusLimit = (VAxis as ServoDrive).Parameters.Find("MinusLimit").ValueAsDouble;
                double plusLimit = (VAxis as ServoDrive).Parameters.Find("PlusLimit").ValueAsDouble;

                if (d < minusLimit)
                {
                    d = minusLimit;
                }
                if (d > plusLimit)
                {
                    d = plusLimit;
                }
                x.VPosition = d;
                FormerTeach.UpdateDataGrid();
            }
        }

        [RelayCommand]
        public void ChangeVSpeed(FormerMove x)
        {
            double d = x.VSpeed;

            if (Calculator.ShowCalculator(ref d, "Enter Value"))
            {
                if (d < 0.01)
                {
                    d = 0.01;
                }
                if (d > 100)
                {
                    d = 100;
                }
                x.VSpeed = d;
                FormerTeach.UpdateDataGrid();
            }
        }

        [RelayCommand]
        public void ChangeDelay(FormerMove x)
        {
            double d = x.Delay;

            if (Calculator.ShowCalculator(ref d, "Enter Value"))
            {
                if (d < 0)
                {
                    d = 0;
                }
                if (d > 100)
                {
                    d = 100;
                }
                x.Delay = d;
                FormerTeach.UpdateDataGrid();
            }
        }

        [RelayCommand]
        public void ClearAll()
        {
            Machine.CurrentProgram.FormerCycles[FormSelected].ClearAll();
            FormerTeach.UpdateDataGrid();
        }

        [RelayCommand]
        public void AddCurrent()
        {
            FormerMove fm = new FormerMove
            {
                HPosition = Machine.CurPosition(HAxis.axisno) / HAxis.DisplayMultiplier,
                VPosition = Machine.CurPosition(VAxis.axisno) / VAxis.DisplayMultiplier,
                //TODO: Select speeds
                HSpeed = 100,
                VSpeed = 100
            };
            Machine.CurrentProgram.FormerCycles[FormSelected].AddPoint(fm);
            FormerTeach.UpdateDataGrid();
        }

        [RelayCommand]
        public void DeleteLast()
        {
            Machine.CurrentProgram.FormerCycles[FormSelected].DeleteLast();
            FormerTeach.UpdateDataGrid();
        }
    }
}