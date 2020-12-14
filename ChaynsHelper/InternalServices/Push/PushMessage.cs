namespace ChaynsHelper.InternalServices.Push
{
    public class PushMessage
    {
        public bool ForceSend { get; set; } = true;
        public bool SaveSend { get; set; } = false;
        public int MediaType { get; set; } = 3;
        public string Text { get; set; }
        public bool PushSendToAnyLocation { get; set; } = false;
        public PushSender Sender  { get; set; }
        public PushReceiver Receiver { get; set; }
    }
}