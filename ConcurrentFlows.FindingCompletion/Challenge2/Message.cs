namespace ConcurrentFlows.FindingCompletion.Challenge2 {    

    public class Message {

        public Message(int value, int generateNewMessages, bool wasProcessed = false) {
            this.Value = value;
            this.GenerateNewMessages = generateNewMessages;
            this.WasProcessed = wasProcessed;
        }

        public int Value {
            get;
        }

        public int GenerateNewMessages {
            get;
        }

        public bool WasProcessed { get; set; }
    }
}
