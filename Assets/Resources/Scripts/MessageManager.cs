using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message
{
    public delegate void MessageAction(object invoker);
    event MessageAction ActionList;

    public void Add (MessageAction act)
    {
        ActionList += act;
    }

    public void Remove(MessageAction act)
    {
        ActionList -= act;
    }

    public void Trigger(object invoker)
    {
        if (ActionList != null)
            ActionList(invoker);
    }
}

public class MessageManager : MonoBehaviour
{
    Dictionary<string, Message> _messages;
    Dictionary<string, Message> messages
    {
        get
        {
            if (_messages == null)
                _messages = new Dictionary<string, Message>();
            return _messages;
        }
        set { _messages = value; }
    }

    public void Add(string name, Message.MessageAction act)
    {
        Message message;
        if (!messages.TryGetValue(name, out message))
        {
            message = new Message();
            messages.Add(name, message);
        }
        message.Add(act);
    }

    public void Remove(string name, Message.MessageAction act)
    {
        Message message;
        if (messages.TryGetValue(name, out message))
        {
            message.Remove(act);
        }
    }

    public void Trigger(string name, object invoker)
    {
        Message message;
        if (messages.TryGetValue(name, out message))
        {
            message.Trigger(invoker);
        }
    }
}
