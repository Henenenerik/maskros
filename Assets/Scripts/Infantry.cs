using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infantry : Unit
{
    [Header("Unit Characteristics")]
    public int BaseMovementPoints;
    public float BaseInitiative;
    public float BaseStrength;
    public float BaseMorale;

    [Header("Unit Statistics")]
    public float CurrentInitiative;

    private bool selected;
    private SpriteRenderer highlight;

    // Start is called before the first frame update
    void Start()
    {
        highlight = transform.GetChild(0).GetComponent<SpriteRenderer>(); // Will break if children are rearranged
        highlight.enabled = false;
        selected = false;
        movementPoints = BaseMovementPoints;
        remainingMovementPoints = movementPoints;
        initiative = BaseInitiative;
        maxMorale = currentMorale = BaseMorale;
        orderHighlight = Instantiate(orderHighlightPrefab);
        orderHighlight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentInitiative = base.GetInitiative();
    }

    override public void Select()
    {
        highlight.enabled = !highlight.enabled;
        selected = !selected;
    }

    public override void ToggleHighlight()
    {
        highlight.enabled = !highlight.enabled;
    }
}
