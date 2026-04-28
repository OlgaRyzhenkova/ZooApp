using System;
using System.ComponentModel.DataAnnotations;

namespace ZooApp.Models
{
    public class AccountingUnit
    {
        private Animal _animal = new();
        private DateTime _arrivalDate = DateTime.Now;
        private int _keepingCost;

        public Animal Animal
        {
            get => _animal;
            set => _animal = value
                ?? throw new ArgumentNullException(
                    nameof(value), "Тварина не може бути відсутньою (null).");
        }

        public DateTime ArrivalDate
        {
            get => _arrivalDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException(
                        "Дата прибуття не може бути у майбутньому.");
                _arrivalDate = value;
            }
        }

        [Range(0, 1_000_000,
            ErrorMessage = "Вартість утримання: від 0 до 1 000 000.")]
        public int KeepingCost
        {
            get => _keepingCost;
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Вартість утримання не може бути від’ємною.");
                if (value > 1_000_000)
                    throw new ArgumentException(
                        "Вартість утримання: максимум 1 000 000.");
                _keepingCost = value;
            }
        }

        public AccountingUnit() { }

        public AccountingUnit(Animal animal, DateTime arrivalDate, int keepingCost)
        {
            Animal = animal;
            ArrivalDate = arrivalDate;
            KeepingCost = keepingCost;
        }

        public override string ToString() =>
            $"{Animal}\n" +
            $"Дата прибуття: {ArrivalDate:dd.MM.yyyy}, " +
            $"Вартість утримання: {KeepingCost} грн";

        public string ToShortString() =>
            $"{Animal.ToShortString()} | " +
            $"{ArrivalDate:dd.MM.yyyy} | {KeepingCost} грн";
    }
}