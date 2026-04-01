using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EthModbus.Models.Modbus
{
    public class DiscreteCoil : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ushort Address { get; set; }
        public string? CoilName { get; set; }
        public bool IsWritable { get; set; }


        private bool _value;
        public bool Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }

        }

        private bool _isValid;
        public bool IsValid 
        {

            get => _isValid;
            set {_isValid = value; OnPropertyChanged();  }
        }


        private DateTime _latUpdated;
        public DateTime LastUpdated 
        
        {

            get => _latUpdated;

            set { _latUpdated = value;  OnPropertyChanged(); }  
        
        
        }


        private string? _error;
        public string? Error
        {

            get => _error;

            set { _error = value; OnPropertyChanged(); }        


        }
       
    }
}
