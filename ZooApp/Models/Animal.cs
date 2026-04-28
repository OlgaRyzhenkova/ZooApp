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

        [Required(ErrorMessage = "Вид тварини є обов'язковим")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Вид: від 2 до 100 символів")]
        public string Species
        {
            get => _species;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва виду не може бути порожньою.");
                if (value.Trim().Length < 2)
                    throw new ArgumentException("Вид: мінімум 2 символи.");
                _species = value.Trim();
            }
        }

        [Required(ErrorMessage = "Країна походження є обов'язковою")]
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

        [Required(ErrorMessage = "Кличка є обов'язковою")]
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
                    throw new ArgumentException("Дата народження не може бути у майбутньому.");
                if (value < new DateTime(1900, 1, 1))
                    throw new ArgumentException("Дата народження некоректна (раніше 1900 року).");
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