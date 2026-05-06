using System;
using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    public class Animal
    {
        private string _species = string.Empty;
        private string _countryOfOrigin = string.Empty;
        private string _name = string.Empty;
        private DateTime _birthDate = DateTime.Now.AddYears(-1);

        [Required(ErrorMessage = "Вид тварини обов'язковий")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Вид тварини: від 2 до 100 символів")]
        public string Species
        {
            get => _species;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Вид тварини не може бути порожнім.");
                if (value.Trim().Length < 2)
                    throw new ArgumentException("Вид тварини: мінімум 2 символи.");
                _species = value.Trim();
            }
        }

        [Required(ErrorMessage = "Країна походження обов'язкова")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Країна походження: від 2 до 100 символів")]
        public string CountryOfOrigin
        {
            get => _countryOfOrigin;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Країна походження не може бути порожньою.");
                _countryOfOrigin = value.Trim();
            }
        }

        [Required(ErrorMessage = "Кличка тварини обов'язкова")]
        [StringLength(50, MinimumLength = 1,
            ErrorMessage = "Кличка: від 1 до 50 символів")]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Кличка тварини не може бути порожньою.");
                _name = value.Trim();
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException(
                        "Дата народження не може бути в майбутньому.");
                if (value < new DateTime(1900, 1, 1))
                    throw new ArgumentException("Некоректна дата (до 1900 р.).");
                _birthDate = value;
            }
        }

        public Animal() { }

        public Animal(string species, string countryOfOrigin,
                      string name, DateTime birthDate)
        {
            Species = species;
            CountryOfOrigin = countryOfOrigin;
            Name = name;
            BirthDate = birthDate;
        }

        public override string ToString() =>
            $"Вид: {Species}, Кличка: {Name}, " +
            $"Країна: {CountryOfOrigin}, " +
            $"Дата народження: {BirthDate:dd.MM.yyyy}";

        public string ToShortString() => $"{Name} ({Species})";
    }
}