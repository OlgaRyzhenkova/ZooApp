using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using ZooApp.Enums;

namespace ZooApp.Models
{
    public class Room
    {
        private RoomType _roomType;
        private int _number;
        private int _size;
        private int _cleaningCost;
        private List<AccountingUnit> _animals = new();

        public RoomType RoomType
        {
            get => _roomType;
            set => _roomType = value;
        }

        [Range(1, 9_999, ErrorMessage = "Номер приміщення: від 1 до 9999")]
        public int Number
        {
            get => _number;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Номер приміщення повинен бути більше 0.");
                if (value > 9_999)
                    throw new ArgumentException("Номер приміщення: максимум 9999.");
                _number = value;
            }
        }

        [Range(1, 100_000, ErrorMessage = "Площа: від 1 до 100 000 м²")]
        public int Size
        {
            get => _size;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Площа приміщення повинна бути більше 0.");
                _size = value;
            }
        }

        [Range(0, 1_000_000,
            ErrorMessage = "Вартість прибирання: від 0 до 1 000 000 грн.")]
        public int CleaningCost
        {
            get => _cleaningCost;
            set
            {
                if (value < 0)
                    throw new ArgumentException(
                        "Вартість прибирання не може бути від'ємною.");
                _cleaningCost = value;
            }
        }

        public List<AccountingUnit> Animals
        {
            get => _animals;
            set => _animals = value ?? new List<AccountingUnit>();
        }

        public int TotalKeepingCost => _animals.Sum(a => a.KeepingCost);

        /// <summary>Тип приміщення з емоджі для відображення у таблиці.</summary>
        public string DisplayType => _roomType switch
        {
            RoomType.Клітка => "🦁 Клітка",
            RoomType.Вольєр => "🦅 Вольєр",
            RoomType.Акваріум => "🐠 Акваріум",
            RoomType.Тераріум => "🦎 Тераріум",
            _ => _roomType.ToString()
        };

        public Room() { }

        public Room(RoomType roomType, int number, int size, int cleaningCost)
        {
            RoomType = roomType;
            Number = number;
            Size = size;
            CleaningCost = cleaningCost;
        }

        public void AddAnimal(AccountingUnit unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));
            _animals.Add(unit);
        }

        public void RemoveAnimal(AccountingUnit unit) => _animals.Remove(unit);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Приміщення №{Number} ({DisplayType})");
            sb.AppendLine($"Площа: {Size} м², Вартість прибирання: {CleaningCost} грн.");
            sb.AppendLine($"Кількість тварин: {_animals.Count}");
            foreach (var a in _animals)
                sb.AppendLine($"  • {a.ToShortString()}");
            sb.AppendLine($"Загальна вартість утримання: {TotalKeepingCost} грн.");
            return sb.ToString();
        }

        public string ToShortString() =>
            $"Приміщення №{Number} ({DisplayType}) | " +
            $"Тварин: {_animals.Count} | " +
            $"Вартість утримання: {TotalKeepingCost} грн.";
    }
}