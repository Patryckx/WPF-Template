using Example.Models;
using Example.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services
{
    public class AppStateService : IAppStateService, INotifyPropertyChanged
    {

        private ProcessStatus _appGeneralStatus = ProcessStatus.Idle;

        private ProcessStatus _dcVoltageStatus = ProcessStatus.Idle;

        private ProcessStatus _ResistanceTestStatus = ProcessStatus.Idle;

        private ProcessStatus _LCDVisionTestStatus = ProcessStatus.Idle;



        public ProcessStatus AppGeneralStatus
        {
            get { return _appGeneralStatus; }

            set
            {
                if (_appGeneralStatus == value)
                    return;

                _appGeneralStatus = value;

                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(AppGeneralStatus)));
            }
        }


        public ProcessStatus DCVoltageStatus
        {
            get => _dcVoltageStatus;
            set
            {
                if (_dcVoltageStatus == value)
                    return;

                _dcVoltageStatus = value;

                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(DCVoltageStatus)));
            }
        }


        public ProcessStatus ResistanceTestStatus
        {
            get => _ResistanceTestStatus;
            set
            {
                if (_ResistanceTestStatus == value)
                    return;

                _ResistanceTestStatus = value;

                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(ResistanceTestStatus)));

            }
        }


        public ProcessStatus LCDVisionTestStatus
        {
            get => _LCDVisionTestStatus;

            set
            {
                if (_LCDVisionTestStatus == value)
                    return;

                _LCDVisionTestStatus = value;

                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(LCDVisionTestStatus)));


            }
        }







         public event PropertyChangedEventHandler? PropertyChanged;
    }
}