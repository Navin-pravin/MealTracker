using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace AljasAuthApi.Models
{
public class ChangePasswordRequest
{
    public string UserId { get; set; } = null!;
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
}
}