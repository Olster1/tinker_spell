using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerTrigger : MonoBehaviour
{
	public bool follow;
	private CameraFollowPlayer cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform.parent.GetComponent<CameraFollowPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
      GameObject gm = other.gameObject;
      if(gm.name == "Player" || gm.name == "Spell") {
      	if(follow) {
      		cam.FollowEntity();
  		} else {
  			cam.StopFollowingEntity();
  		}
      }
    }
}
