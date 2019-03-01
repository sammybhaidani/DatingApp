using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }

        //return a type user
        //username identifies the user in the database, stored in a variable
        //password compared to the hash password
        public async Task<User> Login(string username, string password)
        {
            //go to database _context.Users
            //return a username if it matches, otherwise will return null
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if(user == null)
            return null;


            //comparing password, will return true or false
            if(!VerifyPasswordHash(password,user.PasswordHash, user.PasswordSalt))
            //if password doesnt match return null
            return null;

            return user;

        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
           {
               
               //will compute a hash from the password but also use a key passed in (passwordSalt)
               var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //for loop to calculate and compare against each element in the byte array
                for(int i = 0; i < computedHash.Length;i++)
                {
                if (computedHash[i] != passwordHash[i]) return false;
                }

           }
           // if passwords match
           return true;
        }

        public async Task<User> Register(User user, string password)
        {
            //hash will be stored in byte array variables here
            byte[]passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
            
        }
        //not returning anything
        //setting password salt and hash to a randomly generated key
        // then computing the hash
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //randomly generated key
            //uses HMACSHA512 class to generate hash and password salt
           using(var hmac = new System.Security.Cryptography.HMACSHA512())
           {
                passwordSalt = hmac.Key;
                //takes byte array
                //get password as byte array
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                //givers the password as an array of bytes

           }
            
        }

        public async Task<bool> UserExists(string username)
        {
            //comparing user against ANY user in the database
            if (await _context.Users.AnyAsync(x => x.Username == username))
            // has found a matching username
            return true;
            //if not
            return false;

            
        }
    }
}