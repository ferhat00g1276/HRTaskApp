
using Microsoft.AspNetCore.SignalR;

namespace API
{
    public class CommunicationHub:Hub
    {
        public async Task JoinGroupByRole(string role)
        {
            if (role == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            else if (role == "Manager")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Managers");
            }
            else if (role == "Employee")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Employees");
            }
        }

        // Qruplara mesaj göndərmək
        public async Task SendMessageToRole(string role, string message)
        {
            if (role == "Admin")
            {
                await Clients.Group("Admins").SendAsync("ReceiveMessage", message);
            }
            else if (role == "Manager")
            {
                await Clients.Group("Managers").SendAsync("ReceiveMessage", message);
            }
            else if (role == "Employee")
            {
                await Clients.Group("Employees").SendAsync("ReceiveMessage", message);
            }
        }

        // Qoşulan vaxtı istifadəçini roluna uyğun olaraq qrupa əlavə etmə
        //public override async Task OnConnectedAsync()
        //{
        //    var role = Context.GetHttpContext().Request.Query["role"]; // HTTP URL-dən rolu alırıq
        //    if (!string.IsNullOrEmpty(role))
        //    {
        //        await JoinGroupByRole(role);
        //    }
        //    await base.OnConnectedAsync();
        //}
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            string role = httpContext.Request.Query["role"];
            if (string.IsNullOrEmpty(role))
            {
                role = httpContext.Request.Headers["role"];
            }

            Console.WriteLine($"Received role: {role}");

            if (string.IsNullOrEmpty(role) || !new[] { "Admin", "Manager", "Employee" }.Contains(role))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Role is required or invalid.");
                return; // disconnect etmə
            }

            await JoinGroupByRole(role);
            await base.OnConnectedAsync();
        }


    }
}
