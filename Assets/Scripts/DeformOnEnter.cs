using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformOnEnter : MonoBehaviour
{
	public Animator anim;
	public AudioSource rustleSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == "Player" && !other.isTrigger) {
            anim.SetTrigger("rustle");
            rustleSound.Play();

        }
    }
}
