using System;
using System.Collections;
using UnityEngine;

public class CooldownTimer
{
    public bool IsRunning { get; private set; }
    private float cooldownDuration;
    Action onCooldownFinished; //An action (method) that is run when the cooldown is finished
    MonoBehaviour coroutineRunner; //The object running the coroutine

    public CooldownTimer(MonoBehaviour coroutineRunner)
    {
        this.coroutineRunner = coroutineRunner;
    }

    public void Start(float duration, Action onCooldownFinished)
    {
        if (!IsRunning && coroutineRunner != null)
        {
            this.cooldownDuration = duration;
            this.onCooldownFinished = onCooldownFinished;

            IsRunning = true;
            coroutineRunner.StartCoroutine(CooldownCoroutine());
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownDuration);
        IsRunning = false;
        onCooldownFinished?.Invoke();
    }

    //--- I commented out these methods but kept them since they might be useful if we need cooldown timers in other contexts than player death (goobie) ---

    //public void Restart()
    //{
    //    if (coroutineRunner != null)
    //    {
    //        // Stop any existing coroutine before starting a new one
    //        if (IsRunning)
    //        {
    //            coroutineRunner.StopCoroutine(CooldownCoroutine());
    //        }
    //        IsRunning = true;
    //        coroutineRunner.StartCoroutine(CooldownCoroutine());
    //    }
    //}

    //public void Stop()
    //{
    //    if (IsRunning && coroutineRunner != null)
    //    {
    //        coroutineRunner.StopCoroutine(CooldownCoroutine());
    //        IsRunning = false;
    //    }
    //}
}