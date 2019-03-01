namespace DatingApp.API.Dtos
{
    public class UserForLoginDto
    {
        //api will check to see if username matches with whats in the DB
        public string Username { get; set; }

//compute hash and compare to hash in DB
        public string Password { get; set; }
    }
}