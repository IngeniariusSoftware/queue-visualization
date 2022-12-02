namespace queue_simulation;

public class Program
{
    public static readonly Random Random = new();

    public static readonly double Tolerance = 0.0001;

    public static void Main()
    {
        int testsCount = 1;
        int staticServersCount = 4;
        int tasksCount = 100_000;
        double meanTasksInterval = 60.0;
        double standardDeviationTasksInterval = 15.0;
        double meanWorkDuration = 220.0;
        double standardDeviationWorkDuration = 60.0;

        var tasks = GenerateTasks(tasksCount, meanTasksInterval, standardDeviationTasksInterval, meanWorkDuration,
            standardDeviationWorkDuration);

        var simulation = new QueueSimulation(tasks, staticServersCount, 0, false);
        var statistics1 = simulation.Statistics;


        simulation = new QueueSimulation(tasks, staticServersCount, 0, true);
        var statistics2 = simulation.Statistics;

        for (int i = 0; i < testsCount; i++)
        {
            tasks = GenerateTasks(tasksCount, meanTasksInterval, standardDeviationTasksInterval, meanWorkDuration,
                standardDeviationWorkDuration);

            simulation = new QueueSimulation(tasks, staticServersCount, 0, false);
            statistics1 += simulation.Statistics;

            simulation = new QueueSimulation(tasks, staticServersCount, 0, true);
            statistics2 += simulation.Statistics;
        }

        Console.WriteLine(statistics1 / (testsCount + 1));
        Console.WriteLine(statistics2 / (testsCount + 1));
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