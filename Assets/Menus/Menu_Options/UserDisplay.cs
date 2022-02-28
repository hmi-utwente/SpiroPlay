using System.Collections;
using System.Collections.Generic;
using Spirometry.SpiroController;
using UnityEngine;

public class UserDisplay : MonoBehaviour
{
    #pragma warning disable 649
    [SerializeField] private TMPro.TextMeshProUGUI outputText;
    #pragma warning restore 649
    
    // Start is called before the first frame update
    private void Start()
    {
        var user = SessionManager.CurrentUser;
        var data = "";
        data += "Naam: " + user.userName + "\n";
        data += "Leeftijd: " + user.age + "\n";
        data += "Datum van registratie: " + user.creationDate + "\n";
        data += "Datum van laatste speelsessie: " + user.lastPlayedDate + "\n";
        data += "Datum van laatste level-unlock: " + user.lastUnlockDate + "\n";
        var id = user.currentSessionID;
        data += "Huidige Sessie (SID): " + id;
        data += (id == 0) ? " (Nog niet begonnen) \n" : "\n";
        data += "Munten: " + user.coins + "\n";
        data += "Voorspelde FVC: " + user.predictedFvc + "\n";
        data += "Voorspelde PEF: " + user.predictedPef + "\n";
        data += "\n";
        data += "Metafoor status: \n";
        
        //add the status of all metafors
        var metadata = SessionManager.Instance.settings.MetaphorInfo;
        for (var index = 0; index < user.metaphorStatus.Length; index++)
        {
            var status = user.metaphorStatus[index];
            data += metadata[index].displayName;
            data += ": " + (status ? "speelbaar" : "vergrendeld") + "\n";
        }

        outputText.text = data;
    }
}
