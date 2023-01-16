using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum Terrain { Open, Forest, Mountain, Lake };

    private int x;
    private int y;

    private Terrain terrainType;

    private bool showText;

    private bool zoneOfControlToggle = false;
    private Color originalColor;

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

        gameObject.GetComponent<SpriteRenderer>().color = color;
        originalColor = color;
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
        GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, Color.red, zoneOfControlToggle ? 0.0f : 0.4f);
        zoneOfControlToggle = true;
    }

    public void UntoggleZoneOfControlIndication()
    {
        if (zoneOfControlToggle)
        {
            zoneOfControlToggle = false;
            GetComponent<SpriteRenderer>().color = originalColor;
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
