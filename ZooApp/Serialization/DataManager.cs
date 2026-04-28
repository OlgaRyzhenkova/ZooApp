using System;
using System.IO;
using Newtonsoft.Json;
using ZooApp.DTOs;

namespace ZooApp.Serialization
{
    public static class DataManager
    {
        private static readonly string SavePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "zoo_data.json");

        private static readonly JsonSerializerSettings Settings = new()
        {
            Formatting = Formatting.Indented,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.None
        };

        public static void Save(RoomDTO dto)
        {
            var json = JsonConvert.SerializeObject(dto, Settings);
            File.WriteAllText(SavePath, json);
        }

        public static RoomDTO? Load()
        {
            if (!File.Exists(SavePath)) return null;
            try
            {
                var json = File.ReadAllText(SavePath);
                return JsonConvert.DeserializeObject<RoomDTO>(json, Settings);
            }
            catch { return null; }
        }

        public static string SaveFilePath => SavePath;
    }
}