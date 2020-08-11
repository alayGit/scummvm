
namespace ManagedCommon.Interfaces
{
    public interface IGameClientStore<out T> where T : class, ITrashable
    {
        void AssociateConnectionWithGame(string connectionId, string gameId);
        T GetByConnectionId(string connectionId);
        T GetByGameId(string gameId);
        void Store(string gameId);
        string GetGameIdByConnectionId(string connectionId);
    }
}