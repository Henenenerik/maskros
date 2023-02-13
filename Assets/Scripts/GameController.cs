using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector] public Map map;
    [HideInInspector] public UIController uIController;

    [Header("Prefabs")]
    public GameObject Infantry;

    [Header("Team Settings")]
    [SerializeField] Color Team1;
    [SerializeField] Color Team2;

    [Header("Debug Settings")]
    [SerializeField] private bool enableTileCoordinates;
    [SerializeField] private GameObject prefab_turnSteppingButton;

    private bool turnComplete = true;
    private bool last_enableTileCoordinates;

    private Color[] teamColors;
    private List<Unit> units;
    private float nextTurnTimeStamp = 0;

    [Header("Debug Information")]
    public TurnPhase turnPhase;


    private void Awake()
    {
        WorldGenerator worldGenerator = GetComponent<WorldGenerator>();
        map = worldGenerator.GenerateMap();
        enableTileCoordinates = worldGenerator.EnableTileCoordinates;
        last_enableTileCoordinates = enableTileCoordinates;

        units = new List<Unit>();

        teamColors = new Color[] { Team1, Team2 };

        uIController = GameObject.Find("UIManager").GetComponent<UIController>();
        uIController.NextPhase(turnPhase);
    }

    void Start()
    {
        (int w, int h) = map.GetMapSize();
        if (w > 7 && h > 4)
        {
            Unit u;
            var unitTemplates = new List<(int, int, int)> { (12, 10, 0), (14, 10, 0), (13, 6, 1), (15, 7, 1) };
            unitTemplates.ForEach(item =>
            {
                u = CreateInfantryAt(item.Item1, item.Item2, item.Item3);
                units.Add(u);
                map.TrackUnit(u);
            });
        }
    }

    private Unit CreateInfantryAt(int x, int y, int team)
    {
        GameObject obj = Instantiate(Infantry);
        
        Infantry inf = obj.GetComponent<Infantry>();
        inf.SetGridPosition(x, y);
        inf.SetTeam(team, teamColors[team]);
        
        Vector3 pos = map.IndiciesToPosition(x, y);
        pos.z = -5f;
        obj.transform.position = pos;

        return inf;
    }

    void Update()
    {
        if (last_enableTileCoordinates != enableTileCoordinates)
        {
            map.ApplyToTiles((x) => { x.ToggleText(); });

            last_enableTileCoordinates = enableTileCoordinates;
        }
    }
    
    private void NextPhase()
    {
        OnPhaseEnd();
        turnPhase = (TurnPhase)((((int)turnPhase) + 1) % 3);
        uIController.NextPhase(turnPhase);
        OnPhaseStart();
    }

    private void OnPhaseStart()
    {
        if (turnPhase == TurnPhase.Movement)
        {
            map.ToggleTileMovementHighlighting(0);
            map.UpdateZoneOfControlMap(0);
        }
        else if (turnPhase == TurnPhase.Combat)
        {
            foreach(var unit in units)
            {
                if (unit.MarkedForCombat)
                {
                    unit.ToggleHighlight();
                }
            }
        }
    }

    private void OnPhaseEnd()
    {
        if (turnPhase == TurnPhase.Movement)
        {
            map.MarkUnitsForCombat();
            map.ToggleTileMovementHighlighting(0);
        }
        else if (turnPhase == TurnPhase.Combat)
        {
            foreach (var unit in units)
            {
                if (unit.MarkedForCombat)
                {
                    unit.ToggleHighlight();
                }
                unit.MarkedForCombat = false;
            }
        }
    }

    // ################ MAP FUNCTIONS ###############

    public (int, int) GetMapSize()
    {
        return map.GetMapSize();
    }

    public Vector2 GetMapCorner()
    {
        return map.GetCornerPosition();
    }

    public void NextTurnButton()
    {
        if (Time.time - nextTurnTimeStamp < 0.05f) { return; }

        foreach (Unit u in units)
        {
            u.ExecuteMove();
            u.NextTurn();
        }

        nextTurnTimeStamp = Time.time;
        NextPhase();
    }

    public bool AreUnitsSelectable()
    {
        return turnComplete;
    }

    public void ShowPossibleMoves(Unit u)
    {
        var res = map.FindPossibleMoves(u);
        foreach(var tile in res)
        {
            tile.TogglePossibleSelection();
        }
    }

    public void HidePossibleMoves(Unit u)
    {
        map.ApplyToTiles(t => t.UntogglePossibleSelection());
    }
}
