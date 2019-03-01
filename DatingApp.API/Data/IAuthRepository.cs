using System.Threading.Tasks;
using DatingApp.API.Models; //for user

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         //registering the user
        Task <User> Register(User user, string password);

        //login - the information the user has to supply with 
        //to check their username and password with whats in the database
        Task<User> Login(string username, string password);


         //does user exist, will check against to see if the username
         //is already taken
        Task<bool> UserExists(string username);






    }
}