using MyFirstEBot;
using PersonsInformation.Dal.Entities;

namespace PersonInformation.Bll.Services;

public class UserService : IUserService
{
    private readonly MainContext _mainContext;
    public UserService(MainContext maincontext)
    {
        _mainContext = maincontext;
    }
    public async Task<long> AddUser(UserInfo user)
    {
        _mainContext.Users.Add(user);
        _mainContext.SaveChanges();
        return user.BotUserId;
    }

    public async Task DeleteUser(long Id)
    {
        var user = await GetUserByID(Id);
        _mainContext.Users.Remove(user);
        _mainContext.SaveChanges();
    }

    public async Task<List<UserInfo>> GetAllUser()
    {
        return _mainContext.Users.ToList();
    }

    public async Task<UserInfo> GetUserByID(long ID)
    {
        var user = _mainContext.Users.FirstOrDefault(ui => ui.BotUserId == ID);
        if (user == null)
        {
            throw new Exception("UserInfo Not Found");
        }
        return user;
    }

    public async Task UpdateUser(UserInfo userInfo)
    {
        var oldUser = await GetUserByID(userInfo.BotUserId);
        oldUser.BotUserId = userInfo.BotUserId;
        oldUser.FirstName = userInfo.FirstName;
        oldUser.LastName = userInfo.LastName;
        oldUser.Email = userInfo.Email;
        oldUser.Address = userInfo.Address;
        oldUser.PhoneNumber = userInfo.PhoneNumber;
        _mainContext.SaveChanges();
    }
}
