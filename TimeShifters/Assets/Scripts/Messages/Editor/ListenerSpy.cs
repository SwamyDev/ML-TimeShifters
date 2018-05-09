namespace Messages.Tests
{
    public class ListenerSpy
    {
        public bool HasReceivedMessage { get; private set; }
        public object ReceivedData { get; private set; }
    
        public void Method(Message msg)
        {
            HasReceivedMessage = true;
            ReceivedData = msg.GetData<object>();
        }
    }
}