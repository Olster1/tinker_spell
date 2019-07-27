using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [System.Serializable]
public class JournalItem {
	public string id;
	public string title;
	public string synopsis;
	public bool completed;
	public JournalItem(string title, string synopsis, bool completed) {
    	this.title = title;
    	this.synopsis = synopsis;
    	this.completed = completed;
	}
}

public class Journal : MonoBehaviour
{
	public List<JournalItem> items;
	public Animator journalAnimator;
    // Start is called before the first frame update
    void Start()
    {
        items = new List<JournalItem>();
    }

    public bool hasJournalItem(int id) {
    	return false;
    }

    public void AddJournalItem(string title, string synopsis) {
    	items.Add(new JournalItem(title, synopsis, false));
		journalAnimator.SetTrigger("go");
    }

    public void AddJournalItem(JournalItem item) {
    	items.Add(item);
    	journalAnimator.SetTrigger("go");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
