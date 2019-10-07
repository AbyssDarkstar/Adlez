using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class Chest
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }

        public Item Contents;

        public bool Open;

        [JsonIgnore]
        public ChestBehaviour ChestBehaviour { get; set; }

        public Chest()
        {
            Open = false;
            Position = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}