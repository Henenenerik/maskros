using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum Terrain { Open, Forest, Mountain, Lake };

    private int x;
    private int y;

    private Terrain terrainType;

    private bool showText;

    public bool IsPossibleMoveTarget { get; private set; } = false;
    private bool zoneOfControlToggle = false;
    private Color originalColor;

    [SerializeField]
    private SpriteRenderer[] renderers;
    private const int BaseRenderer = 0;
    private const int ZoneOfControlRenderer = 1;
    private const int HighlightRenderer = 2;


    public (int, int) GetIndicies() { return (x, y); }

    public void SetIndicies(int x, int y) { this.x = x; this.y = y; AdjustText(); }

    public void ToggleText()
    {
        showText = !showText;
        TextMesh text = transform.GetChild(0).GetComponent<TextMesh>();
        text.gameObject.SetActive(showText);
    }

    private void AdjustText()
    {
        TextMesh text = transform.GetChild(0).GetComponent<TextMesh>();

        text.text = string.Format("{0},{1}", x, y);

        text.gameObject.SetActive(showText);
    }

    public void InitTile(int x, int y, bool showText)
    {
        this.showText = showText;
        SetIndicies(x, y);

        terrainType = Terrain.Open;
    }

    public void InitTile(int x, int y, bool showText, Color color)
    {
        this.showText = showText;
        SetIndicies(x, y);
        terrainType = ColorToTerrain(color);

        renderers[BaseRenderer].color = color;
        originalColor = color;
        renderers[ZoneOfControlRenderer].enabled = false;
        renderers[HighlightRenderer].enabled = false;
    }

    public static Terrain ColorToTerrain(Color color)
    { 
        if (color == Color.black)
        {
            return Terrain.Mountain;
        }
        if (color == Color.green)
        {
            return Terrain.Forest;
        }
        if (color == Color.blue)
        {
            return Terrain.Lake;
        }
        return Terrain.Open;
    }

    public float GetMovementCost()
    {
        switch (terrainType)
        {
            case Terrain.Open:
                return 1;
            case Terrain.Forest:
                return 2;
            case Terrain.Lake:
                return 20;
            case Terrain.Mountain:
                return 5;
            default:
                return 10000;
        }
    }

    public void ToggleZoneOfControlIndication()
    {
        renderers[ZoneOfControlRenderer].enabled = true;
        zoneOfControlToggle = true;
    }

    public void UntoggleZoneOfControlIndication()
    {
        if (zoneOfControlToggle)
        {
            zoneOfControlToggle = false;
            renderers[ZoneOfControlRenderer].enabled = false;
        }
    }

    public void TogglePossibleSelection()
    {
        if (!renderers[ZoneOfControlRenderer].enabled)
        {
            renderers[HighlightRenderer].enabled = true;
        }
        IsPossibleMoveTarget = true;
    }

    public void UntogglePossibleSelection()
    {
        if (IsPossibleMoveTarget)
        {
            renderers[HighlightRenderer].enabled = false;
            IsPossibleMoveTarget = false;
        }
    }

    public void Deconstruct()
    {
        if (Application.isPlaying)
        {
            Destroy(gameObject);
        }
        else if (Application.isEditor) // Will not work when building project.
        {
            //Debug.Log("Tile Deconstructed");
            DestroyImmediate(gameObject);
        }
    }
}
