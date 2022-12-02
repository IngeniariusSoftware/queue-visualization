namespace queue_simulation;

public class Server
{
    public readonly int Id;

    public double AllWorksDuration { get; private set; }

    public readonly bool IsHasQueue;

    public int TasksCount => _queue.Count;

    public bool IsWork => TasksCount > 0;

    public bool IsAvailable => TasksCount == 0 || IsHasQueue;

    private readonly Queue<ServerTask> _queue;

    public event EventHandler<ServerTask> ServerWorkStarted = delegate { };

    public Server(int id, bool isHasQueue)
    {
        Id = id;
        _queue = new Queue<ServerTask>();
        IsHasQueue = isHasQueue;
        AllWorksDuration = 0.0;
    }

    public void AddTask(double time, ServerTask task)
    {
        if (!IsHasQueue && _queue.Count > 0) throw new IndexOutOfRangeException(nameof(task));

        _queue.Enqueue(task);

        if (_queue.Count == 1)
        {
            StartWork(time);
        }
    }

    public void EndWork(double time)
    {
        _queue.Dequeue();
        if (_queue.Count == 0) return;

        StartWork(time);
    }

    private void StartWork(double time)
    {
        ServerTask task = _queue.Peek();
        task.StartWork(time, this);
        AllWorksDuration += task.WorkDuration;
        ServerWorkStarted(this, task);
    }
}