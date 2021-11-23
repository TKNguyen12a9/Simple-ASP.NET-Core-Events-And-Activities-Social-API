using System;
using Application.Core;

namespace Application.Activities
{
	public class ActivityParams : PagingParams
	{
		public bool IsGoing { get; set; } = false;
		public bool IsHosting { get; set; } = false;
		public DateTime StartDate { get; set; } = DateTime.UtcNow;
	}
}