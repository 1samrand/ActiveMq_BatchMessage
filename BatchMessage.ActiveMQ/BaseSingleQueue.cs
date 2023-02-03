using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BatchMessage.ActiveMQ
{
 
    public abstract class BaseSingleQueue : BaseActiveMQClient
    {
 
        public BaseSingleQueue()
            : base()
        {
        }

 
        public BaseSingleQueue(TimeSpan timeOut)
            : this()
        {
            this.timeOut = timeOut;
        }

        private TimeSpan timeOut = new TimeSpan(0, 0, 2);
        public abstract string QueueName { get; }

        public string ReceiveMessage()
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(this.QueueName);
                var comsumer = session.CreateConsumer(queue);
          
                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;

                if (message != null)
                    return message.Text;
            }
            connection.Stop();
            return null;
        }

        public T ReceiveMessage<T>() where T : class
        {
            T t;
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(this.QueueName);
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

        public T ReceiveMessage<T>(out string originMessage) where T : class
        {
            originMessage = null;

            using (ISession session = CreateSession(AcknowledgementMode.ClientAcknowledge))
            {
                var queue = session.GetQueue(this.QueueName);
                var comsumer = session.CreateConsumer(queue);

                ITextMessage message = comsumer.Receive(timeOut) as ITextMessage;
                if (message == null || string.IsNullOrWhiteSpace(message.Text))
                    return default(T);

                originMessage = message.Text;
                try
                {
                    T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Text);
                    message.Acknowledge();
                    return t;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Stop();
                }
            }
        }

        public void SendTextMessage<T>(params T[] msgs) where T : class
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(this.QueueName);
                var producer = session.CreateProducer(queue);

                JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
                serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                foreach (var item in msgs)
                {
                    producer.Send(new ActiveMQTextMessage(JsonConvert.SerializeObject(item, serializerSettings)));
                }
            }
            connection.Stop();
        }

        public void SendTextMessage(params string[] msgs)
        {
            using (ISession session = CreateSession())
            {
                var queue = session.GetQueue(this.QueueName);
                var producer = session.CreateProducer(queue);

                foreach (var msg in msgs)
                {
                    producer.Send(new ActiveMQTextMessage(msg));
                }
            }
            connection.Stop();
        }
    }
}
