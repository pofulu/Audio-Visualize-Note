using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SoundFileOpener : MonoBehaviour {

    public InputField pathBox;

    private void Start()
    {
#if UNITY_EDITOR
        string str = UnityEditor.EditorUtility.OpenFilePanel("Open a sound file...", "", "wav, ogg");
        pathBox.text = "file://" + str;
#endif
    }

    public void PlayFile()
    {
        WWW w = new WWW(pathBox.text);
        while (!w.isDone){ } //shh... don't tell anyone
        if (w.error != null)
            print(w.error);
        AudioClip clip = w.GetAudioClip(true,false);
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }
}
