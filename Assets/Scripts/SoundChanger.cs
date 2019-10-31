using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundChanger : MonoBehaviour
{
	private SoundMixer mixer;
	public SoundMixer.MusicId idToSet;
  public bool onlyOnce;
  private bool done;
    // Start is called before the first frame update
    void Awake()
    {
        mixer = Camera.main.GetComponent<SoundMixer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other) {
      if((onlyOnce && !done) || !onlyOnce)
   		if (other.gameObject.name == "Player") {
   			mixer.SetSound(idToSet);
        done = true;
   		}
   	}
}
