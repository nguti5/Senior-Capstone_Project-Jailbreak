using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HallwayLightPlacer : MonoBehaviour
{
    [Header("Hallway size (the blue arrow points down the hall)")]
    [SerializeField] private float length = 40f;
    [SerializeField] private float width = 4f;
    [SerializeField] private float ceilingHeight = 3f;

    [Header("Optional: read the size from the room")]
[SerializeField] private Renderer room;   // the hallway's Mesh Renderer

    [Header("Spacing")]
    [SerializeField] private float spacing = 4f;

    [Header("Which sides")]
    [SerializeField] private bool leftWall = true;
    [SerializeField] private bool rightWall = true;
    [SerializeField] private bool ceiling = true;
    [SerializeField] private bool staggerSides = true;   // right wall shifted half a step, for more even light

    [Header("Placement")]
    [SerializeField] private float wallHeight = 1.8f;
    [SerializeField] private float wallInset = 0.3f;     // distance from the wall
    [SerializeField] private float ceilingInset = 0.3f;

    [Header("Light settings")]
    [SerializeField] private Color color = new Color(0f, 0.9f, 1f);
    [SerializeField] private float intensity = 3f;
    [SerializeField] private float range = 7f;

    [ContextMenu("Build Lights")]
    public void Build()
    {
        Clear();

        int rows = Mathf.Max(1, Mathf.FloorToInt(length / spacing));

        Transform left = leftWall ? MakeContainer("Left Lights") : null;
        Transform right = rightWall ? MakeContainer("Right Lights") : null;
        Transform top = ceiling ? MakeContainer("Ceiling Lights") : null;

        for (int i = 0; i < rows; i++)
        {
            float z = (i + 0.5f) * spacing;

            if (left != null)
                MakeLight(left, new Vector3(-width * 0.5f + wallInset, wallHeight, z));

            if (right != null)
            {
                float rz = z + (staggerSides ? spacing * 0.5f : 0f);
                if (rz <= length)
                    MakeLight(right, new Vector3(width * 0.5f - wallInset, wallHeight, rz));
            }

            if (top != null)
                MakeLight(top, new Vector3(0f, ceilingHeight - ceilingInset, z));
        }
    }

    [ContextMenu("Fit To Room And Build")]
    public void FitToRoomAndBuild()
    {
        if (room == null) { Debug.LogError("Assign the room's Mesh Renderer first."); return; }

        Bounds b = room.bounds;   // world-space size of the whole hallway
        transform.rotation = Quaternion.identity;
        transform.position = new Vector3(b.center.x, b.min.y, b.min.z);   // floor, middle, start of the hall

        length = b.size.z;
        width = b.size.x;
        ceilingHeight = b.size.y;

        Build();
    }

    [ContextMenu("Clear Lights")]
    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying) { Undo.DestroyObjectImmediate(child); continue; }
#endif
            Destroy(child);
        }
    }

    private Transform MakeContainer(string containerName)
    {
        var go = new GameObject(containerName);
        go.transform.SetParent(transform, false);
        Register(go);
        return go.transform;
    }

    private void MakeLight(Transform parent, Vector3 localPosition)
    {
        var go = new GameObject("Hall Light");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;

        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.intensity = intensity;
        l.range = range;
        l.shadows = LightShadows.None;

        Register(go);
    }

    private static void Register(GameObject go)
    {
#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(go, "Build hallway lights");
#endif
    }
}