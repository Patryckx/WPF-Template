using Example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Example.Controls
{
    /// <summary>
    /// Lógica de interacción para ProcessIndicator.xaml
    /// </summary>
    public partial class ProcessIndicator : UserControl
    {
        public ProcessIndicator()
        {
            InitializeComponent();

            UpdateColor();
        }

        public static readonly DependencyProperty StatusProperty=
            DependencyProperty.Register(
                nameof(Status),
                typeof(ProcessStatus),
                typeof(ProcessIndicator),
                new PropertyMetadata(ProcessStatus.Idle,OnStatusChanged));

        public ProcessStatus Status
        {
            get=> (ProcessStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control=(ProcessIndicator)d;
            control.UpdateColor();
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(ProcessIndicator),
                new PropertyMetadata("Indicator"));
    
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void UpdateColor()
        {
            if (IndicatorBorder == null)
                return;
            switch(Status)
            {
                case ProcessStatus.Sucess:
                    IndicatorBorder.Background = Brushes.Green;
                    break;


                case ProcessStatus.Idle:
                    IndicatorBorder.Background = Brushes.LightGray;
                    break;



                case ProcessStatus.OnProcess:
                    IndicatorBorder.Background= Brushes.LightYellow;
                    break;


                case ProcessStatus.Failed: 
                    IndicatorBorder.Background= Brushes.Red;
                    break;


                default:
                    IndicatorBorder.Background = Brushes.White;
                    break;
            }
        }

    }
}
