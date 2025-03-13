using PersonInformation.Bll.Services;
using PersonsInformation.Dal.Entities;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = PersonsInformation.Dal.Entities.User;

namespace TgBot_UserInfo;

public class GetPersonInformationBot
{
    private static string BotToken = "7828943013:AAHmx9L8KhlQRI1kGRFqYKYm0v55qZHEhBY";
    private long AdminID = 597339158;

    private List<long> allChat = new List<long>();

    private TelegramBotClient BotClient = new TelegramBotClient(BotToken);

    private Dictionary<long, string> UserForUserInfo = new Dictionary<long, string>();

    private Dictionary<long, UserInfo> UserInfos = new Dictionary<long, UserInfo>();

    private readonly IUserService _userService;
    private readonly IUserInfoService _userInfoService;

    public GetPersonInformationBot(IUserInfoService userInfoService, IUserService userService)
    {
        _userInfoService = userInfoService;
        _userService = userService;
    }

    public string EscapeMarkdownV2(string text)
    {
        string[] specialChars = { "[", "]", "(", ")", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
        foreach (var ch in specialChars)
        {
            text = text.Replace(ch, "\\" + ch);
        }
        return text;
    }


    private bool ValidateFNameAndLName(string name)
    {
        foreach (var l in name)
        {
            if (!char.IsLetter(l) || l == ' ')
            {
                return false;
            }
        }
        return !string.IsNullOrEmpty(name) && name.Length <= 50;
    }
    private bool ValidatePhone(string phone)
    {
        foreach (var l in phone)
        {
            if (!char.IsDigit(l) || l == ' ')
            {
                return false;
            }
        }
        return phone.Length == 9;
    }
    private bool ValidateEmail(string email)
    {
        email.ToLower();

        return email.EndsWith("@gmail.com") && !string.IsNullOrEmpty(email) && email.Length <= 200 && email.Length > 10;
    }
    public async Task StartBot()
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.InlineQuery } };

        Console.WriteLine("Your bot is starting");

        BotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions
            );

        Console.ReadKey();
    }

    public IUserService Get_userService()
    {
        return _userService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, IUserService _userService, CancellationToken cancellationToken)
    {

        if (update.Type == UpdateType.Message)
        {

            var message = update.Message;
            var user = message.Chat;
            User userObject;
            Console.WriteLine($"{user.Id},  {user.FirstName}, {message.Text}");
            // bu notificatsion
            if (message.Text == "AllChat")
            {
                if (userObject.ChatId == AdminID)
                {
                    await bot.SendTextMessageAsync(user.Id, "Input words : ", cancellationToken: cancellationToken);
                    allChat.Add(AdminID);
                }
            }
            else if (allChat.Contains(AdminID))
            {
                var users = await _userService.GetAllUser();
                foreach (var u in users)
                {
                    await bot.SendTextMessageAsync(u.UserId, message.Text, cancellationToken: cancellationToken);
                }
            }

            if (message.Text == "Input information")
            {
                if (userObject.UserInfo is null)
                {
                    try
                    {
                        UserInfos.Add(user.Id, new UserInfo());
                        UserForUserInfo.Add(user.Id, "Name");
                    }
                    catch (Exception ex)
                    {
                        UserInfos.Remove(user.Id);
                        UserInfos.Add(user.Id, new UserInfo());

                        UserForUserInfo.Remove(user.Id);
                        UserForUserInfo.Add(user.Id, "Name");
                    }

                    await bot.SendTextMessageAsync(user.Id, "Input your name : ", cancellationToken: cancellationToken);
                }
                else if (userObject.UserInfo is not null)
                {
                    var userInformation = await _userInfoService.GetUserInfByID(userObject.BotUserId);
                    var userInfo = $"~You Has Already Have Informations~\n\n*Name* : _{userInformation.FirstName}_\n" +
                        $"*Last Name* : _{userInformation.LastName}_\n" +
                        $"*Email* : {userInformation.Email}\n" +
                        $"*PhoneNumber* : {userInformation.PhoneNumber}\n" +
                        $"*Adress* : `{userInformation.Address}`\n" +
                        $"*DateOfBirth* : *{userInformation.DateOfBirth}*";

                    await bot.SendTextMessageAsync(user.Id, EscapeMarkdownV2(userInfo), cancellationToken: cancellationToken, parseMode: ParseMode.MarkdownV2);
                    return;
                }
            }
            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Ism")
            {
                var validate = ValidateFNameAndLName(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Correct to Input your name !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.FirstName = message.Text;
                var ch = info.FirstName[0];
                info.FirstName = info.FirstName.Remove(0, 1);
                info.FirstName = char.ToUpper(ch) + info.FirstName;
                UserForUserInfo[user.Id] = "Last";
                await bot.SendTextMessageAsync(user.Id, "Input Last name : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Last")
            {
                var validate = ValidateFNameAndLName(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Correct to Input your last name !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.LastName = message.Text;
                var ch = info.LastName[0];
                info.LastName = info.LastName.Remove(0, 1);
                info.LastName = char.ToUpper(ch) + info.LastName;
                UserForUserInfo[user.Id] = "Ema";
                await bot.SendTextMessageAsync(user.Id, "Input last name : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Ema")
            {
                var validate = ValidateEmail(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Correct to Input Email !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.Email = message.Text;
                info.Email.ToLower();
                UserForUserInfo[user.Id] = "Pho";
                await bot.SendTextMessageAsync(user.Id, "Input phone number (form of 909009090):", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Pho")
            {
                var validate = ValidatePhone(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Correct to Input your phone number !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.PhoneNumber = message.Text;
                info.PhoneNumber = "+998" + info.PhoneNumber;
                UserForUserInfo[user.Id] = "Adr";
                await bot.SendTextMessageAsync(user.Id, "Input your Adress : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Adr")
            {
                if (message.Text.Length > 200 && !string.IsNullOrEmpty(message.Text))
                {
                    await bot.SendTextMessageAsync(user.Id, "Correct to Input your Adress !!!", cancellationToken: cancellationToken);
                    return;
                }

            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "birth")
            {
                var info = UserInfos[user.Id];
                info.UserId = userObject.BotUserId;
                info.DateOfBirth = message.Date;

                await _userInfoService.AddUserInfo(info);

                UserInfos.Remove(user.Id);
                UserForUserInfo.Remove(user.Id);
                await bot.SendTextMessageAsync(user.Id, "User Info Saqlandi", cancellationToken: cancellationToken);
            }




            if (message.Text == "See all information")
            {
                UserInfo userInformation;
                try
                {
                    userInformation = await _userInfoService.GetUserInfByID(userObject.BotUserId);//userID
                }
                catch (Exception ex)
                {
                    await bot.SendTextMessageAsync(user.Id, "User Info not found", cancellationToken: cancellationToken);
                    return;
                }

                var userInfo = $"*Ism* : _{userInformation.FirstName}_\n" +
                    $"*Familiya* : _{userInformation.LastName}_\n" +
                    $"*Email* : {userInformation.Email}\n" +
                    $"*PhoneNumber* : {userInformation.PhoneNumber}\n" +
                    $"*Adress* : `{userInformation.Address}`\n" +
                    $"*DateOfBirth* : *{userInformation.DateOfBirth}*";

                await bot.SendTextMessageAsync(user.Id, EscapeMarkdownV2(userInfo), cancellationToken: cancellationToken, parseMode: ParseMode.MarkdownV2);
            }


            if (message.Text == "Delete Information")
            {
                var userInformation = await _userService.GetUserByID(user.Id);
                if (userInformation.UserInfo is null)
                {
                    await bot.SendTextMessageAsync(user.Id, "Delete Information for \nThe first add the information", cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    await _userInfoService.DeleteUserInfo(userInformation.UserInfo.UserId);

                    await bot.SendTextMessageAsync(user.Id, "Delete information", cancellationToken: cancellationToken);
                }
            }

            if (message.Text == "/start")
            {

                if (userObject == null)
                {
                    userObject = new User()
                    {
                        DateOfBirth = DateTime.UtcNow,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        PhoneNumber = null,
                        ChatId = user.Id,

                    };


                    await _userService.AddUser(userObject);
                }
                else
                {

                    userObject.FirstName = user.FirstName;
                    userObject.LastName = user.LastName;
                    await _userService.UpdateUser(userObject);
                }

                var keyboard = new ReplyKeyboardMarkup(new[]
            {
                    new[]
                    {
                        new KeyboardButton("Fill Info"),
                        new KeyboardButton("Get all Info"),
                    },
                    new[]
                    {
                        new KeyboardButton("Delete Info"),
                    },
                })
                { ResizeKeyboard = true };

                await bot.SendTextMessageAsync(user.Id, "!!!", replyMarkup: keyboard);
                return;
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var id = update.CallbackQuery.From.Id;

            var text = update.CallbackQuery.Data;

            CallbackQuery res = update.CallbackQuery;
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
    }
}
