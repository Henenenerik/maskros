using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public bool MarkedForCombat { get; set; }

    [SerializeField]
    protected SpriteRenderer[] HighlightRenderers;
    protected const int SelectedHighlight = 0;
    protected const int MarkedForCombatHighlight = 1;

    [SerializeField]
    protected GameObject ghostOrder;

    private int team;

    protected float movementPoints;
    private float CombatStrength;
    protected float currentMorale;
    protected float maxMorale;

    protected float initiative;

    private int x;
    private int y;

    public UnitDirection direction = UnitDirection.North;
    private UnitDirection ghostDirection;

    protected float remainingMovementPoints;

    private List<Tile> currentMoveCommand;

    public abstract void Select();

    public abstract void ToggleHighlight();

    public (int, int) GetGridPosition()
    {
        return (x, y);
    }

    public UnitDirection GetDirection => direction;

    public void SetGhostDirection(Vector3 target)
    {
        var hmm = (target - ghostOrder.transform.position);
        var euler = Quaternion.LookRotation(Vector3.forward, hmm.normalized).eulerAngles;
        euler = new Vector3(euler.x, euler.y, Mathf.Round(euler.z / 60) * 60);
        ghostDirection = UnitDirectionHelper.rotationToDirection(euler.z);
        ghostOrder.transform.rotation = Quaternion.Euler(euler);
    }

    public List<(int, int)> GetZoneOfControl()
    {
        var directionModifiers = UnitDirectionHelper.directionToIndiciesModifiers(direction);
        return directionModifiers.Select(modifiers => (x + modifiers.Item1, (modifiers.Item1 != 0 && x % 2 != 0 ? y + 1 : y) + modifiers.Item2)).ToList();
     }

    public void SetGridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetGridPosition(Vector2 pos)
    {
        this.x = (int)pos.x;
        this.y = (int)pos.y;
    }
    
    public void SetTeam(int team, Color teamColor)
    {
        this.team = team;
        gameObject.GetComponent<SpriteRenderer>().color = teamColor;
    }

    public int GetTeam() { return this.team; }

    public float GetInitiative() { return this.initiative * currentMorale / maxMorale * remainingMovementPoints / movementPoints ; }

    public void MoveCommand(Map map, Tile target)
    {
        List<Tile> path = map.FindPath(this, target);
        currentMoveCommand = path;
        map.PlanMove(this, path.Count > 0 ? path.Last().GetIndicies() : GetGridPosition());
        UpdateUIPath();
    }

    private void UpdateUIPath()
    {
        if (currentMoveCommand == null) { return; }
        if (currentMoveCommand.Count > 0)
        {
            Vector3[] verticies = new Vector3[currentMoveCommand.Count];
            int i = verticies.Length - 1;
            foreach (Tile t in currentMoveCommand)
            {
                verticies[i] = t.transform.position + new Vector3(0f, 0f, -1f);
                i -= 1;
            }
            LineRenderer lr = gameObject.GetComponent<LineRenderer>();
            lr.positionCount = verticies.Length;
            lr.SetPositions(verticies);
            ghostOrder.SetActive(true);
            ghostOrder.transform.position = currentMoveCommand[currentMoveCommand.Count - 1].transform.position + new Vector3(0f, 0f, -1.5f);
        }
        else
        {
            ghostOrder.gameObject.SetActive(false);
            gameObject.GetComponent<LineRenderer>().positionCount = 0;
        }
    }

    public void NextTurn()
    {
        remainingMovementPoints = movementPoints;
    }

    public void ExecuteMove()
    {
        if (currentMoveCommand != null && currentMoveCommand.Count > 1)
        {
            Tile nextTile = currentMoveCommand[0];
            currentMoveCommand.RemoveAt(0);
            float remainingMovementPoint = movementPoints;

            while (currentMoveCommand.Count > 0)
            {
                nextTile = currentMoveCommand[0];
                remainingMovementPoint -= nextTile.GetMovementCost();
                if (remainingMovementPoint > 0)
                {
                    currentMoveCommand.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            
            Vector3 nextPos = nextTile.transform.position;
            transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
            (int x, int y) = nextTile.GetIndicies();
            SetGridPosition(x, y);
            direction = ghostDirection;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, UnitDirectionHelper.directionToRotation(direction)));
        } else if (direction != ghostDirection)
        {
            direction = ghostDirection;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, UnitDirectionHelper.directionToRotation(direction)));
        }
        UpdateUIPath();
    }
}
