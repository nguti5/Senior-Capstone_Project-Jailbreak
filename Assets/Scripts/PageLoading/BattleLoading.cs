using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleLoading : MonoBehaviour
{
    IEnumerator Start(){
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneLoader.NextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (timer < SceneLoader.WaitSeconds || op.progress < 0.9f){
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}