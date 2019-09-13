using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalagmite : MonoBehaviour
{
	public GameObject stalagmiteObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
      GameObject gm = other.gameObject;
      if(gm.name == "Player" || gm.name == "Spell") {
      	Instantiate(stalagmiteObj, transform);
      }
    }
}
