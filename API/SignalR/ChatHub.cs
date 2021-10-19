using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Application.Comments;
using CloudinaryDotNet.Actions;
using Domain;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(CreateComment.Command command)
        {
            var comment = await _mediator.Send(command);
            // when comment is sent, "ReceiveComment" will be executed
            // and passback comment.value. Any connected clients whose belongs to 
            // specific activity will be able to see the comments 
            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            // [activityId] as the key to get all comments
            // e.g: activityId: [cmt1, cmt2, cmt3, ...]
            var activityId = httpContext.Request.Query["activityId"];
            // add connectionId to specific activity
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            var result = await _mediator.Send(new ListComments.Query { ActivityId = Guid.Parse(activityId) });
            // send all comments in current activity to users those belong to 
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}