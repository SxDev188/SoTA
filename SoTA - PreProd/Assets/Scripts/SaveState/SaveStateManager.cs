using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//A Singelton since it is a Manager
public class SaveStateManager : MonoBehaviour
{
    public static SaveStateManager Instance { get; private set; }
    [SerializeField] private List<GameObject> buttons = new List<GameObject>();
    private List<bool> savedStateButtons = new List<bool>();
    private GameObject player;
    private Vector3 savedStatePlayer;
    private List<SaveData> saves = new List<SaveData>();
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
        player = GameObject.FindGameObjectWithTag("Player");
    }


    private void OnSave()
    {
        Save();
        Debug.Log("Saved");
    }
    private void OnLoad()
    {
        Load();
        Debug.Log("Loaded");
    }
    private void Save()
    {
        //saved = true;
        savedStateButtons.Clear();
        foreach (GameObject button in buttons)
        {
            ButtonScript buttonScript = button.GetComponent<ButtonScript>();
            savedStateButtons.Add(buttonScript.IsActive);
        }
        savedStatePlayer = player.transform.position;
    }
    private void Load()
    {
        if (savedStateButtons is not null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                ButtonScript buttonScript = buttons[i].GetComponent<ButtonScript>();
                if (buttonScript.IsActive != savedStateButtons[i])
                {
                    buttonScript.Interact();
                }

            }
            
            player.transform.position = savedStatePlayer;
        }
    }
}
