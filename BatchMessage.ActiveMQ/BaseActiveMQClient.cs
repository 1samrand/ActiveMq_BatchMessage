using Apache.NMS;
using BatchMessage.ActiveMQ.Configurations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BatchMessage.ActiveMQ
{
    public abstract class BaseActiveMQClient
    {
        protected static readonly ActiveMQSection section;
        
        static BaseActiveMQClient()
        {
            section = ConfigurationManager.GetSection("ActiveMQSection") as ActiveMQSection;
        }

        protected ActiveMQDB ActiveMQDB;
        private IConnectionFactory connectionFactory;
        private object syncRoot = new object();
 
        protected IConnection connection;
        protected abstract string DbName { get; }

 
        private int exceptionCount = 0;
 
        private readonly int maxExceptionCount = 5;

        public BaseActiveMQClient()
        {
            if (section == null)
                throw new ActiveMQException("未找到ActiveMQ配置信息");

            this.ActiveMQDB = section.ActiveMQDBs[DbName];

            if (this.ActiveMQDB == null)
                throw new ActiveMQException(string.Format("未找到配置信息，DBName：{0}", DbName));

            this.connectionFactory = new NMSConnectionFactory(this.ActiveMQDB.Url);

            this.connection = CreateConnection();
        }

        private IConnection CreateConnection()
        {
            IConnection conn = connectionFactory.CreateConnection(this.ActiveMQDB.UserName, this.ActiveMQDB.Password);
            conn.ConnectionInterruptedListener += conn_ConnectionInterruptedListener;
            return conn;
        }

        void conn_ConnectionInterruptedListener()
        {
            ResetConnection();
        }

        private void ResetConnection()
        {
            lock (syncRoot)
            {
                if (this.connection != null)
                {
                    this.connection.Dispose();
                }

                this.connection = CreateConnection();
            }
        }

        protected ISession CreateSession(AcknowledgementMode acknowledgementMode = AcknowledgementMode.Transactional)
        {
            try
            {
                if (!connection.IsStarted)
                    connection.Start();

                return this.connection.CreateSession(acknowledgementMode);
            }
            catch (Apache.NMS.ActiveMQ.IOException ex)
            {
                CounterException();
                throw ex;
            }
            catch (Apache.NMS.ActiveMQ.ConnectionClosedException ex)
            {
                CounterException();
                throw ex;
            }
            catch (Exception ex)
            {
                CounterException();
                throw ex;
            }
        }

        private void CounterException()
        {
            System.Threading.Interlocked.Increment(ref exceptionCount);
            if (System.Threading.Interlocked.CompareExchange(ref exceptionCount, 0, maxExceptionCount) == maxExceptionCount)
                ResetConnection();
        }
    }
}
