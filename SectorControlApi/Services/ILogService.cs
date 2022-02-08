namespace SectorControlApi.Services
{
    public interface ILogService
    {
        void Info(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message);
    }
}
