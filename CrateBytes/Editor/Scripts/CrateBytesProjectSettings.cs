using UnityEngine;

[CreateAssetMenu(fileName = "CrateBytesProjectSettings", menuName = "CrateBytes/CrateBytesProjectSettings")]
public class CrateBytesProjectSettings : ScriptableObject
{
    [HideInInspector]
    public string ProjectKey
    {
        get 
        {
            if (string.IsNullOrEmpty(projectKey))
            {
                Debug.LogError("Project Key is not set!");
            }
            return projectKey;
        }
        set { projectKey = value; }
    }

    [SerializeField] private string projectKey = "";


    [HideInInspector]
    public string DomainURL
    {
        get
        {
            if (string.IsNullOrEmpty(domainURL))
            {
                Debug.LogError("No domain url set");
            }
            return domainURL;
        }
        set { domainURL = value; }
    }

    [Tooltip("Not using our public backend? Change the domain here!")]
    [SerializeField] private string domainURL = "https://cratebytes.com/api/game/";


}
