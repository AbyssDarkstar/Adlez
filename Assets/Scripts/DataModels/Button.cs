using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class Button
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }

        public string Action { get; set; }

        [JsonIgnore]
        public ButtonBehaviour ButtonBehaviour { get; set; }

    }
}