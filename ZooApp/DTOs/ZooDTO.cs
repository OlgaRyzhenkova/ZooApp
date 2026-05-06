using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ZooApp.Models;

namespace ZooApp.DTOs
{
    public class ZooDTO : ValidatableDTO
    {
        private string _name = "Зоопарк";
        private List<RoomDTO> _rooms = new();

        [Required(ErrorMessage = "Назва зоопарку обов'язкова")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Назва: від 2 до 100 символів")]
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value ?? string.Empty);
        }

        public List<RoomDTO> Rooms
        {
            get => _rooms;
            set => SetField(ref _rooms, value ?? new());
        }

        public static ZooDTO FromModel(Zoo zoo) => new()
        {
            Name = zoo.Name,
            Rooms = zoo.Rooms.Select(RoomDTO.FromModel).ToList()
        };

        public Zoo ToModel()
        {
            var zoo = new Zoo(_name);
            foreach (var r in Rooms)
                zoo.AddRoom(r.ToModel());
            return zoo;
        }
    }
}