
using UnityEngine;

public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO hitEffect)
    {
        SetHitEffectColorGradient(hitEffect.colorGradient);

        SetHitEffectParticalStartValues(hitEffect.duration, hitEffect.startParticalSize, hitEffect.startParticalSpeed,
            hitEffect.startLifeTime, hitEffect.effectGravity, hitEffect.maxParticalNumber);

        SetHitEffectParticalEmision(hitEffect.emissionRate, hitEffect.burstParticalNumber);

        SetHitEffectParticalSprite(hitEffect.sprite); 

        SetHitEffectVelocityOverLifetime(hitEffect.velocityOverLifeTimeMin, hitEffect.velocityOverLifeTimeMax);

    }

    private void SetHitEffectVelocityOverLifetime(Vector3 velocityOverLifeTimeMin, Vector3 velocityOverLifeTimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifeTimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifeTimeMax.x;
        velocityOverLifetimeModule.x = minMaxCurveX;

        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifeTimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifeTimeMax.y;
        velocityOverLifetimeModule.y = minMaxCurveY;

        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = velocityOverLifeTimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifeTimeMax.z;
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }

    private void SetHitEffectParticalSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetHitEffectParticalEmision(int emissionRate, int burstParticalNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticalNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;


    }

    private void SetHitEffectParticalStartValues(float duration, float startParticalSize, float startParticalSpeed, float startLifeTime, float effectGravity, int maxParticalNumber)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticleSystem.main;

        mainModule.duration = duration;

        mainModule.startSize = startParticalSize;

        mainModule.startSpeed = startParticalSpeed;

        mainModule.startLifetime = startLifeTime;

        mainModule.gravityModifier = effectGravity;

        mainModule.maxParticles = maxParticalNumber;

    }

    private void SetHitEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }
}
