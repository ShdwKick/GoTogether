using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using Server.Data;

namespace GoTogether
{
    public class Subsription
    {
        private readonly DatabaseConnection _databaseConnection;

        public Subsription(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        [Subscribe(With = nameof(SubscribeToMessagesByChatId))]
        [Topic("Chat_{chatId}")]
        public Task<Message> OnMessageReceived([EventMessage] Message Message)
        {
            return Task.FromResult(Message);
        }
        
        public ValueTask<ISourceStream<Message>> SubscribeToMessagesByChatId(Guid chatId, [Service] ITopicEventReceiver eventReceiver)
        {
            return eventReceiver.SubscribeAsync<Message>($"Chat_{chatId}");
        }

        [Subscribe(With = nameof(SubscribeToRoomUsersListChanged))]
        [Topic("Room_{chatId}")]
        public Task<Message> OnRoomUserListChangeReceived([EventMessage] Message Message)
        {
            return Task.FromResult(Message);
        }

        public ValueTask<ISourceStream<TripUserListChange>> SubscribeToRoomUsersListChanged(Guid roomId, [Service] ITopicEventReceiver eventReceiver)
        {
            return eventReceiver.SubscribeAsync<TripUserListChange>($"Room_{roomId}");
        }
    }


}
