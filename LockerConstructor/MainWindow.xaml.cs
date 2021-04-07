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
using Xceed.Wpf.Toolkit;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace LockerConstructor
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LockerExporter Exporter { get; set; }
        private List<LockInfos> Locks = new List<LockInfos>();

        public MainWindow()
        {
            InitializeComponent();
            Exporter = new LockerExporter(@"\\Serv-kalysse\EDatas\Dev\Datas\LockerConstuctor\locker_list.txt");
            FillTypeSerrBox();
            Locker newLocker = new Locker("h1", 300, "Serrure monnayeur,129.001,209.001", 0, 0, 0);
            FillLockerInfos(newLocker);
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

                    LockInfos infos = new LockInfos
                    {
                        Name = name,
                        Code1 = line[1],
                        Code2 = line[2]
                    };
                    Locks.Add(infos);
                }
                TypeSerrBox.SelectedIndex = 0;
            }
        }

        private Button CreateLockerButton(string type)
        {
            Button btn = new Button { Width = 30, Height = 30, Content = type };
            btn.MouseDown += Btn_MouseDown;
            btn.Click += Btn_Click;
            return btn;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            int newIdx = LockerPanel.Children.IndexOf(sender as Button);
            int oldIdx = GetSelectedLockerIndex();

            if (oldIdx != -1)
            {
                (LockerPanel.Children[oldIdx] as Button).BorderBrush = Brushes.Gray;
            }
            if (oldIdx == newIdx)
            {
                AddButton.IsEnabled = true;
            }
            else
            {
                (LockerPanel.Children[newIdx] as Button).BorderBrush = Brushes.Red;
                FillLockerInfos(Exporter.GetLockerAt(newIdx));
                AddButton.IsEnabled = false;
            }
        }

        private void FillLockerInfos(Locker locker)
        {
            Debug.WriteLine("Writing : " + locker);

            // type
            LockTypeBox.SelectedIndex = int.Parse(locker.Type[1] + "") - 1;

            // entraxe
            EntraxeUpDown.Value = locker.Entraxe;

            // kit patere pieds
            PatereCheck.IsChecked = (locker.KitPiedPat) >= 2;
            PiedCheck.IsChecked = (locker.KitPiedPat % 2) != 0;

            // barre penderie
            PenderieBox.SelectedIndex = locker.IsPend;

            // type plaque
            PlaqueBox.SelectedIndex = locker.TypePlaq;

            // type serrure
            TypeSerrBox.SelectedIndex = TypeSerrBox.Items.IndexOf(locker.TypeSerr.Split(',')[0]);

            DisableIncompatibleOptions();
        }

        private void DisableIncompatibleOptions()
        {
            // disable penderie if incompatible options
            PenderieBox.IsEnabled = LockTypeBox.SelectedIndex < 2 && !PatereCheck.IsChecked.GetValueOrDefault();
            if (!PenderieBox.IsEnabled)
                PenderieBox.SelectedIndex = 0;
        }

        private void Btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                int idx = LockerPanel.Children.IndexOf(sender as Button);
                int oldIdx = GetSelectedLockerIndex();
                if (oldIdx != -1)
                    (LockerPanel.Children[oldIdx] as Button).BorderBrush = Brushes.Gray;

                //_selectedLockerIdx = idx;


                ContextMenu cm = new ContextMenu();
                MenuItem delItem = new MenuItem();
                MenuItem insLItem = new MenuItem();
                MenuItem insRItem = new MenuItem();
                MenuItem dupItem = new MenuItem();
                MenuItem moveLeftItem = new MenuItem();
                MenuItem moveRightItem = new MenuItem();

                moveLeftItem.IsEnabled = idx != 0;
                moveRightItem.IsEnabled = idx != LockerPanel.Children.Count - 1;

                delItem.Header = "Supprimer casier";
                insLItem.Header = "Insérer casier à gauche";
                insRItem.Header = "Insérer casier à droite";
                dupItem.Header = "Dupliquer casier";
                moveLeftItem.Header = "Déplacer à gauche";
                moveRightItem.Header = "Déplacer à droite";
                delItem.Click += (s, ev) => DelItem_Click(sender, ev);
                insLItem.Click += (s, ev) => InsLItem_Click(sender, ev);
                insRItem.Click += (s, ev) => InsRItem_Click(sender, ev);
                moveLeftItem.Click += (s, ev) => MoveLeftItem_Click(sender, ev);
                moveRightItem.Click += (s, ev) => MoveRightItem_Click(sender, ev);
                dupItem.Click += (s, ev) => DuplicateItem_Click(sender, ev);
                cm.Items.Add(insLItem);
                cm.Items.Add(insRItem);
                cm.Items.Add(moveLeftItem);
                cm.Items.Add(moveRightItem);
                cm.Items.Add(dupItem);
                cm.Items.Add(delItem);

                cm.IsOpen = true;
            }

        }

        private void MoveRightItem_Click(object sender, RoutedEventArgs ev)
        {
            int selectedIdx = LockerPanel.Children.IndexOf(sender as Button);
            Exporter.Swap(selectedIdx, selectedIdx + 1);
            string type1 = (LockerPanel.Children[selectedIdx] as Button).Content.ToString();
            string type2 = (LockerPanel.Children[selectedIdx + 1] as Button).Content.ToString();

            (LockerPanel.Children[selectedIdx] as Button).Content = type2;
            (LockerPanel.Children[selectedIdx + 1] as Button).Content = type1;
        }

        private void MoveLeftItem_Click(object sender, RoutedEventArgs ev)
        {
            int selectedIdx = LockerPanel.Children.IndexOf(sender as Button);
            Exporter.Swap(selectedIdx - 1, selectedIdx);
            string type1 = (LockerPanel.Children[selectedIdx] as Button).Content.ToString();
            string type2 = (LockerPanel.Children[selectedIdx - 1] as Button).Content.ToString();

            (LockerPanel.Children[selectedIdx] as Button).Content = type2;
            (LockerPanel.Children[selectedIdx - 1] as Button).Content = type1;
        }

        private void DuplicateItem_Click(object sender, RoutedEventArgs ev)
        {
            int selectedIdx = LockerPanel.Children.IndexOf(sender as Button);
            Locker currentLocker = Exporter.GetLockerAt(selectedIdx);
            InsertRight(selectedIdx, currentLocker);
        }

        private void InsRItem_Click(object sender, RoutedEventArgs e)
        {
            int selectedIdx = LockerPanel.Children.IndexOf(sender as Button);
            Locker locker = CreateLockerFromInfos();
            InsertRight(selectedIdx, locker);
        }

        private void InsertRight(int idx, Locker locker)
        {
            Button btn = CreateLockerButton(locker.Type);
            LockerPanel.Children.Insert(idx + 1, btn);
            Exporter.InsertLocker(idx + 1, locker);
        }

        private void InsLItem_Click(object sender, RoutedEventArgs e)
        {
            int idx = LockerPanel.Children.IndexOf(sender as Button);
            Locker locker = CreateLockerFromInfos();
            Button btn = CreateLockerButton(locker.Type);
//            _selectedLockerIdx++;

            LockerPanel.Children.Insert(idx, btn);
            Exporter.InsertLocker(idx, locker);
        }

        private void DelItem_Click(object sender, RoutedEventArgs e)
        {
            int idx = LockerPanel.Children.IndexOf(sender as Button);
            LockerPanel.Children.Remove(sender as Button);
            Exporter.DeleteLocker(idx);
        }



        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

            Exporter.Export();
            Application.Current.Shutdown();
        }
        private void ExportButtonUnder_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() != true)
                return;

            Exporter.ExportToFile(saveFileDialog.FileName);
        }

        private void PatereCheck_Checked(object sender, RoutedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void PatereCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void PiedCheck_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void LockConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            Process proc = Process.Start(@"\\SERV-KALYSSE\EDatas\Logiciels\LockConfig\LockConfig.exe");

            proc.WaitForExit();
            FillTypeSerrBox();
        }
        private void EntraxeUpDown_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void PenderieBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void PlaqueBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void TypeSerrBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedLockerInfos();
        }
        private void LockTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedLockerInfos();
            int idx = GetSelectedLockerIndex();
            if (idx != -1)
                (LockerPanel.Children[idx] as Button).Content = Exporter.GetLockerAt(idx).Type;
        }

        private void UpdateSelectedLockerInfos()
        {
            int idx = GetSelectedLockerIndex();
            if (idx != -1)
                Exporter.Set(idx, CreateLockerFromInfos());
            DisableIncompatibleOptions();
        }

        private Locker CreateLockerFromInfos()
        {
            string type = "";
            int entraxe = EntraxeUpDown.Value.GetValueOrDefault();
            string typeSerr = "";
            int kitPiedPat = 0;
            int isPend = 0;
            int typePlaq = 0;

            if ((LockTypeBox.SelectedItem as ComboBoxItem) != null)
            {
                type = (LockTypeBox.SelectedItem as ComboBoxItem).Content.ToString();
            }

            // type serrure
            if (TypeSerrBox.SelectedItem != null)
            {
                string typeSerrSelected = TypeSerrBox.SelectedItem.ToString();

                LockInfos selectedLock = Locks.Find(l =>
                {
                    return l.Name == typeSerrSelected;
                });

                typeSerr = $"{selectedLock.Name},{selectedLock.Code1},{selectedLock.Code2}";
            }


            // type plaque
            if ((PlaqueBox.SelectedItem as ComboBoxItem) != null)
            {
                string typePlaqSelected = (PlaqueBox.SelectedItem as ComboBoxItem).Content.ToString();

                if (typePlaqSelected == "Plaquette gravée 80x30 Gravoply")
                    typePlaq = 1;
                else if (typePlaqSelected == "Plaquette 70x40 Ojmar marquage Kalysse")
                    typePlaq = 2;
                else
                    typePlaq = 0;
            }

            // pied / patere
            if ((bool)PiedCheck.IsChecked)
                kitPiedPat++;
            if ((bool)PatereCheck.IsChecked)
                kitPiedPat += 2;

            // cintres
            if ((PenderieBox.SelectedItem as ComboBoxItem) != null)
            {
                string isPendSelected = (PenderieBox.SelectedItem as ComboBoxItem).Content.ToString();

                if (isPendSelected == "Oui avec cintres")
                    isPend = 1;
                else if (isPendSelected == "Oui sans cintres")
                    isPend = 2;
                else
                    isPend = 0;
            }

            return new Locker(type, entraxe, typeSerr, kitPiedPat, isPend, typePlaq);
        }
        private int GetSelectedLockerIndex()
        {
            int idx = 0;
            foreach (Button button in LockerPanel.Children)
            {
                if (button.BorderBrush == Brushes.Red)
                    return idx;

                idx++;
            }
            return -1;
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() != true)
                return;

            NumberPromptDialog dlg = new NumberPromptDialog();
            dlg.Owner = this;

            if (dlg.ShowDialog() != true)
                return;

            List<Locker> lockers = LockerReader.Read(openFileDialog.FileName);
            for (int i = 0; i < dlg.Value; i++)
            {
                foreach (var locker in lockers)
                {
                    AddLocker(locker);
                }
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddLocker(CreateLockerFromInfos());
            
        }

        /*
         * Add locker to exporter and display the new button
         */
        private void AddLocker(Locker locker)
        {
            Button btn = CreateLockerButton(locker.Type);
            LockerPanel.Children.Add(btn);
            Exporter.AddLocker(locker);
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            NumberPromptDialog dlg = new NumberPromptDialog();
            dlg.Owner = this;

            if (dlg.ShowDialog() != true)
                return;

            List<Locker> l = Exporter.CopyLockers();
            for (int i = 1; i < dlg.Value; i++)
                foreach (var locker in l)
                {
                    AddLocker(locker);
                }
        }

    }
}
