using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydrogodActivate : MonoBehaviour
{
	public GameObject[] dogs;
	private bool set;
    // Start is called before the first frame update
    void Start()
    {
        set = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
    	if(other.gameObject.name == "Player" && !set) {
    		for(int i = 0; i < dogs.Length; ++i) {
    			dogs[i].SetActive(true);
    			Animator a = dogs[i].GetComponent<Animator>();
    			a.Play("run", -1, Random.Range(0, 1.0f));
    		}
    		set = true;
    	}
    }
}
