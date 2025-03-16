using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//A Singelton since it is a Manager
public class SaveStateManager : MonoBehaviour
{
    public static SaveStateManager Instance { get; private set; }
    [SerializeField] private List<Linus_ButtonScript> buttons = new List<Linus_ButtonScript>();
    private List<Linus_ButtonScript> savedStateButtons;
    private GameObject player;
    private GameObject savedStatePlayer;
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
    public void Save()
    {
        //saved = true;
        savedStatePlayer.CloneViaSerialization(player);
        savedStateButtons = new List<Linus_ButtonScript>(buttons);
    }
    public void Load()
    {
        if (savedStateButtons is not null)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].IsActive != savedStateButtons[i].IsActive)
                {
                    buttons[i].Interact();
                }

            }
            player = savedStatePlayer;
        }
    }
}
