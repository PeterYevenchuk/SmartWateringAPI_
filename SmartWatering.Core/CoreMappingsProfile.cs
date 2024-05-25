using AutoMapper;
using SmartWatering.Core.SensorInfo;
using SmartWatering.Core.UserInfo;
using SmartWatering.Core.UserInfo.CreateUser;
using SmartWatering.Core.UserMessages;
using SmartWatering.Core.WeatherSettings.Messages;
using SmartWatering.DAL.Models;

namespace SmartWatering.Core;

public class CoreMappingsProfile : Profile
{
    public CoreMappingsProfile()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(x => x.Password, a => a.Ignore());

        CreateMap<User, UserDTO>();

        CreateMap<UserDTO, User>();

        CreateMap<SensorInformation, SensorInfoDTO>();

        CreateMap<Watering, WateringDTO>();

        CreateMap<ResultMessage, MessageModel>();
    }
}
