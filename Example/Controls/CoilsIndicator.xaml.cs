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
    public partial class CoilsIndicator : UserControl
    {
        public CoilsIndicator()
        {
            InitializeComponent();

        }



        public static readonly DependencyProperty StatusProperty=
            DependencyProperty.Register(
                nameof(Status),
                typeof(CoilStatus),
                typeof(CoilsIndicator),
                new PropertyMetadata(CoilStatus.Idle,OnStatusChanged));


        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CoilsIndicator)d;

            // Fuerza reevaluación visual
            control.Dispatcher.Invoke(() =>
            {
                control.InvalidateVisual();
            });
        }
        public CoilStatus Status
        {
            get=> (CoilStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }


        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(CoilsIndicator),
                new PropertyMetadata("Indicator"));
    
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        

    }
}
