namespace Messages
{
    public class Message
    {
        public string Id { get; private set; }
        public bool HasData { get; private set; }
        
        private readonly object itsData;

        public Message(string id, object data = null)
        {
            itsData = data;
            Id = id;
            HasData = itsData != null;
        }

        public T GetData<T>()
        {
            return (T)itsData;
        }
    }
}