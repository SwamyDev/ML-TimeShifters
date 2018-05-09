using System.Collections.Generic;
using NUnit.Framework;

namespace Messages.Tests
{
    public class MessagesTests
    {
        private MessageDispatcher dispatcher;
        private MessageReceiver receiver;

        [SetUp]
        public void SetUp()
        {
            dispatcher = new MessageDispatcher();
            receiver = new MessageReceiver(dispatcher);
        }

        [Test]
        public void MessageHasCorrectId()
        {
            Assert.AreEqual("MessageId", new Message("MessageId").Id);
        }

        [Test]
        public void MessageHasNoDataInitially()
        {
            Assert.IsFalse(new Message("Irrelevant").HasData);
        }

        [Test]
        public void OptinallyCreateWithData()
        {
            var data = new DataDummy();
            var message = new Message("Irrelevant", data);

            Assert.IsTrue(message.HasData);
            Assert.AreSame(data, message.GetData<DataDummy>());
        }

        [Test]
        public void MessageDispatcherSingleton()
        {
            Assert.NotNull(MessageDispatcher.Instance);
        }

        [Test]
        public void BroadcastMessageButNoOneListens()
        {
            dispatcher.Broadcast("AMessage");
        }

        [Test]
        public void SubscriberReceivesBroadcastedMessage()
        {
            var listener = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listener.Method);

            dispatcher.Broadcast("AMessage");

            Assert.IsTrue(listener.HasReceivedMessage);
        }

        [Test]
        public void BroadcastWithDataPassesOnDataObject()
        {
            var listener = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listener.Method);
            var data = new DataDummy();

            dispatcher.BroadcastWithData("AMessage", data);

            Assert.AreSame(data, listener.ReceivedData);
        }

        [Test]
        public void MultipleSubscribers()
        {
            var anotherReceiver = new MessageReceiver(dispatcher);
            var listenerA = new ListenerSpy();
            var listenerB = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listenerA.Method);
            anotherReceiver.SubscribeToMessage("AMessage", listenerB.Method);

            dispatcher.Broadcast("AMessage");

            Assert.IsTrue(listenerA.HasReceivedMessage);
            Assert.IsTrue(listenerB.HasReceivedMessage);
        }

        [Test]
        public void UnsubscribeListener()
        {
            var listener = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listener.Method);
            receiver.UnsubscribeFromMessage("AMessage");

            dispatcher.Broadcast("AMessage");

            Assert.IsFalse(listener.HasReceivedMessage);
        }

        [Test]
        public void UnsubscribeMissingListener()
        {
            receiver.UnsubscribeFromMessage("AMessage");
        }

        [Test]
        public void MultipleMessages()
        {
            var listenerA = new ListenerSpy();
            var listenerB = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listenerA.Method);
            receiver.SubscribeToMessage("AnotherMessage", listenerB.Method);

            dispatcher.Broadcast("AMessage");

            Assert.IsTrue(listenerA.HasReceivedMessage);
            Assert.IsFalse(listenerB.HasReceivedMessage);
        }

        [Test]
        public void UnsubscribesMultipleListenersFromMessage()
        {
            var listenerA = new ListenerSpy();
            var listenerB = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listenerA.Method);
            receiver.SubscribeToMessage("AMessage", listenerB.Method);
            receiver.UnsubscribeFromMessage("AMessage");

            dispatcher.Broadcast("AMessage");

            Assert.IsFalse(listenerA.HasReceivedMessage);
            Assert.IsFalse(listenerB.HasReceivedMessage);
        }

        [Test]
        public void UnsubscribesOnlySpecifiedMessage()
        {
            var listenerA = new ListenerSpy();
            var listenerB = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listenerA.Method);
            receiver.SubscribeToMessage("AnotherMessage", listenerB.Method);
            receiver.UnsubscribeFromMessage("AMessage");

            dispatcher.Broadcast("AMessage");
            dispatcher.Broadcast("AnotherMessage");

            Assert.IsFalse(listenerA.HasReceivedMessage);
            Assert.IsTrue(listenerB.HasReceivedMessage);
        }

        [Test]
        public void UnsubscribeFromAllMessages()
        {
            var listenerA = new ListenerSpy();
            var listenerB = new ListenerSpy();
            receiver.SubscribeToMessage("AMessage", listenerA.Method);
            receiver.SubscribeToMessage("AnotherMessage", listenerB.Method);
            receiver.UnsubscribeFromAllMessages();

            dispatcher.Broadcast("AMessage");
            dispatcher.Broadcast("AnotherMessage");

            Assert.IsFalse(listenerA.HasReceivedMessage);
            Assert.IsFalse(listenerB.HasReceivedMessage);
        }

        [Test]
        public void PriorityOrdering()
        {
            var anotherReceiver = new MessageReceiver(dispatcher);
            var receivedMessages = new List<string>();
            receiver.SubscribeToMessage("AMessage", m => receivedMessages.Add("LowPrio"));
            anotherReceiver.SubscribeToMessage("AMessage", m => receivedMessages.Add("HighPrio"), 10);

            dispatcher.Broadcast("AMessage");

            Assert.AreEqual("HighPrio", receivedMessages[0]);
            Assert.AreEqual("LowPrio", receivedMessages[1]);
        }

        private class DataDummy
        {
        }
    }
}