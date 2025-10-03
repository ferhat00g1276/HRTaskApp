using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace API
{
    public class ChatHub:Hub
    {
        //public override async Task OnConnectedAsync()
        //{
        //        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} A new user has connected.");
        //}
        //public async Task JoinGroupByRole(string role)
        //{
        //    if (role == "Admin")
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        //    }
        //    else if (role == "Manager")
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, "Managers");
        //    }
        //    else if (role == "Employee")
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, "Employees");
        //    }
        //}

        //// Qruplara mesaj göndərmək
        //public async Task SendMessageToRole(string role, string message)
        //{
        //    if (role == "Admin")
        //    {
        //        await Clients.Group("Admins").SendAsync("ReceiveMessage", message);
        //    }
        //    else if (role == "Manager")
        //    {
        //        await Clients.Group("Managers").SendAsync("ReceiveMessage", message);
        //    }
        //    else if (role == "Employee")
        //    {
        //        await Clients.Group("Employees").SendAsync("ReceiveMessage", message);
        //    }
        //}
        public async Task SendMessage(string message)
        {
            var role = GetUserRoleFromToken();

            if (role == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Authentication failed.");
                return;
            }

            string groupName = GetGroupName(role.Value);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"[{role}] {message}");
        }

        // Admin-only: Send message to any role group
        public async Task SendMessageToSpecificRole(UserRole targetRole, string message)
        {
            var senderRole = GetUserRoleFromToken();

            if (senderRole != UserRole.Admin)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Only admins can send messages to other roles.");
                return;
            }

            string groupName = GetGroupName(targetRole);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"[Admin to {targetRole}] {message}");
        }

        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} A new user has connected.");

        //    var httpContext = Context.GetHttpContext();
        //    string role = httpContext.Request.Query["role"];
        //    if (string.IsNullOrEmpty(role))
        //    {
        //        role = httpContext.Request.Headers["role"];
        //    }

        //    Console.WriteLine($"Received role: {role}");

        //    if (string.IsNullOrEmpty(role) || !new[] { "Admin", "Manager", "Employee" }.Contains(role))
        //    {
        //        await Clients.Caller.SendAsync("ReceiveMessage", "Role is required or invalid.");
        //        return; // disconnect etmə
        //    }

        //    await JoinGroupByRole(role);
        //    await base.OnConnectedAsync();
        //}
        //public override async Task OnConnectedAsync()
        //{
        //    var httpContext = Context.GetHttpContext();


        //    Console.WriteLine("=== Connection Debug Info ===");
        //    Console.WriteLine($"Connection ID: {Context.ConnectionId}");
        //    Console.WriteLine($"Query String: {httpContext.Request.QueryString}");

        //    foreach (var query in httpContext.Request.Query)
        //    {
        //        Console.WriteLine($"Query Param - Key: {query.Key}, Value: {query.Value}");
        //    }

        //    foreach (var header in httpContext.Request.Headers)
        //    {
        //        Console.WriteLine($"Header - Key: {header.Key}, Value: {header.Value}");
        //    }
        //    Console.WriteLine("============================");


        //    string role = httpContext.Request.Query["role"].ToString();


        //    if (string.IsNullOrEmpty(role))
        //    {
        //        role = httpContext.Request.Headers["role"].ToString();
        //    }

        //    Console.WriteLine($"Extracted role: '{role}'");

        //    if (string.IsNullOrEmpty(role) || !new[] { "Admin", "Manager", "Employee" }.Contains(role))
        //    {
        //        await Clients.Caller.SendAsync("ReceiveMessage", $"Role is required or invalid. Received: '{role}'");

        //        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} connected without valid role.");
        //    }
        //    else
        //    {
        //        await JoinGroupByRole(role);
        //        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} ({role}) has connected.");

        //    }

        //    await base.OnConnectedAsync();
        //}
        public override async Task OnConnectedAsync()
        {
            var role = GetUserRoleFromToken();

            if (role == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Authentication failed. Invalid or missing token.");
                Context.Abort();
                return;
            }

            Console.WriteLine($"=== Connection Info ===");
            Console.WriteLine($"Connection ID: {Context.ConnectionId}");
            Console.WriteLine($"User Role: {role}");
            Console.WriteLine("======================");

            await JoinGroupByRole(role.Value);
            await Clients.All.SendAsync("ReceiveMessage", $"User ({role}) has connected.");
         
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            string role = httpContext?.Request.Query["role"].ToString();

            if (string.IsNullOrEmpty(role))
            {
                role = httpContext?.Request.Headers["role"].ToString();
            }

            Console.WriteLine($"=== Disconnection Info ===");
            Console.WriteLine($"Connection ID: {Context.ConnectionId}");
            Console.WriteLine($"Role: {role}");
            Console.WriteLine("=========================");

            
            if (!string.IsNullOrEmpty(role) && new[] { "Admin", "Manager", "Employee" }.Contains(role))
            {
                if (role == "Admin")
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
                }
                else if (role == "Manager")
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Managers");
                }
                else if (role == "Employee")
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Employees");
                }

                await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} ({role}) has disconnected.");
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task JoinGroupByRole(UserRole role)
        {
            string groupName = GetGroupName(role);
            await Clients.All.SendAsync("ReceiveMessage", $"User ({role}) has connected.joining group {groupName}");
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        private UserRole? GetUserRoleFromToken()
        {
            var roleClaim = Context.User?.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role ||
                c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            if (roleClaim == null || string.IsNullOrEmpty(roleClaim.Value))
            {
                return null;
            }

            if (Enum.TryParse<UserRole>(roleClaim.Value, true, out var role))
            {
                return role;
            }

            return null;
        }

        private string GetGroupName(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Admins",
                UserRole.Manager => "Managers",
                UserRole.Employee => "Employees",
                _ => throw new ArgumentException($"Invalid role: {role}")
            };
        }
    }
}
