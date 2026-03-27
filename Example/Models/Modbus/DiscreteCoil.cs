using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthModbus.Models.Modbus
{
    public class DiscreteCoil
    {
        ///<summary>
        ///Direccion coil
        ///</summary>
        ///
        public ushort Address { get; set; }

        ///<summary>
        ///Valor logico coil
        ///</summary>
        ///
        public bool Value { get; set; }

        ///<summary>
        ///Nombre coil
        ///</summary>
        ///
        public string CoilName { get; set; }

        ///<summary>
        ///Indica si la coil puede ser manipulada
        ///</summary>
        ///
        public bool IsWritable { get; set; }

        ///<summary>
        ///Ultima vez que se actualizo el valor
        ///</summary>
        ///
        public DateTime LastUpdated { get; set; }

        ///<summary>
        ///Indica si el valor es valido (ultima lectura correcta)
        ///</summary>
        ///
        public bool IsValid { get; set; }

        ///<summary>
        ///Mensaje de error si la lectura falla
        ///</summary>
        ///
        public string? Error { get; set; }
    }
}
