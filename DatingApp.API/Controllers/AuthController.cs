using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        // register method
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            //send username and convert to lowercase therfore we dont accept duplicate usernames

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            //check if username is already taken

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            //create user

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username

            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            //takes string of route name and object itself
            // need to get an individual user
            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userforLoginDto)
        {
            //Check if user exists
            //Checks if they have a user name and password that is stored in the DB for that particular user
            var userFromRepo = await _repo.Login(userforLoginDto.Username.ToLower(), userforLoginDto.Password);
            // if user not found in DB
            if (userFromRepo == null)
                return Unauthorized();



            //build a token to return to the user
            //token will contain user ID and users Username
            //token can be validated to server with out DB call
            // meaning the server will take a look inside the token
            // and doesnt need to get the username or ID, This is whats created below

            // (2 CLAIMS)
            // (1) - users id
            // (2) - users username
            var claims = new[]
            {
                //token will be a string
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //Signing the token to make sure it's valid
            
            //security key created
            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_config.GetSection("AppSettings:Token").Value));

            //takes security key generated above and encrypting key with hashing algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //security token descriptor, contains token expiry date and signing credentials

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //create a jwt token security handler, which allows us to create the token
            // based on the token descriptor
            // stored in token
            var tokenHandler = new JwtSecurityTokenHandler();


            var token = tokenHandler.CreateToken(tokenDescriptor);

            //write token method , to write our token into our reponse that we're sending back to the client
            return Ok(new {
            token = tokenHandler.WriteToken(token)

            });
            




        }



    }
}