namespace TaskTrack.Service.Helpers;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}