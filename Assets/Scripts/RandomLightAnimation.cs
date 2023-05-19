using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightAnimation : MonoBehaviour
{
    [SerializeField] private Light fireplaceLight;
    [SerializeField] private float randomAnimationTime;
    [SerializeField] private float randomIntensityShift;
    [SerializeField] private float animationTime;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float minimiumLightIntensity;

    private float timer;
    private float intensity;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            intensity = Random.Range(minimiumLightIntensity, randomIntensityShift);

            timer = animationTime + Random.Range(-randomAnimationTime, randomAnimationTime);
        }

        fireplaceLight.intensity = Mathf.Lerp(fireplaceLight.intensity, intensity, Time.deltaTime * animationSpeed);
    }
}
