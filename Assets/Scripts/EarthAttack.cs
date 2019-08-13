using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthAttack : MonoBehaviour
{
	private SpriteRenderer sp;

    public AudioClip earthAudio;
    public GameObject soundObj;
    // Start is called before the first frame update
    void Start()
    {
        sp = gameObject.GetComponent<SpriteRenderer>();

        GameObject sound = Instantiate(soundObj, transform.position,  Quaternion.identity);
        sound.GetComponent<DestroyOnSoundEnd>().AddClip(earthAudio);
    }

    public void rePosBoxes() {
        // Vector2 tempOffset = box1.offset;
        // tempOffset.x *= -1;
        // box1.offset = tempOffset;

        // tempOffset = box2.offset;
        // tempOffset.x *= -1;
        // box2.offset = tempOffset;

        // tempOffset = box3.offset;
        // tempOffset.x *= -1;
        // box3.offset = tempOffset;

        // tempOffset = box4.offset;
        // tempOffset.x *= -1;
        // box4.offset = tempOffset;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateBox1() {
        //box1.enabled = true;
    }

    public void activateBox2() {
        // box2.enabled = true;
    }

    public void activateBox3() {
        // box3.enabled = true;
    }
    public void activateBox4() {
        // box4.enabled = true;
    }

    public void alpha75() {
    	sp.color = new Color(1, 1, 1, 0.75f);
    }
    public void alpha50() {
    	sp.color = new Color(1, 1, 1, 0.50f);
    }

    public void alpha25() {
    	sp.color = new Color(1, 1, 1, 0.25f);
    }

    public void alpha0() {
    	sp.color = new Color(1, 1, 1, 0);
    }
}
