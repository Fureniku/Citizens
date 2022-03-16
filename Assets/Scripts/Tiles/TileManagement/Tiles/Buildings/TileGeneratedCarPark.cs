using System;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileGeneratedCarPark : MonoBehaviour {
    
    [SerializeField] private EnumTile entrance;
    [SerializeField] private EnumTile edge;
    [SerializeField] private EnumTile corner;
    [SerializeField] private EnumTile inner;
    [SerializeField] private EnumTile ramp;

    private bool generated = false;
    [SerializeField] protected int minWidth = 4;
    [SerializeField] protected int maxWidth = 10;
    
    [SerializeField] protected int minLength = 3;
    [SerializeField] protected int maxLength = 10;

    [SerializeField] private int minHeight = 2;
    [SerializeField] private int maxnHeight = 20;
    
    [SerializeField, ReadOnly] protected int width = 1;
    [SerializeField, ReadOnly] protected int length = 1;
    [SerializeField, ReadOnly] protected int height = 1;

    void Start() {
        width = Random.Range(minWidth, maxWidth);
        length = Random.Range(minLength, maxLength);
        height = Random.Range(minHeight, maxnHeight);
    }

    public void SetWidth(int w) => width = w;
    public void SetLength(int l) => length = l;
    public void SetHeight(int h) => height = h;
    
    void Update() {
        if (World.Instance.GetWorldState() == EnumWorldState.COMPLETE && !generated) {
            Debug.Log("Generating");
            Generate();
        }
    }

    private void Generate() {
        for (int l = 0; l < length; l++) {
            for (int w = 0; w < width; w++) {
                TilePos worldPos = TilePos.GetGridPosFromLocation(transform.position);
                TilePos placePos = new TilePos(worldPos.x + l, worldPos.z + w);
                EnumTileDirection rot = EnumTileDirection.NORTH;
                int id = SelectGameObject(l, w, ref rot);
                World.Instance.GetGridManager().SetTile(placePos, id, rot);
                World.Instance.GetGridManager().GetTile(placePos).GetComponent<TileBuildingSegment>().MakeReady(height);
            }
        }

        generated = true;
    }

    private int SelectGameObject(int l, int w, ref EnumTileDirection rot) {
        //ok this is gonna be fun
        //Check for entrance, at the front of the building
        if (l == 0) {
            //Arrays are zero-based, so minus 1 where appropriate.
            if (width % 2 == 0) { //Even-width building
                if (w == width / 2 || w == width / 2 - 1) {
                    return TileRegistry.GetTile(entrance).GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)Math.Floor(width / 2.0)) {
                    return TileRegistry.GetTile(entrance).GetId();
                }
            }
        }
        //Check for ramps, every level one row behind where the entrance is
        if (l == 1) {
            if (width % 2 == 0) { //Even-width building
                if (w == width / 2 || w == width / 2 - 1) {
                    return TileRegistry.GetTile(ramp).GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)Math.Floor(width / 2.0)) {
                    return TileRegistry.GetTile(ramp).GetId();
                }
            }
        }

        if (w == 0) {
            if (l == 0) {
                rot = EnumTileDirection.WEST;
                return TileRegistry.GetTile(corner).GetId();
            }
            if (l == length-1) {
                rot = EnumTileDirection.SOUTH;
                return TileRegistry.GetTile(corner).GetId();
            }

            rot = EnumTileDirection.WEST;
            return TileRegistry.GetTile(edge).GetId();
        }
        if (w == width-1) {
            if (l == 0) {
                rot = EnumTileDirection.NORTH;
                return TileRegistry.GetTile(corner).GetId();
            }
            if (l == length-1) {
                rot = EnumTileDirection.EAST;
                return TileRegistry.GetTile(corner).GetId();
            }
            rot = EnumTileDirection.EAST;
            return TileRegistry.GetTile(edge).GetId();
        }

        if (l == 0) {
            return TileRegistry.GetTile(edge).GetId();
        }

        if (l == length - 1) {
            rot = EnumTileDirection.SOUTH;
            return TileRegistry.GetTile(edge).GetId();
        }
        
        return TileRegistry.GetTile(inner).GetId();
    }
}