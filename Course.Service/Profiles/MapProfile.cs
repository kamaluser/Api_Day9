using AutoMapper;
using Course.Core.Entities;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Dtos.StudentDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Profiles
{
    public class MapProfile:Profile
    {
        private readonly IHttpContextAccessor _accessor;

        public MapProfile(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            var uriBuilder = new UriBuilder(_accessor.HttpContext.Request.Scheme, _accessor.HttpContext.Request.Host.Host, _accessor.HttpContext.Request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            string baseUrl = uriBuilder.Uri.AbsoluteUri;


            //group

            CreateMap<Group, GroupGetDto>();
            CreateMap<GroupCreateDto, Group>();

            //student

            CreateMap<StudentCreateDto, Student>();
            CreateMap<Student, StudentGetDto>()
                .ForMember(dest=>dest.Age, s=>s.MapFrom(s=>DateTime.Now.Year-s.BirthDate.Year))
                .ForMember(dest=>dest.PhotoUrl, s=>s.MapFrom(s=>baseUrl+"uploads/student/"+s.Photo));
        }
    }
}
