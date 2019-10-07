using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Scripts.DataModels
{
    public class Door
    {
        public bool IsBossDoor;
        public bool IsSpecialDoor;
        public Direction Orientation;
        public Vector3 Position;
        public Vector3 Position2;

        [JsonIgnore]
        public DoorBehaviour DoorBehaviour { get; set; }

        public Door()
        {
            IsBossDoor = false;
            IsSpecialDoor = false;
            Orientation = Direction.Up;
            Position = new Vector3(0.0f, 0.0f, 0.0f);
            Position2 = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}