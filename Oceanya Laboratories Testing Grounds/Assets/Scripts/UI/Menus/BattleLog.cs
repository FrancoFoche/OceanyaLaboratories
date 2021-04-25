using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLog : MonoBehaviour
{
    public int maxMessages;

    public GameObject chatPanel, textObject;
    public InputField chatBox;

    public Color playerMessage, battleStatus, battleEffect, allyTurn, enemyTurn, outOfCombatTurn; 

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

        switch (messageType)
        {
            case Message.MessageType.AllyTurn:
            case Message.MessageType.EnemyTurn:
            case Message.MessageType.BattleStatus:
            case Message.MessageType.OutOfCombatTurn:
                newMessage.textObject.alignment = TextAnchor.MiddleCenter;
                break;

            case Message.MessageType.BattleEffect:
            case Message.MessageType.PlayerMessage:
                newMessage.textObject.alignment = TextAnchor.MiddleLeft;
                break;

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

            case Message.MessageType.AllyTurn:
                color = allyTurn;
                break;

            case Message.MessageType.EnemyTurn:
                color = enemyTurn;
                break;

            case Message.MessageType.OutOfCombatTurn:
                color = outOfCombatTurn;
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

    /// <summary>
    /// Sends a message to the log that says it is the character's turn
    /// </summary>
    /// <param name="character"></param>
    /// <param name="mode">1 = Normal turn. 2 = Repeated turn. ("Again") 3 = Non-Ordered </param>
    public void LogTurn(Character character, int mode = 1)
    {
        string messageToSend = "";
        Message.MessageType type = Message.MessageType.AllyTurn;

        switch (mode)
        {
            case 1:
                messageToSend = $"*{character.name}'s Turn*";
                break;
            case 2:
                messageToSend = $"*{character.name}'s Turn. Again*";
                break;
            case 3:
                messageToSend = $"*{character.name}'s (Non-Ordered) Turn.*";
                break;
            default:
                Debug.LogError("Invalid Turn Mode");
                break;
        }

        switch (character.team)
        {
            case Team.Enemy:
                type = Message.MessageType.EnemyTurn;
                break;
            case Team.Ally:
                type = Message.MessageType.AllyTurn;
                break;
            case Team.OutOfCombat:
                type = Message.MessageType.OutOfCombatTurn;
                break;
        }

        SendMessageToChat(messageToSend, type);
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
        AllyTurn,
        EnemyTurn,
        OutOfCombatTurn,
        BattleStatus,
        BattleEffect
    }
}