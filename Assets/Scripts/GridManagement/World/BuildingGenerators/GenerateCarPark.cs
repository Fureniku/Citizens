using System;
using Loading.States;
using Newtonsoft.Json.Linq;
using Tiles.TileManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateCarPark : GenerateBuildingBase {
    
    protected override int SelectGameObject(int w, int l, ref EnumDirection rot) {
        //Most generator classes are relatively similar so this is copy-pasted with slight changes
        //Annoyingly things generate differently enough that most buildings need this custom written.
        
        //Check for entrance, at the front of the building
        if (l == 0) {
            //Arrays are zero-based, so minus 1 where appropriate.
            if (width % 2 == 0) { //Even-width building
                if (w == width / 2 || w == width / 2 - 1) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.MULTI_CAR_PARK_ENTRANCE.GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)Math.Floor(width / 2.0)) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.MULTI_CAR_PARK_ENTRANCE.GetId();
                }
            }
        }
        //Check for ramps, every level one row behind where the entrance is
        if (l == 1) {
            if (width % 2 == 0) { //Even-width building
                if (w == width / 2 || w == width / 2 - 1) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.MULTI_CAR_PARK_RAMP.GetId();
                }
            }
            else { //Odd-width building
                if (w == (int)Math.Floor(width / 2.0)) {
                    rot = EnumDirection.WEST;
                    return TileRegistry.MULTI_CAR_PARK_RAMP.GetId();
                }
            }
        }

        if (w == 0) {
            if (l == 0) {
                rot = EnumDirection.WEST;
                return TileRegistry.MULTI_CAR_PARK_CORNER.GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.NORTH;
                return TileRegistry.MULTI_CAR_PARK_CORNER.GetId();
            }

            rot = EnumDirection.NORTH;
            return TileRegistry.MULTI_CAR_PARK_EDGE.GetId();
        }
        if (w == width-1) {
            if (l == 0) {
                rot = EnumDirection.SOUTH;
                return TileRegistry.MULTI_CAR_PARK_CORNER.GetId();
            }
            if (l == length-1) {
                rot = EnumDirection.EAST;
                return TileRegistry.MULTI_CAR_PARK_CORNER.GetId();
            }
            rot = EnumDirection.SOUTH;
            return TileRegistry.MULTI_CAR_PARK_EDGE.GetId();
        }

        if (l == 0) {
            rot = EnumDirection.WEST;
            return TileRegistry.MULTI_CAR_PARK_EDGE.GetId();
        }

        if (l == length - 1) {
            rot = EnumDirection.EAST;
            return TileRegistry.MULTI_CAR_PARK_EDGE.GetId();
        }
        
        return TileRegistry.MULTI_CAR_PARK_INNER.GetId();
    }
    
    public override void PostGenerate() {}

    public GenerateCarPark(TilePos startPos, int width, int minHeight, int maxHeight, int length) 
        : base(startPos, width, minHeight, maxHeight, length) {}
    
    public GenerateCarPark(TilePos startPos, int minWidth, int maxWidth, int minHeight, int maxHeight, int minLength, int maxLength) 
        : base(startPos, minWidth, maxWidth, minHeight, maxHeight, minLength, maxLength) {}
}