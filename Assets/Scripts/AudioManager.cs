using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject audioPrefab;
    public static AudioManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
        Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Play3D(AudioClip clip , Vector3 position)
    {
        GameObject audioObj = Instantiate(audioPrefab , position, Quaternion.identity);
        AudioSource audioSource = audioObj.GetComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.Play();

        Destroy(audioObj , clip.length);

    }
}
