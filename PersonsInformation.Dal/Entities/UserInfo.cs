using MyFirstEBot;
namespace PersonsInformation.Dal.Entities;

public class UserInfo
{
    public long UserInfoId { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public object UserInfo { get; set; }
}
