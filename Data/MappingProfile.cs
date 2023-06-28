using AutoMapper;
using ChatApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Data
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Entities.Inquiry, Models.Inquiry>()
				.ReverseMap()
				.ForMember(e => e.Time, opt => opt.MapFrom(_ => DateTime.UtcNow));
		}
	}
}
