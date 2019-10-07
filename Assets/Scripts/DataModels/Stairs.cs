using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class Stairs
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public int TextureId { get; set; }
        public string Destination { get; set; }
    }
}