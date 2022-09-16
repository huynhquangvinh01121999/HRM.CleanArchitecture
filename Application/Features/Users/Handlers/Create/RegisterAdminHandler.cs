﻿using Application.DTOs.AuthenticateDto;
using Application.DTOs.ResultDto;
using Application.Features.Users.Commands.Create;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Helpers;

namespace Application.Features.Users.Handlers.Create
{
    public class RegisterAdminHandler : IRequestHandler<RegisterAdminCommand, HandlerResult<RegisterResponse>>
    {
        private readonly UserManager<AppUsers> _userManager;
        private readonly RoleManager<AppRoles> _roleManager;

        public RegisterAdminHandler(UserManager<AppUsers> userManager, RoleManager<AppRoles> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<HandlerResult<RegisterResponse>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            var isUsername = RegexHandle.IsUsername(request.Username);

            if (!isUsername)
                return new HandlerResult<RegisterResponse>().Failed(Constant.Message.USERNAME_ERROR);

            var userExists = await _userManager.FindByNameAsync(request.Username);

            if (userExists != null)
                return new HandlerResult<RegisterResponse>().Failed(Constant.Message.USER_EXIST);

            // validation
            var isPhoneNumber = RegexHandle.IsPhoneNumber(request.PhoneNumber);
            if (!isPhoneNumber)
                return new HandlerResult<RegisterResponse>().Failed("Invalid phone number!");

            var isEmail = RegexHandle.IsEmail(request.Email);
            if (!isEmail)
                return new HandlerResult<RegisterResponse>().Failed("Invalid email!");

            AppUsers user = new AppUsers()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, request.Password);

            // add role user for account
            if (await _roleManager.RoleExistsAsync(Constant.RoleValue.Admin))
                await _userManager.AddToRoleAsync(user, Constant.RoleValue.Admin);

            if (!result.Succeeded)
            {
                string message = "";
                foreach (var item in result.Errors)
                {
                    message += item.Description + " ";
                }
                return new HandlerResult<RegisterResponse>().Failed(message);
            }

            return new HandlerResult<RegisterResponse>().Successed(Constant.Message.CREATED_SUCCESSES, new RegisterResponse
            {
                UserId = user.Id,
                Username = user.UserName
            });
        }
    }
}
