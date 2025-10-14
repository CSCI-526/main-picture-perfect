using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthBarUI : MonoBehaviour
{
    public GameObject heartPrefab;
    public Sprite heartSprite;

    private List<Image> hearts = new List<Image>();

    public void InitializeHearts(int maxHits)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        hearts.Clear();

        for (int i = 0; i < maxHits; i++)
        {
            var heart = Instantiate(heartPrefab, transform);
            var img = heart.GetComponent<Image>();
            img.sprite = heartSprite;
            hearts.Add(img);
        }
    }

    public void UpdateHearts(int currentHits, int maxHits)
    {
        int health = Mathf.Clamp(maxHits - currentHits, 0, maxHits);

        for (int i = 0; i < hearts.Count; i++)
            hearts[i].enabled = i < health;
    }
}
