
using UnityEngine;

public class KeycardDoorUnlock : MonoBehaviour
{
    [SerializeField] private SceneDoor sceneDoor;
    [SerializeField] private LegacyCatalogManager catalogManager;

    private void Update()
    {
        if (sceneDoor == null || catalogManager == null)
            return;

        if (!catalogManager.IsInventoryLoaded)
            return;

        bool hasKeycard = catalogManager.HasItem("tutorial_key");

        sceneDoor.SetLocked(!hasKeycard);
    }
}
