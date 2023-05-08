using Models;

namespace Services
{
    public static class QueueService
    {
        private static readonly Queue<QueueModel> _queue;

        static QueueService()
        {
            _queue = new Queue<QueueModel>();
        }

        public static void Enqueue(string method, string data)
        {
            _queue.Enqueue(new()
            {
                Method = method,
                Data = data
            });
        }

        public static void Dequeue()
        {
            Task.Run(() =>
            {
                do
                {
                    if (!_queue.TryDequeue(out var item))
                        continue;

                    // ToDo

                } while (true);
            });
        }
    }
}
