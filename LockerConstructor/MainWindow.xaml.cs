using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace LockerConstructor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LockerExporter Exporter { get; set; }

        private int ActiveLocker { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Exporter = new LockerExporter(@"\\Serv-kalysse\EDatas\Dev\Datas\LockerConstuctor\locker_list.txt");
            LockTypeBox.SelectedIndex = 0;
            PenderieBox.SelectedIndex = 0;
            PlaqueBox.SelectedIndex = 0;
            EntraxeUpDown.Text = "300";
            FillTypeSerrBox();
        }

        private void FillTypeSerrBox()
        {
            string[] lines = File.ReadAllLines(@"\\Serv-kalysse\EDatas\Dev\Datas\Casiers\serrures_config.txt");

            if (lines.Length > 2)
            {
                TypeSerrBox.Items.Clear();
                for (int i = 2; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(';');

                    if (line.Length != 3) continue;
                    string name = line[0];
                    TypeSerrBox.Items.Add(name);
                }
                TypeSerrBox.SelectedIndex = 0;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string type = LockTypeBox.Text;
            Button btn = new Button { Width = 30, Height = 30, Content = type};

            btn.MouseDown += Btn_MouseDown;
            btn.Click += Btn_Click;
            LockerPanel.Children.Add(btn);
            Exporter.AddLocker(type);
            Debug.WriteLine(LockerPanel.Children.IndexOf(btn));
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Click");
        }

        private void Btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Mouse Down");
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ContextMenu cm = new ContextMenu();
                MenuItem delItem = new MenuItem();
                MenuItem insLItem = new MenuItem();
                MenuItem insRItem = new MenuItem();

                delItem.Header = "Supprimer casier";
                insLItem.Header = "Insérer casier à gauche";
                insRItem.Header = "Insérer casier à droite";
                delItem.Click += DelItem_Click;
                insLItem.Click += InsLItem_Click;
                insRItem.Click += InsRItem_Click;
                cm.Items.Add(delItem);
                cm.Items.Add(insLItem);
                cm.Items.Add(insRItem);
                //ActiveLocker = ((Button) sender)
                cm.IsOpen = true;
            }
        }

        private void InsRItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InsLItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DelItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            Exporter.Export();
        }

        private void PatereCheck_Checked(object sender, RoutedEventArgs e)
        {
            PenderieBox.IsEnabled = false;
            PenderieBox.Text = "";
        }

        private void PatereCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            PenderieBox.IsEnabled = true;
            PenderieBox.SelectedIndex = 0;
        }

        private void LockConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            Process proc = Process.Start(@"\\SERV-KALYSSE\EDatas\Logiciels\LockConfig\LockConfig.exe");

            proc.WaitForExit();
            FillTypeSerrBox();
        }
    }
}
