using System;
using System.Collections.Generic;

public class UnitDirectionHelper
{
    // To be added to x and y values. Starts at UnitDirection.North going clockwise.
    private static readonly (int, int)[] directionModifiers = new (int, int)[] { (-1, 0), (0, 1), (1, 0), (1, -1), (0, -1), (-1, -1) };

    public static List<(int, int)> directionToIndiciesModifiers(UnitDirection direction)
    {
        return direction switch
        {
            UnitDirection.North => new List<(int, int)> { directionModifiers[0], directionModifiers [1], directionModifiers[2]},
            UnitDirection.NorthEast => new List<(int, int)> { directionModifiers[1], directionModifiers[2], directionModifiers[3] },
            UnitDirection.SouthEast => new List<(int, int)> { directionModifiers[2], directionModifiers[3], directionModifiers[4] },
            UnitDirection.South => new List<(int, int)> { directionModifiers[3], directionModifiers[4], directionModifiers[5] },
            UnitDirection.SouthWest => new List<(int, int)> { directionModifiers[4], directionModifiers[5], directionModifiers[0] },
            UnitDirection.NorthWest => new List<(int, int)> { directionModifiers[5], directionModifiers[0], directionModifiers[1] },
            _ => throw new System.Exception("Unhandled UnitDirection")
        };
    }

    public static float directionToRotation(UnitDirection direction)
    {
        return direction switch
        {
            UnitDirection.North => 0f,
            UnitDirection.NorthEast => -60f,
            UnitDirection.SouthEast => -120f,
            UnitDirection.South => -180f,
            UnitDirection.SouthWest => -240f,
            UnitDirection.NorthWest => -300f,
            _ => throw new System.Exception("Unhandled UnitDirection")
        };
    }

    public static UnitDirection rotationToDirection(float rotation)
    {
        if (rotation < 0f)
        {
            rotation = -rotation;
        }
        rotation %= 360;
        if (rotation < 60)
        {
            return UnitDirection.North;
        }
        if (rotation < 120)
        {
            return UnitDirection.NorthWest;
        }
        if (rotation < 180)
        {
            return UnitDirection.SouthWest;
        }
        if (rotation < 240)
        {
            return UnitDirection.South;
        }
        if (rotation < 300)
        {
            return UnitDirection.SouthEast;
        }
        return UnitDirection.NorthEast;
    }

}
