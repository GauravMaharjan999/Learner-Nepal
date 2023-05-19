﻿using System;
using System.Threading.Tasks;
using Kachuwa.Identity.Extensions;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC.Hubs
{
    public class BaseHub<T> : Hub<T> where T : class
    {
        private readonly IRTCConnectionManager _connectionManager;

        public BaseHub(IRTCConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            var rtcUser = new RTCUser();
            var context = ContextResolver.Context;
            string ua = context.Request.Headers["User-Agent"].ToString();
            var userAgent = new UserAgent(ua);
            rtcUser.UserDevice = userAgent.Browser.ToString();
            rtcUser.ConnectionIds.Add(connectionId);
            rtcUser.IdentityUserId = context.User.Identity.GetIdentityUserId();
            rtcUser.SessionId = context.Session.Id;
            rtcUser.GroupName = "";
            rtcUser.HubName = this.GetType().Name;
            //rtcUser.IsFromMobile = false;
            //rtcUser.IsFromWeb = false;
            rtcUser.UserRoles = string.Join(',', context.User.Identity.GetRoles());
            rtcUser.ConnectionId = connectionId;

            _connectionManager.AddUser(rtcUser);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager.RemoveUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
       
    }

    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class BaseHub : Hub
    {
        public readonly IRTCConnectionManager ConnectionManager;
        
        public BaseHub(IRTCConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public   override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            var rtcUser = new RTCUser();
            var context = ContextResolver.Context;
            string ua = context.Request.Headers["User-Agent"].ToString();
            var userAgent = new UserAgent(ua);
            rtcUser.UserDevice = userAgent.Browser.ToString();
            rtcUser.ConnectionIds.Add(connectionId);
            rtcUser.IdentityUserId = context.User.Identity.GetIdentityUserId();
            rtcUser.SessionId = context.Session.Id;
            rtcUser.GroupName = "";
            rtcUser.HubName = this.GetType().Name;
            //rtcUser.IsFromMobile = false;
            rtcUser.IsFromWeb = true;
            rtcUser.UserRoles = string.Join(',', context.User.Identity.GetRoles());
            rtcUser.ConnectionId = connectionId;
            ConnectionManager.AddUser(rtcUser);
          
                try
                {
                    var status =  ConnectionManager.GetOnlineUserStatus().Result;
                     Clients.All.SendAsync("OnUserChange", status);
                }
                catch (Exception e)
                {
                    
                }
               
            
         
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionManager.RemoveUser(Context.ConnectionId);
            try
            {
                var status = ConnectionManager.GetOnlineUserStatus().Result;
                Clients.All.SendAsync("OnUserChange", status);
            }
            catch (Exception e)
            {

            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}