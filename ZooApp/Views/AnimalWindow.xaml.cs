using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZooApp.DTOs;

namespace ZooApp.Views
{
    public partial class AnimalWindow : Window
    {
        private AnimalDTO _dto;
        private AnimalDTO _originalDto;
        private bool _savedByButton;

        public AnimalDTO? ResultDto { get; private set; }

        public AnimalWindow(AnimalDTO existing)
        {
            InitializeComponent();
            _dto = existing.Clone();
            _originalDto = existing.Clone();
            DataContext = _dto;
            BirthDatePicker.SelectedDate = _dto.BirthDate;
            _dto.PropertyChanged += (_, _) => UpdatePreview();
            UpdatePreview();
        }

        private void BirthDatePicker_SelectedDateChanged(
            object? sender, SelectionChangedEventArgs e)
        {
            if (!BirthDatePicker.SelectedDate.HasValue) return;
            var date = BirthDatePicker.SelectedDate.Value;

            if (date > DateTime.Now)
            {
                MessageBox.Show(
                    "Дата народження не може бути у майбутньому.",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                BirthDatePicker.SelectedDate = _dto.BirthDate;
                return;
            }

            if (date < new DateTime(1900, 1, 1))
            {
                MessageBox.Show("Некоректна дата народження (раніше 1900 року).",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                BirthDatePicker.SelectedDate = _dto.BirthDate;
                return;
            }

            _dto.BirthDate = date;
        }

        private void UpdatePreview()
        {
            try { PreviewText.Text = _dto.ToModel().ToString(); }
            catch { PreviewText.Text = "(введіть коректні дані)"; }
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
                "Зберегти зміни в даних тварини?", "Підтвердження",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (res == MessageBoxResult.Cancel) { e.Cancel = true; return; }

            if (res == MessageBoxResult.Yes)
            {
                if (!ValidateAndApply()) { e.Cancel = true; return; }
                DialogResult = true;
            }
            else DialogResult = false;

            base.OnClosing(e);
        }

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
                _ = _dto.ToModel();
                ResultDto = _dto;
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