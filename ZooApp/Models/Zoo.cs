using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZooApp.Models
{
    public class Zoo
    {
        private string _name = "Зоопарк";
        private List<Room> _rooms = new();

        [Required(ErrorMessage = "Назва зоопарку обов'язкова")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Назва: від 2 до 100 символів")]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва зоопарку не може бути порожньою.");
                _name = value.Trim();
            }
        }

        public List<Room> Rooms
        {
            get => _rooms;
            set => _rooms = value ?? new List<Room>();
        }

        public int TotalAnimals => _rooms.Sum(r => r.Animals.Count);
        public int TotalKeepingCost => _rooms.Sum(r => r.TotalKeepingCost);

        public Zoo() { }
        public Zoo(string name) { Name = name; }

        public void AddRoom(Room room)
        {
            if (room == null) throw new ArgumentNullException(nameof(room));
            _rooms.Add(room);
        }

        public void RemoveRoom(Room room) => _rooms.Remove(room);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Зоопарк: {Name}");
            sb.AppendLine($"Приміщень: {_rooms.Count}, Тварин всього: {TotalAnimals}");
            foreach (var r in _rooms)
                sb.AppendLine($"  • {r.ToShortString()}");
            return sb.ToString();
        }

        public string ToShortString() =>
            $"{Name} | Приміщень: {_rooms.Count} | " +
            $"Тварин: {TotalAnimals} | " +
            $"Загальна вартість: {TotalKeepingCost} грн.";
    }
}