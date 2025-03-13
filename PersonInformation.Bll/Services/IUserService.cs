using PersonsInformation.Dal.Entities;

namespace PersonInformation.Bll.Services
{
    public interface IUserService
    {
        Task<long> AddUser(UserInfo user);
        Task DeleteUser(long Id);
        Task<UserInfo> GetUserByID(long ID);
        Task UpdateUser(UserInfo user);
        Task<List<UserInfo>> GetAllUser();
        Task AddUser(User userObject);
        Task UpdateUser(User userObject);
    }
}