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

        [Required(ErrorMessage = "Species is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Species: 2 to 100 characters")]
        public string Species
        {
            get => _species;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Species cannot be empty.");
                if (value.Trim().Length < 2)
                    throw new ArgumentException("Species: minimum 2 characters.");
                _species = value.Trim();
            }
        }

        [Required(ErrorMessage = "Country of origin is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Country of origin: 2 to 100 characters")]
        public string CountryOfOrigin
        {
            get => _countryOfOrigin;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Country of origin cannot be empty.");
                _countryOfOrigin = value.Trim();
            }
        }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 1,
            ErrorMessage = "Name: 1 to 50 characters")]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Animal name cannot be empty.");
                _name = value.Trim();
            }
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Birth date cannot be in the future.");
                if (value < new DateTime(1900, 1, 1))
                    throw new ArgumentException("Birth date is invalid (before 1900).");
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
            $"Species: {Species}, Name: {Name}, " +
            $"Country: {CountryOfOrigin}, " +
            $"Birth date: {BirthDate:dd.MM.yyyy}";

        public string ToShortString() => $"{Name} ({Species})";
    }
}