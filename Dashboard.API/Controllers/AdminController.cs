using AutoMapper;
using Dashboard.API.Data;
using Dashboard.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Dashboard.API.Models;
using System.Security.Claims;
using System;
using System.Linq;
using GoogleMaps.LocationServices;
using Microsoft.Extensions.Configuration;

namespace Dashboard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly ISettingsRepository _settingsRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AdminController(IUserRepository userRepo, ISettingsRepository settingsRepo, IMapper mapper, IConfiguration config)
        {
            _config = config;
            _mapper = mapper;
            _userRepo = userRepo;
            _settingsRepo = settingsRepo;
        }

        private async Task<bool> AdminValidate()
        {
            string clientUsername = User.FindFirst(ClaimTypes.Name).Value;
            return await _userRepo.IsAdmin(clientUsername);
        }

        [HttpPost]
        [Route("debug")]
        public async Task<IActionResult> Debug()
        {
            if (!await AdminValidate())
            {
                return Unauthorized("You need to be an admin for that!");
            }

            var geoCodingApiKey = _config.GetSection("AppSettings:GeoCodingAPIKey").Value;
            var locationService = new GoogleLocationService("AIzaSyDy4xtLv_hAsSB_qlJmmhGYlaVEsBKcjsw");
            var address = locationService.GetAddressFromLatLang(47.5337201, 21.5813013);
            //_settingsRepo.AddDefaultSettings("john");
            return Ok(address);
        }

        [HttpGet]
        [Route("userlist")]
        public async Task<IActionResult> GetUsers()
        {
            if (!await AdminValidate())
            {
                return Unauthorized("You need to be an admin for that!");
            }

            var users = await _userRepo.GetAllUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet]
        [Route("user/{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            if (!await AdminValidate())
            {
                return Unauthorized("You need to be an admin for that!");
            }

            var user = await _userRepo.GetUser(username);
            var userToReturn = _mapper.Map<UserForSingleDTO>(user);
            return Ok(userToReturn);
        }
    }
}