﻿using System.Numerics;
using AutoMapper;
using IntermediaryTransactionsApp.Db.Models;
using IntermediaryTransactionsApp.Dtos.RoleDto;
using IntermediaryTransactionsApp.Dtos.UserDto;

namespace IntermediaryTransactionsApp.Config
{
	public class MapperConfig : Profile
	{
		public MapperConfig()
		{
			CreateMap<Users, CreateUserRequest>().ReverseMap();
			CreateMap<CreateUserResponse, Users>().ReverseMap();

			CreateMap<Role, RoleResponse>();


			CreateMap<Users, GetUserResponse>()
				.ForMember(dest => dest.role, opt => opt.MapFrom(src => src.Role));

		}
	}
}
