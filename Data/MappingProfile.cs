using AutoMapper;

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
