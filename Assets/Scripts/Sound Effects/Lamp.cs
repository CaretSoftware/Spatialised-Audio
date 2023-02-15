using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private AudioSource source;
    public GameObject pointLight;
    public float minPitch;

    // Start is called before the first frame update
    void Start()
    {
        if (pointLight.activeSelf){

            source = GetComponent<AudioSource>();
            source.loop = true;
            float minVol = 0.2f;
            float maxVol = 0.5f;
            source.volume = Random.Range(minVol, maxVol);
            source.time = Random.Range(0, source.clip.length);
            source.pitch = Random.Range(minPitch, 1);

            source.Play();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
