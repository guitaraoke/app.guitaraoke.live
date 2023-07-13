using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace GuitaraokeWebApp.Data;

public interface IUserTracker {
	User GetUser();
}