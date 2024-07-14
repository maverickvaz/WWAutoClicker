using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WWAutoClicker
{
    public class KeyTimer
    {
        private readonly Timer timer;
        private readonly int repeatCount;
        private readonly int intervalMilliseconds;
        private readonly KeySimulator keySimulator;
        private CancellationTokenSource cancellationTokenSource;
        private readonly string key;

        public KeyTimer(string key, int repeatCount, int intervalSeconds, KeySimulator keySimulator)
        {
            this.key = key;
            this.repeatCount = repeatCount == 0 ? 1 : repeatCount; // If repeat count is 0, set it to 1
            this.intervalMilliseconds = intervalSeconds * 1000;
            this.keySimulator = keySimulator;
            timer = new Timer(TimerCallback);
        }

        public async void Start(int startTimeSeconds)
        {
            cancellationTokenSource = new CancellationTokenSource();

            if (startTimeSeconds > 0)
            {
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Starting delay for {startTimeSeconds} seconds for key: {key}");
                try
                {
                    await Task.Delay(startTimeSeconds * 1000, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    // Handle the task cancellation gracefully
                    Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Start delay canceled for key: {key}");
                    return;
                }
            }

            if (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                timer.Change(0, Timeout.Infinite); // Set timer to trigger immediately and not repeat
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Timer for key: {key} started");
            }
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            cancellationTokenSource?.Cancel();
            Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Timer for key: {key} stopped");
        }

        private async void TimerCallback(object state)
        {
            var cancellationToken = cancellationTokenSource.Token;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await Task.Run(() => keySimulator.ExecuteKeyPresses(key, repeatCount, cancellationToken), cancellationToken); // Run the key press simulation
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Operation canceled for key: {key}");
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Exception occurred: {ex.Message}");
                return;
            }

            stopwatch.Stop();
            int elapsed = (int)stopwatch.ElapsedMilliseconds;

            if (intervalMilliseconds == 0)
            {
                // If interval is 0, we loop indefinitely
                timer.Change(0, Timeout.Infinite);
                //Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Restarting timer for key: {key} with infinite loop");
                return;
            }

            int remainingInterval = intervalMilliseconds - elapsed;

            if (remainingInterval > 0)
            {
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Waiting for {remainingInterval} ms");
                try
                {
                    await Task.Delay(remainingInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Waiting canceled for key: {key}");
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Exception occurred during wait: {ex.Message}");
                    return;
                }
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                timer.Change(0, Timeout.Infinite); // Restart the timer for the next cycle
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Restarting timer for key: {key}");
            }
        }
    }
}