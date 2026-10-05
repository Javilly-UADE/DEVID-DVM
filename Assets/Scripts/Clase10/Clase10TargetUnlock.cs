using UnityEngine;

public class Clase10TargetUnlock : MonoBehaviour
{
    [SerializeField]
    private string cardName;

    [SerializeField]
    private Clase10CollectionManager collectionManager;

    private bool unlocked;

    public void Unlock()
    {
        if (unlocked)
        {
            return;
        }

        unlocked = true;

        collectionManager.UnlockCard(cardName);
    }
}