using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateQuote : MonoBehaviour
{
	public Animator quoteAnimator;
    public AudioSource soundSource;
    public GameObject uiImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
        	quoteAnimator.SetTrigger("OffscreenIn");
            soundSource.Play();
            uiImage.SetActive(false);

        }
    }


    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
        	quoteAnimator.SetTrigger("OffscreenOut");
            soundSource.Stop();
            uiImage.SetActive(true);
        }
    }
}
