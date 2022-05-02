using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindowController : MonoBehaviour {

    //first entry = lowest/most recent message
    [SerializeField] private Text[] messageStamp = new Text[8];
    [SerializeField] private Text[] message = new Text[8];

    public void ShiftUpMessages() {
        for (int i = message.Length - 2; i >= 0; i--) {
            message[i + 1].text = message[i].text;
            messageStamp[i + 1].text = messageStamp[i].text;
        }
    }

    public void AddNewMessage(string sender, string msg) {
        if (!GetComponent<Image>().enabled) {
            GetComponent<Image>().enabled = true;
        }
        ShiftUpMessages();
        message[0].text = msg;
        string timeStamp = "[" + System.DateTime.Now.ToString("HH:mm:ss") + "] ";
        messageStamp[0].text = timeStamp + sender + ":";
    }
}
