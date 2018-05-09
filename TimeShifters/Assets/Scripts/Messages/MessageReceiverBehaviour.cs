using System;
using UnityEngine;

namespace Messages
{
    public abstract class MessageReceiverBehaviour : MonoBehaviour 
    {
        private MessageReceiver itsMessageReceiver;

        public virtual void Awake()
        {
            itsMessageReceiver = new MessageReceiver(MessageDispatcher.Instance);
        }

        protected void SubscribeToMessage(string messageId, Action<Message> listener, int priority = 0)
        {
            if (itsMessageReceiver != null) itsMessageReceiver.SubscribeToMessage(messageId, listener, priority);
        }

        protected void UndsubscribeFromMessage(string messageId)
        {
            if (itsMessageReceiver != null) itsMessageReceiver.UnsubscribeFromMessage(messageId);
        }

        protected void UnsubscribeFromAllMessages()
        {
            if (itsMessageReceiver != null) itsMessageReceiver.UnsubscribeFromAllMessages();
        }
    }
}
