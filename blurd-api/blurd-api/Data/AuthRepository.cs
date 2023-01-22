using blurd_api.Models;
using Microsoft.EntityFrameworkCore;

namespace blurd_api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
            if(user is null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) 
            {
                response.Success = false;
                response.Message = "Wrong password";

            }
            else
            {
                response.Data = user.Id.ToString();
            }


            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();
            if(await UserExsists(user.Username))
            {
                response.Success = false;
                response.Message = "User alredy exists";
                return response;
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.users.Add(user);
            await _context.SaveChangesAsync();
   
            response.Data = user.Id;
            return response;
        }

        public async Task<bool> UserExsists(string username)
        {
            if (await _context.users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
