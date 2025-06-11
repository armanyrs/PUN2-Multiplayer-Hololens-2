using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RobotScript : MonoBehaviour
{
    public GameObject robotObject;
    public UnityEvent onKilled;
    public UnityEvent onSpawned;

    private Animator robotAnimator;
    private float killTimer = 0f;
    private bool timerPaused = false;

    void Start()
    {
        if (robotObject != null)
        {
            robotAnimator = robotObject.GetComponent<Animator>();
        }
    }

    public void SpawnRobot()
    {
        if (robotObject.activeSelf == false)
        {
            robotObject.SetActive(true);
            ResetTimer(30f);
            onSpawned.Invoke();
        }
    }

    public void KillRobot()
    {
        if (robotObject.activeSelf == true)
        {
            robotObject.SetActive(false);
            onKilled.Invoke();
        }
    }

    void Update()
    {
        if (killTimer >= 0 && !timerPaused)
        {
            killTimer -= Time.deltaTime;
            if (killTimer <= 0)
            {
                KillRobot();
            }
        }
    }

    public void ResetTimer(float timer = 15f)
    {
        if (killTimer < timer)
            killTimer = timer;
    }

    public void PauseTimer()
    {
        timerPaused = true;
    }

    public void PlayTimer()
    {
        timerPaused = false;
    }

    // fking throwaways dude
    public void ResetTimer(string _)
    {
        this.ResetTimer(15f);
    }

    public void ResetTimer(Vector2 _)
    {
        this.ResetTimer(15f);
    }
}
