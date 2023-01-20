﻿namespace blurd_api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public float Coins { get; set; } = 0.0f;
        public float TotalCoins { get; set; } = 0.0f;
    }
}
