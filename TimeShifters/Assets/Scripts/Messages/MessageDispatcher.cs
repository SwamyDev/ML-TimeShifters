using System;
using System.Collections.Generic;
using System.Linq;

namespace Messages
{
    public class MessageDispatcher
    {
        private class SubscriberRegistry
        {
            private readonly Dictionary<string, List<Action<Message>>> itsListeners = new Dictionary<string, List<Action<Message>>>();
            public int Priority { get; private set; }

            public SubscriberRegistry(int priority)
            {
                Priority = priority;
            }

            public void Register(string messageId, Action<Message> listener)
            {
                if (MessageIsMissing(messageId))
                    itsListeners[messageId] = new List<Action<Message>>();
                
                itsListeners[messageId].Add(listener);
            }

            private bool MessageIsMissing(string messageId)
            {
                return itsListeners.ContainsKey(messageId) == false;
            }

            public void Dispatch(string messageId, Message message)
            {
                if (MessageIsMissing(messageId)) return;
                
                foreach (var listener in itsListeners[messageId])
                    listener(message);
            }

            public void Remove(string messageId)
            {
                itsListeners.Remove(messageId);
            }
        }
        
        public static readonly MessageDispatcher Instance = new MessageDispatcher();
        private readonly Dictionary<MessageReceiver, SubscriberRegistry> itsSubscribers = new Dictionary<MessageReceiver, SubscriberRegistry>();

        public void SubscribeToMessage(string messageId, MessageReceiver receiver, Action<Message> listener, int priority)
        {
            if (SubscriberIsMissing(receiver))
                itsSubscribers[receiver] = new SubscriberRegistry(priority);
            
            itsSubscribers[receiver].Register(messageId, listener);
        }

        private bool SubscriberIsMissing(MessageReceiver receiver)
        {
            return itsSubscribers.ContainsKey(receiver) == false;
        }

        public void UnsubscribeFromMessage(string messageId, MessageReceiver receiver)
        {
            if (SubscriberIsMissing(receiver)) return;
            
            itsSubscribers[receiver].Remove(messageId);
        }

        public void UnsubscribeReceiver(MessageReceiver receiver)
        {
            itsSubscribers.Remove(receiver);
        }

        public void Broadcast(string messageId)
        {
            BroadcastWithData(messageId, null);
        }

        public void BroadcastWithData(string messageId, object data)
        {
            var orderSubscribers = itsSubscribers.Values.OrderByDescending(s => s.Priority);
            foreach (var subscriber in orderSubscribers)
                subscriber.Dispatch(messageId, new Message(messageId, data));
        }
    }
}