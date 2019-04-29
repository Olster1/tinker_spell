using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallTextScript : MonoBehaviour
{
	public TextWriter textWriter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CancelFontWriting() {
    	textWriter.CancelFontWriting();
    }
    public void ClearFontWriter() {
    	textWriter.ClearFontWriter();
    }
    public void BeginTextWriting() {
    	textWriter.ActivateFontWriter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
