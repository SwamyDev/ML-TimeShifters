using System;

namespace Messages
{
    public class MessageReceiver
    {
        private readonly MessageDispatcher itsDispatcher;

        public MessageReceiver(MessageDispatcher dispatcher)
        {
            itsDispatcher = dispatcher;
        }

        public void SubscribeToMessage(string messageId, Action<Message> listener, int priority = 0)
        {
            itsDispatcher.SubscribeToMessage(messageId, this, listener, priority);
        }

        public void UnsubscribeFromMessage(string messageId)
        {
            itsDispatcher.UnsubscribeFromMessage(messageId, this);
        }

        public void UnsubscribeFromAllMessages()
        {            
            itsDispatcher.UnsubscribeReceiver(this);
        }
    }
}