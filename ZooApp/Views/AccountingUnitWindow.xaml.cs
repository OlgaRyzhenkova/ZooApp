using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZooApp.DTOs;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class AccountingUnitWindow : Window
    {
        private AccountingUnit _target;
        private AccountingUnitDTO _dto;
        private AccountingUnitDTO _originalDto;
        private bool _savedByButton;

        public AccountingUnit? ResultUnit { get; private set; }

        public AccountingUnitWindow(AccountingUnit? existing)
        {
            InitializeComponent();

            if (existing != null)
            {
                _target = existing;
                _dto = AccountingUnitDTO.FromModel(existing);
                _originalDto = _dto.Clone();
                Title = WindowTitle.Text = "✏️  Редагування облікової одиниці";
            }
            else
            {
                _target = new AccountingUnit(
                    new Animal("(вид)", "(країна)", "(кличка)",
                               DateTime.Now.AddYears(-1)),
                    DateTime.Now, 0);
                _dto = AccountingUnitDTO.FromModel(_target);
                _originalDto = _dto.Clone();
                Title = WindowTitle.Text = "➕  Нова облікова одиниця";
            }

            DataContext = _dto;
            ArrivalDatePicker.SelectedDate = _dto.ArrivalDate;
            RefreshAnimalDisplay();
        }

        private void ArrivalDatePicker_SelectedDateChanged(
            object? sender, SelectionChangedEventArgs e)
        {
            if (!ArrivalDatePicker.SelectedDate.HasValue) return;
            var date = ArrivalDatePicker.SelectedDate.Value;
            if (date > DateTime.Now)
            {
                MessageBox.Show(
                    "Дата прибуття не може бути у майбутньому.",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                ArrivalDatePicker.SelectedDate = _dto.ArrivalDate;
                return;
            }
            _dto.ArrivalDate = date;
        }

        private void EditAnimalBtn_Click(object sender, RoutedEventArgs e)
        {
            var win = new AnimalWindow(_dto.Animal) { Owner = this };
            if (win.ShowDialog() == true && win.ResultDto != null)
            {
                _dto.Animal = win.ResultDto;
                RefreshAnimalDisplay();
            }
        }

        private void RefreshAnimalDisplay()
        {
            AnimalDisplayText.Text =
                string.IsNullOrWhiteSpace(_dto.Animal.Name) ||
                _dto.Animal.Name == "(кличка)"
                    ? "(тварину не вказано)"
                    : _dto.Animal.ToModel().ToString();
        }

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
            DialogResult = false;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_savedByButton) { base.OnClosing(e); return; }

            var res = MessageBox.Show(
                "Зберегти зміни?", "Підтвердження",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (res == MessageBoxResult.Cancel) { e.Cancel = true; return; }

            if (res == MessageBoxResult.Yes)
            {
                if (!ValidateAndApply()) { e.Cancel = true; return; }
                DialogResult = true;
            }
            else
            {
                _originalDto.ApplyTo(_target);
                DialogResult = false;
            }
            base.OnClosing(e);
        }

        private bool ValidateAndApply()
        {
            if (!_dto.IsValid() || !_dto.Animal.IsValid())
            {
                var errors = new System.Collections.Generic.List<string>();
                errors.AddRange(_dto.GetValidationErrors());
                errors.AddRange(_dto.Animal.GetValidationErrors());
                MessageBox.Show(
                    "Будь ласка, виправте помилки:\n" + string.Join("\n", errors),
                    "Помилки валідації",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            try
            {
                _dto.ApplyTo(_target);
                ResultUnit = _target;
                return true;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Помилка даних",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }
    }
}