using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZooApp.DTOs;
using ZooApp.Models;
using ZooApp.Serialization;

namespace ZooApp.Views
{
    public partial class MainWindow : Window
    {
        private Zoo _zoo = new("Зоопарк");
        private ObservableCollection<Room> _rooms = new();

        public MainWindow()
        {
            InitializeComponent();
            TryLoadData();
            RoomsGrid.ItemsSource = _rooms;
            UpdateStats();
            SavePathText.Text = $"Файл збереження: {DataManager.SaveFilePath}";
        }

        private void TryLoadData()
        {
            var loaded = DataManager.Load();
            if (loaded == null) return;
            try
            {
                _zoo = loaded.ToModel();
                _rooms.Clear();
                foreach (var room in _zoo.Rooms)
                    _rooms.Add(room);
            }
            catch { }
        }

        // ── Таблиця приміщень ─────────────────────────────────────────────────

        private void RoomsGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            bool sel = RoomsGrid.SelectedItem != null;
            EditRoomBtn.IsEnabled = sel;
            DeleteRoomBtn.IsEnabled = sel;
        }

        private void RoomsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (RoomsGrid.SelectedItem is Room) EditRoomBtn_Click(sender, e);
        }

        // ── Кнопки ───────────────────────────────────────────────────────────

        private void AddRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new RoomWindow(null) { Owner = this };
            if (win.ShowDialog() == true && win.ResultRoom != null)
            {
                _zoo.AddRoom(win.ResultRoom);
                _rooms.Add(win.ResultRoom);
                UpdateStats();
            }
        }

        private void EditRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsGrid.SelectedItem is not Room selected) return;
            var win = new RoomWindow(selected) { Owner = this };
            if (win.ShowDialog() == true)
            {
                int idx = _rooms.IndexOf(selected);
                if (idx >= 0)
                {
                    _rooms.RemoveAt(idx);
                    _rooms.Insert(idx, selected);
                    RoomsGrid.SelectedIndex = idx;
                }
                UpdateStats();
            }
        }

        private void DeleteRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsGrid.SelectedItem is not Room selected) return;
            var res = MessageBox.Show(
                $"Видалити приміщення №{selected.Number} ({selected.DisplayType})?\n" +
                $"Разом з ним буде видалено {selected.Animals.Count} тварин.",
                "Підтвердження видалення",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                _zoo.RemoveRoom(selected);
                _rooms.Remove(selected);
                UpdateStats();
            }
        }

        // ── Меню ─────────────────────────────────────────────────────────────

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            MessageBox.Show("Дані збережено успішно.", "Збережено",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

        private void MenuAbout_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show(
                "Зоопарк — Управління приміщеннями\n" +
                "Лабораторна робота №4-максі, Варіант 4\n\n" +
                "Класи: Тварина → Одиниця обліку → Приміщення → Зоопарк\n" +
                "Серіалізація: Newtonsoft.Json\n" +
                "Валідація: IDataErrorInfo + Data Annotations + setters",
                "Про програму",
                MessageBoxButton.OK, MessageBoxImage.Information);

        // ── Закриття — автозбереження ─────────────────────────────────────────

        private void Window_Closing(object sender, CancelEventArgs e) =>
            SaveData();

        // ── Допоміжні ────────────────────────────────────────────────────────

        private void SaveData()
        {
            // Синхронізуємо _zoo.Rooms з актуальною ObservableCollection
            _zoo.Rooms.Clear();
            foreach (var room in _rooms)
                _zoo.Rooms.Add(room);

            DataManager.Save(ZooDTO.FromModel(_zoo));
        }

        private void UpdateStats()
        {
            ZooNameText.Text = _zoo.Name;
            TotalRoomsText.Text = $"{_zoo.Rooms.Count} приміщень";
            TotalAnimalsText.Text = $"{_zoo.TotalAnimals} тварин";
            TotalCostText.Text = $"{_zoo.TotalKeepingCost} грн.";
        }
    }
}