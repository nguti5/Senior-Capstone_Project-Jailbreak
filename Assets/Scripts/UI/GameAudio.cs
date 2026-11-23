using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameAudio : MonoBehaviour
{
    private const string VolumeKey = "Audio.MasterVolume";
    private const string MuteKey = "Audio.Muted";
    private const int AudioOptionsWindowId = 12001;

    private static GameAudio instance;

    private AudioSource music;
    private AudioListener fallbackListener;
    private AudioClip backgroundTrack;
    private AudioClip battleTrack;
    private float volume;
    private bool muted;
    private bool optionsOpen;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (instance != null) return;

        var host = new GameObject(nameof(GameAudio));
        DontDestroyOnLoad(host);
        instance = host.AddComponent<GameAudio>();
    }

    private void Awake()
    {
        backgroundTrack = Resources.Load<AudioClip>("Audio/background_music");
        battleTrack = Resources.Load<AudioClip>("Audio/battle_music");

        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        music.loop = true;
        music.spatialBlend = 0f;

        volume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));
        muted = PlayerPrefs.GetInt(MuteKey, 0) != 0;
        ApplySettings();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void Start()
    {
        // The Editor can start Play Mode with a scene already open.
        RefreshAudioListener();
        PlayMusicForScene(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshAudioListener();
        if (mode == LoadSceneMode.Additive) return;

        pinned = false;
        CloseOptions();
        PlayMusicForScene(scene);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        RefreshAudioListener();
    }

    private void OnActiveSceneChanged(Scene previousScene, Scene activeScene)
    {
        // An additive scene takes over the soundtrack when it becomes active.
        if (!activeScene.isLoaded) return;

        RefreshAudioListener();
        PlayMusicForScene(activeScene);
    }

    private void RefreshAudioListener()
    {
        foreach (var listener in FindObjectsByType<AudioListener>(FindObjectsSortMode.None))
        {
            if (listener == fallbackListener || !listener.isActiveAndEnabled) continue;

            if (fallbackListener != null)
                fallbackListener.enabled = false;
            return;
        }

        if (fallbackListener == null)
            fallbackListener = gameObject.AddComponent<AudioListener>();
        else
            fallbackListener.enabled = true;
    }

    private void PlayMusicForScene(Scene scene)
    {
        if (!scene.IsValid()) return;

        bool isBattle = scene.name.IndexOf("Battle", StringComparison.OrdinalIgnoreCase) >= 0;
        AudioClip nextTrack = isBattle ? battleTrack : backgroundTrack;

        if (music.clip == nextTrack && music.isPlaying) return;

        music.Stop();
        music.clip = nextTrack;

        if (nextTrack != null)
            music.Play();
        else
            Debug.LogWarning($"Music clip missing for scene '{scene.name}'.");
    }

    private bool pinned;

    public static void PinOptions(bool pin)
    {
        if (instance == null) return;
        instance.pinned = pin;
        instance.optionsOpen = pin;
    }

    public static void OpenOptions()
    {
        if (instance != null) instance.optionsOpen = true;
    }

    private void ApplySettings()
    {
        AudioListener.volume = muted ? 0f : volume;
    }

    private void CloseOptions()
    {
        if (!optionsOpen) return;

        optionsOpen = false;
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.SetInt(MuteKey, muted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnGUI()
    {
        if (!optionsOpen) return;

        const float windowWidth = 320f;
        const float windowHeight = 170f;
        const float targetScale = 2f;

        // Scale the whole window, including controls and text, and fit smaller views.
        float scale = Mathf.Min(targetScale, Screen.width / windowWidth, Screen.height / windowHeight);
        var bounds = new Rect(
            (Screen.width / scale - windowWidth) / 2f,
            (Screen.height / scale - windowHeight) / 2f,
            windowWidth,
            windowHeight);

        Matrix4x4 previousMatrix = GUI.matrix;
        try
        {
            GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
            GUI.ModalWindow(AudioOptionsWindowId, bounds, DrawOptions, "Audio Options");
        }
        finally
        {
            GUI.matrix = previousMatrix;
        }
    }

    private void DrawOptions(int windowId)
    {
        GUILayout.Label($"Overall volume ({Mathf.RoundToInt(volume * 100f)}%)");
        volume = GUILayout.HorizontalSlider(volume, 0f, 1f);
        muted = GUILayout.Toggle(muted, "Mute");
        ApplySettings();

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Done")) CloseOptions();

        GUILayout.FlexibleSpace();
        if (!pinned && GUILayout.Button("Done")) CloseOptions();
    }
}
