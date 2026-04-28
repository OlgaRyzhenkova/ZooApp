using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ZooApp.Enums;
using ZooApp.Models;

namespace ZooApp.DTOs
{
    public class RoomDTO : ValidatableDTO
    {
        private RoomType _roomType;
        private int _number = 1;
        private int _size = 10;
        private int _cleaningCost;
        private List<AccountingUnitDTO> _animals = new();

        public RoomType RoomType
        {
            get => _roomType;
            set => SetField(ref _roomType, value);
        }

        [Range(1, 9_999, ErrorMessage = "Номер приміщення: від 1 до 9999")]
        public int Number
        {
            get => _number;
            set => SetField(ref _number, value);
        }

        [Range(1, 100_000, ErrorMessage = "Площа: від 1 до 100 000 м²")]
        public int Size
        {
            get => _size;
            set => SetField(ref _size, value);
        }

        [Range(0, 1_000_000,
            ErrorMessage = "Вартість прибирання: від 0 до 1 000 000 грн")]
        public int CleaningCost
        {
            get => _cleaningCost;
            set => SetField(ref _cleaningCost, value);
        }

        public List<AccountingUnitDTO> Animals
        {
            get => _animals;
            set => SetField(ref _animals, value ?? new());
        }

        public static RoomDTO FromModel(Room room) => new()
        {
            RoomType = room.RoomType,
            Number = room.Number,
            Size = room.Size,
            CleaningCost = room.CleaningCost,
            Animals = room.Animals
                               .Select(AccountingUnitDTO.FromModel)
                               .ToList()
        };

        public Room ToModel()
        {
            var room = new Room(RoomType, Number, Size, CleaningCost);
            foreach (var a in Animals)
                room.AddAnimal(a.ToModel());
            return room;
        }
    }
}