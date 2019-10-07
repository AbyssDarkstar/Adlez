using Assets.Scripts.DataModels;
using Assets.Scripts.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    private int mapTileWidth;
    private int mapTileHeight;

    private int[,] terrainMap;
    private int[,] navMap;

    [HideInInspector]
    public Map map;

    public GameObject DoorPrefab;
    public GameObject ChestPrefab;
    public GameObject KeyPrefab;
    public GameObject SwordPrefab;
    public GameObject BushPrefab;
    public GameObject ButtonPrefab;
    public GameObject GemPrefab;
    public GameObject MessageDisplay;
    public PlayerController PlayerController;

    private void Awake()
    {
        try
        {
            map = JsonConvert.DeserializeObject<Map>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Maps", "Dungeon1.txt")));
        }
        catch (Exception ex)
        {
            MessageDisplay.GetComponent<TextMeshProUGUI>().text = ex.Message;
            MessageDisplay.SetActive(true);
        }

        mapTileWidth = map.Width * map.RoomScale;
        mapTileHeight = map.Height * map.RoomScale;

        CreateTerrainAndNavMaps();
        CreateTerrainMesh();
    }

    private void CreateTerrainMesh()
    {
        Mesh mesh = new Mesh();

        var vertices = new Vector3[4 * (mapTileWidth * mapTileHeight)];
        var uv = new Vector2[4 * (mapTileWidth * mapTileHeight)];
        var triangles = new int[6 * (mapTileWidth * mapTileHeight)];

        int tileSize = 1;
        var atlasSize = new Vector2(256, 256);

        for (int i = 0; i < mapTileWidth; i++)
        {
            for (int j = 0; j < mapTileHeight; j++)
            {
                int index = i * mapTileHeight + j;

                vertices[index * 4 + 0] = new Vector3(tileSize * i, tileSize * j);
                vertices[index * 4 + 1] = new Vector3(tileSize * i, tileSize * (j + 1));
                vertices[index * 4 + 2] = new Vector3(tileSize * (i + 1), tileSize * (j + 1));
                vertices[index * 4 + 3] = new Vector3(tileSize * (i + 1), tileSize * j);

                var tile = GetTexturePosition(terrainMap[i, j], atlasSize, new Vector2(32, 32));
                uv[index * 4 + 0] = new Vector2(tile.xMin / atlasSize.x, tile.yMin / atlasSize.y);
                uv[index * 4 + 1] = new Vector2(tile.xMin / atlasSize.x, tile.yMax / atlasSize.y);
                uv[index * 4 + 2] = new Vector2(tile.xMax / atlasSize.x, tile.yMax / atlasSize.y);
                uv[index * 4 + 3] = new Vector2(tile.xMax / atlasSize.x, tile.yMin / atlasSize.y);

                triangles[index * 6 + 0] = index * 4 + 0;
                triangles[index * 6 + 1] = index * 4 + 1;
                triangles[index * 6 + 2] = index * 4 + 2;

                triangles[index * 6 + 3] = index * 4 + 0;
                triangles[index * 6 + 4] = index * 4 + 2;
                triangles[index * 6 + 5] = index * 4 + 3;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateTerrainAndNavMaps()
    {
        terrainMap = new int[mapTileWidth, mapTileHeight];
        navMap = new int[mapTileWidth, mapTileHeight];

        for (int i = 0; i < mapTileWidth; i++)
        {
            for (int j = 0; j < mapTileHeight; j++)
            {
                terrainMap[i, j] = 0;
                navMap[i, j] = 0;
            }
        }

        foreach (var room in map.Rooms)
        {
            AddRoom(room);
        }
    }

    private void AddRoom(Room room)
    {
        var x = room.Position.x - 1;
        var y = room.Position.y - 1;
        for (int i = 0; i < map.RoomScale; i++)
        {
            for (int j = 0; j < map.RoomScale; j++)
            {
                terrainMap[RoomToWorldCoordinate(i, x, map.RoomScale), RoomToWorldCoordinate(j, y, map.RoomScale)] = 1;
            }
        }

        foreach (var wall in room.Walls)
        {
            terrainMap[RoomToWorldCoordinate(wall.x, x, map.RoomScale), RoomToWorldCoordinate(wall.y, y, map.RoomScale)] = (int)wall.z;
            navMap[RoomToWorldCoordinate(wall.x, x, map.RoomScale), RoomToWorldCoordinate(wall.y, y, map.RoomScale)] = 1;
        }

        foreach (var floor in room.Floors)
        {
            terrainMap[RoomToWorldCoordinate(floor.x, x, map.RoomScale), RoomToWorldCoordinate(floor.y, y, map.RoomScale)] = (int)floor.z;
        }

        foreach (var door in room.Doors)
        {
            var d = Instantiate(DoorPrefab);
            var db = d.GetComponent<DoorBehaviour>();
            db.Door = door;
            d.transform.position = new Vector3(RoomToWorldCoordinate(db.Position.x, x, map.RoomScale), RoomToWorldCoordinate(db.Position.y, y, map.RoomScale), db.Position.z);
            d.name = $"{room.Name}.Door{db.Orientation}";
            if (db.IsSpecialDoor) { d.name = $"{room.Name}.DoorSpecial"; }
            db.SetOrientation(db.Orientation);

            navMap[RoomToWorldCoordinate(db.Position.x, x, map.RoomScale) - 1, RoomToWorldCoordinate(db.Position.y, y, map.RoomScale) - 1] = 2;
            navMap[RoomToWorldCoordinate(db.Position2.x, x, map.RoomScale) - 1, RoomToWorldCoordinate(db.Position2.y, y, map.RoomScale) - 1] = 2;
        }

        foreach (var chest in room.Chests)
        {
            var c = Instantiate(ChestPrefab);
            var cb = c.GetComponent<ChestBehaviour>();
            cb.Chest = chest;
            c.transform.position = new Vector3(RoomToWorldCoordinate(cb.Position.x, x, map.RoomScale), RoomToWorldCoordinate(cb.Position.y, y, map.RoomScale), cb.Position.z);
            c.name = $"{room.Name}.{chest.Name}";

            navMap[RoomToWorldCoordinate(cb.Position.x, x, map.RoomScale) - 1, RoomToWorldCoordinate(cb.Position.y, y, map.RoomScale) - 1] = 3;
        }

        foreach (var stairs in room.Stairs)
        {
            terrainMap[RoomToWorldCoordinate(stairs.Position.x, x, map.RoomScale) - 1, RoomToWorldCoordinate(stairs.Position.y, y, map.RoomScale) - 1] = stairs.TextureId;
            navMap[RoomToWorldCoordinate(stairs.Position.x, x, map.RoomScale) - 1, RoomToWorldCoordinate(stairs.Position.y, y, map.RoomScale) - 1] = 4;
        }

        int bushCount = 0;
        foreach (var bush in room.Bushes)
        {
            var b = Instantiate(BushPrefab);
            var bb = b.GetComponent<BushBehaviour>();
            bb.Bush = bush;
            b.transform.position = new Vector3(RoomToWorldCoordinate(bb.Position.x, x, map.RoomScale), RoomToWorldCoordinate(bb.Position.y, y, map.RoomScale), bb.Position.z);
            b.name = $"{room.Name}.Bush{++bushCount}";

            navMap[RoomToWorldCoordinate(bb.Position.x, x, map.RoomScale) - 1, RoomToWorldCoordinate(bb.Position.y, y, map.RoomScale) - 1] = 5;
        }

        foreach (var button in room.Buttons)
        {
            var b = Instantiate(ButtonPrefab);
            var bb = b.GetComponent<ButtonBehaviour>();
            bb.Button = button;
            b.transform.position = new Vector3(RoomToWorldCoordinate(bb.Position.x, x, map.RoomScale), RoomToWorldCoordinate(bb.Position.y, y, map.RoomScale), bb.Position.z);
            b.name = $"{room.Name}.Button{button.Name}";
        }

        if(room.Name.Equals("DungeonBoss", StringComparison.OrdinalIgnoreCase))
        {
            var c = Instantiate(GemPrefab);
            var cb = c.GetComponent<ButtonBehaviour>();
            c.transform.position = new Vector3(RoomToWorldCoordinate(5, x, map.RoomScale), RoomToWorldCoordinate(5, y, map.RoomScale), 2);
            c.name = $"{room.Name}.VictoryGem";
        }
    }

    private int RoomToWorldCoordinate(float coordinate, float roomPosition, int scale)
    {
        return RoomToWorldCoordinate(coordinate, (int)roomPosition, scale);
    }

    private int RoomToWorldCoordinate(float coordinate, int roomPosition, int scale)
    {
        return (int)coordinate + (roomPosition * map.RoomScale);
    }

    private Vector3 WorldToRoomCoordinates(Vector3 coordinates, int scale, int z = 0)
    {
        var v = new Vector3(coordinates.x % scale, coordinates.y % scale, z);

        if (v.x == 0) { v.x += 10; }
        if (v.y == 0) { v.y += 10; }

        return v;
    }

    private Rect GetTexturePosition(int textureId, Vector2 atlasSize, Vector2 textureSize)
    {
        var columnCount = atlasSize.x / textureSize.x;
        var rowCount = atlasSize.y / textureSize.y;
        var textureCount = columnCount * rowCount;

        if (textureId > textureCount - 1)
        {
            throw new ArgumentOutOfRangeException("Texture Id Invalid.");
        }

        Vector2 pos = new Vector2(textureId % columnCount, (float)Math.Floor(textureId / rowCount));

        return new Rect(textureSize.x * pos.x, atlasSize.y - (textureSize.y * (pos.y + 1)), textureSize.x, textureSize.y);
    }

    public Room GetRoomAtPosition(Vector3 position)
    {
        var x = Math.Floor((position.x - 1) / map.RoomScale) + 1;
        var y = Math.Floor((position.y - 1) / map.RoomScale) + 1;

        return map.Rooms.FirstOrDefault(r => r.Position.x == x && r.Position.y == y);
    }

    public Room GetRoomByName(string name)
    {
        return map.Rooms.FirstOrDefault(r => r.Name.Equals(name));
    }

    public IInteractable GetInteractableAtPosition(Vector3 position)
    {
        var room = GetRoomAtPosition(position);
        var roomPos = WorldToRoomCoordinates(position, map.RoomScale, 2);

        if (room != null)
        {
            var door = room.Doors.FirstOrDefault(d => d.Position == roomPos || d.Position2 == roomPos);

            if (door != null) { return door.DoorBehaviour; }

            var chest = room.Chests.FirstOrDefault(c => c.Position == roomPos);

            if (chest != null) { return chest.ChestBehaviour; }
        }

        return null;
    }

    public Stairs GetStairsAtPosition(Vector3 position)
    {
        var room = GetRoomAtPosition(position);
        var roomPos = WorldToRoomCoordinates(position, map.RoomScale);

        if (room != null)
        {
            var stairs = room.Stairs.FirstOrDefault(s => s.Position == roomPos);
            return stairs;
        }

        // Should never happen
        return null;
    }

    public BushBehaviour GetBushAtPosition(Vector3 position)
    {
        var room = GetRoomAtPosition(position);
        var roomPos = WorldToRoomCoordinates(position, map.RoomScale, 2);

        if (room != null)
        {
            var bush = room.Bushes.FirstOrDefault(s => s.Position == roomPos);
            return bush.BushBehaviour;
        }

        // Should never happen
        return null;
    }

    public bool CanEnterTile(float x, float y, bool attack = false)
    {
        var x1 = (int)x - 1;
        var y1 = (int)y - 1;

        if (x1 < 0 || y1 < 0 || x1 >= mapTileWidth || y1 >= mapTileHeight)
        {
            MessageDisplay.SetActive(true);
            Invoke("HideMessage", 1f);
            return false;
        }

        // 0 = Walkable
        // 1 = Wall
        // 2 = Door
        // 3 = Chest
        // 4 = Stairs
        // 5 = Bush

        if (navMap[x1, y1] == 0)
        {
            return true;
        }

        if (navMap[x1, y1] == 4)
        {
            var sourceStairs = GetStairsAtPosition(new Vector3(x, y, 0));
            var destinationStairs = GetDestinationStairs(sourceStairs.Destination, out Room room);

            var sx = RoomToWorldCoordinate(destinationStairs.Position.x, room.Position.x - 1, map.RoomScale);
            var sy = RoomToWorldCoordinate(destinationStairs.Position.y, room.Position.y - 1, map.RoomScale);

            var targetPosition = new Vector3(sx, sy, 0);

            PlayerController.teleportPending = targetPosition;
            return true;
        }

        if (navMap[x1, y1] == 5 && attack)
        {
            var bush = GetBushAtPosition(new Vector3(x, y, 0));
            bush.Break();
            navMap[x1, y1] = 0;
            return true;
        }

        if (navMap[x1, y1] == 2)
        {
            var door = GetInteractableAtPosition(new Vector3(x, y, 0)) as DoorBehaviour;

            if (door != null)
            {
                if (door.CanInteract(PlayerController))
                {
                    door.Interact(PlayerController);
                    navMap[x1, y1] = 0;
                    if (navMap[Math.Max(x1 - 1, 0), y1] == 2)
                    {
                        navMap[Math.Max(x1 - 1, 0), y1] = 0;
                    }
                    else if (navMap[Math.Min(x1 + 1, mapTileWidth - 1), y1] == 2)
                    {
                        navMap[Math.Min(x1 + 1, mapTileWidth - 1), y1] = 0;
                    }
                    else if (navMap[x1, Math.Max(y1 - 1, 0)] == 2)
                    {
                        navMap[x1, Math.Max(y1 - 1, 0)] = 0;
                    }
                    else if (navMap[x1, Math.Min(y1 + 1, mapTileHeight - 1)] == 2)
                    {
                        navMap[x1, Math.Min(y1 + 1, mapTileHeight - 1)] = 0;
                    }
                }
            }
        }

        if (navMap[x1, y1] == 3)
        {
            var chest = GetInteractableAtPosition(new Vector3(x, y, 0)) as ChestBehaviour;

            if (chest != null && !chest.IsOpen)
            {
                if (PlayerController.transform.position.y == y - 1)
                {
                    chest.Interact(PlayerController);
                }
            }

            return false;
        }

        return false;
    }

    public Stairs GetDestinationStairs(string name, out Room targetRoom)
    {
        foreach (var room in map.Rooms)
        {
            var stairs = room.Stairs.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (stairs != null)
            {
                targetRoom = room;
                return stairs;
            }
        }

        // This should never happen
        targetRoom = null;
        return null;
    }

    public KeyBehaviour SpawnKey(Vector3 position, bool bossKey, bool falling)
    {
        var k = Instantiate(KeyPrefab);
        var kb = k.GetComponent<KeyBehaviour>();
        kb.IsBossKey = bossKey;
        k.transform.position = position;

        return kb;
    }

    public SwordBehaviour SpawnSword(Vector3 position)
    {
        var s = Instantiate(SwordPrefab);
        var sb = s.GetComponent<SwordBehaviour>();
        s.transform.position = position;

        return sb;
    }

    public void TriggerButton(string metaData)
    {
        var split = metaData.Split(',');

        switch (split[0])
        {
            case "Destroy:Door":
                var split2 = split[1].Split(':');
                switch (split2[0])
                {
                    case "Name":
                        var go = GameObject.Find(split2[1]);
                        var t = go.transform.position;
                        navMap[(int)t.x - 1, (int)t.y - 1] = 0;
                        Destroy(go);

                        break;
                }
                                
                break;
            default:
                break;
        }

    }

    public void HideMessage()
    {
        MessageDisplay.SetActive(false);
    }
}