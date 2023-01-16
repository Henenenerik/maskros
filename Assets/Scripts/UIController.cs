using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject currentPhaseText;

    public void NextPhase(TurnPhase nextPhase)
    {
        currentPhaseText.GetComponent<Text>().text = "Current phase: " + GetPhaseText(nextPhase);
    }

    private string GetPhaseText(TurnPhase nextPhase)
    {
        return nextPhase switch
        {
            TurnPhase.ArtilleryFire => "Artillery fire",
            TurnPhase.Movement => "Movement",
            TurnPhase.Combat => "Combat",
            _ => throw new System.Exception("Missing text for TurnPhase"),
        };
    }
}
