/*
 * SignalR Example Created by Mitchel Sellers (msellers@iowacomputergurus.com)
 * 
 * Sample adapted from various online resources
 * 
 * No Warranties expressed no implied are included.  Use at your own risk.  If distributed 
 * please leave this statement at the top of all code files.  
 * 
 * Questions or feedback 
 * Email: msellers@iowacomputergurus.com
 * Blog: http://www.mitchelsellers.com
 */

using System;
using System.Net.Http;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRListener
{
    /// <summary>
    ///     Console application designed to be an admini listener to a SignalR Hub for demo purposes!
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///     Our hard coded server URI
        /// </summary>
        private const string ServerUri = "http://localhost:8080/signalr";

        /// <summary>
        ///     Holds the reference to the connection
        /// </summary>
        /// <value>The connection.</value>
        private static HubConnection Connection { get; set; }

        /// <summary>
        ///     Gets or sets the hub proxy. Used for communication
        /// </summary>
        /// <value>The hub proxy.</value>
        private static IHubProxy HubProxy { get; set; }

        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            //Declare the connection & Register handler for closing
            Connection = new HubConnection(ServerUri);
            Connection.Closed += Connection_Closed;

            //Setup our proxy to tie into the hub we want to work with
            HubProxy = Connection.CreateHubProxy("MyHub");

            //Use the .On syntax over "Subscribe" to allow for strong typing
            //Like others listen for regular messages
            HubProxy.On<string, string>("AddMessage", (name, message) =>
                Console.WriteLine("{0}: {1}", name, message)
                );

            //Also look for "Time Reports"
            HubProxy.On<string, DateTime>("TimeReport", (name, currentTime) =>
                Console.WriteLine("{0}: Current time is {1}", name, currentTime)
                );

            //Open up the connection
            try
            {
                Connection.Start();
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Unable to connect to server: Start server before connecting clients.");
                //No connection: Don't enable Send button or show chat UI
                return;
            }

            //Notify user and avoid exit
            Console.WriteLine("Listener Started, press enter to exit");
            Console.ReadLine();
        }

        /// <summary>
        ///     Connection_s the closed.
        /// </summary>
        private static void Connection_Closed()
        {
            Console.WriteLine("Lost Connection, restart application to try again!");
            //TODO: Better handle as really the app should be exited
        }
    }
}