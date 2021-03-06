using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ubereats_user_auth.Model;

namespace ubereats_user_auth.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserLoginRegister : ControllerBase
    {
        public class UserData
        {
            public string Mail { get; set; }
            public string Password { get; set; }
        }

        private readonly UberEatsAuthDBContext _context;

        public UserLoginRegister(UberEatsAuthDBContext context)
        {
            _context = context;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] UserData _userData)
        {
            
            if (_userData.Mail != null && _userData.Password != null)
            {
                User user = await GetUser(_userData.Mail, _userData.Password);

                if (user != null)
                {
                    if (user.Role.Trim() != "Customer" || user.IsValid == false)
                        return Unauthorized("Can't connect with this account");

                    user.Password = "";
                    return user;
                }
                else
                {
                    return BadRequest("Cannot find user");
                }
            }
            else
            {
                return BadRequest("lack of params");
            }
        }

        // POST: api/auth/login
        [HttpPost("login/admin")]
        public async Task<ActionResult<User>> LoginAdmin([FromBody] UserData _userData)
        {
            try
            {
                if (_userData.Mail != null && _userData.Password != null)
                {
                    User user = await GetUser(_userData.Mail, _userData.Password);

                    if (user != null)
                    {
                        if (user.Role.Trim() == "Customer" || user.IsValid != true)
                            return Unauthorized("Invalid credentials");

                        user.Password = "";
                        return user;
                    }
                    else
                    {
                        return BadRequest("Cannot find user");
                    }
                }
                else
                {
                    return BadRequest("lack of params");
                }
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

           
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            try
            {
                if (_context.Users.Any(client => client.Email == user.Email))
                    return Unauthorized();

                User userR = new User()
                {
                    Email = user.Email,
                    Password = ComputeSha256Hash(user.Password),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = "Customer",
                    IsValid = true
                };
                _context.Users.Add(userR);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
                throw;
            }

        }

        private async Task<User> GetUser(string identifiant, string password)
        {
            return await _context.Users.Where(x => x.Email.Trim() == identifiant && x.Password.Trim() == ComputeSha256Hash(password)).FirstOrDefaultAsync();
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
