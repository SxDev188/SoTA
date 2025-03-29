using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData 
{
    private Vector3 playerPosition;
    private Vector3[] boulderPositions;
    private bool[] buttonsActive;

    public Vector3 PlayerPosition {  get { return playerPosition; } }
    public Vector3[] BoulderPositions {  get { return boulderPositions; } }
    public bool[] ButtonsActive {  get { return buttonsActive; } }


    public SaveData(Vector3 playerPosition, Vector3[] boulderPositions, bool[] buttonsActive) 
    { 
        this.playerPosition = playerPosition;
        this.boulderPositions = boulderPositions;
        this.buttonsActive = buttonsActive;
    }


}
