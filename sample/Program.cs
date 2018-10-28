using System.IO.MemoryMappedFiles;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Open shared memory
            MemoryMappedFile share_mem = MemoryMappedFile.CreateNew("shared_memory", 1024);
            MemoryMappedViewAccessor accessor = share_mem.CreateViewAccessor();

            // Write data to shared memory
            string str = "Hello World";
            char[] data = str.ToCharArray();
            accessor.Write(0, data.Length);
            accessor.WriteArray<char>(sizeof(int), data, 0, data.Length);

            // Dispose accessor
            accessor.Dispose();

            // Sleep
            while (true) System.Threading.Thread.Sleep(1000);
        }
    }
}
