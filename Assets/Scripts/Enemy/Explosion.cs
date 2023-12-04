using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem explosionParticle;

    /// <summary>
    /// Play particle
    /// </summary>
    public void ExplosionPlay()
    {
        explosionParticle.Play();
    }
}
