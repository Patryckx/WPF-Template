using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Services.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message);

        void ShowError(string message);

        void ShowWarning(string message);

        bool AskYesNo(string question);

        bool AskOkCancel(string question);

    }
}
