using AutoMapper;
using RealtimeChat.MVC.Dtos.LoginDto;
using RealtimeChat.MVC.Dtos.RegisterDto;
using RtChat.EntityLayer.Concrete;

namespace RealtimeChat.MVC.Mapping
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CreateNewUserDto, User>().ReverseMap();
            CreateMap<LoginUserDto, User>().ReverseMap();
        }
    }
}
