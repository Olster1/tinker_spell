using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DestroyOnSoundEnd : MonoBehaviour
{
	public AudioSource audiosource;
	public AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        
        // Assert.IsTrue(audiosource.isPlaying);
    }

    public void AddClip(AudioClip clip) {
    	audiosource.clip = clip;
        audiosource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audiosource.isPlaying) {
        	Destroy(gameObject);
        }
    }
}
