using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrateBytes;
using System;

public class SessionExample : MonoBehaviour {
    [SerializeField] private Button startSessionButton;
    [SerializeField] private Button stopSessionButton;
    [SerializeField] private Button heartbeatSessionButton;
    [SerializeField] private Text timeSinceLastHeartBeatText;

    private bool authenticated;
    private bool sessionStarted;
    private DateTime timeSinceLastHeartbeat;

    private void Start() {
        startSessionButton.interactable = false;
        stopSessionButton.interactable = false;
        heartbeatSessionButton.interactable = false;

        CrateBytesManager.instance.GuestLogin("", (GuestLoginResponse res) => {
            Debug.Log("Authenticated as a guest with id of " + res.playerId);
            authenticated = true;
            
            startSessionButton.interactable = true;
        });
    }

    private void Update() {
        if (sessionStarted) {
            TimeSpan timeSinceLastHeartbeatSpan = DateTime.Now - timeSinceLastHeartbeat;

            if (timeSinceLastHeartbeatSpan.TotalMinutes > 10)
                timeSinceLastHeartBeatText.text = "Session has Expired";
            else
                timeSinceLastHeartBeatText.text = "Time since last heartbeat: " + timeSinceLastHeartbeatSpan.ToString(@"hh\:mm\:ss");
        }else {
            timeSinceLastHeartBeatText.text = "No session started";
        }
    }

    public void StartSession() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to start a session");
            return;
        }

        CrateBytesManager.instance.StartSession();

        sessionStarted = true;
        timeSinceLastHeartbeat = DateTime.Now;

        startSessionButton.interactable = false;
        stopSessionButton.interactable = true;
        heartbeatSessionButton.interactable = true;

        Debug.Log("Session started");
    }

    public void EndSession() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to end a session");
            return;
        }

        CrateBytesManager.instance.EndSession();

        sessionStarted = false;

        startSessionButton.interactable = true;
        stopSessionButton.interactable = false;
        heartbeatSessionButton.interactable = false;

        Debug.Log("Session stopped");
    }

    public void HeartbeatSession() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to Heartbeat a session");
            return;
        }

        timeSinceLastHeartbeat = DateTime.Now;

        CrateBytesManager.instance.HeartbeatSession();

        Debug.Log("Heartbeat");
    }
    
}
