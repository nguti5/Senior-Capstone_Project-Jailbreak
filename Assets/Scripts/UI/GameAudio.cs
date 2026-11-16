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
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive) return;

        CloseOptions();

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

        var bounds = new Rect(
            (Screen.width - 320f) / 2f,
            (Screen.height - 170f) / 2f,
            320f,
            170f);

        GUI.ModalWindow(AudioOptionsWindowId, bounds, DrawOptions, "Audio Options");
    }

    private void DrawOptions(int windowId)
    {
        GUILayout.Label($"Overall volume ({Mathf.RoundToInt(volume * 100f)}%)");
        volume = GUILayout.HorizontalSlider(volume, 0f, 1f);
        muted = GUILayout.Toggle(muted, "Mute");
        ApplySettings();

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Done")) CloseOptions();
    }
}