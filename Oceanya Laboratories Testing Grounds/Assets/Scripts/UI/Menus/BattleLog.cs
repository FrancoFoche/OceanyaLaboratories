using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour
{
    public int maxMessages;

    public GameObject chatPanel, textObject;
    public InputField chatBox;

    public Color playerMessage, battleStatus, battleEffect; 

    [SerializeField]
    List<Message> messageList = new List<Message>();

    private void Update()
    {
        if(chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("GameMaster: " + chatBox.text, Message.MessageType.PlayerMessage);
                chatBox.text = "";
            }

        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }
    }

    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if(messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        Debug.Log(text);
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);
        
        if(messageType == Message.MessageType.BattleStatus)
        {
            newMessage.textObject.alignment = TextAnchor.MiddleCenter;
        }

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.MessageType messageType)
    {
        Color color = battleEffect;

        switch (messageType)
        {
            case Message.MessageType.PlayerMessage:
                color = playerMessage;
                break;

            case Message.MessageType.BattleStatus:
                color = battleStatus;
                break;

            case Message.MessageType.BattleEffect:
                color = battleEffect;
                break;
        }

        return color;
    }

    public void LogBattleStatus(string text)
    {
        SendMessageToChat("*" + text + "*", Message.MessageType.BattleStatus);
    }

    public void LogBattleEffect(string text)
    {
        SendMessageToChat("BattleLog: " + text, Message.MessageType.BattleEffect);
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        PlayerMessage,
        BattleStatus,
        BattleEffect
    }
}