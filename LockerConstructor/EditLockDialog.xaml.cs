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
using System.Windows.Shapes;

namespace LockerConstructor
{
    /// <summary>
    /// Logique d'interaction pour EditLockDialog.xaml
    /// </summary>
    public partial class EditLockDialog : Window
    {
        public string SelectedType { get; set; }
        public EditLockDialog()
        {
            InitializeComponent();
        }


        private void EditLockButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedType = LockTypeBox.Text;

            DialogResult = SelectedType != null;
        }
    }
}
