using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Map
{

    private Tile[,] map;
    private Dictionary<(int, int), Unit> positionToUnit = new Dictionary<(int, int), Unit>(); // TODO PERFORMANCE: Could probably be converted to array to increase performance
    private Dictionary<Unit, (int, int)> unitToPosition = new Dictionary<Unit, (int, int)>();
    private int map_x;
    private int map_y;
    private Dictionary<(int, int), List<Unit>> positionToUnitZoneOfControl = new Dictionary<(int, int), List<Unit>>();
    private Dictionary<(int, int), Unit> tilesOccupiedNextTurn = new Dictionary<(int, int), Unit>();
    private Dictionary<Unit, (int, int)> unitToPositionNextTurn = new Dictionary<Unit, (int, int)>();

    private bool zoneOfControlToggled = false;

    public enum OccupiedStatus { Friend, Enemy, Empty };

    struct PFNode : IEquatable<PFNode>
    {
        public int x;
        public int y;
        public int prev_x;
        public int prev_y;
        public float cost;
        public float heuristic;

        public PFNode(int x, int y, int prev_x, int prev_y, float cost, float heuristic)
        {
            this.x = x;
            this.y = y;
            this.prev_x = prev_x;
            this.prev_y = prev_y;
            this.cost = cost;
            this.heuristic = heuristic;
        }

        public bool Equals(PFNode other)
        {
            return this.x == other.x && this.y == other.y;
        }
    }

    public Map(Tile[,] map, int width, int height)
    {
        this.map = map;
        map_x = width;
        map_y = height;
    }

    public void ClearMap()
    {
        for (int i = 0; i < map_x; i++)
        {
            for (int j = 0; j < map_y; j++)
            {
                if (map[i, j] != null)
                {
                    map[i, j].Deconstruct();
                }
            }
        }
    }

    public (int, int) GetMapSize()
    {
        return (map_x, map_y);
    }

    public Vector2 GetCornerPosition()
    {
        Vector3 pos = map[0, 0].transform.position;
        return new Vector2(pos.x, pos.y);
    }

    public Vector3 IndiciesToPosition(int x, int y)
    {
        return map[x, y].transform.position;
    }

    public void Neighbours(ref List<Tile> res, int x, int y)
    {
        res.Clear();
        if (y - 1 >= 0) { res.Add(map[x, y - 1]); }
        if (y + 1 < map_y) { res.Add(map[x, y + 1]); }
        if (x - 1 >= 0) { res.Add(map[x - 1, y]); }
        if (x + 1 < map_x) { res.Add(map[x + 1, y]); }

        if (x % 2 == 0)
        {
            if (x - 1 >= 0 && y - 1 >= 0) { res.Add(map[x - 1, y - 1]); }
            if (x + 1 < map_x && y - 1 >= 0) { res.Add(map[x + 1, y - 1]); }
        }
        else
        {
            if (x - 1 >= 0 && y + 1 < map_y) { res.Add(map[x - 1, y + 1]); }
            if (x + 1 < map_x && y + 1 < map_y) { res.Add(map[x + 1, y + 1]); }
        }
    }

    public void ApplyToTiles(Action<Tile> f)
    {
        foreach (Tile t in map)
        {
            f(t);
        }
    }

    public List<Tile> FindPath(Unit unit, Tile target)
    {
        (int target_x, int target_y) = target.GetIndicies();
        return FindPath(unit, target_x, target_y);
    }

    /// <summary>
    /// Path finding using A* and euclidean distance heuristic.
    /// </summary>
    /// <param name="unit"> Unit looking for path. </param>
    /// <param name="target_x"> Target x position </param>
    /// <param name="target_y"> Target y position </param>
    /// <returns> List of Tile objects which constitutes the shortest path to the target. </returns>
    public List<Tile> FindPath(Unit unit, int target_x, int target_y)
    {
        PriorityQueue<PFNode> frontier = new PriorityQueue<PFNode>();

        (int origin_x, int origin_y) = unit.GetGridPosition();

        PFNode start = new PFNode(origin_x, origin_y, -1, -1, 0, Heuristic(origin_x, origin_y));

        frontier.Enqueue(start.cost + start.heuristic, start);
        List<Tile> result = new List<Tile>();
        List<Tile> neighbours = new List<Tile>();
        Dictionary<(int, int), PFNode> explored = new Dictionary<(int, int), PFNode>
        {
            { (start.x, start.y), start }
        };

        void ReconstructPath(PFNode start)
        {
            result.Add(map[start.x, start.y]);

            while (start.prev_x >= 0 || start.prev_y >= 0)
            {
                if (explored.TryGetValue((start.prev_x, start.prev_y), out PFNode next))
                {
                    result.Add(map[next.x, next.y]);
                    start = next;
                }
                else
                {
                    throw new Exception("Broken path.");
                }

            }

            result.Reverse();
        }

        float Heuristic(int x, int y)
        {
            return Vector2.Distance(new Vector2(target_x, target_y), new Vector2(x, y));
        }

        while (frontier.Count > 0)
        {
            PFNode explore = frontier.Dequeue();

            bool startingTile = start.x == explore.x && start.y == explore.y;
            bool inZoneOfControl = positionToUnitZoneOfControl.ContainsKey((explore.x, explore.y));
            bool occupiedInNextTurn = tilesOccupiedNextTurn.TryGetValue((explore.x, explore.y), out Unit occupiedBy) && occupiedBy != unit;

            if (occupiedInNextTurn)
            {
                continue;
            }
            if (explore.x == target_x && explore.y == target_y)
            {
                ReconstructPath(explore);
                break;
            }
            else if (startingTile || (!inZoneOfControl && !occupiedInNextTurn))
            {
                Neighbours(ref neighbours, explore.x, explore.y);
                foreach (Tile t in neighbours)
                {
                    (int x, int y) = t.GetIndicies();
                    float newDist = explore.cost + t.GetMovementCost();

                    PFNode neighbour = new PFNode(x, y, explore.x, explore.y, newDist, Heuristic(x, y));
                    if (explored.TryGetValue((neighbour.x, neighbour.y), out PFNode existing))
                    {
                        if (existing.cost + existing.heuristic > neighbour.cost + neighbour.heuristic)
                        {
                            explored.Remove((existing.x, existing.y));
                            explored.Add((neighbour.x, neighbour.y), neighbour);
                            if (frontier.IsInQueue(neighbour))
                            {
                                frontier.UpdatePriority(neighbour.cost + neighbour.heuristic, existing);
                            }
                            else
                            {
                                frontier.Enqueue(neighbour.cost + neighbour.heuristic, neighbour);
                            }
                        }
                    }
                    else
                    {
                        explored.Add((neighbour.x, neighbour.y), neighbour);
                        if (frontier.IsInQueue(neighbour))
                        {
                            frontier.UpdatePriority(neighbour.cost, existing);
                        }
                        else
                        {
                            frontier.Enqueue(neighbour.cost, neighbour);
                        }
                    }

                }
            }

        }

        return result;
    }

    public void TrackUnit(Unit unit)
    {
        (int x, int y) = unit.GetGridPosition();
        positionToUnit.Add((x, y), unit);
        unitToPosition.Add(unit, (x, y));
    }

    public void UpdateZoneOfControlMap(int team)
    {
        positionToUnitZoneOfControl.Clear();
        tilesOccupiedNextTurn.Clear();
        unitToPositionNextTurn.Clear();
        foreach (var unit in unitToPosition.Keys)
        {
            if (unit.GetTeam() != team)
            {
                AddToGlobalZoneOfControl(unit);
            }
            tilesOccupiedNextTurn.Add(unit.GetGridPosition(), unit);
            unitToPositionNextTurn.Add(unit, unit.GetGridPosition());
        }
    }

    private void AddToGlobalZoneOfControl(Unit unit)
    {
        foreach (var tilePosition in unit.GetZoneOfControl())
        {
            if (positionToUnitZoneOfControl.ContainsKey(tilePosition))
            {
                positionToUnitZoneOfControl[tilePosition].Add(unit);
            }
            else
            {
                positionToUnitZoneOfControl.Add(tilePosition, new List<Unit> { unit });
            }
        }
    }

    public bool PlanMove(Unit unit, (int, int) tile)
    {
        var oldPlannedPosition = unitToPositionNextTurn[unit];
        unitToPositionNextTurn.Remove(unit);
        unitToPositionNextTurn.Add(unit, tile);

        tilesOccupiedNextTurn.Remove(oldPlannedPosition);
        tilesOccupiedNextTurn.Add(tile, unit);
        return true;
    }

    public void MoveUnit(Unit unit)
    {
        (int x, int y) = unit.GetGridPosition();
        positionToUnit.Add((x, y), unit);
        unitToPosition.Add(unit, (x, y));
    }

    public OccupiedStatus ProbeTile(int x, int y, int team)
    {
        Unit unit;
        if (positionToUnit.TryGetValue((x, y), out unit))
        {
            return unit.GetTeam() == team ? OccupiedStatus.Friend : OccupiedStatus.Enemy;
        }
        else
        {
            return OccupiedStatus.Empty;
        }
    }

    public void ToggleZoneOfControlIndication()
    {
        if (zoneOfControlToggled)
        {
            ApplyToTiles(t => t.UntoggleZoneOfControlIndication());
        }
        else
        {
            if (unitToPosition != null)
            {
                foreach ((int x, int y) in unitToPosition.Keys.SelectMany(key => key.GetZoneOfControl()).Distinct())
                {
                    map[x, y].ToggleZoneOfControlIndication();
                }
            }
        }
        zoneOfControlToggled = !zoneOfControlToggled;
    }
}
