using System;
using System.Linq;
using Application.Activities;
using AutoMapper;
using Domain;
using Application.Profiles;
using Application.Comments;
using Application.Interfaces;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        // private readonly IUserAccessor _userAccessor;

        // public MappingProfiles(IUserAccessor userAccessor)
        // {
        //     _userAccessor = userAccessor;
        // }
        public MappingProfiles()
        {
            // _userAccessor = userAccessor;
            // for handling: to know yourself is included in follower list of another user
            string currentUsername = null;
            CreateMap<Activity, Activity>();

            CreateMap<Activity, ActivityDTO>()
                .ForMember(d => d.HostUsername,
                    o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost).ApplicationUser.UserName));

            CreateMap<ActivityAttendee, AttendeeDTO>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.ApplicationUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.ApplicationUser.Bio))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.ApplicationUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.ApplicationUser.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.ApplicationUser.Followings.Count))
                .ForMember(d => d.Following, o => o.MapFrom(s => s.ApplicationUser.Followers.Any(x => x.Observer.UserName == currentUsername)));

            CreateMap<ApplicationUser, AttendeeProfile>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(s => s.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingsCount, o => o.MapFrom(s => s.Followings.Count))
                .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));

            CreateMap<Comment, CommentDTO>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}

