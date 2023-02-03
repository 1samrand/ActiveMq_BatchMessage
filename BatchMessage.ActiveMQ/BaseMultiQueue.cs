using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace BatchMessage.ActiveMQ
{
 
    public abstract class BaseMultiQueue : BaseActiveMQClient
    {
      
        public BaseMultiQueue()
            : base()
        {
        }
 
        public BaseMultiQueue(TimeSpan timeOut)
            : this()
        {
            this.timeOut = timeOut;
        }

        private TimeSpan timeOut = new TimeSpan(0, 0, 2);

        public string ReceiveMessage(string queueName)
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var comsumer = session.CreateConsumer(queue);

                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;
                session.Commit();
                if (message != null)
                    return message.Text;
            }
            connection.Stop();
            return null;
        }
        public List<ITextMessage> ReceiveBulkMessage(string queueName,int bulkcount,bool forceToRollback)
        {
            List<ITextMessage> lst = new List<ITextMessage>();
            using (ISession session = CreateSession())
            {
                try
                {
                    var queue = session.GetQueue(queueName);
                    var comsumer = session.CreateConsumer(queue);
                    for (int i = 0; lst.Count < bulkcount; i++)
                    {
                        ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;
                        if (message == null) return lst;
                        lst.Add(message);
                    }
                    //do some processing on List<ITextMessage>()
                    if (forceToRollback)
                    {
                        Console.WriteLine($"{bulkcount} rollback");
                        session.Rollback();
                    }
                    else
                    {
                        Console.WriteLine($"{bulkcount} commit");
                        session.Commit();
                    }

                }
                catch (Exception)
                {
                    session.Rollback();
                }
                


            }
            connection.Stop();
            return lst;
        }
        public T ReceiveMessage<T>(string queueName) where T : class
        {
            T t;
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var comsumer = session.CreateConsumer(queue);

                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;
                if (message == null || string.IsNullOrWhiteSpace(message.Text))
                    t = default(T);
                else
                    t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Text);
            }
            connection.Stop();
            return t;
        }
        public void SendBulkTextMessage(string queueName,int numberOfMessage)
        {
            Stopwatch watcher = new Stopwatch();
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(queueName);
                var producer = session.CreateProducer(queue);

                watcher.Start();
                //foreach (var msg in msgs)
                for (int i = 1; i <= numberOfMessage; i++)
                {
                    producer.Send(new ActiveMQTextMessage($"raya {i.ToString()}"));
                }
                session.Commit();
                watcher.Stop();
            }
            var elapsed = watcher.Elapsed;
            connection.Stop();
        }
    }
}
