using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrateBytes;

public class MetadataManager : MonoBehaviour {
    [SerializeField] private InputField MetadataInputField;
    [SerializeField] private Button GetMetadataButton;
    [SerializeField] private Button AddUpdateMetadataButton;
    [SerializeField] private Button DeleteMetadataButton;

    private bool authenticated;

    private void Start() {
        GetMetadataButton.interactable = false;
        AddUpdateMetadataButton.interactable = false;
        DeleteMetadataButton.interactable = false;
        string playerId = PlayerPrefs.GetString("playerId", "");

        CrateBytesManager.instance.GuestLogin(playerId, (GuestLoginResponse res) => {
            Debug.Log("Authenticated as a guest with id of " + res.playerId);
            authenticated = true;

            GetMetadataButton.interactable = true;
            AddUpdateMetadataButton.interactable = true;
            DeleteMetadataButton.interactable = true;

            PlayerPrefs.SetString("playerId", res.playerId);

            GetMetadata();
        });
    }

    public void GetMetadata() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to get metadata");
            return;
        }

        CrateBytesManager.instance.GetMetadata((GetMetadataResponse res) => {
            MetadataInputField.text = res.data;
        });
    }

    public void AddUpdateMetadata() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to add/update metadata");
            return;
        }

        CrateBytesManager.instance.AddUpdateMetadata(MetadataInputField.text, (AddUpdateMetadataResponse res) => {
            MetadataInputField.text = res.data;

            Debug.Log("Metadata added/updated");
        });
    }

    public void DeleteMetadata() {
        if (!authenticated) {
            Debug.LogError("You must be authenticated to delete metadata");
            return;
        }

        CrateBytesManager.instance.DeleteMetadata((string res) => {
            MetadataInputField.text = "";

            Debug.Log("Metadata deleted");
        });
    }
}
