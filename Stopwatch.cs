using System;
using System.Threading;
using System.Threading.Tasks;

class TimerApp
{
    private TimeSpan _elapsedDuration;
    private bool _active;
    private CancellationTokenSource _cancelSource;

    public delegate void TimerEventHandler(string info);
    public event TimerEventHandler OnInitiated;
    public event TimerEventHandler OnPaused;
    public event TimerEventHandler OnCleared;

    public TimerApp()
    {
        _elapsedDuration = TimeSpan.Zero;
        _active = false;
    }

    public void Begin()
    {
        if (_active)
        {
            OnInitiated?.Invoke("Timer is already active.");
            return;
        }

        _active = true;
        _cancelSource = new CancellationTokenSource();
        OnInitiated?.Invoke("Timer started.");

        Task.Run(() => TrackTime(_cancelSource.Token));
    }

    public void Pause()
    {
        if (!_active)
        {
            OnPaused?.Invoke("Timer is not active.");
            return;
        }

        _active = false;
        _cancelSource.Cancel();
        OnPaused?.Invoke("Timer paused.");
    }

    public void Clear()
    {
        Pause();
        _elapsedDuration = TimeSpan.Zero;
        OnCleared?.Invoke("Timer cleared.");
        Begin();
    }

    private async Task TrackTime(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(1000);
            if (_active)
            {
                _elapsedDuration = _elapsedDuration.Add(TimeSpan.FromSeconds(1));
                Console.WriteLine($"Elapsed Time: {_elapsedDuration}");
            }
        }
    }
}

class Application
{
    static void Main(string[] args)
    {
        var timer = new TimerApp();

        timer.OnInitiated += info => Console.WriteLine(info);
        timer.OnPaused += info => Console.WriteLine(info);
        timer.OnCleared += info => Console.WriteLine(info);

        Console.WriteLine("Timer Console Program");
        Console.WriteLine("Options: B to Begin, P to Pause, C to Clear, E to Exit");

        bool running = true;

        while (running)
        {
            Console.Write("Enter option: ");
            string input = Console.ReadLine().ToUpper();

            switch (input)
            {
                case "B":
                    timer.Begin();
                    break;

                case "P":
                    timer.Pause();
                    break;

                case "C":
                    timer.Clear();
                    break;

                case "E":
                    running = false;
                    timer.Pause();
                    Console.WriteLine("Exiting program. Goodbye.");
                    break;

                default:
                    Console.WriteLine("Invalid option. Use B, P, C, or E.");
                    break;
            }
        }
    }
}
