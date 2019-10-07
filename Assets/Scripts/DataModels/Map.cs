using System.Collections.Generic;

namespace Assets.Scripts.DataModels
{
    public class Map
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public int RoomScale { get; set; }

        public List<Room> Rooms { get; set; }

        public Map()
        {
            Rooms = new List<Room>();
        }

        public Map(int width, int height, int roomScale)
        {
            Width = width;
            Height = height;
            RoomScale = roomScale;
            Rooms = new List<Room>();
        }
    }
}
