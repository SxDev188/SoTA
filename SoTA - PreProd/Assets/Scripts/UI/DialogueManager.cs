using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    bool inADialogue;
    bool typingLetters;
    float textDelay = 0.1f;

    string completedDialogue;

    Queue<SO_Dialogue.Info> dialogueQueue;

    [SerializeField] GameObject dialogueBox;
    [SerializeField] TMP_Text dialogueText;

    // StartCoroutine(TypeText(info));
    public IEnumerator TypeText(SO_Dialogue.Info info)
    {
        completedDialogue = info.dialouge;

        typingLetters = true;
        for (int i = 0; i < info.dialouge.ToCharArray().Length; i++)
        {
            yield return new WaitForSeconds(textDelay);
            dialogueText.text += info.dialouge.ToCharArray()[i];
        }
        typingLetters = false;
    }

    public void EndDialogue()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerInput>().enabled = true;
        dialogueBox.SetActive(false);
        inADialogue = false;
    }

    public void CompleteText()
    {
        dialogueText.text = completedDialogue;
    }

    void OnInteract(InputValue value)
    {
        if (inADialogue)
        {
            DeQueue();
        }
    }

    public void Queue(SO_Dialogue dialogue)
    {
        if (inADialogue)
        {
            return;
        }

        GameObject.FindWithTag("Player").GetComponent<PlayerInput>().enabled = false;
        dialogueBox.SetActive(true);
        inADialogue = true;
        dialogueQueue.Clear();
        foreach (SO_Dialogue.Info line in dialogue.dialogueInfo)
        {
            dialogueQueue.Enqueue(line);
        }
        DeQueue();
    }

    void DeQueue()
    {
        if (typingLetters)
        {
            CompleteText();
            StopAllCoroutines();
            typingLetters = false;
            return;
        }
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        SO_Dialogue.Info info = dialogueQueue.Dequeue();
        completedDialogue = info.dialouge;
        dialogueText.text = "";
        StartCoroutine(TypeText(info));
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        dialogueQueue = new Queue<SO_Dialogue.Info>();
    }
}
