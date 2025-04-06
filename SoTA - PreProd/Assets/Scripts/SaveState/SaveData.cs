using UnityEngine;

public class SaveData 
{
    public bool[] ButtonsActive { get { return buttonsActive; } }
    public Vector3 PlayerPosition { get { return playerPosition; } }
    public Vector3[] BoulderPositions { get { return boulderPositions; } }

    private bool[] buttonsActive;

    private Vector3 playerPosition;
    private Vector3[] boulderPositions;

    public SaveData(Vector3 playerPosition, Vector3[] boulderPositions, bool[] buttonsActive) 
    { 
        this.playerPosition = playerPosition;
        this.boulderPositions = boulderPositions;
        this.buttonsActive = buttonsActive;
    }

}
