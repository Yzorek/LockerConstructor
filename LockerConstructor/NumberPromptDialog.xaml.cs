using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace LockerConstructor
{
    /// <summary>
    /// Logique d'interaction pour PromptDialog.xaml
    /// </summary>
    public partial class NumberPromptDialog : Window
    {
        public int Value { get; set; }
        public NumberPromptDialog()
        {
            InitializeComponent();
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (NumberUpDown.Value.GetValueOrDefault() > 0)
            {
                Value = NumberUpDown.Value.GetValueOrDefault();
                DialogResult = true;
            }
        }
    }
}
