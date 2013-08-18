using PubNubUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PubNubSender
{
    public class PubNubSender
    {
        private static void Main()
        {
            // Start the HTML5 Pubnub client
            Process.Start("..\\..\\PubNubClient.html");

            Thread.Sleep(2000);

            var pubNubApi = new PubNubApi(
                "pub-c-4a077e28-832a-4b58-aa75-2a551f0933ef",               // PUBLISH_KEY
                "sub-c-3a79639c-059e-11e3-8dc9-02ee2ddab7fe",               // SUBSCRIBE_KEY
                "sec-c-ZTAxYTk2ZGMtNzRiNi00ZTkwLTg4ZWEtOTMxOTk4NzAyNGIw",   // SECRET_KEY
                true                                                        // SSL_ON?
            );

            string channel = "chat-channel";

            // Publish a sample message to Pubnub
            List<object> publishResult = pubNubApi.Publish(channel, "Hello Pubnub!");
            Console.WriteLine(
                "Publish Success: " + publishResult[0].ToString() + "\n" +
                "Publish Info: " + publishResult[1]
            );

            // Show PubNub server time
            object serverTime = pubNubApi.Time();
            Console.WriteLine("Server Time: " + serverTime.ToString());

            // Subscribe for receiving messages (in a background task to avoid blocking)
            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(
                () =>
                pubNubApi.Subscribe(
                    channel,
                    delegate(object message)
                    {
                        Console.WriteLine("Received Message -> '" + message + "'");
                        return true;
                    }
                )
            );
            t.Start();

            // Read messages from the console and publish them to PubNub
            while (true)
            {
                Console.Write("Enter a message to be sent to Pubnub: ");
                string message = Console.ReadLine();
                pubNubApi.Publish(channel, message);
                Console.WriteLine("Message {0} sent.", message);
            }
        }
    }
}
