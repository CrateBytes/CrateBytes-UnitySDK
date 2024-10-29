using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrateBytes;
using UnityEngine.UI;

public class LeaderboardExample : MonoBehaviour {
    [SerializeField] private string leaderboardId;
    [SerializeField] private Button SubmitScoreButton;
    [SerializeField] private Button GetScoreButton;
    private bool authenticated;

    private void Start() {
        SubmitScoreButton.interactable = false;
        GetScoreButton.interactable = false;
        CrateBytesManager.instance.GuestLogin("", (GuestLoginResponse res) => {
            Debug.Log("Authenticated as a guest with id of " + res.playerId);
            authenticated = true;

            SubmitScoreButton.interactable = true;
            GetScoreButton.interactable = true;
        });
    }

    public void GetLeaderboard() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to get the leaderboard");
            return;
        }

        CrateBytesManager.instance.GetLeaderboard(leaderboardId, 0, (LeaderboardResponse res) => {
            Debug.Log("Leaderboard:");
            foreach (LeaderboardEntry entry in res.entries) {
                Debug.Log($"Player [{entry.player.playerId}] has a score of {entry.score}");
            }
        });
    }

    public void SubmitScore(InputField scoreInput) {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to submit a score");
            return;
        }

        CrateBytesManager.instance.SubmitScoreToLeaderboard(leaderboardId, int.Parse(scoreInput.text), (string res) => {
            Debug.Log("Score submitted");
        });
    }
}
