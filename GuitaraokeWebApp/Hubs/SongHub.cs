using Microsoft.AspNetCore.SignalR;

namespace GuitaraokeWebApp.Hubs {
	public class SongHub : Hub {
		public async Task JoinGroup(string groupName) {
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			await Clients.Caller.SendAsync("blah", "SongHub", $"Current user added to {groupName} group");
		}
	}
}
