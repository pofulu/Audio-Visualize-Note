using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Osc), typeof(UDPPacketIO))]
public class OSCManager : MonoBehaviour
{
    [System.Serializable]
    public class OscEvent : UnityEvent<string> { }

    [System.Serializable]
    public class MessageStruct
    {
        public string address;
        public List<string> message;
        public OscEvent onReceivedNewMessage;
    }

    private Osc oscHandler;
    private OscMessage lastMessage;
    private OscMessage oscMessage;

    [Header("UDP Setting")]
    [ContextMenuItem("Set IP", "SetIP")]
    public string remoteIp = "127.0.0.1";
    [ContextMenuItem("Set Send Port", "SetSendPort")]
    public int sendToPort = 8888;
    [ContextMenuItem("Set Listen Port", "SetListenPort")]
    public int listenerPort = 12000;

    [Header("Receiver")]
    public OscEvent RecieverPrinter;
    public List<MessageStruct> addressEvent;

    [Header("Sender")]
    public OscEvent SenderPrinter;
    public int pulseCount = 10;
    [ContextMenuItem("Send", "Send")]
    [ContextMenuItem("Send Pulse", "SendInPulse")]
    [Tooltip("Example:/value 0 \nRight click this filed to invoke Send method.")]
    public string message;
    public string address { get; set; }

    private void Start()
    {
        Initial();
    }

    private void Update()
    {
        if (oscMessage != lastMessage)
        {
            UpdateAddressEvent(oscMessage);
            RecieverPrinter.Invoke(string.Format("{0} {1}", oscMessage.Address, ArrayListToString(oscMessage.Values)));
            lastMessage = oscMessage;
        }
    }

    #region Receiver
    public void AllMessageHandler(OscMessage message)
    {
        // 不能把UnityEvent放在這個Handler裡面Invoke，因為UnityEvent必須在MainThread裡執行，所以要把所有UnityEvent拉到Update裡面
        oscMessage = message;
    }
    private void UpdateAddressEvent(OscMessage oscMessage)
    {
        foreach (var receiver in addressEvent)
        {
            if (oscMessage.Address == receiver.address)
            {
                string msg = "";

                if (oscMessage.Values.Count > 1)
                {
                    for (int i = 0; i < oscMessage.Values.Count; i++)
                    {
                        AsQueue(receiver.message, oscMessage.Values.Count, oscMessage.Values[i].ToString());
                        msg += oscMessage.Values[i].ToString();

                        if (i != oscMessage.Values.Count - 1)
                        {
                            msg += ",";
                        }
                    }
                }
                else
                {
                    msg = oscMessage.Values[0].ToString();
                    AsQueue(receiver.message, oscMessage.Values.Count, oscMessage.Values[0].ToString());
                }

                receiver.onReceivedNewMessage.Invoke(msg);
            }
        }
    }
    #endregion

    #region Sender
    public void Send()
    {
        SendOscMessage(message);
    }
    public void SendInPulse()
    {
        SendOscMessageInPulse(message);
    }

    public void SendOscMessage(string content)
    {
        string text = string.IsNullOrEmpty(address) ? content : address + " " + content;

        OscMessage msg = Osc.StringToOscMessage(text);
        oscHandler.Send(msg);

        SenderPrinter.Invoke(text);
    }
    public void SendOscMessageInPulse(string content)
    {
        StartCoroutine(SendPulse(content));
    }

    private IEnumerator SendPulse(string content)
    {
        int count = 0;
        string pulseSuffix = "1";

        string text = string.IsNullOrEmpty(address) ? content : address + " " + content;

        while (count < pulseCount)
        {
            if (count >= pulseCount / 2)
            {
                pulseSuffix = "0";
            }

            string msg = text + " " + pulseSuffix;

            SendOscMessage(msg);
            SenderPrinter.Invoke(msg);

            count++;
            yield return null;
        }
    }
    #endregion

    #region SettingAPI
    public void SetIP(string ip)
    {
        remoteIp = ip;
        Cancel();
        Initial();
    }
    public void SetSendPort(string port)
    {
        sendToPort = int.Parse(port);
        Cancel();
        Initial();
    }
    public void SetListenPort(string port)
    {
        listenerPort = int.Parse(port);
        Cancel();
        Initial();
    }

    public void Initial()
    {
        if (oscHandler != null) return;

        UDPPacketIO udp = GetComponent<UDPPacketIO>();
        udp.init(remoteIp, sendToPort, listenerPort);

        oscHandler = GetComponent<Osc>();
        oscHandler.init(udp);

        oscHandler.SetAllMessageHandler(AllMessageHandler);
    }
    public void Cancel()
    {
        if (oscHandler == null) return;

        oscHandler.Cancel();
        oscHandler = null;
    }
    #endregion

    ~OSCManager()
    {
        if (oscHandler != null)
        {
            oscHandler.Cancel();
        }

        // speed up finalization
        oscHandler = null;
        System.GC.Collect();
    }

    void OnDisable()
    {
        Debug.Log("closing OSC UDP socket in OnDisable");
        oscHandler.Cancel();
        oscHandler = null;
    }

    private List<T> AsQueue<T>(List<T> list, int size, T add)
    {
        if (list.Count < size)
        {
            list.Add(add);
        }
        else
        {
            list.Remove(list[0]);
            list.Add(add);
        }
        return list;
    }
    private string ArrayListToString(ArrayList arrayList)
    {
        string text = "";

        for (int i = 0; i < arrayList.Count; i++)
        {
            text += arrayList[i].ToString();

            if (i != arrayList.Count - 1)
            {
                text += ",";
            }
        }
        return text;
    }
}