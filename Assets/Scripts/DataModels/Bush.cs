using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class Bush
    {
        public Vector3 Position { get; set; }

        [JsonIgnore]
        public BushBehaviour BushBehaviour { get; set; }

        public Bush()
        {
            Position = new Vector3(0, 0, 0);
        }
    }
}