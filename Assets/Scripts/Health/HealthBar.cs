using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Space(10)]
    [Header("GAMEOBJECT REFERENCE")]

    [SerializeField] GameObject healthBar;

    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1, 1);
    }
}
