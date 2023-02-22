using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOpenClose : MonoBehaviour
{
    private Animator myAnimator;
    private Animator additionalAnimator;
    public bool objectOpen;
    public bool objectOpenAdditional;
    public GameObject animateAdditional;
    private bool hasAdditional = false;
    float myNormalizedTime;
    public AudioSource audioSource;
    [SerializeField] private AudioClip[] openSounds;
    [SerializeField] private AudioClip[] closeSounds;
    [SerializeField] private bool hasPlayedOpenSound = false;
    [SerializeField] private bool hasPlayedCloseSound = false;

    // Open or close animator state in start depending on selection.
    // Additional object with animator. For example another door when double doors. 
    void Start()
    {
        audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        audioSource.pitch = 2f;
        // If there is no animator in the gameobject itself, get the parent animator.
        myAnimator = GetComponent<Animator>();
        if (myAnimator == null)
        {
            myAnimator = GetComponentInParent<Animator>();
        }
        

        if (objectOpen == true)
        {
            myAnimator.Play("Open", 0, 1.5f);
            
        }
        if (animateAdditional != null)
            if (animateAdditional.GetComponent<SimpleOpenClose>())
            {
                additionalAnimator = animateAdditional.GetComponent<Animator>();
                hasAdditional = true;
                objectOpenAdditional = animateAdditional.GetComponent<SimpleOpenClose>().objectOpen;
            }
        else
            {
                hasAdditional = false;
            }
    }

    private void PlayOpenDoorSound()
    {
        if (!audioSource.isPlaying)
        {
            int i = Random.Range(0, openSounds.Length);

            if (hasPlayedOpenSound == false)
            {
                audioSource.clip = openSounds[i];
                audioSource.PlayOneShot(audioSource.clip);
                hasPlayedOpenSound = true;
            }
            else if (hasPlayedOpenSound == true)
            {
                if (i == 0)
                {
                    audioSource.clip = openSounds[1];
                    audioSource.PlayOneShot(audioSource.clip);
                }
                else
                {
                    audioSource.clip = openSounds[0];
                    audioSource.PlayOneShot(audioSource.clip);
                }
            }
        }
    }

    private void PlayCloseDoorSound()
    {
        if (!audioSource.isPlaying)
        {
            int y = Random.Range(0, closeSounds.Length);

            if (hasPlayedCloseSound == false && objectOpen == true)
            {
                audioSource.clip = closeSounds[y];
                audioSource.PlayOneShot(audioSource.clip);
                hasPlayedCloseSound = true;
            }
            else if (hasPlayedCloseSound == true && objectOpen == true)
            {
                if (y == 0)
                {
                    audioSource.clip = closeSounds[1];
                    audioSource.PlayOneShot(audioSource.clip);
                }
                else
                {
                    audioSource.clip = closeSounds[0];
                    audioSource.PlayOneShot(audioSource.clip);
                }
            }
        }
    }

    // Player clicks object. Method called from SimplePlayerUse script.

    void ObjectClicked()
    {

        myNormalizedTime = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (hasAdditional == false)
        {
            if (myNormalizedTime >= 1.0)
            {
                if (objectOpen == true)
                {
                    myAnimator.Play("Close", 0, 0.0f);
                    objectOpen = false;
                    PlayCloseDoorSound();
                }

                else
                {
                    myAnimator.Play("Open", 0, 0.0f);
                    objectOpen = true;
                    PlayOpenDoorSound();
                }
            }
        }

        if (hasAdditional == true && myNormalizedTime >= 1.0)
        {
            if (objectOpen == true)
            {
                myAnimator.Play("Close", 0, 0.0f);
                objectOpen = false;
                PlayCloseDoorSound();
                animateAdditional.GetComponent<SimpleOpenClose>().objectOpenAdditional = false;

                if (objectOpenAdditional == true)
                {
                    additionalAnimator.Play("Close", 0, 0.0f);
                    objectOpenAdditional = false;
                    PlayCloseDoorSound();
                    animateAdditional.GetComponent<SimpleOpenClose>().objectOpen = false;
                }

            }

            else
            {
                myAnimator.Play("Open", 0, 0.0f);
                objectOpen = true;
                PlayOpenDoorSound();
                animateAdditional.GetComponent<SimpleOpenClose>().objectOpenAdditional = true;

                if (objectOpenAdditional == false)
                {
                    additionalAnimator.Play("Open", 0, 0.0f);
                    objectOpenAdditional = true;
                    PlayOpenDoorSound();
                    animateAdditional.GetComponent<SimpleOpenClose>().objectOpen = true;

                }

            }

        }

    }

}
