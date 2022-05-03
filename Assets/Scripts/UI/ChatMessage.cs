using UnityEngine;
using UnityEngine.UI;

public class ChatMessage {
    
    private readonly string messageStamp;
    private readonly string message;

    private float alpha = 1.0f;

    private const int fadeStartTime = 180;
    private const int fadeTime = 60;

    private int fadeTimer = 0;

    public ChatMessage(string stamp, string msg) {
        messageStamp = stamp;
        message = msg;
    }

    public void ResetFade() {
        alpha = 1;
        fadeTimer = 0;
    }

    public string GetStamp() {
        return messageStamp;
    }

    public string GetMessage() {
        return message;
    }

    public float GetAlpha() {
        return alpha;
    }

    public void Update() {
        if (fadeTimer > fadeStartTime && fadeTimer < fadeTime + fadeStartTime) {
            int deltaTime = fadeStartTime + fadeTime - fadeTimer;
            float percent = deltaTime / (float) fadeTime;
            
            alpha = 1*percent;
        }

        fadeTimer++;
    }
}