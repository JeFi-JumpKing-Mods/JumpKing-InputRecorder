using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace InputRecorder.States;

public class TasWriter : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly BlockingCollection<Action> _taskQueue = new();
    private readonly Thread _workerThread;
    private const int MaxFlushLine = 50;
    private int _lineCounter;

    public TasWriter(string path)
    {
        _writer = new StreamWriter(path);
        _workerThread = new Thread(ProcessQueue) { IsBackground = true };
        _workerThread.Start();
    }

    public void WriteLineQueued(string text)
    {
        foreach (string line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None)) {
            _taskQueue.Add(() => _writer.WriteLine(line));
            _lineCounter++;
#if DEBUG
            Debug.WriteLine($"TAS:{line}");
#endif

            if (_lineCounter >= MaxFlushLine)
            {
                FlushQueued();
            }
        }
    }

    public void FlushQueued()
    {
        _taskQueue.Add(() => _writer.Flush());
        _lineCounter = 0;
    }

    private void ProcessQueue()
    {
        foreach (var task in _taskQueue.GetConsumingEnumerable())
        {
            try
            {
                task.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"InputRecorder-TasWriter Error: {ex.Message}");
            }
        }

        _writer.Close();
    }

    public void CloseQueued()
    {
        _taskQueue.CompleteAdding();
    }

    public void Dispose()
    {
        CloseQueued();
        _workerThread.Join();
        _writer.Dispose();
    }
}
