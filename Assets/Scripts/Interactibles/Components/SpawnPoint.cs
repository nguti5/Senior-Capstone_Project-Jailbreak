using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnId;

    [Tooltip("Used when the room is started without a door: Play from the editor, or resuming from a save.")]
    [SerializeField] private bool isDefault;

    private void Start()
    {
        string pending = SceneTransition.PendingSpawnId;
        bool matchesDoor = !string.IsNullOrEmpty(pending) && pending == spawnId;
        bool useAsDefault = string.IsNullOrEmpty(pending) && isDefault;

        if (!matchesDoor && !useAsDefault) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("SpawnPoint: no object tagged 'Player' found.", this);
            return;
        }

        // CharacterController overrides transform changes unless disabled first
        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        player.transform.SetPositionAndRotation(transform.position, transform.rotation);

        if (controller != null) controller.enabled = true;

        if (matchesDoor) SceneTransition.ClearPendingSpawn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}