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
using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRWpfClient
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ServerURI = "http://localhost:8080/signalr";

        public MainWindow()
        {
            InitializeComponent();
            UsernameTextBox.Text = "WPF Client";
            ConnectToSignalR();
        }

        private IHubProxy HubProxy { get; set; }
        private HubConnection Connection { get; set; }

        /// <summary>
        ///     Connects to signal r.
        /// </summary>
        private void ConnectToSignalR()
        {
            Connection = new HubConnection(ServerURI);
            Connection.Closed += Connection_Closed;

            //Connect to the hub we want specifically
            HubProxy = Connection.CreateHubProxy("MyHub");

            //Register handlers for the events we want.  We only care about messages!
            //NOTE: Must invoke as messages coming in on other thread
            HubProxy.On<string, string>("AddMessage", (name, message) =>
                Dispatcher.Invoke(() =>
                    OutputRichTextBox.AppendText(String.Format("{0}: {1}\r", name, message))
                    )
                );

            //Lets go!
            try
            {
                Connection.Start();
            }
            catch (HttpRequestException)
            {
                OutputRichTextBox.AppendText("Unable to connect to server: Start server before connecting clients.");
                //No connection: Don't enable Send button or show chat UI
            }
        }

        /// <summary>
        ///     Connection_s the closed.
        /// </summary>
        private void Connection_Closed()
        {
            OutputRichTextBox.AppendText("Connection Closed!  Exit application now!");
            //TODO: Handle gracefully, if this occurs application will fail!
        }

        /// <summary>
        ///     Handles the Click event of the SendButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            //For now, report both the message & time.  Could be done otherwise
            HubProxy.Invoke("Send", UsernameTextBox.Text, ToSendTextBox.Text);
            HubProxy.Invoke("ReportTime", UsernameTextBox.Text, DateTime.Now);
            ToSendTextBox.Text = String.Empty;
            ToSendTextBox.Focus();
        }

        /// <summary>
        ///     Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Be graceful & nice when leaving
            Connection.Stop();
        }
    }
}