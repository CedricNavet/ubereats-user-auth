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
                if (user.Role != "Customer" || user.Role != "Commercial" || user.Role != "Admin" && user.IsValid == true)
                    return BadRequest("Invalid credentials");
                if (user != null)
                {
                    return user;
                }
                else
                {
                    return BadRequest("Invalid credentials");
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

            if (_userData.Mail != null && _userData.Password != null)
            {
                User user = await GetUser(_userData.Mail, _userData.Password);

                if(user.Role != "Commercial" || user.Role != "Admin" || user.Role != "Restorer" || user.Role == "Developer" && user.IsValid == true)
                    return BadRequest("Invalid credentials");

                if (user != null)
                {
                    return user;
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest("lack of params");
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

                new User()
                {
                    Email = user.Email,
                    Password = ComputeSha256Hash(user.Password),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = "Customer",
                    IsValid = true
                };
                _context.Users.Add(user);
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
            return await _context.Users.Where(x => x.Email == identifiant && x.Password == ComputeSha256Hash(password)).FirstOrDefaultAsync();
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
