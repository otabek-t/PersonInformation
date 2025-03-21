﻿namespace PersonsInformation.Dal.Entities;

public class User
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public UserInfo UserInfo { get; set; }
}
