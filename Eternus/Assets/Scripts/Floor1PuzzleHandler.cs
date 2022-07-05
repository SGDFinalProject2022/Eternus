using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Floor1PuzzleHandler : MonoBehaviour
{
    int activePowerBoxes = 0;
    [SerializeField] int neededPowerBoxes = 2;
    [SerializeField] UI uiController;

    public UnityEvent onActivate;

    public void UpdatePower()
    {
        activePowerBoxes++;
        if(activePowerBoxes == neededPowerBoxes)
        {
            uiController.ShowObjective("All breakers have been activated.");
            onActivate.Invoke();
        }
        else
        {
            uiController.ShowObjective(activePowerBoxes + " of " + neededPowerBoxes + " breakers activated.");
        }
    }
}
