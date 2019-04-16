using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySystem : MonoBehaviour
{
    public GameObject text = null;
    public string interrupt = "";
    List<string> story = null;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        story = new List<string>();

        text = transform.GetChild(0).gameObject;
        ShowMessage("Finally...");

        story.Add("After 17 years of research I've found it");
        story.Add("I have to be careful");
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 8f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;

            if(interrupt != "")
            {
                ShowMessage(interrupt);
                interrupt = "";
            }
            else if(story.Count > 0)
            {
                ShowMessage(story[0]);
                story.RemoveAt(0);
            }
        }
    }

    void ShowMessage(string message)
    {
        text.GetComponent<TMPro.TextMeshProUGUI>().text = message;
        text.GetComponent<Animator>().SetTrigger("Run");
    }
}
