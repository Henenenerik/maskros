using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TeamSettings
{
    public static Color GetTeamColor(int team)
    {
        switch (team)
        {
            case 0:
                return Color.red;
            case 1:
                return Color.blue;
            default:
                return Color.white;
        }
    }

}
