using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollyExample
{
    class Program
    {
        static int global_dummy_value = -2;

        static void Main(string[] args)
        {
            //Sync Example
            var anystingkey = "same for demo";
            var cr = new CacheableResults<int>();
            for (int i = 1; i < 20; i++)
            {
                var r = cr.Execute(() => { System.Threading.Thread.Sleep(1000); Console.WriteLine("in"); return global_dummy_value++; }, anystingkey);
                Console.WriteLine(r);
                if (i % 3 == 0) cr.Clear(anystingkey);
            }

            //Async Example
            var cr_async = new CacheableResults<string>();
            for (int i = 1; i < 20; i++)
            {
                var rtask = cr_async.ExecuteAsync(async()  => { await Task.Delay(1000); Console.WriteLine("in"); return "cheese"; }, anystingkey);
                rtask.Wait();
                var r = rtask.Result;
                Console.WriteLine(r);
                if (i % 3 == 0) cr_async.Clear(anystingkey);
            }
        }


    }
}
