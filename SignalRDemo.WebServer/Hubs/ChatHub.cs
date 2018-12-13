using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRDemo.WebServer.Entities;
using SignalRDemo.WebServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SignalRDemo.WebServer.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IActiveUserRepository _activeUserRepository;

        // Constructor
        public ChatHub(ILogger<ChatHub> logger, IActiveUserRepository activeUserRepository)
        {
            _logger = logger;                                   // Inject Logger
            _activeUserRepository = activeUserRepository;       // Inject ActiveUser Repository
        }

        // This method is called when any user connects to the hub
        public override Task OnConnectedAsync()
        {
            // Send message to all connected users in the Hub
            Clients.All.SendAsync("ReceiveMessage", "", $"CurrentConnectionID: {Context.ConnectionId} - {Context.User.Identity.Name} - {Environment.MachineName}");

            // Add user to connected users
            if (!string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                AddUser(Context.ConnectionId, Context.User.Identity.Name);

                UserHandler.ConnectedIds.Add(Context.User.Identity.Name);
            }

            return base.OnConnectedAsync();
        }

        // This method is called when any user disconnects to the hub
        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Send message to all connected users in the Hub
            Clients.All.SendAsync("ReceiveMessage", "UserDisconnected", $"CurrentConnectionID: {Context.ConnectionId}");

            // Remove user to connected users
            if (!string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                RemoveUser(Context.User.Identity.Name);
                UserHandler.ConnectedIds.Remove(Context.User.Identity.Name);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public ChannelReader<int> Counter(int delay)
        {
            try
            {
                var channel = Channel.CreateUnbounded<int>();

                // We don't want to await WriteItems, otherwise we'd end up waiting 
                // for all the items to be written before returning the channel back to
                // the client.
                _ = WriteItems(channel.Writer, delay);

                return channel.Reader;


                async Task WriteItems(ChannelWriter<int> writer, int delay2)
                {
                    //for (var i = 0; i < count2; i++)
                    //{
                    //    await writer.WriteAsync(i);
                    //    await Task.Delay(delay2);
                    //}

                    await writer.WriteAsync(UserHandler.ConnectedIds.Count);
                    await Task.Delay(1000);

                    writer.TryComplete();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, $"{message}");
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        // Send message to a user
        public async Task SendToUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveDirectMessage", $"{Context.UserIdentifier}", $"{message}");
        }

        // Send a message to everyone
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("ReceiveChatMessage", $"{Context.UserIdentifier}: {message}");
        }

        //public ChannelReader<int> Counter(int count, int delay)
        //{
        //    var channel = Channel.CreateUnbounded<int>();

        //    // We don't want to await WriteItems, otherwise we'd end up waiting 
        //    // for all the items to be written before returning the channel back to
        //    // the client.
        //    _ = WriteItems(channel.Writer, count, delay);

        //    return channel.Reader;
        //}

        //private async Task WriteItems(ChannelWriter<int> writer, int count, int delay)
        //{
        //    for (var i = 0; i < count; i++)
        //    {
        //        await writer.WriteAsync(i);
        //        await Task.Delay(delay);
        //    }

        //    writer.TryComplete();
        //}

        //Other

        /// <summary>
        /// Add a user to the database calling the repository
        /// </summary>
        /// <param name="ConnectionId">This is generated by SignalR</param>
        /// <param name="NTLogin">From current user</param>
        private void AddUser(string ConnectionId, string NTLogin)
        {

            var Entity = new ActiveUser
            {
                ConnectionID = ConnectionId,
                NTLogin = NTLogin,
                UserIPAddress = "1.1.1.1",
                UserMachineName = "machine"
            };

            _activeUserRepository.AddActiveUser(Entity);

            var saved = _activeUserRepository.Save();
        }

        private void RemoveUser(string ntlogin)
        {
            _activeUserRepository.RemoveActiveUser(ntlogin);

            var saved = _activeUserRepository.Save();
        }
    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
}
