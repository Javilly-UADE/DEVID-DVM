using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clase10CollectionManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text unlockedText;

    private HashSet<string> unlockedCards =
        new HashSet<string>();

    private void Start()
    {
        unlockedText.text =
            "CARTAS DESBLOQUEADAS: 0";
    }

    public void UnlockCard(string cardName)
    {
        if (unlockedCards.Contains(cardName))
        {
            return;
        }

        unlockedCards.Add(cardName);

        unlockedText.text =
            $"DESBLOQUEADO: {cardName}\n" +
            $"TOTAL: {unlockedCards.Count}";

        Debug.Log(
            $"Carta desbloqueada: {cardName}"
        );
    }
}