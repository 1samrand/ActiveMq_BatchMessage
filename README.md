# Introduction 
This code is the use of a feature such as the use of a group of messages in the queue for better performance of the application, of course, it should be noted that the use of this topic depends on the type of your application.

# Getting Started
 1.make sure you have ActiveMq in your pc
 2.the entry point of application is BatchMessage.ActiveMQ.Test/program.cs 
 3.you can use this method : SendBulkTextMessage(string queueName, int numberOfMessage) to push test message to the queue
 4.with the help of this method : ReceiveBulkMessage(string queueName, int bulkcount, bool forceToRollback) we can read chunk of message that exist in ActiveMq
 
 
  in this case to implement chunk of meesage we are using AcknowledgementMode =  AcknowledgementMode.Transactional 
  and for each bulk of message you have commit and rollback ability.
  
  enjoy it...
