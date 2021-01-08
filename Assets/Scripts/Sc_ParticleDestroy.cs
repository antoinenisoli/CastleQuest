using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ParticleDestroy : MonoBehaviour
{
    ParticleSystem fx => GetComponent<ParticleSystem>();

    private void Awake()
    {
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        fx.Play();
        yield return new WaitForSeconds(fx.main.duration + fx.main.startLifetimeMultiplier);
        Destroy(gameObject);
    }
}
