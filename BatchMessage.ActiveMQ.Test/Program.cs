using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchMessage.ActiveMQ.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiQueueTest();

            Console.ReadLine();
        }

        static void MultiQueueTest()
        {
            TestMultiQueue queue = new TestMultiQueue();
            string queue1 = "TestQueue1";
            TestSendAndReceiveMessage(queue, queue1);
        }
        private static void TestSendAndReceiveMessage(TestMultiQueue queue, string queueName)
        {
            int totalMessage = 120;
            int bulkSize = 30;
            queue.SendBulkTextMessage(queueName, totalMessage);

            queue.ReceiveBulkMessage(queueName, bulkSize, false);

            queue.ReceiveBulkMessage(queueName, bulkSize, true);

            queue.ReceiveBulkMessage(queueName, bulkSize, false);

        }
        class TestMultiQueue : BaseMultiQueue
        {
            protected override string DbName
            {
                get { return "db1"; }
            }
        }
    }
}
