using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// ConcurrentQueue<string?> q = new ConcurrentQueue<string?>();
Queue<string?> q = new Queue<string?>();

using (StreamReader sr = new StreamReader("big_file.txt"))
{
    while (true)
    {
        Task<string?> readTask = sr.ReadLineAsync();

        while (q.TryDequeue(out string? message))
        {
            System.Console.WriteLine(message);
        }

        string? line = await readTask;
        if (line == null)
        {
            break;
        }

        q.Enqueue(line);
    }
}