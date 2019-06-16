using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateQuote : MonoBehaviour
{
	public Animator quoteAnimator;
    public AudioSource soundSource;
    public GameObject uiImage;
    public string[] dialog;
    public TextWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && Input.GetButtonDown("Fire1")) {
        	quoteAnimator.SetTrigger("OffscreenIn");
            writer.stringArray = dialog;
            // soundSource.Play();
            // uiImage.SetActive(false);

        }
    }


    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
        	// quoteAnimator.SetTrigger("OffscreenOut");
         //    soundSource.Stop();
         //    uiImage.SetActive(true);
        }
    }
}
