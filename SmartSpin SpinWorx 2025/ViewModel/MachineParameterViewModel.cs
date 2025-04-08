using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSpin.ViewModel
{
    public partial class MachineParameterViewModel : MyViewModelBase
    {
        [ObservableProperty]
        private ParametersViewModel machine;

        [ObservableProperty]
        private ParametersViewModel axis1;

        [ObservableProperty]
        private ParametersViewModel axis2;

        [ObservableProperty]
        private ParametersViewModel axisX;

        [ObservableProperty]
        private ParametersViewModel axisZ;

        [ObservableProperty]
        private ParametersViewModel axisB;

        [ObservableProperty]
        private ParametersViewModel axisX1;

        [ObservableProperty]
        private ParametersViewModel axisY1;

        [ObservableProperty]
        private ParametersViewModel axisZ1;

        [ObservableProperty]
        private ParametersViewModel spindle;
    }
}
