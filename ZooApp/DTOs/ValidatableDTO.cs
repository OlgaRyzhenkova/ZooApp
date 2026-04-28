using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ZooApp.DTOs
{
    public abstract class ValidatableDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        protected bool SetField<T>(ref T field, T value,
                                   [CallerMemberName] string prop = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(prop);
            return true;
        }

        public string Error
        {
            get
            {
                var results = new List<ValidationResult>();
                Validator.TryValidateObject(
                    this, new ValidationContext(this), results, true);
                return string.Join("\n", results.Select(r => r.ErrorMessage));
            }
        }

        public string this[string columnName]
        {
            get
            {
                var prop = GetType().GetProperty(columnName);
                if (prop == null) return string.Empty;
                var ctx = new ValidationContext(this) { MemberName = columnName };
                var results = new List<ValidationResult>();
                Validator.TryValidateProperty(prop.GetValue(this), ctx, results);
                return results.Count > 0
                    ? (results[0].ErrorMessage ?? string.Empty)
                    : string.Empty;
            }
        }

        public bool IsValid()
        {
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                this, new ValidationContext(this), results, true);
        }

        public IReadOnlyList<string> GetValidationErrors()
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(
                this, new ValidationContext(this), results, true);
            return results.Select(r => r.ErrorMessage ?? string.Empty).ToList();
        }
    }
}