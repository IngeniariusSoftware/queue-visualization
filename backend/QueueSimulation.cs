namespace queue_simulation;

using System.Collections.Generic;

public class QueueSimulation
{
    public readonly SortedList<double, List<ServerTask>> Timeline;

    public readonly QueueStatistics Statistics;

    private readonly List<Server> _staticServers;

    private readonly List<Server> _dynamicServers;

    private readonly Queue<ServerTask> _tasksGeneralQueue;

    private double _simulationTime;

    public QueueSimulation(List<ServerTask> tasks, int staticServersCount, int dynamicServersCount, bool isHasQueue)
    {
        _simulationTime = 0.0;
        Timeline = new SortedList<double, List<ServerTask>>(tasks.Count << 1);
        _tasksGeneralQueue = new Queue<ServerTask>();

        tasks.ForEach(t =>
        {
            t.Reset();
            AddToTimeline(t);
        });

        _staticServers = GenerateServers(0, staticServersCount, isHasQueue);
        _dynamicServers = GenerateServers(_staticServers.Count, dynamicServersCount, isHasQueue);
        Simulate();
        Statistics = new QueueStatistics(tasks, staticServersCount, dynamicServersCount,
            _staticServers.Concat(_dynamicServers).ToList(), Timeline);
    }

    private void AddToTimeline(ServerTask task)
    {
        double time = task.IsServed ? task.LeaveTime : task.ArrivalTime;

        if (!Timeline.ContainsKey(time))
        {
            Timeline[time] = new List<ServerTask>();
        }

        Timeline[time].Add(task);
    }

    private void Simulate()
    {
        for (int i = 0; i < Timeline.Count; i++)
        {
            _simulationTime = Timeline.GetKeyAtIndex(i);
            List<ServerTask> events = Timeline.GetValueAtIndex(i);
            HandleTimelineEvents(events);
            CheckTasks();
        }
    }

    private List<Server> GenerateServers(int startId, int serversCount, bool isHasQueue)
    {
        var servers = new List<Server>();
        for (int i = 0; i < serversCount; i++)
        {
            var server = new Server(startId + i, isHasQueue);
            server.ServerWorkStarted += OnServerWorkStarted;
            servers.Add(server);
        }

        return servers;
    }

    private void OnServerWorkStarted(object? _,  ServerTask task) => AddToTimeline(task);

    private void HandleTimelineEvents(List<ServerTask> events)
    {
        foreach (ServerTask task in events)
        {
            if (task.IsServed)
            {
                task.ServedByServer!.EndWork(_simulationTime);
            }
            else
            {
                _tasksGeneralQueue.Enqueue(task);
            }
        }
    }

    private void CheckTasks()
    {
        while (_tasksGeneralQueue.Count > 0)
        {
            List<Server> availableServers = GetAvailableServers();
            if (availableServers.Count == 0) break;

            Server server = availableServers[Program.Random.Next(0, availableServers.Count)];
            ServerTask task = _tasksGeneralQueue.Dequeue();
            server.AddTask(_simulationTime, task);
        }
    }

    // TODO ToList
    private List<Server> GetAvailableServers()
    {
        bool isAllStaticServersWork = _staticServers.TrueForAll(s => s.IsWork);
        List<Server> availableServers = _staticServers.FindAll(s => s.IsAvailable);
        if (isAllStaticServersWork)
        {
            availableServers.AddRange(_dynamicServers.FindAll(s => s.IsAvailable));
        }

        if (availableServers.Count == 0) return availableServers;

        int minTasksCount = availableServers.Min(s => s.TasksCount);
        return availableServers.FindAll(s => s.TasksCount == minTasksCount);
    }
}