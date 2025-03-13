using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstEBot;
using PersonsInformation.Dal.Entities;

namespace PersonInformation.Bll.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly MainContext _mainContext;
        public UserInfoService(MainContext maincontext)
        {
            _mainContext = maincontext;
        }
        public async Task<long> AddUserInfo(UserInfo userInfo)
        {
            _mainContext.UserInfos.Add(userInfo);
            _mainContext.SaveChanges();
            return userInfo.UserInfoId;
        }

        public async Task DeleteUserInfo(long Id)
        {
            var userInfo = await GetUserInfByID(Id);
            _mainContext.UserInfos.Remove(userInfo);
            _mainContext.SaveChanges();
        }

        public async Task<List<UserInfo>> GetAllUserInfos()
        {
            return _mainContext.UserInfos.ToList();
        }

        public async Task<UserInfo> GetUserInfByID(long ID)
        {
            var userInfo = _mainContext.UserInfos.FirstOrDefault(ui => ui.UserId == ID);
            if (userInfo == null)
            {
                throw new Exception("UserInfo Not Found");
            }
            return userInfo;
        }

        public async Task UpdateUserInfo(UserInfo userInfo)
        {
            var oldPersonInfo = await GetUserInfByID(userInfo.UserId);
            oldPersonInfo.UserInfoId = userInfo.UserId;
            oldPersonInfo.UserId = userInfo.UserId;
            oldPersonInfo.FirstName = userInfo.FirstName;
            oldPersonInfo.LastName = userInfo.LastName;
            oldPersonInfo.Address = userInfo.Address;
            oldPersonInfo.PhoneNumber = userInfo.PhoneNumber;
            oldPersonInfo.Email = userInfo.Email;
            _mainContext.SaveChanges();
        }
    }
}
