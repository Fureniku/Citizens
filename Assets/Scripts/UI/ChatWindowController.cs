using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindowController : MonoBehaviour {

    //first entry = lowest/most recent message
    [SerializeField] private Text[] messageStamp = new Text[8];
    [SerializeField] private Text[] message = new Text[8];

    private List<ChatMessage> chatMessages = new List<ChatMessage>();

    [SerializeField] private int fadeStartTime = 180;
    [SerializeField] private int fadeTime = 60;
    [SerializeField] private float boxTransparancy = 0.6f;

    private bool fade = true;
    private int fadeTimer = 0;

    public void AddNewMessage(string sender, string msg) {
        string timeStamp = "[" + System.DateTime.Now.ToString("HH:mm:ss") + "] " + sender + ":";

        chatMessages.Insert(0, new ChatMessage(timeStamp, msg));

        if (chatMessages.Count > 10) {
            chatMessages.RemoveAt(10);
        }
    }

    public void ShowChatWindow() {
        Image img = GetComponent<Image>();
        if (!img.enabled) {
            img.enabled = true;
        }
        
        fadeTimer = 0;
        img.color = SetAlpha(img.color, 0.6f);
        
        for (int i = 0; i < chatMessages.Count; i++) {
            chatMessages[i].ResetFade();
        }
    }

    private void UpdateChats() {
        for (int i = 0; i < message.Length; i++) {
            if (chatMessages.Count > i) {
                message[i].text = chatMessages[i].GetMessage();
                messageStamp[i].text = chatMessages[i].GetStamp();

                message[i].color = SetAlpha(message[i].color, chatMessages[i].GetAlpha());
                messageStamp[i].color = SetAlpha(messageStamp[i].color, chatMessages[i].GetAlpha());
                chatMessages[i].Update();
            }
        }
    }

    public void FixedUpdate() {
        UpdateChats();
        
        Image img = GetComponent<Image>();
        if (img.enabled) {
            if (fadeTimer > fadeStartTime && fadeTimer < fadeTime + fadeStartTime) {
                int deltaTime = fadeStartTime + fadeTime - fadeTimer;
                float percent = deltaTime / (float) fadeTime;
                
                img.color = SetAlpha(img.color, boxTransparancy*percent);

                if (deltaTime == 1) {
                    img.enabled = false;
                }
            }

            if (fade) fadeTimer++;
        }
    }

    public void DisableFade() {
        fade = false;
        fadeTimer = 0;
    }

    public void EnableFade() {
        fade = true;
    }

    private Color SetAlpha(Color colour, float alpha) {
        colour.a = alpha;
        return colour;
    }
}
