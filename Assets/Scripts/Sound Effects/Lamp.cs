using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Lamp : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] public GameObject lightGameObject;
    private static Random random = new Random(1234);
    [SerializeField] private bool randomVolume = true;
    [SerializeField, Range(0f, 1f)] private float nonRandomVolume = .5f;
    
    void Awake()
    {
        if (lightGameObject.activeSelf) {
            source = GetComponent<AudioSource>();
            source.loop = true;
            float minVol = 0.2f;
            float maxVol = 0.5f;
            
            // I used nextFloat to have the randomness the same between sessions
            float randVol = random.NextFloat() % (maxVol - minVol); 
            float volume = minVol + randVol;
            
            source.volume = volume;

            // some light sources i wanted to control the value manually
            if (!randomVolume)
                source.volume = nonRandomVolume;
            
            //source.time = Random.Range(0, source.clip.length);
            //source.pitch = Random.Range(minPitch, 1);

            source.Play();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
