using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;

    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1, 1);
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = healthBar.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;

        while(currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            healthBar.transform.localScale = new Vector3(currentScale, 1, 1);
            yield return null;
        }

        healthBar.transform.localScale = new Vector3(normalizedValue, 1, 1);
    }
}
