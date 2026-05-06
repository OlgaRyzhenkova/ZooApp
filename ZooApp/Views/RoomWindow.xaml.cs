using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZooApp.DTOs;
using ZooApp.Enums;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class RoomWindow : Window
    {
        private Room _target;
        private RoomDTO _dto;
        private RoomDTO _fieldsSnapshot;
        private List<AccountingUnitDTO> _animalsSnapshot;
        private ObservableCollection<AccountingUnit> _animals = new();
        private bool _savedByButton;

        public Room? ResultRoom { get; private set; }

        // ── Конструктор ───────────────────────────────────────────────────────
        public RoomWindow(Room? existing)
        {
            InitializeComponent();

            RoomTypeCombo.ItemsSource = Enum.GetValues(typeof(RoomType));

            if (existing != null)
            {
                _target = existing;
                _dto = RoomDTO.FromModel(existing);
                _fieldsSnapshot = _dto.Clone();
                _animalsSnapshot = existing.Animals
                                           .Select(AccountingUnitDTO.FromModel)
                                           .ToList();
                foreach (var a in existing.Animals)
                    _animals.Add(a);
                Title = WindowTitle.Text = $"✏️  Редагування приміщення №{existing.Number}";
            }
            else
            {
                _target = new Room(RoomType.Клітка, 1, 10, 0);
                _dto = RoomDTO.FromModel(_target);
                _fieldsSnapshot = _dto.Clone();
                _animalsSnapshot = new List<AccountingUnitDTO>();
                Title = WindowTitle.Text = "➕  Нове приміщення";
            }

            DataContext = _dto;
            RoomTypeCombo.SelectedItem = _dto.RoomType;
            AnimalsGrid.ItemsSource = _animals;
            UpdateShortInfo();
        }

        // ── Поля приміщення ───────────────────────────────────────────────────

        private void RoomTypeCombo_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (RoomTypeCombo.SelectedItem is RoomType rt)
            {
                _dto.RoomType = rt;
                UpdateShortInfo();
            }
        }

        private void RoomField_LostFocus(object sender, RoutedEventArgs e) =>
            UpdateShortInfo();

        // ── Кнопки тварин ────────────────────────────────────────────────────

        private void AnimalsGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            bool sel = AnimalsGrid.SelectedItem != null;
            EditAnimalBtn.IsEnabled = sel;
            DeleteAnimalBtn.IsEnabled = sel;
        }

        private void AnimalsGrid_MouseDoubleClick(
            object sender, MouseButtonEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is AccountingUnit)
                EditAnimalBtn_Click(sender, e);
        }

        private void AddAnimalBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new AccountingUnitWindow(null) { Owner = this };
            if (win.ShowDialog() == true && win.ResultUnit != null)
            {
                _animals.Add(win.ResultUnit);
                UpdateShortInfo();
            }
        }

        private void EditAnimalBtn_Click(object sender, RoutedEventArgs e)
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

        private void DeleteAnimalBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not AccountingUnit selected) return;
            var res = MessageBox.Show(
                $"Видалити тварину «{selected.Animal.ToShortString()}» з приміщення?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                _animals.Remove(selected);
                UpdateShortInfo();
            }
        }

        // ── Зберегти і закрити / Скасувати і закрити ─────────────────────────

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAndApply()) return;
            _savedByButton = true;
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            _savedByButton = true;
            RestoreSnapshot();
            DialogResult = false;
            Close();
        }

        // ── Закриття хрестиком ────────────────────────────────────────────────

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_savedByButton) { base.OnClosing(e); return; }

            var res = MessageBox.Show(
                "Зберегти зміни у даних приміщення?",
                "Підтвердження",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (res == MessageBoxResult.Cancel) { e.Cancel = true; return; }

            if (res == MessageBoxResult.Yes)
            {
                if (!ValidateAndApply()) { e.Cancel = true; return; }
                DialogResult = true;
            }
            else
            {
                RestoreSnapshot();
                DialogResult = false;
            }
            base.OnClosing(e);
        }

        // ── Валідація та збереження ───────────────────────────────────────────

        private bool ValidateAndApply()
        {
            if (!_dto.IsValid())
            {
                MessageBox.Show(
                    "Будь ласка, виправте помилки:\n" +
                    string.Join("\n", _dto.GetValidationErrors()),
                    "Помилки валідації",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            try
            {
                _dto.ApplyTo(_target);
                _target.Animals = new List<AccountingUnit>(_animals);
                ResultRoom = _target;
                return true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Помилка даних",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void RestoreSnapshot()
        {
            _fieldsSnapshot.ApplyTo(_target);
            _target.Animals = _animalsSnapshot
                .Select(dto => dto.ToModel())
                .ToList();
        }

        private void UpdateShortInfo()
        {
            try
            {
                var tempRoom = new Room(
                    _dto.RoomType,
                    _dto.Number > 0 ? _dto.Number : 1,
                    _dto.Size > 0 ? _dto.Size : 1,
                    _dto.CleaningCost);
                foreach (var a in _animals) tempRoom.AddAnimal(a);
                RoomShortInfo.Text = tempRoom.ToShortString();
            }
            catch { RoomShortInfo.Text = string.Empty; }
        }
    }
}