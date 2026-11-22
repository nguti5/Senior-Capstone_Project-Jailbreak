using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBar;

   //sets health bar to full.
   
    public void SetHP(float normalizedValue)
    {
        healthBar.transform.localScale = new Vector3(normalizedValue, 1, 1);
    }

    //changes size of healthbar when health value changes (healing or damage).
    public IEnumerator SetSmoothHP(float target)
    {
        float current = healthBar.transform.localScale.x;
        float speed = Mathf.Abs(target - current);

        while(!Mathf.Approximately(current, target))
        {
            current = Mathf.MoveTowards(current, target, speed * Time.deltaTime);
            healthBar.transform.localScale = new Vector3(current, 1, 1);
            yield return null;
        }

        healthBar.transform.localScale = new Vector3(target, 1, 1);
    }
}
