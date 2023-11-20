using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        SetShootEffectColorGradient(shootEffect.colorGradient);

        SetShootEffectParticalStartValues(shootEffect.duration, shootEffect.startParticalSize, shootEffect.startParticalSpeed, 
            shootEffect.startLifeTime, shootEffect.effectGravity, shootEffect.maxParticalNumber);

        SetShootEffectParticalEmision(shootEffect.emissionRate, shootEffect.burstParticalNumber);

        SetEmmitterRotation(aimAngle);

        SetShootEffectParticalSprite(shootEffect.sprite);

        SetShootEffectVelocityOverLifetime(shootEffect.velocityOverLifeTimeMin, shootEffect.velocityOverLifeTimeMax);

    }

    private void SetShootEffectVelocityOverLifetime(Vector3 velocityOverLifeTimeMin, Vector3 velocityOverLifeTimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

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

    private void SetShootEffectParticalSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetEmmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0,0,aimAngle);
    }

    private void SetShootEffectParticalEmision(int emissionRate, int burstParticalNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticalNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;


    }

    private void SetShootEffectParticalStartValues(float duration, float startParticalSize, float startParticalSpeed, float startLifeTime, float effectGravity, int maxParticalNumber)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;

        mainModule.duration = duration;

        mainModule.startSize = startParticalSize;

        mainModule.startSpeed = startParticalSpeed;

        mainModule.startLifetime = startLifeTime;

        mainModule.gravityModifier = effectGravity;

        mainModule.maxParticles = maxParticalNumber;

    }

    private void SetShootEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }
}
