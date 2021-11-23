using System;
using System.Text.Json.Serialization;

namespace Application.Profiles
{
	public class UserActivityProfileDTO
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public string Category { get; set; }
		public DateTime Date { get; set; }
		[JsonIgnore] // hide this property to client
		public string HostUsername { get; set; }
	}
}