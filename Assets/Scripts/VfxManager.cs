using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    public static VfxManager vfxManager;
   private void Awake()
    {
        if(vfxManager == null)
            vfxManager = this;
        
        else
            Destroy(this);
        
    }

    public void PlayerVfx(GameObject effectObj , Vector3 effectPos)
    {
        GameObject VfxObj = Instantiate(effectObj, effectPos,Quaternion.identity);
        ParticleSystem[] particleSystems = VfxObj.GetComponentsInChildren<ParticleSystem>();

        float maxParticleLength = 0f;

        foreach(ParticleSystem eachParticleSys in particleSystems)
        {
            float currentKnownMaxLength = eachParticleSys.main.duration + eachParticleSys.main.startLifetime.constantMax;
            if(currentKnownMaxLength > maxParticleLength)
                maxParticleLength = currentKnownMaxLength;
        }

        Destroy(VfxObj , maxParticleLength);
    }
}
