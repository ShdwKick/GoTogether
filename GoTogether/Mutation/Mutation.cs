﻿using System.Diagnostics;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Data.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Linq;
using GraphQLServer.Services.RecoveryService;
using GraphQLServer.Services.TripService;
using Server.Services;

namespace GraphQLServer
{
    public class Mutation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataBaseConnection _dataBaseConnection;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ITripService _tripService;


        public Mutation(IHttpContextAccessor httpContextAccessor, DataBaseConnection dataBaseConnection,
            IEmailService emailService, IUserService userService, ITripService tripService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataBaseConnection = dataBaseConnection;
            _emailService = emailService;
            _userService = userService;
            _tripService = tripService;
        }


        [GraphQLDescription(
            "Отправляет на указанную почту письмо с 6-ти значным числовым кодом для востановления пароля")]
        public async Task<bool> SendRecoveryEmail(string address)
        {
            if (!Helpers.IsValidEmail(address))
                throw new ArgumentException("INVALID_EMAIL_ADDRESS_PROBLEM");
            return await _emailService?.SendRecoveryEmail(address);
        }

        [GraphQLDescription("Отправляет на почту письмо для подтверждения почты, адрес берётся из jwt токена")]
        public async Task<bool> SendEmailConfirmationEmail()
        {
            return await _emailService?.SendEmailConfirmationEMail();
        }

        [GraphQLDescription("Мутация для создания пользователя, возвращает jwt токен")]
        public async Task<string> CreateUser(UserForCreate user, Guid? roleGuid = null)
        {
            return await _userService.CreateUser(user, roleGuid);
        }

        [GraphQLDescription("Мутация для авторизации пользователя, возвращает новый jwt токен")]
        public async Task<string> LoginUser(string login, string password)
        {
            return await _userService.LoginUser(login, password);
        }

        [GraphQLDescription("Мутация для создания новой поезки, возвращает полную информацию о поездке")]
        public async Task<Trip> CreateTrip(TripForCreate trip)
        {
            var author = await Helpers.GetUserFromHeader(_dataBaseConnection, _httpContextAccessor);

            var newTrip = new Trip()
            {
                id = Guid.NewGuid(),
                c_name = trip.c_name,
                c_description = trip.c_description,
                d_start_date = trip.d_start_date,
                d_end_date = trip.d_end_date,
                f_author = author.id,
            };

            await _dataBaseConnection.Trips.AddAsync(newTrip);

            await _dataBaseConnection.SaveChangesAsync();
            return newTrip;
        }

        // public async Task<string> LoginViaVK(string vk_token)
        // {
        //     var userData = await _dataBaseConnection.Users.FirstOrDefaultAsync(q => q.c_vk_token == vk_token);
        //
        //     if (userData == null)
        //         throw new ArgumentException("USER_NOT_FOUND_PROBLEM");
        //
        //     return await GenerateNewTokenForUser(userData);
        // }
        //
        // public async Task<string> LoginViaYandex(string yandex_token)
        // {
        //     var userData = await _dataBaseConnection.Users.FirstOrDefaultAsync(q => q.c_vk_token == yandex_token);
        //
        //     if (userData == null)
        //         throw new ArgumentException("USER_NOT_FOUND_PROBLEM");
        //
        //     return await GenerateNewTokenForUser(userData);
        // }
        //
        // public async Task<string> LoginViaGoogle(string google_token)
        // {
        //     var userData = await _dataBaseConnection.Users.FirstOrDefaultAsync(q => q.c_vk_token == google_token);
        //
        //     if (userData == null)
        //         throw new ArgumentException("USER_NOT_FOUND_PROBLEM");
        //
        //     return await GenerateNewTokenForUser(userData);
        // }


        //TODO: убрать после тестирования регистрации
        [GraphQLDescription("Временная мутация для дропа бд пользователей :)")]
        public async Task DeleteAllUsers()
        {
            _dataBaseConnection.Users.ExecuteDelete();
            await _dataBaseConnection.SaveChangesAsync();
        }


        [GraphQLDescription(
            "Если jwt токен просрочен, кидаешь его сюда и сервер пытается его обновить, " +
            "если всё хорошо то отправляет новый токен, иначе ловишь ошибку в лицо")]
        public async Task<string> TryRefreshToken(string oldToken)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(oldToken) as JwtSecurityToken;
            if (jwtToken == null) throw new ArgumentException("INVALID_TOKEN_PROBLEM");

            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (claim == null) throw new ArgumentException("INVALID_TOKEN_CLAIMS_PROBLEM");

            var user = await _dataBaseConnection.Users.FirstOrDefaultAsync(u => u.id.ToString() == claim.Value);
            if (user == null) throw new ArgumentException("TOKEN_GENERATION_USER_NOT_FOUND_PROBLEM");

            var authorizationToken =
                await _dataBaseConnection.Authorization.FirstOrDefaultAsync(q => q.id == user.f_authorization_token);
            if (authorizationToken == null) throw new ArgumentException("OLD_TOKEN_NOT_FOUND_PROBLEM");

            if (Helpers.ComputeHash(oldToken) != authorizationToken.c_hash)
                throw new ArgumentException("CORRUPTED_TOKEN_DETECTED_PROBLEM");


            authorizationToken.c_token =
                new JwtSecurityTokenHandler().WriteToken(Helpers.GenerateNewToken(user.id.ToString()));
            authorizationToken.c_hash = Helpers.ComputeHash(authorizationToken.c_token);
            await _dataBaseConnection.SaveChangesAsync();
            return authorizationToken.c_token;
        }


        [GraphQLDescription(
            "Присылаешь код с почты, почту и новый пароль, " +
            "если всё ок то обновляется пароль и возвращается true, иначе экзепш в лицо")]
        public async Task<bool> PasswordRecovery(string email, string code, string newPassword)
        {
            var user = await _dataBaseConnection.Users.FirstOrDefaultAsync(u => u.c_email == email);
            if (user == null)
                throw new ArgumentException("USER_NOT_FOUND_PROBLEM");

            var recoveryCode = await _dataBaseConnection.RecoveryCodes.FirstOrDefaultAsync(u => u.c_email == email);
            if (recoveryCode == null)
                throw new ArgumentException("RECOVERY_CODE_NOT_FOUND_PROBLEM");

            if (DateTime.UtcNow > recoveryCode.d_expiration_time)
                throw new ArgumentException("RECOVERY_CODE_WAS_EXPIRED_PROBLEM");

            if (!recoveryCode.n_code.ToString().Equals(code))
                throw new ArgumentException("RECOVERY_CODE_NOT_CORRECT_PROBLEM");

            user.c_password = Helpers.ComputeHash(newPassword);
            await _dataBaseConnection.SaveChangesAsync();
            return true;
        }

        [Authorize]
        [GraphQLDescription("AUTHORIZE- Мутация для обновления пароля, true если всё прошло хорошо")]
        public async Task<bool> RefreshPassword(string oldPassword, string newPassword)
        {
            var user = await Helpers.GetUserFromHeader(_dataBaseConnection, _httpContextAccessor);

            var oldPasswordHash = Helpers.ComputeHash(oldPassword);
            if (oldPasswordHash != user.c_password)
                throw new ArgumentException("OLD_PASSWORD_NOT_CORRECT_PROBLEM");

            user.c_password = Helpers.ComputeHash(newPassword);
            await _dataBaseConnection.SaveChangesAsync();
            return true;
        }

        [Authorize]
        [GraphQLDescription("Отправить письма с приглашением для присоединенния к поездке(комнате)")]
        public async Task<bool> SendInviteEmail(List<string> emails, Guid tripId)
        {
            //TODO: добавить проверку роли того кто пытается ивайтить
            foreach (var email in emails)
            {
                await _emailService.SendInviteEmail(email, tripId);
            }

            return true;
        }
    }
}