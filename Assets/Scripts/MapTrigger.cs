using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
	private MiniMap map;
	public MiniMapType type;
  private BoxCollider2D box;
    // Start is called before the first frame update
    void Start()
    {
    	map = Camera.main.GetComponent<MiniMap>();
      box = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerStay2D(Collider2D other) {
   		if (other.gameObject.name == "Player") {
   			map.SetCurrentMap(type, box, other.gameObject.transform);
   		}
   	}
}
