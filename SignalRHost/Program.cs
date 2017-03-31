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
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace SignalRHost
{
    /// <summary>
    /// ost application for a SignalR Server
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            //Setup the web application using Owin
            string url = "http://localhost:8080";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }

    /// <summary>
    /// OWIN startup class used to handle configuration.  Configuration of Cors & SignalR
    /// </summary>
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(); //Can change path if needed using overloads
        }
    }

    /// <summary>
    /// Represents a communication hub to be used with SignalR
    /// </summary>
    public class MyHub : Hub
    {
        /// <summary>
        /// Method that can be called by the clients to send a message
        /// </summary>
        /// <param name="name">The client's name.</param>
        /// <param name="message">The message.</param>
        public void Send(string name, string message)
        {
            //NOTE: Can omit sender by using Context.ConnectionId & Clients.AllExcept();
            Clients.All.addMessage(name, message);
        }

        /// <summary>
        /// Method allowing a client to report the current time.  Example of custom types
        /// </summary>
        /// <param name="name">The client's name.</param>
        /// <param name="current">The current time.</param>
        public void ReportTime(string name, DateTime current)
        {
            //NOTE: Can translate to other client call if needed
            //Clients.All.addMessage(name, "Reported Current Time of: " + current);
            Clients.All.timeReport(name, current);
        }

        #region Overrides
        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
        public override Task OnConnected()
        {
            Console.WriteLine("Client connected: " + Context.ConnectionId);
            Send("Server", Context.ConnectionId + " Joined");

            //Could use conditional send to message someone
            Clients.Client(Context.ConnectionId).addMessage("Server:", "You missed out!  Glad you are here! ");

            return base.OnConnected();
        }

        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">true, if stop was called on the client closing the connection gracefully;
        /// false, if the connection has been lost for longer than the
        /// <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
        /// Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            Console.WriteLine("Client Disconnected " + Context.ConnectionId + " Was Graceful: " + stopCalled);
            return base.OnDisconnected(stopCalled);
        }
        #endregion
    }
}