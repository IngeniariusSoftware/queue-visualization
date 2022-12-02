﻿namespace queue_simulation;

public class QueueStatistics
{
    public readonly int TasksCount;

    public readonly int StaticServersCount;

    public readonly int DynamicServersCount;

    public readonly bool IsServersHaveQueue;

    public readonly double AverageDurationBetweenTasks;

    public readonly double ServersAverageWorkDuration;

    public double SimulationTime { get; private set; }

    public double TasksWaitingProbability { get; private set; }

    public double TasksAverageWaitingDuration { get; private set; }

    public double TasksAverageWaitingDurationForWaited { get; private set; }

    public double ServersIdleProbability { get; private set; }

    public double AverageQueueSize { get; private set; }

    public double[] DurationByWorkingServers;

    public readonly SortedDictionary<double, int> WorkingServersByTime;

    public QueueStatistics(List<ServerTask> tasks, int staticServersCount, int dynamicServersCount, List<Server> servers,
        SortedList<double, List<ServerTask>> timeline)
    {
        TasksCount = tasks.Count;
        StaticServersCount = staticServersCount;
        DynamicServersCount = dynamicServersCount;
        IsServersHaveQueue = servers[0].IsHasQueue;
        SimulationTime = timeline.Last().Key;
        TasksWaitingProbability = (double)tasks.Count(t => t.WaitingDuration > Program.Tolerance) / tasks.Count;
        TasksAverageWaitingDuration = tasks.Average(t => t.WaitingDuration);
        var waitedTasks = tasks.FindAll(t => t.WaitingDuration > Program.Tolerance).ToList();
        TasksAverageWaitingDurationForWaited = waitedTasks.Count > 0 ? waitedTasks.Average(t => t.WaitingDuration) : 0.0;
        AverageDurationBetweenTasks =
            tasks.Select((t, i) => t.ArrivalTime - (i > 0 ? tasks[i - 1].ArrivalTime : 0.0)).Average();
        ServersIdleProbability = 1.0 - servers.Average(s => s.AllWorksDuration) / SimulationTime;
        ServersAverageWorkDuration = servers.Sum(s => s.AllWorksDuration) / TasksCount;
        AverageQueueSize = GetAverageQueueSize(waitedTasks);
        WorkingServersByTime = new SortedDictionary<double, int>();
        DurationByWorkingServers = GetDurationByWorkingServers(tasks, servers.Count);
    }

    private static double GetAverageQueueSize(IReadOnlyCollection<ServerTask> waitedTasks)
    {
        List<double> arrivalTimes = waitedTasks.Select(t => t.ArrivalTime).ToList();
        List<double> endWaitingTimes = waitedTasks.Select(t => t.ArrivalTime + t.WaitingDuration).ToList();
        arrivalTimes.Sort();
        endWaitingTimes.Sort();

        int arrivalIndex = 0;
        int endWaitingIndex = 0;
        int waitedCount = 0;
        double lastTime = 0.0;
        double totalWaitedDuration = 0.0;
        double timelineDuration = 0.0;

        while (endWaitingIndex < endWaitingTimes.Count)
        {
            if (arrivalIndex < arrivalTimes.Count && arrivalTimes[arrivalIndex] <= endWaitingTimes[endWaitingIndex])
            {
                if (waitedCount > 0)
                {
                    double duration = arrivalTimes[arrivalIndex] - lastTime;
                    totalWaitedDuration += waitedCount * duration;
                    timelineDuration += duration;
                }

                lastTime = arrivalTimes[arrivalIndex];
                arrivalIndex++;
                waitedCount++;
            }
            else
            {
                double duration = endWaitingTimes[endWaitingIndex] - lastTime;
                totalWaitedDuration += waitedCount * duration;
                timelineDuration += duration;

                lastTime = endWaitingTimes[endWaitingIndex];
                endWaitingIndex++;
                waitedCount--;
            }
        }

        if (waitedCount != 0) throw new IndexOutOfRangeException(nameof(waitedCount));

        return totalWaitedDuration / timelineDuration;
    }

    private double[] GetDurationByWorkingServers(IReadOnlyCollection<ServerTask> tasks, int serversCount)
    {
        var durationByWorkingServers = new double[serversCount + 1];
        List<double> startWorkTime = tasks.Select(t => t.ArrivalTime + t.WaitingDuration).ToList();
        List<double> endWorkTime = tasks.Select(t => t.LeaveTime).ToList();
        startWorkTime.Sort();
        endWorkTime.Sort();

        int startWorkIndex = 0;
        int endWorkIndex = 0;
        int workingServersCount = 0;
        double lastTime = 0.0;

        while (endWorkIndex < endWorkTime.Count)
        {
            if (startWorkIndex < startWorkTime.Count && startWorkTime[startWorkIndex] <= endWorkTime[endWorkIndex])
            {
                double duration = startWorkTime[startWorkIndex] - lastTime;
                if (duration > Program.Tolerance)
                {
                    durationByWorkingServers[workingServersCount] += duration;
                }

                WorkingServersByTime[lastTime] = workingServersCount;
                lastTime = startWorkTime[startWorkIndex];
                startWorkIndex++;
                workingServersCount++;
            }
            else
            {
                double duration = endWorkTime[endWorkIndex] - lastTime;
                if (duration > Program.Tolerance)
                {
                    durationByWorkingServers[workingServersCount] += duration;
                }

                WorkingServersByTime[lastTime] = workingServersCount;
                lastTime = endWorkTime[endWorkIndex];
                endWorkIndex++;
                workingServersCount--;
            }
        }

        if (workingServersCount != 0) throw new IndexOutOfRangeException(nameof(workingServersCount));

        return durationByWorkingServers;
    }

    public override string ToString()
    {
        return
            $"{nameof(TasksCount)}: {TasksCount}\n" +
            $"{nameof(StaticServersCount)}: {StaticServersCount}\n" +
            $"{nameof(DynamicServersCount)}: {DynamicServersCount}\n" +
            $"{nameof(IsServersHaveQueue)}: {IsServersHaveQueue}\n" +
            $"{nameof(SimulationTime)}: {SimulationTime:f4}\n" +
            $"{nameof(TasksWaitingProbability)}: {TasksWaitingProbability:f4}\n" +
            $"{nameof(TasksAverageWaitingDuration)}: {TasksAverageWaitingDuration:f4}\n" +
            $"{nameof(TasksAverageWaitingDurationForWaited)}: {TasksAverageWaitingDurationForWaited:f4}\n" +
            $"{nameof(AverageDurationBetweenTasks)}: {AverageDurationBetweenTasks:f4}\n" +
            $"{nameof(ServersIdleProbability)}: {ServersIdleProbability:f4}\n" +
            $"{nameof(ServersAverageWorkDuration)}: {ServersAverageWorkDuration:f4}\n" +
            $"{nameof(AverageQueueSize)}: {AverageQueueSize:f4}\n" +
            $"{nameof(DurationByWorkingServers)}: {string.Concat(DurationByWorkingServers.Select((time, count) => $"{count}: {time:f4}; "))}\n";
    }

    public static QueueStatistics operator +(QueueStatistics s1, QueueStatistics s2)
    {
        s1.SimulationTime += s2.SimulationTime;
        s1.TasksWaitingProbability += s2.TasksWaitingProbability;
        s1.TasksAverageWaitingDuration += s2.TasksAverageWaitingDuration;
        s1.TasksAverageWaitingDurationForWaited += s2.TasksAverageWaitingDurationForWaited;
        s1.ServersIdleProbability += s2.ServersIdleProbability;
        s1.AverageQueueSize += s2.AverageQueueSize;
        return s1;
    }

    public static QueueStatistics operator /(QueueStatistics s1, int n)
    {
        s1.SimulationTime /= n;
        s1.TasksWaitingProbability /= n;
        s1.TasksAverageWaitingDuration /= n;
        s1.TasksAverageWaitingDurationForWaited /= n;
        s1.ServersIdleProbability /= n;
        s1.AverageQueueSize /= n;
        return s1;
    }
}