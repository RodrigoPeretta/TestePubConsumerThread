using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TesteThread
{
    public class ThreadObject
    {
        public int Thread { get; set; }

        public int Number { get; set; }
        public ThreadObject(int _thread, int _number)
        {
            Thread = _thread;
            Number = _number;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var messageQueue = new BlockingCollection<ThreadObject>();
            
            var Consumer = new ConsumerQueue(messageQueue);


            Task.Run(() =>
            {
                Consumer.StartProcessing();
            });

            IList<Task> listTask = new List<Task>();

            for (int i = 1; i <= 8; i++)
            {
                var pubQueue = new PublishQueue(i * 10, (i * 10) + 9, i);
                listTask.Add(Task.Run(() => { pubQueue.Publish(messageQueue); }));
            }

            Task.WaitAll(listTask.ToArray());

            Console.Read();
        }
    }

    public class ConsumerQueue
    {
        private readonly BlockingCollection<ThreadObject> messageQueue;
        public ConsumerQueue(BlockingCollection<ThreadObject> messageQueue)
        {
            this.messageQueue = messageQueue;
        }
        public void StartProcessing()
        {
            while (true)
            {
                var message = messageQueue.Take();

                Console.WriteLine($"Thread={message.Thread} n => ({message.Number})");
            }
        }
    }

    public class PublishQueue
    {
        public int start { get; set; }
        public int end { get; set; }

        public int ThreadNumber { get; set; }
        public PublishQueue(int _start, int _end, int _threadNumber)
        {
            start = _start;
            end = _end;
            ThreadNumber = _threadNumber;
        }

        public void Publish(BlockingCollection<ThreadObject> queue)
        {
            for (int i = 0; i < 10; i++)
            {
                queue.Add(new ThreadObject(ThreadNumber, new Random().Next(start, end)));
            }

        }
    }


}
