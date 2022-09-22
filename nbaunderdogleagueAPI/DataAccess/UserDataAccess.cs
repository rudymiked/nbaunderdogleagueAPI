namespace nbaunderdogleagueAPI.DataAccess
{
    public interface IGetUserDataAccess
    {
        List<User> GetUserData();
    }
    public class UserDataAccess : IGetUserDataAccess
    {
        public List<User> GetUserData()
        {
            List<User> userData = new()
            {
                new User()
                {
                    Email = "rudymiked@gmail.com",
                    Team = "Lakers"
                },
                new User()
                {
                    Email = "andrewleeshields@gmail.com",
                    Team = "Celtics"
                }
            };

            return userData;
        }
    }
}
