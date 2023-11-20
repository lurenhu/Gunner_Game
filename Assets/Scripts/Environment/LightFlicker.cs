using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    Light2D light2D;
    [SerializeField] float lightIntensityMin;
    [SerializeField] float lightIntensityMax;
    [SerializeField] float lightFlickerTimeMin;
    [SerializeField] float lightFlickerTimeMax;
    float lightFlickerTime;

    private void Awake()
    {
        light2D = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        lightFlickerTime = Random.Range(lightFlickerTimeMin,lightFlickerTimeMax);
    }

    private void Update()
    {
        if (light2D == null)
        {
            return;
        }

        lightFlickerTime -= Time.deltaTime;

        if (lightFlickerTime < 0)
        {
            lightFlickerTime = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);

            RandomiseLightIntensity();
        }
    }

    private void RandomiseLightIntensity()
    {
        light2D.intensity = Random.Range(lightIntensityMin,lightIntensityMax);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtility.ValidateChackPositveRange(this, nameof(lightFlickerTimeMin), lightFlickerTimeMin, nameof(lightFlickerTimeMax), lightFlickerTimeMax, false);
        HelperUtility.ValidateChackPositveRange(this, nameof(lightIntensityMin), lightIntensityMin, nameof(lightIntensityMax), lightIntensityMax, false);

    }


#endif
    #endregion


}
