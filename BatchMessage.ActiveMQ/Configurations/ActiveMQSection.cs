using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BatchMessage.ActiveMQ.Configurations
{
    public sealed class ActiveMQSection : ConfigurationSection
    {
        [ConfigurationProperty("ActiveMQDBs")]
        [ConfigurationCollection(typeof(ActiveMQDB), AddItemName = "ActiveMQDB")]
        public ActiveMQDBCollection ActiveMQDBs
        {
            get
            { return (ActiveMQDBCollection)this["ActiveMQDBs"]; }
            set
            { this["ActiveMQDBs"] = value; }
        }

        public ActiveMQDB GetDB(string dbName)
        {
            return this.ActiveMQDBs[dbName];
        }
    }

    public class ActiveMQDBCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ActiveMQDB();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ActiveMQDB)element).DBName;
        }

        public new ActiveMQDB this[string dbName]
        {
            get { return (ActiveMQDB)BaseGet(dbName); }
        }
    }

    public class ActiveMQDB : ConfigurationElement
    {
        [ConfigurationProperty("DBName", IsRequired = true)]
        public string DBName
        {
            get
            {
                return this["DBName"].ToString();
            }
            set
            {
                this["DBName"] = value;
            }
        }

        [ConfigurationProperty("Url", IsRequired = true)]
        public string Url
        {
            get
            {
                return this["Url"].ToString();
            }
            set
            {
                this["Url"] = value;
            }
        }

        [ConfigurationProperty("UserName")]
        public string UserName
        {
            get
            {
                return this["UserName"].ToString();
            }
            set
            {
                this["UserName"] = value;
            }
        }


        [ConfigurationProperty("Password")]
        public string Password
        {
            get
            {
                return this["Password"].ToString();
            }
            set
            {
                this["Password"] = value;
            }
        }
    }

}
