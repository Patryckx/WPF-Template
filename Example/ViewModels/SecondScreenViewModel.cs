using ConfigIniLib;
using ConfigIniLib.interfaces;
using EthModbus.Services;
using Example.Services;
using Example.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using SqlUtilityLibrary;
using SqlUtilityLibrary.Interfaces;
using SqlUtilityLibrary.Models;
using SqlUtilityLibrary.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.ViewModels.Base;


namespace Example.ViewModels
{
    public class SecondScreenViewModel :ViewModelBase
    {
        public ConnectionViewModel _ConnectionVM { get; }

        private readonly IDataService _database;

        private DataView? _coilInfo;

        public DataView? CoilInfo
        {
            get => _coilInfo;
            set
            {
                _coilInfo = value;
                OnPropertyChanged();
            }
        }


        private readonly IAppStateService _appState;

        public SecondScreenViewModel(
            ConnectionViewModel connectionVM,
            IDataService database,
            IAppStateService appState)
                {
                    _ConnectionVM = connectionVM;
                    _database = database;
                    _appState = appState;

                    LoadCoilsInfo();
                }


        private async void LoadCoilsInfo()
        {
            DataTable dt =
                await _database.ExecuteQueryAsync(
                    "SELECT * FROM Coils_registers");

            CoilInfo = dt.DefaultView;
        }

    }
}
