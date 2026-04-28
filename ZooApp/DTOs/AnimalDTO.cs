using System;
using System.ComponentModel.DataAnnotations;
using ZooApp.Models;

namespace ZooApp.DTOs
{
    public class AnimalDTO : ValidatableDTO
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
            set => SetField(ref _species, value ?? string.Empty);
        }

        [Required(ErrorMessage = "Країна походження є обов'язковою")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Країна походження: від 2 до 100 символів")]
        public string CountryOfOrigin
        {
            get => _countryOfOrigin;
            set => SetField(ref _countryOfOrigin, value ?? string.Empty);
        }

        [Required(ErrorMessage = "Кличка є обов'язковою")]
        [StringLength(50, MinimumLength = 1,
            ErrorMessage = "Кличка: від 1 до 50 символів")]
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value ?? string.Empty);
        }

        public DateTime BirthDate
        {
            get => _birthDate;
            set => SetField(ref _birthDate, value);
        }

        public static AnimalDTO FromModel(Animal a) => new()
        {
            Species = a.Species,
            CountryOfOrigin = a.CountryOfOrigin,
            Name = a.Name,
            BirthDate = a.BirthDate
        };

        public Animal ToModel() =>
            new(Species.Trim(), CountryOfOrigin.Trim(), Name.Trim(), BirthDate);

        public void ApplyTo(Animal animal)
        {
            animal.Species = Species.Trim();
            animal.CountryOfOrigin = CountryOfOrigin.Trim();
            animal.Name = Name.Trim();
            animal.BirthDate = BirthDate;
        }

        public AnimalDTO Clone() => new()
        {
            Species = Species,
            CountryOfOrigin = CountryOfOrigin,
            Name = Name,
            BirthDate = BirthDate
        };
    }
}