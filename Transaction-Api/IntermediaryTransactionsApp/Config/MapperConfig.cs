using System.Numerics;
using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.UserDto;

namespace IntermediaryTransactionsApp.Config
{
	public class MapperConfig : Profile
	{
		public MapperConfig()
		{
			CreateMap<Users, CreateUserRequest>().ReverseMap();
			CreateMap<CreateUserResponse, Users>().ReverseMap();
			CreateMap<Users, GetUserResponse>()
				.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
				.ForMember(dest => dest.RoleDescription, opt => opt.MapFrom(src => src.Role.Description))
				.ReverseMap();

		}
	}
}
