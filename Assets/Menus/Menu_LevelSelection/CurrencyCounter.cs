using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CurrencyCounter : MonoBehaviour
{
    #pragma warning disable 649
    
    private Button Button => GetComponent<Button>();
    [SerializeField] private TMPro.TextMeshProUGUI counterText;
    
    #pragma warning restore 649
    
    // Start is called before the first frame update
    private void Start()
    {
        Button.onClick.AddListener(TriggerButtonAction);
    }

    public void UpdateCurrency(int Coins)
    {
        //show correct amount of coins of the user, when he/she is logged in
        if (Coins == -99) counterText.text = "logged out";
        else counterText.text = Coins == 0 ? Coins + " Munt" : Coins + " Munten";
    }

    private static void TriggerButtonAction()
    {
        ConnectionCanvas.Instance.Warn_Custom("Krijg munten door levels te spelen");
    }
}
