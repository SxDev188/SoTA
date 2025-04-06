using System.Collections.Generic;
using UnityEngine;

//A Singelton since it is a Manager
public class SaveStateManager : MonoBehaviour
{
    public static SaveStateManager Instance { get; private set; }

    [SerializeField] private bool debugMode = false;

    private List<SaveData> saves = new List<SaveData>();

    private GameObject player;
    private GameObject[] buttons;
    private GameObject[] boulders;

    //Temporary fix I hope or more data added here and removed from other places
    private StarActions starActions;

    //private bool saved = false;
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        SetSaveableObjectReferences();
        Save();
        starActions = GameObject.FindGameObjectWithTag("Star").GetComponent<StarActions>();
    }

    //For Debug Purposes  <<-- If it works, then we should remove?
    private void OnSave()
    {
        if (debugMode)
        {
            Save();
            Debug.Log("Saved");
        }
        
    }
    private void OnLoad()
    {
        if (debugMode)
        {
            Load();
            Debug.Log("Loaded");
        }
       
    }
    private void SetSaveableObjectReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        buttons = GameObject.FindGameObjectsWithTag("Button");
        boulders = GameObject.FindGameObjectsWithTag("Boulder");
    }
    
    public void Save()
    {
        saves.Add(CreateSaveData());
    }
    private SaveData CreateSaveData()
    {
        Vector3 playerPositions = player.transform.position;
        Vector3[] boulderPositions = GetBoulderPostions();
        bool[] buttonsActive = GetButtonsState();

        SaveData saveData = new SaveData(playerPositions, boulderPositions, buttonsActive);
        return saveData;
    }
    private bool[] GetButtonsState()
    {
        bool[] buttonsActive = new bool[buttons.Length];
        int index = 0;
        foreach (GameObject button in buttons)
        {
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            buttonsActive[index++] = buttonScript.IsActive;
        }

        return buttonsActive;
    }
    private Vector3[] GetBoulderPostions()
    {
        Vector3[] bouldersPosition = new Vector3[boulders.Length];
        int index = 0;
        foreach (GameObject boulder in boulders)
        {
            bouldersPosition[index++] = boulder.transform.position;
        }
        return bouldersPosition;
    }
    
    public void Load()
    {
        SaveData dataToLoad = saves[saves.Count-1];
        SetFromSaveData(dataToLoad);
        starActions.Recall();
    }
    private void SetFromSaveData(SaveData saveData)
    {
        SetFromBoulderPositions(saveData);
        SetFromButtonStates(saveData);
        player.transform.position = saveData.PlayerPosition;
    }
    private void SetFromBoulderPositions(SaveData saveData)
    {
        Vector3[] boulderPositions = saveData.BoulderPositions;
        int index = 0;
        foreach(GameObject boulder in boulders)
        {
            boulder.GetComponent<BoulderMoveScript>().Detach();
            boulder.transform.position = boulderPositions[index++];
        }
    }
    private void SetFromButtonStates(SaveData saveData)
    {
        bool[] buttonsActive = saveData.ButtonsActive;
        int index = 0;  
        foreach(GameObject button in buttons)
        {
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            buttonScript.SetState(buttonsActive[index++]);
        }
    }
}
