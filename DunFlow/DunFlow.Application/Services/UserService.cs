using AutoMapper;
using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
using DunFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BaseResponse<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return BaseResponse<IEnumerable<UserDto>>.Success(userDtos);
        }
    }
}
