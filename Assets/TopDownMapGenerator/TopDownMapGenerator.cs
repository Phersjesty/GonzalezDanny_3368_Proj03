using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TopDownMapGenerator : MonoBehaviour
{
    [SerializeField] private TileBase WallTile = null;
    [SerializeField] private TileBase FloorTile = null;

    private Tilemap _map;
    private GameObject _wallMap;
    private int[,] _shadowMap;

    [Header("Room Info")]
    [Range(1, 20)]
    [SerializeField] private int RoomMaxHeight = 8;
    [Range(1, 20)]
    [SerializeField] private int RoomMinHeight = 4;
    [Range(1, 20)]
    [SerializeField] private int RoomMaxWidth = 8;
    [Range(1, 20)]
    [SerializeField] private int RoomMinWidth = 4;
    [Space(5)]
    [SerializeField] private int NumberOfRooms = 10;

    private int _roomMaxHeight;
    private int _roomMaxWidth;

    private void Awake()
    {
        //WallTile = ScriptableObject.CreateInstance<Tile>();

        //FloorTile = ScriptableObject.CreateInstance<Tile>();

        _roomMaxHeight = RoomMaxHeight + 1;
        _roomMaxWidth = RoomMaxWidth + 1;

        _wallMap = new GameObject();
        _wallMap.name = "Wall Tilemap";
        _wallMap.transform.SetParent(transform.parent);
        _wallMap.AddComponent<Tilemap>();
        _wallMap.AddComponent<TilemapRenderer>();
        _wallMap.AddComponent<TilemapCollider2D>();

        _map = GetComponent<Tilemap>();
        _shadowMap = new int[NumberOfRooms, NumberOfRooms];
        _shadowMap[NumberOfRooms / 2, NumberOfRooms / 2] = 1;

        GenerateRoom(NumberOfRooms / 2, NumberOfRooms / 2, true);

        TraverseMap(NumberOfRooms / 2, NumberOfRooms / 2, NumberOfRooms);
      
        CreateHallways(NumberOfRooms / 2, NumberOfRooms / 2);
    }

    void GenerateRoom(int r, int c, bool maxSize = false)
    {
        int height = _roomMaxHeight - 1;
        int width = _roomMaxWidth - 1;

        if (maxSize == false)
        {
            height = Random.Range(RoomMinHeight, _roomMaxHeight);
            width = Random.Range(RoomMinWidth, _roomMaxWidth);
        }

        int startX = (r - (NumberOfRooms / 2)) * _roomMaxWidth - (_roomMaxWidth / 2);
        int startY = (c - (NumberOfRooms / 2)) * _roomMaxHeight - (_roomMaxHeight / 2);

        for(int i = 1; i < width-1; i++)
        {
            for(int j = 1; j < height-1; j++)
            {
                _map.SetTile(new Vector3Int(startX + i, startY + j), FloorTile);
                
            }
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                {
                    //_wallMap.GetComponent<Tilemap>().SetTile(new Vector3Int(startX + i, startY + j), WallTile);
                    //_map.SetColliderType(new Vector3Int(startX + i, startY + j), Tile.ColliderType.Sprite);
                }
            }
        }

    }

    void TraverseMap(int r, int c, int roomsLeft)
    {
        if(roomsLeft <= 0)
        {
            return;
        }

        if(r >= NumberOfRooms || r < 0 || c >= NumberOfRooms || c < 0)
        {
            if (r < 0) TraverseMap(r + 1, c, roomsLeft);
            if (r > _shadowMap.Length) TraverseMap(r - 1, c, roomsLeft);
            if (c < 0) TraverseMap(r, c + 1, roomsLeft);
            if (c > _shadowMap.Length) TraverseMap(r, c - 1, roomsLeft);
            
            return;
        }

        if(_shadowMap[r, c] == 0)
        {
            _shadowMap[r, c] = 1;
            GenerateRoom(r, c);
            roomsLeft -= 1;
        }

        //1 = right, 2 = up, 3 = left, 4 = down
        int random = Random.Range(1, 5);
        switch (random)
        {
            case 1:
                TraverseMap(r + 1, c, roomsLeft);
                break;
            case 2:
                TraverseMap(r, c + 1, roomsLeft);
                break;
            case 3:
                TraverseMap(r - 1, c, roomsLeft);
                break;
            case 4:
                TraverseMap(r, c - 1, roomsLeft);
                break;

        }

        return;
    }

    bool CreateHallways(int r, int c)
    {
        if(r >= NumberOfRooms || r < 0 || c >= NumberOfRooms || c < 0 || _shadowMap[r, c] == 0)
        {
            return (false);
        }

        _shadowMap[r, c]--;

        //DrawHallway(r, c, r, c);

      
        
        if (CreateHallways(r + 1, c) == true)
        {
            DrawHallway(r, c, r + 1, c);
        }
        if (CreateHallways(r - 1, c) == true)
        {
            DrawHallway(r - 1, c, r, c);
        }
        if (CreateHallways(r, c + 1) == true)
        {
            DrawHallway(r, c, r, c + 1);
        }
        if (CreateHallways(r, c - 1) == true)
        {
            DrawHallway(r, c - 1, r, c);
        }
        
        return (true);
    }

    void DrawHallway(int startR, int startC, int finalR, int finalC)
    {
        int startX = (startR - (NumberOfRooms / 2)) * _roomMaxWidth - (_roomMaxWidth / 2) + 1;
        int startY = (startC - (NumberOfRooms / 2)) * _roomMaxHeight - (_roomMaxHeight / 2) + 1;
        //_wallMap.GetComponent<Tilemap>().
        if (startC == finalC)
        {
            for (int i = 1; i < _roomMaxWidth; i++)
            {
                _map.SetTile(new Vector3Int(startX + i, startY), FloorTile);
            }
        }
        if (startR == finalR)
        {
            for (int i = 1; i < _roomMaxHeight; i++)
            {
                _map.SetTile(new Vector3Int(startX, startY + i), FloorTile);
            }
        }
    }
}
