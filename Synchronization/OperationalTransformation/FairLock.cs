using System.Collections.Concurrent;

namespace OperationalTransformation;

public class FairLock
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<TaskCompletionSource<bool>> _queue = new ConcurrentQueue<TaskCompletionSource<bool>>();

    public async Task EnterAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        _queue.Enqueue(tcs);

        // Wait until it's this thread's turn
        if (_queue.TryPeek(out var firstInLine) && firstInLine == tcs)
        {
            await _semaphore.WaitAsync();
            _queue.TryDequeue(out _); // Remove from queue once it's this thread's turn
            tcs.SetResult(true);
        }
        else
        {
            await tcs.Task;
            await _semaphore.WaitAsync();
        }
    }

    public void Exit()
    {
        _semaphore.Release();
        if (_queue.TryPeek(out var nextInLine))
        {
            nextInLine.SetResult(true); // Let the next thread proceed
        }
    }
}