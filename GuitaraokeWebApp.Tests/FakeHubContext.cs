using GuitaraokeWebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GuitaraokeWebApp.Tests;

public class FakeHubContext : IHubContext<SongHub> {
	public class FakeClientProxy : IClientProxy {
		public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = new())
			=> Task.CompletedTask;
	}

	public class FakeHubClients : IHubClients {
		private readonly FakeClientProxy proxy = new();
		public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => proxy;
		public IClientProxy Client(string connectionId) => proxy;
		public IClientProxy Clients(IReadOnlyList<string> connectionIds) => proxy;
		public IClientProxy Group(string groupName) => proxy;
		public IClientProxy Groups(IReadOnlyList<string> groupNames) => proxy;
		public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => proxy;
		public IClientProxy User(string userId) => proxy;
		public IClientProxy Users(IReadOnlyList<string> userIds) => proxy;
		public IClientProxy All => proxy;
	}

	public class FakeGroupManager : IGroupManager {
		public Task AddToGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = new())
			=> Task.CompletedTask;
		public Task RemoveFromGroupAsync(string connectionId, string groupName, CancellationToken cancellationToken = new())
			=> Task.CompletedTask;
	}

	public FakeHubContext() {
		this.Clients = new FakeHubClients();
		this.Groups = new FakeGroupManager();
	}

	public IHubClients Clients { get; }
	public IGroupManager Groups { get; }
}
