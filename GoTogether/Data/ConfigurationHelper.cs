﻿namespace GoTogether.Data
{
    public class ConfigurationHelper
    {
        private static IConfiguration? _configuration;

        public ConfigurationHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }        
        
        public static string GetSalt()
        {
            return _configuration.GetSection("AppSettings:HashSalt").Value;
        }

        public static string GetServerKey()
        {
            return _configuration.GetSection("AppSettings:ServerKey").Value;
        }

        public static string GetBaseUrl()
        {
            return _configuration.GetSection("AppSettings:BaseURL").Value;
        }

        public static string GetIssuer()
        {
            return _configuration.GetSection("AppSettings:Issuer").Value;
        }

        public static string GetAudience()
        {
            return _configuration.GetSection("AppSettings:Audience").Value;
        }
    }
}