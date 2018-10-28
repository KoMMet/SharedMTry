using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sample2
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryMappedFile share_mem = MemoryMappedFile.OpenExisting("shared_memory");
            MemoryMappedViewAccessor accessor = share_mem.CreateViewAccessor();
            CancellationTokenSource cts = new CancellationTokenSource();
            bool stop = false;

             var t1 = Task.Run(() => { WaitKey(cts); });
            // Write data to shared memory
            var t2 = Task.Run(() => { RunReading(share_mem,accessor, cts); });
            Task.WaitAll(t1, t2);


        }

        private static void RunReading(MemoryMappedFile share_mem, MemoryMappedViewAccessor accessor, CancellationTokenSource cts)
        {
            while (true)
            {
                int size = accessor.ReadInt32(0);
                if(size == 0) continue;
                char[] data = new char[size];
                accessor.ReadArray<char>(sizeof(int), data, 0, data.Length);
                string str = new string(data);

                Console.WriteLine("Data = " + str);
                Thread.Sleep(1000);
                if (cts.IsCancellationRequested)
                {
                    // Dispose resource
                    accessor.Dispose();
                    share_mem.Dispose();
                    break;
                }
            }
        }

        private static void WaitKey(CancellationTokenSource ctsTs)
        {
            while(true)
                if (Console.ReadKey().KeyChar == 'q')
                {
                    ctsTs.Cancel();
                    break;
                }
        }
    }
}
