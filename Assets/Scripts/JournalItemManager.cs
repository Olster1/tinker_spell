using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalItemManager : MonoBehaviour
{
	public Text titleText;
	public Text synopsisText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetName(string t) {
    	titleText.text = t;
    }
    public void SetDescription(string s) {
    	synopsisText.text = s;
    }
    public void SetCompletionState(bool c) {
    	if(c) {

		} else {
			
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
