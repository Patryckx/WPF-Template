using Example.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Example.Services
{
    public class DialogService:IDialogService
    {
        public void ShowMessage(string message)
        {
            MessageBox.Show(
                message,
                "Informacion",
                MessageBoxButton.OK,
                MessageBoxImage.Information );
        }

        public void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public void ShowWarning(string message)
        {
           MessageBox.Show(message,
               "Advertencia",
               MessageBoxButton.OK, MessageBoxImage.Warning );
        }

        public bool AskYesNo(string question)
        {
            return MessageBox.Show(
                question,
                "Por favor seleccione una opcion..",
                MessageBoxButton.YesNo, MessageBoxImage.Question)
                
                == MessageBoxResult.Yes;
        }


        public bool AskOkCancel(string question)
        {
            return MessageBox.Show(
                question,
                "Desea continuar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning)

                == MessageBoxResult.Yes;
        }
    }
}
