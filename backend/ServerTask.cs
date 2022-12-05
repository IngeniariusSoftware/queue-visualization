namespace queue_simulation;

public class ServerTask
{
    public readonly int Id;

    public readonly double ArrivalTime;

    public readonly double WorkDuration;

    public double WaitingDuration { get; private set; }

    public Server? ServedBy { get; private set; }

    public bool IsServed => ServedBy is not null;

    public ServerTask(int id, double arrivalTime, double workDuration)
    {
        Id = id;
        ArrivalTime = arrivalTime;
        WorkDuration = workDuration;
    }

    public double LeaveTime => ArrivalTime + WaitingDuration + WorkDuration;

    public void StartWork(double startTime, Server servedByServer)
    {
        WaitingDuration = startTime - ArrivalTime;
        ServedBy = servedByServer;
    }

    public void Reset()
    {
        WaitingDuration = 0.0;
        ServedBy = null;
    }
}