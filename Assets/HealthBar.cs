using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We need this to do UI stuff!

public class HealthBar : MonoBehaviour
{
    float MaxHealth = 100.0f;
    [SerializeField] float currentHealth = 100.0f;

    [SerializeField] RectTransform greenHealthBar;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LoseHealth(10);
        }

        float normalisedHealth = currentHealth / MaxHealth;

        Vector2 size = greenHealthBar.sizeDelta; //We need to make a copy of this first
        size.x = Mathf.Lerp(0, 500, normalisedHealth);
        greenHealthBar.sizeDelta = size;
    }
    public void LoseHealth(float healthLoss)
    {
        currentHealth -= healthLoss;
    }
}
