namespace queue_simulation;

public class Program
{
    public static readonly Random Random = new();

    public static readonly double Tolerance = 0.0001;

    public static void Main()
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";
        Thread.CurrentThread.CurrentCulture = customCulture;
        
        const int testsCount = 1;
        const int staticServersCount = 4;
        const int tasksCount = 300;
        const double meanTasksInterval = 60.0;
        const double standardDeviationTasksInterval = 15.0;
        const double meanWorkDuration = 220.0;
        const double standardDeviationWorkDuration = 60.0;
        var settings = new[] { (0, false), (0, true) };

        //var settings = new[] { (0, false), (0, true), (1, false), (1, true) };
        var statistics = new List<QueueStatistics>();

        for (int i = 0; i < testsCount; i++)
        {
            List<ServerTask> tasks = GenerateTasks(tasksCount, meanTasksInterval, standardDeviationTasksInterval, meanWorkDuration,
                standardDeviationWorkDuration);

            for (int j = 0; j < settings.Length; j++)
            {
                (int dynamicServersCount, bool isHasQueue) = settings[j];
                var simulation = new QueueSimulation(tasks, staticServersCount, dynamicServersCount, isHasQueue);
                //File.WriteAllLines($"{j}.csv",
                //    tasks.FindAll(t => simulation.DynamicServers.Contains(t.ServedBy))
                //        .Select(t =>
                //            $"{t.ArrivalTime + t.WaitingDuration - 1},0\n{t.ArrivalTime + t.WaitingDuration},1\n{t.LeaveTime},1\n{t.LeaveTime + 1},0"));

                if (statistics.Count == j)
                {
                    statistics.Add(simulation.Statistics);
                }
                else
                {
                    statistics[j] += simulation.Statistics;
                }
            }
        }

        //statistics.ForEach(s => Console.WriteLine(s / testsCount));
        statistics.ForEach(s => Console.WriteLine((s / testsCount).ValuesForTable()));

        //simulation = new QueueSimulation(tasksCount, tasksIntervalMin, tasksIntervalMax, 4, 1, workDurationMin, workDurationMax, false);
        //File.WriteAllLines("3.txt", simulation.Statistics.WorkingServersByTime.Select(x => $"{x.Key}, {x.Value}"));
        //Console.WriteLine(simulation.Statistics);
        //simulation = new QueueSimulation(tasksCount, tasksIntervalMin, tasksIntervalMax, 4, 1, workDurationMin, workDurationMax, true);
        //File.WriteAllLines("4.txt", simulation.Statistics.WorkingServersByTime.Select(x => $"{x.Key}, {x.Value}"));
        //Console.WriteLine(simulation.Statistics);
        Console.ReadKey();
    }

    public static List<ServerTask> GenerateTasks(int tasksCount, double meanTasksInterval, double standardDeviationTasksInterval,
        double meanWorkDuration, double standardDeviationWorkDuration)
    {
        var tasks = new List<ServerTask>();
        double arrivalTime = 0;
        for (int i = 0; i < tasksCount; i++)
        {
            arrivalTime += Math.Abs(Math.Round(NextGaussian(meanTasksInterval, standardDeviationTasksInterval))) + 1.0;
            double workDuration = Math.Abs(Math.Round(NextGaussian(meanWorkDuration, standardDeviationWorkDuration))) + 1.0;
            var task = new ServerTask(i, arrivalTime, workDuration);
            tasks.Add(task);
        }

        return tasks;
    }

    public static double NextGaussian(double mean, double standardDeviation)
    {
        double u1 = 1.0 - Random.NextDouble();
        double u2 = 1.0 - Random.NextDouble();
        double randomStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + (standardDeviation * randomStdNormal);
    }
}