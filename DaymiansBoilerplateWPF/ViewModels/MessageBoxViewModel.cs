using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DaymsWPFBoiler.ViewModels.Interfaces;
using GalaSoft.MvvmLight;

namespace DaymsWPFBoiler.ViewModels
{
    public class MessageBoxViewModel : ViewModelBase, IDialogViewModel
    {
        public MessageBoxViewModel()
        {
            Message = string.Empty;
            Caption = string.Empty;
        }

        private string _caption;
        public string Caption
        {
            get => _caption;
            set => Set(ref _caption, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        private MessageBoxButton _buttons;
        public MessageBoxButton Buttons
        {
            get => _buttons;
            set => Set(ref _buttons, value);
        }

        private MessageBoxImage _image;
        public MessageBoxImage Image
        {
            get => _image;
            set => Set(ref _image, value);
        }

        private MessageBoxResult _result;
        public MessageBoxResult Result
        {
            get => _result;
            set => Set(ref _result, value);
        }

        public MessageBoxResult Show(IList<IDialogViewModel> collection)
        {
            collection.Add(this);
            return Result;
        }

    }
}
