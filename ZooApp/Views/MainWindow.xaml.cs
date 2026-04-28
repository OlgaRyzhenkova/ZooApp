using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZooApp.DTOs;
using ZooApp.Enums;
using ZooApp.Models;
using ZooApp.Serialization;

namespace ZooApp.Views
{
    public partial class MainWindow : Window
    {
        private Room _room = new(RoomType.Cage, 1, 50, 200);
        private RoomDTO _roomDto = new();
        private ObservableCollection<AccountingUnit> _animals = new();

        public MainWindow()
        {
            InitializeComponent();
            InitComboBox();
            TryLoadData();
            DataContext = _roomDto;
            RoomTypeCombo.SelectedItem = _roomDto.RoomType;
            AnimalsGrid.ItemsSource = _animals;
            UpdateShortInfo();
            SavePathText.Text = $"Файл збереження: {DataManager.SaveFilePath}";
        }

        private void InitComboBox() =>
            RoomTypeCombo.ItemsSource = System.Enum.GetValues(typeof(RoomType));

        private void TryLoadData()
        {
            var loaded = DataManager.Load();
            if (loaded == null) { _roomDto = RoomDTO.FromModel(_room); return; }
            try
            {
                _room = loaded.ToModel();
                _animals.Clear();
                foreach (var unit in _room.Animals)
                    _animals.Add(unit);
                _roomDto = RoomDTO.FromModel(_room);
            }
            catch { _roomDto = RoomDTO.FromModel(_room); }
        }

        private void RoomTypeCombo_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (RoomTypeCombo.SelectedItem is RoomType rt)
            {
                _roomDto.RoomType = rt;
                _room.RoomType = rt;
                UpdateShortInfo();
            }
        }

        private void RoomField_LostFocus(object sender, RoutedEventArgs e)
        {
            TryApplyRoomFields();
            UpdateShortInfo();
        }

        private bool TryApplyRoomFields()
        {
            if (!_roomDto.IsValid())
            {
                MessageBox.Show(
                    "Будь ласка, виправте помилки в даних приміщення:\n" +
                    string.Join("\n", _roomDto.GetValidationErrors()),
                    "Помилки валідації",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            try
            {
                _room.Number = _roomDto.Number;
                _room.Size = _roomDto.Size;
                _room.CleaningCost = _roomDto.CleaningCost;
                _room.RoomType = _roomDto.RoomType;
                return true;
            }
            catch (System.ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void AnimalsGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            bool selected = AnimalsGrid.SelectedItem != null;
            EditBtn.IsEnabled = selected;
            DeleteBtn.IsEnabled = selected;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new AccountingUnitWindow(null) { Owner = this };
            if (win.ShowDialog() == true && win.ResultUnit != null)
            {
                _room.AddAnimal(win.ResultUnit);
                _animals.Add(win.ResultUnit);
                UpdateShortInfo();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not AccountingUnit selected) return;
            var win = new AccountingUnitWindow(selected) { Owner = this };
            if (win.ShowDialog() == true)
            {
                int idx = _animals.IndexOf(selected);
                if (idx >= 0)
                {
                    _animals.RemoveAt(idx);
                    _animals.Insert(idx, selected);
                    AnimalsGrid.SelectedIndex = idx;
                }
                UpdateShortInfo();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not AccountingUnit selected) return;
            var res = MessageBox.Show(
                $"Видалити \"{selected.Animal.ToShortString()}\" з приміщення?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                _room.RemoveAnimal(selected);
                _animals.Remove(selected);
                UpdateShortInfo();
            }
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            MessageBox.Show("Дані успішно збережено.", "Збережено",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

        private void MenuAbout_Click(object sender, RoutedEventArgs e) =>
            MessageBox.Show(
                "Зоопарк — Управління приміщеннями\n" +
                "Лабораторна робота №4, Варіант 4\n\n" +
                "Класи: Animal, AccountingUnit, Room\n" +
                "Серіалізація: Newtonsoft.Json\n" +
                "Валідація: IDataErrorInfo + Data Annotations + сетери",
                "Про програму",
                MessageBoxButton.OK, MessageBoxImage.Information);

        private void Window_Closing(object sender, CancelEventArgs e) =>
            SaveData();

        private void SaveData()
        {
            TryApplyRoomFields();
            DataManager.Save(RoomDTO.FromModel(_room));
        }

        private void UpdateShortInfo() =>
            ShortInfoText.Text = _room.ToShortString();
    }
}