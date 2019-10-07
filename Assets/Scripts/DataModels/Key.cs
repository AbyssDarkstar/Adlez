using Assets.Scripts.Interfaces;
using Newtonsoft.Json;

namespace Assets.Scripts.DataModels
{
    public class Key : IItem
    {
        [JsonIgnore]
        public KeyBehaviour KeyBehaviour { get; set; }

        public string Type;

        public bool IsBossKey { get; set; }

        public Key()
        {
            Type = "Key";
        }

        public void OnPickUp(PlayerController player)
        {
            player.AddKey(IsBossKey);
        }
    }
}