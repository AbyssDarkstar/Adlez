using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class Room
    {
        public Vector2 Position;
        public string Name;
        public List<Vector3> Walls;
        public List<Vector3> Floors;
        public List<Door> Doors;
        public List<Chest> Chests;
        public List<Stairs> Stairs;
        public List<Bush> Bushes;
        public List<Button> Buttons;

        public Room()
        {
            Walls = new List<Vector3>();
            Floors = new List<Vector3>();
            Doors = new List<Door>();
            Chests = new List<Chest>();
            Stairs = new List<Stairs>();
            Bushes = new List<Bush>();
            Buttons = new List<Button>();
        }

        public Room(int x, int y, string name)
        {
            Name = name;
            Position = new Vector2(x, y);
            Walls = new List<Vector3>();
            Floors = new List<Vector3>();
            Doors = new List<Door>();
            Chests = new List<Chest>();
            Stairs = new List<Stairs>();
            Bushes = new List<Bush>();
            Buttons = new List<Button>();
        }
    }
}