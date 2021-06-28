using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ubereats_user_auth.Model;

namespace ubereats_user_auth.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UberEatsAuthDBContext _context;

        public UsersController(UberEatsAuthDBContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var result = await _context.Users.Where(x => x.Role.Trim() == "Customer").ToListAsync();
            foreach (var item in result)
            {
                item.RefreshToken = "";
                item.Password = "";
            }
            return result;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.IsValid == false)
                return NotFound();

            user.Password = "";
            user.RefreshToken = "";
            return user;
        }


        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> PutUser(int id, User user)
        {
            
            if (id != user.Id)
            {
                return BadRequest("WRONG ID");
            }
            User userInDB = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (userInDB != null)
            {
                userInDB.FirstName = user.FirstName;
                userInDB.LastName = user.LastName;
                userInDB.Email = user.Email;
                userInDB.IsValid = user.IsValid;

                _context.Users.Update(userInDB);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

            return NoContent();
        }

        //// POST: api/Users
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<User>> PostUser(User user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.IsValid = false;
             _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
