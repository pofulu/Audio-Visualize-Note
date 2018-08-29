using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OSCGUI : MonoBehaviour
{
    public OSCManager manager;
    public InputField IP;
    public InputField sendPort;
    public InputField listenPort;
    public StringStream stream;

    public void Start()
    {
        IP.text = manager.remoteIp;
        sendPort.text = manager.sendToPort.ToString();
        listenPort.text = manager.listenerPort.ToString();

        IP.onEndEdit.AddListener(manager.SetIP);
        sendPort.onEndEdit.AddListener(manager.SetSendPort);
        listenPort.onEndEdit.AddListener(manager.SetListenPort);

        stream.streamText.text = "Send Nothing...";

        manager.SenderPrinter.AddListener(Send);
        manager.RecieverPrinter.AddListener(Receive);
    }

    private void Send(string msg)
    {
        string text = "> " + msg;
        stream.Add(text);
    }

    private void Receive(string msg)
    {
        string text = "< " + msg;
        stream.Add(text);
    }
}
