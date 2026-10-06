using System;
using System.Threading.Tasks;
using UnityEngine;
using PlayFab;

public class PlayFabManager : MonoBehaviour
{
    private const string TitleId = "1A3333";
    private const string CustomPlayerId = "UnityPlayerId";

    private PFServiceConfig serviceConfig;
    private PFPlayerEntity playerEntity;

    private bool playFabServicesInitialized = false;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || MICROSOFT_GDK_SUPPORT
    private bool xGameRuntimeInitialized = false;
#endif

    async void Start()
    {
        Debug.Log("Starting PlayFab...");

        bool success = await InitializeAndLogin();

        if (success)
        {
            Debug.Log("PlayFab is ready!");
        }
        else
        {
            Debug.LogError("PlayFab failed to initialize or login.");
        }
    }

    private async Task<bool> InitializeAndLogin()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || MICROSOFT_GDK_SUPPORT
        int xGameRuntimeResult = PlayFab.XGameRuntime.Initialize();

        if (HRESULT.Failed(xGameRuntimeResult))
        {
            Debug.LogError(
                $"Failed to initialize XGameRuntime: 0x{xGameRuntimeResult:X8}"
            );
            return false;
        }

        xGameRuntimeInitialized = true;
#endif

        PFResult initResult = PFServices.Initialize();

        if (initResult.Failed() &&
            initResult.HResult != HRESULT.E_PF_CORE_ALREADY_INITIALIZED &&
            initResult.HResult != HRESULT.E_PF_SERVICES_ALREADY_INITIALIZED)
        {
            Debug.LogError(
                $"Failed to initialize PlayFab: 0x{initResult.HResult:X8}"
            );
            return false;
        }

        playFabServicesInitialized = true;

        string apiEndpoint = $"https://{TitleId}.playfabapi.com";

        PFResult<PFServiceConfig> configResult =
            PFCore.CreateServiceConfig(apiEndpoint, TitleId);

        if (configResult.Failed())
        {
            Debug.LogError(
                $"Failed to create PlayFab configuration: 0x{configResult.HResult:X8}"
            );
            return false;
        }

        serviceConfig = configResult.Result;

        var loginRequest = new PFAuthenticationLoginWithCustomIDRequest
        {
            CustomId = CustomPlayerId,
            CreateAccount = true
        };

        var loginResult =
            await serviceConfig.AuthenticationLoginWithCustomIDAsync(loginRequest);

        if (loginResult.Failed())
        {
            Debug.LogError(
                $"PlayFab login failed: 0x{loginResult.HResult:X8}"
            );
            return false;
        }

        playerEntity = loginResult.Result;

        Debug.Log("Player logged into PlayFab successfully.");

        if (playerEntity.LoginResult.HasValue)
        {
            string playFabId = playerEntity.LoginResult.Value.PlayFabId;
            bool newlyCreated = playerEntity.LoginResult.Value.NewlyCreated;

            Debug.Log($"PlayFab ID: {playFabId}");
            Debug.Log(
                newlyCreated
                    ? "New PlayFab account created."
                    : "Existing PlayFab account logged in."
            );
        }

        return true;
    }

    private async void OnDestroy()
    {
        if (playerEntity != null)
        {
            playerEntity.Dispose();
            playerEntity = null;
        }

        if (serviceConfig != null)
        {
            serviceConfig.Dispose();
            serviceConfig = null;
        }

        if (playFabServicesInitialized)
        {
            await PFServices.UninitializeAsync();
            playFabServicesInitialized = false;
        }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || MICROSOFT_GDK_SUPPORT
        if (xGameRuntimeInitialized)
        {
            PlayFab.XGameRuntime.Uninitialize();
            xGameRuntimeInitialized = false;
        }
#endif
    }
}