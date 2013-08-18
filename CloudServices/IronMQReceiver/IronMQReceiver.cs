using IronMQ;
using IronMQUtils;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading;

internal class IronMQReceiver
{
    private static IQueue queue;

    static IronMQReceiver()
    {
        string projectId = ConfigurationManager.AppSettings["IRON_MQ_PROJECT_ID"];
        var token = ConfigurationManager.AppSettings["IRON_MQ_TOKEN"];
        var client = new Client(projectId, token);
        queue = client.Queue("IronMQChat");
    }

    private static void ReceiveMessage()
    {
        var message = queue.Get();

        if (message != null)
        {
            ChatMessage chatMessage = JsonConvert.DeserializeObject<ChatMessage>(message.Body);
            Console.WriteLine("Message received from {0} : {1}", chatMessage.SendingHost, chatMessage.Text);
            queue.DeleteMessage(message);
        }

        Thread.Sleep(100);
    }

    private static void Main()
    {
        while (true)
        {
            ReceiveMessage();
        }
    }
}
