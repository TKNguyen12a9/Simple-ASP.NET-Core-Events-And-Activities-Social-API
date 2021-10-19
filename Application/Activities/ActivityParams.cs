using System;
using Application.Core;

namespace Application.Activities
{
	public class ActivityParams : PagingParams
	{
		public bool IsGoging { get; set; }
		public bool IsHosting { get; set; }
		public DateTime StartDate { get; set; } = DateTime.UtcNow;
	}
}