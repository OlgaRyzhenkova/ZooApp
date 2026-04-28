using System;
using System.ComponentModel.DataAnnotations;
using ZooApp.Models;

namespace ZooApp.DTOs
{
    public class AccountingUnitDTO : ValidatableDTO
    {
        private AnimalDTO _animal = new();
        private DateTime _arrivalDate = DateTime.Now;
        private int _keepingCost;

        public AnimalDTO Animal
        {
            get => _animal;
            set => SetField(ref _animal, value ?? new AnimalDTO());
        }

        public DateTime ArrivalDate
        {
            get => _arrivalDate;
            set => SetField(ref _arrivalDate, value);
        }

        [Range(0, 1_000_000,
            ErrorMessage = "Вартість утримання: від 0 до 1 000 000 грн")]
        public int KeepingCost
        {
            get => _keepingCost;
            set => SetField(ref _keepingCost, value);
        }

        public static AccountingUnitDTO FromModel(AccountingUnit u) => new()
        {
            Animal = AnimalDTO.FromModel(u.Animal),
            ArrivalDate = u.ArrivalDate,
            KeepingCost = u.KeepingCost
        };

        public AccountingUnit ToModel() =>
            new(Animal.ToModel(), ArrivalDate, KeepingCost);

        public void ApplyTo(AccountingUnit unit)
        {
            Animal.ApplyTo(unit.Animal);
            unit.ArrivalDate = ArrivalDate;
            unit.KeepingCost = KeepingCost;
        }

        public AccountingUnitDTO Clone() => new()
        {
            Animal = Animal.Clone(),
            ArrivalDate = ArrivalDate,
            KeepingCost = KeepingCost
        };
    }
}