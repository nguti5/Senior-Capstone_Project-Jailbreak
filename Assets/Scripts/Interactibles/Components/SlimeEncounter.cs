
using UnityEngine;

public sealed class SlimeEncounter : MonoBehaviour, IInteractible
{
    [SerializeField, Min(0.1f)] private float wanderRadius = 2.5f;
    [SerializeField, Min(0.1f)] private float moveSpeed = 0.6f;

    private static readonly Vector3 ClearanceCenter = new Vector3(0f, 0.24f, 0f);
    private static readonly Vector3 ClearanceHalfSize = new Vector3(0.41f, 0.20f, 0.41f);

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Vector3 destination;
    private float waitUntil;

    private bool battleStarting;

    private void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        destination = spawnPosition;
    }

    private void Update()
    {
        if (battleStarting ||
            BattleTransition.InBattle ||
            Time.timeScale == 0f ||
            Time.time < waitUntil)
            return;

        Vector3 position = transform.position;

        if ((destination - position).sqrMagnitude < 0.0025f)
        {
            Vector2 offset = Random.insideUnitCircle * wanderRadius;
            destination = spawnPosition + new Vector3(offset.x, 0f, offset.y);
        }

        Vector3 nextPosition = Vector3.MoveTowards(
            position,
            destination,
            moveSpeed * Time.deltaTime
        );

        Vector3 step = nextPosition - position;
        float distance = step.magnitude;

        if (distance <= Mathf.Epsilon)
            return;

        Vector3 direction = step / distance;

        if (Physics.BoxCast(
            position + ClearanceCenter,
            ClearanceHalfSize,
            direction,
            Quaternion.identity,
            distance + 0.04f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            destination = position;
            waitUntil = Time.time + 0.8f;
            return;
        }

        transform.SetPositionAndRotation(
            nextPosition,
            Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(direction),
                180f * Time.deltaTime
            )
        );

        if (nextPosition == destination)
            waitUntil = Time.time + Random.Range(0.8f, 1.8f);
    }

    public void Interact(GameObject interactor)
    {
        if (!isActiveAndEnabled ||
            battleStarting ||
            BattleTransition.InBattle ||
            Time.timeScale == 0f)
            return;

        SceneTransitionManager manager =
            SceneTransitionManager.Instance;

        if (manager != null)
        {
            if (manager.IsTransitioning)
                return;

            battleStarting = true;

            manager.BeginBattleTransition(StartSlimeBattle);
        }
        else
        {
            Debug.LogWarning(
                "SceneTransitionManager not found. Starting battle normally."
            );

            StartSlimeBattle();
        }
    }

    private void StartSlimeBattle()
    {
        battleStarting = false;

        // Use the enemy configured in BattleScene1.
        BattleTransition.StartBattle(
            null,
            0,
            OnBattleEnded
        );
    }

    private void OnBattleEnded(BattleResult result)
    {
        if (this == null || result == BattleResult.Fled)
            return;

        GameObject replacement = Instantiate(
            gameObject,
            spawnPosition,
            spawnRotation,
            transform.parent
        );

        replacement.name = gameObject.name;

        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
