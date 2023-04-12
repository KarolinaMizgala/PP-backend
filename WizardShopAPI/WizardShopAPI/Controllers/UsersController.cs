using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WizardShopAPI.DTOs;
using WizardShopAPI.Managers;
using WizardShopAPI.Mappers;
using WizardShopAPI.Models;

namespace WizardShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WizardShopDbContext _context;

        public UsersController(WizardShopDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

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

        // POST: api/Users/Register
        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterDto _userDto)
        {
            //validation check            
            if (!ModelState.IsValid)
            {
                return BadRequest("Wrong input format");
            }
            //checking if its a new user--> checking if email and username are unique
            if (!UniqueEmail(_userDto.Email))
            {
                return BadRequest("Email is not unique");
            }
            if (!UniqueUsername(_userDto.Username))
            {
                return BadRequest("Username is not unique");
            }

            int newId = GetNewUserId();

            //password hashing
            PasswordManager passManager = new PasswordManager(_userDto.Password);
            _userDto.Password = passManager.ComputedHashedPassword;

            User user = UserMapper.RegisterDtoToUser(ref _userDto, ref newId);

            //for future login
            user.PasswordSalt = passManager.Salt;

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // POST: api/Users/Login
        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<User>> GetUser([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await _context.Users.Where(x => x.Email.Equals(loginDto.Email)).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }
            //checking password
            string hashedPasswordFromDB = user.Password;
            string saltFromDB = user.PasswordSalt;

            PasswordManager passwordManager = new PasswordManager(loginDto.Password, saltFromDB);
            if (!passwordManager.Compare(hashedPasswordFromDB))
            {
                return Unauthorized("Incorrect password");
            }

            return user;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        /// <summary>
        /// Checks if user with passed id already exists in database
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>true if user exists</returns>
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }

        /// <summary>
        /// Calculates new, unique user id
        /// </summary>
        /// <returns>user id</returns>
        private int GetNewUserId()
        {
            if (!_context.Users.Any())
            {
                return 1;
            }

            return _context.Users.Max(x => x.UserId) + 1;
        }

        /// <summary>
        /// Checks if passed email value already exists in database
        /// </summary>
        /// <param name="email">email value</param>
        /// <returns>true if email is unique</returns>
        private bool UniqueEmail(String email)
        {
            return !_context.Users.Any(x => x.Email == email);
        }

        /// <summary>
        /// Checks if passed username value already exists in database
        /// </summary>
        /// <param name="username">username value</param>
        /// <returns>true if username is unique</returns>
        private bool UniqueUsername(String username)
        {
            return !_context.Users.Any(x => x.Username == username);
        }
    }
}
