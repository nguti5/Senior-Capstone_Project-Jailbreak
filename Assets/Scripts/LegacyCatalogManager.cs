
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class LegacyCatalogManager : MonoBehaviour
{
    private const string TitleId = "1A3333";
    private const string CustomPlayerId = "UnityPlayerId_Test3";
    private const string CatalogVersion = "TutorialCatalog";
    private string sessionTicket;
    private HashSet<string> ownedItemIds = new HashSet<string>();

    public bool HasItem(string itemId)
    {
        return ownedItemIds.Contains(itemId);
    }
    public List<string> GetOwnedItemIds()
    {
        return new List<string>(ownedItemIds);
    }
    public bool IsInventoryLoaded { get; private set; } = false;
    [Serializable]
    private class LoginRequest
    {
        public string TitleId;
        public string CustomId;
        public bool CreateAccount;
    }

    [Serializable]
    private class LoginResponse
    {
        public LoginResult data;
    }

    [Serializable]
    private class LoginResult
    {
        public string SessionTicket;
    }

    [Serializable]
    private class CatalogRequest
    {
        public string CatalogVersion;
    }

    [Serializable]
    private class CatalogResponse
    {
        public CatalogResult data;
    }

    [Serializable]
    private class CatalogResult
    {
        public CatalogItem[] Catalog;
    }

    [Serializable]
    private class CatalogItem
    {
        public string ItemId;
        public string DisplayName;
    }
    [Serializable]
    private class InventoryResponse
    {
        public InventoryResult data;
    }

    [Serializable]
    private class InventoryResult
    {
        public InventoryItem[] Inventory;
    }

    [Serializable]
    private class InventoryItem
    {
        public string ItemId;
        public string ItemInstanceId;
        public int RemainingUses;
    }
    [Serializable]
    private class CloudScriptRequest
    {
        public string FunctionName;
        public bool GeneratePlayStreamEvent;
    }

    [Serializable]
    private class CloudScriptResponse
    {
        public CloudScriptResult data;
    }

    [Serializable]
    private class CloudScriptResult
    {
        public bool FunctionResultTooLarge;
        public string Error;
        public TutorialKeyClaimResult FunctionResult;
    }

    [Serializable]
    private class TutorialKeyClaimResult
    {
        public bool success;
        public bool alreadyClaimed;
        public string message;
    }

    private IEnumerator Start()
    {
        var login = new LoginRequest
        {
            TitleId = TitleId,
            CustomId = CustomPlayerId,
            CreateAccount = true
        };

        string loginUrl =
            $"https://{TitleId}.playfabapi.com/Client/LoginWithCustomID";

        using (UnityWebRequest request =
            CreatePostRequest(loginUrl, JsonUtility.ToJson(login)))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Legacy login failed: " +
                    request.downloadHandler.text
                );
                yield break;
            }

            LoginResponse response =
                JsonUtility.FromJson<LoginResponse>(
                    request.downloadHandler.text
                );

            sessionTicket = response?.data?.SessionTicket;

            if (string.IsNullOrEmpty(sessionTicket))
            {
                Debug.LogError("Legacy login returned no session ticket.");
                yield break;
            }

            Debug.Log("Legacy PlayFab login successful!");

            yield return StartCoroutine(LoadCatalog(sessionTicket));
            yield return StartCoroutine(LoadInventory(sessionTicket));
            yield return StartCoroutine(TestCloudScript(sessionTicket));
        }
    }

    private IEnumerator LoadCatalog(string sessionTicket)
    {
        string url =
            $"https://{TitleId}.playfabapi.com/Client/GetCatalogItems";

        var body = new CatalogRequest
        {
            CatalogVersion = CatalogVersion
        };

        using (UnityWebRequest request =
            CreatePostRequest(url, JsonUtility.ToJson(body)))
        {
            request.SetRequestHeader("X-Authorization", sessionTicket);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Catalog request failed: " +
                    request.downloadHandler.text
                );
                yield break;
            }

            CatalogResponse response =
                JsonUtility.FromJson<CatalogResponse>(
                    request.downloadHandler.text
                );

            CatalogItem[] items = response?.data?.Catalog;

            if (items == null)
            {
                Debug.LogError("Catalog response contained no item list.");
                yield break;
            }

            Debug.Log($"TutorialCatalog loaded: {items.Length} items.");

            foreach (CatalogItem item in items)
            {
                Debug.Log($"{item.ItemId} - {item.DisplayName}");
            }
        }
    }

    private IEnumerator LoadInventory(string sessionTicket)
    {
        IsInventoryLoaded = false;
        string url =
            $"https://{TitleId}.playfabapi.com/Client/GetUserInventory";

        using (UnityWebRequest request =
            CreatePostRequest(url, "{}"))
        {
            request.SetRequestHeader("X-Authorization", sessionTicket);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Inventory request failed: " +
                    request.downloadHandler.text
                );
                yield break;
            }

            InventoryResponse response =
                JsonUtility.FromJson<InventoryResponse>(
                    request.downloadHandler.text
                );
            ownedItemIds.Clear();

            InventoryItem[] items = response?.data?.Inventory;

            if (items == null)
            {
                Debug.LogError("Inventory response contained no item list.");
                yield break;
            }

            Debug.Log($"Player inventory loaded: {items.Length} items.");

            foreach (InventoryItem item in items)
            {
                ownedItemIds.Add(item.ItemId);

                Debug.Log(
                    $"Owned item: {item.ItemId} | " +
                    $"Instance: {item.ItemInstanceId} | " +
                    $"Remaining uses: {item.RemainingUses}"
                );
            }
            IsInventoryLoaded = true;
            Debug.Log("PlayFab inventory is ready!");
        }
    }

    private IEnumerator TestCloudScript(string sessionTicket)
    {
        string url =
            $"https://{TitleId}.playfabapi.com/Client/ExecuteCloudScript";

        var body = new CloudScriptRequest
        {
            FunctionName = "testTutorialInventory",
            GeneratePlayStreamEvent = false
        };

        using (UnityWebRequest request =
            CreatePostRequest(url, JsonUtility.ToJson(body)))
        {
            request.SetRequestHeader("X-Authorization", sessionTicket);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "CloudScript request failed: " +
                    request.downloadHandler.text
                );
                yield break;
            }

            Debug.Log(
                "CloudScript response: " +
                request.downloadHandler.text
            );
        }
    }

    private UnityWebRequest CreatePostRequest(string url, string json)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }


    public void ClaimTutorialKey(Action<bool> onComplete)
    {
        if (string.IsNullOrEmpty(sessionTicket))
        {
            Debug.LogWarning("PlayFab login is not ready yet.");
            onComplete?.Invoke(false);
            return;
        }

        StartCoroutine(ClaimTutorialKeyRequest(onComplete));
    }


    private IEnumerator ClaimTutorialKeyRequest(Action<bool> onComplete)
    {
        string url =
            $"https://{TitleId}.playfabapi.com/Client/ExecuteCloudScript";

        var body = new CloudScriptRequest
        {
            FunctionName = "claimTutorialKey",
            GeneratePlayStreamEvent = false
        };

        using (UnityWebRequest request =
            CreatePostRequest(url, JsonUtility.ToJson(body)))
        {
            request.SetRequestHeader("X-Authorization", sessionTicket);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    "Tutorial key claim request failed: " +
                    request.downloadHandler.text
                );

                onComplete?.Invoke(false);
                yield break;
            }


            Debug.Log(
                "Tutorial key claim response: " +
                request.downloadHandler.text
            );

            CloudScriptResponse response =
                JsonUtility.FromJson<CloudScriptResponse>(
                    request.downloadHandler.text
                );

            if (response?.data?.FunctionResult != null &&
    response.data.FunctionResult.success)
            {
                Debug.Log("PlayFab confirmed the player owns the tutorial key!");

                yield return StartCoroutine(LoadInventory(sessionTicket));

                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError(
                    "PlayFab did not confirm the tutorial key claim."
                );

                onComplete?.Invoke(false);
            }

        }
    }

}
