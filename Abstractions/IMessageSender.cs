namespace StoryBot.Core.Abstractions
{
    public interface IMessageSender<T>
    {
        void Send(long userId, T message);
    }
}
