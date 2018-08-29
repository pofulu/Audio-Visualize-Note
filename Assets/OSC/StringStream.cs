using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringStream : MonoBehaviour
{
    public Text streamText;
    //public TMP_Text TMPStreamText;
    public int lineCount;

    [HideInInspector]
    public List<string> stream;
    [HideInInspector]
    public string display;

    public void Add(string msg)
    {
        stream = AsQueue(stream,lineCount, msg);
        display = "";

        foreach (string text in stream)
        {
            display += text.ToString() + "\n";
        }

        streamText.text = display;
        //TMPStreamText.text = display;
    }

    List<T> AsQueue<T>(List<T> list, int size, T type)
    {
        if (list.Count < size)
        {
            list.Add(type);
        }
        else
        {
            list.Remove(list[0]);
            list.Add(type);
        }
        return list;
    }
}
