using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static readonly string[] Layers = new string[] { "Tile" };
    public GameObject Camera;
    public GameObject Game;

    private Camera mainCamera;
    private GameController gameController;

    private List<Unit> selected;

    public int CurrentTeam;

    public GameObject objectHit;

    private bool successfulMoveCommand = false;
    private Tile targetTile;

    private void Awake()
    {
        mainCamera = Camera.GetComponent<Camera>();
        gameController = Game.GetComponent<GameController>();
        selected = new List<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NextTurn()
    {
        foreach(Unit u in selected)
        {
            u.Select();
        }
        selected.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.turnPhase != TurnPhase.Movement)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            int layermask = (1 << 6) + (1 << 7);
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, layermask))
            {
                if (!hit.collider.gameObject.CompareTag("Tile") && gameController.AreUnitsSelectable())
                {
                    Unit u = hit.collider.gameObject.GetComponent<Unit>();
                    if (u.GetTeam() == CurrentTeam)
                    {
                        if (selected.Contains(u))
                        {
                            u.Select();
                            selected.Remove(u);
                            gameController.HidePossibleMoves(u);
                        }
                        else if (selected.Count == 0 ||
                            (selected.Count == 1 && 
                            u != selected[0]))
                        {
                            if (selected.Count == 1)
                            {
                                selected[0].Select();
                                gameController.HidePossibleMoves(selected[0]);
                                selected.RemoveAt(0);
                            }
                            u.Select();
                            gameController.ShowPossibleMoves(u);
                            selected.Add(u);
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            int layermask = LayerMask.GetMask(Layers);
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue, layermask))
            {
                objectHit = hit.collider.gameObject;
                targetTile = hit.collider.gameObject.GetComponent<Tile>();
                if (!targetTile.IsPossibleMoveTarget)
                {
                    return;
                }
                successfulMoveCommand = true;
                foreach (Unit u in selected)
                {
                    u.MoveCommand(gameController.map, targetTile);
                }
            }
        }
        else if (Input.GetMouseButton(1) && successfulMoveCommand)
        {
            foreach (Unit u in selected)
            {
                u.SetGhostDirection(mainCamera.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        else if (Input.GetMouseButtonUp(1) && successfulMoveCommand)
        {
            foreach (Unit u in selected)
            {
                u.Select();
                gameController.HidePossibleMoves(u);
            }
            selected.Clear();
            successfulMoveCommand = false;
        }
    }
}
