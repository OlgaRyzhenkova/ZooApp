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
                    nameof(value), "Тварина не може бути null.");
        }

        public DateTime ArrivalDate
        {
            get => _arrivalDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException(
                        "Дата надходження не може бути в майбутньому.");
                _arrivalDate = value;
            }
        }

        [Range(0, 1_000_000,
            ErrorMessage = "Вартість утримання: від 0 до 1 000 000 грн.")]
        public int KeepingCost
        {
            get => _keepingCost;
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Вартість утримання не може бути від'ємною.");
                if (value > 1_000_000)
                    throw new ArgumentException(
                        "Вартість утримання: максимум 1 000 000 грн.");
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
            $"Дата надходження: {ArrivalDate:dd.MM.yyyy}, " +
            $"Вартість утримання: {KeepingCost} грн.";

        public string ToShortString() =>
            $"{Animal.ToShortString()} | " +
            $"{ArrivalDate:dd.MM.yyyy} | {KeepingCost} грн.";
    }
}