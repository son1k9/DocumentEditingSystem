using API.Domain.ValueObjects;
using API.Dtos.Read;
using API.Dtos.Write;
using API.Infrastructure.Services.Interfaces;
using API.TokenConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace DocumentEditingSystem.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserManagementService _userManagementService;

		public UsersController(IUserManagementService userManagementService)
		{
			_userManagementService = userManagementService;
		}

		[HttpPost("Authorize")]
		public IResult Autorize(string username, string password)
		{
			try
			{
				Username name = new Username(username);
				Password pwd = new Password(password);
				var token = _userManagementService.Autorize(name, pwd);
				var response = new { access_token = token.Result.AccessToken, refresh_token = token.Result.RefreshToken, username = username };
				return Results.Json(response);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
			
		}

		[HttpPost("GetAccessToken")]
		public async Task<IResult> GetAccessToken(string refreshToken, string username)
		{
			try
			{
				Username name = new Username(username);
				var jwt = await _userManagementService.GetAccessToken(name, refreshToken);
				var response = new { access_token = jwt, username = username };
				return Results.Json(response);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}

		}

		[HttpPost("Register")]
		public async Task<IResult> Register(UserW user)
		{
			try
			{
				var token = await _userManagementService.Register(user);
				var response = new { access_token = token.AccessToken, refresh_token = token.RefreshToken};
				return Results.Json(response);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
			
		}

		[HttpPost("ChangeName")]
		[Authorize]
		public async Task<IResult> ChangeName(string firstName, string lastName)
		{
			string userName = User.Identity.Name;

			try
			{
				Name newName = new Name(firstName, lastName);
				Username username = new Username(userName);
				UserR userR = await _userManagementService.ChangeName(newName, username);
				return Results.Ok(userR);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
		}

		[HttpPost("ChangeEmail")]
		[Authorize]
		public async Task<IResult> ChangeEmail(string email)
		{
			string userName = User.Identity.Name;

			try
			{
				Email newEmail = new Email(email);
				Username username = new Username(userName);
				UserR userR = await _userManagementService.ChangeEmail(newEmail, username);
				return Results.Ok(userR);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
		}

		[HttpPost("ChangePassword")]
		[Authorize]
		public async Task<IResult> ChangePassword(string oldPassword, string newPassword)
		{
			string userName = User.Identity.Name;

			try
			{
				Password newPwd = new Password(newPassword);
				Password oldPwd = new Password(oldPassword);
				Username username = new Username(userName);
				UserR userR = await _userManagementService.ChangePassword(newPwd, oldPwd, username);
				return Results.Ok(userR);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
		}

		[HttpPost("ChangePhoneNumber")]
		[Authorize]
		public async Task<IResult> ChangePhoneNumber(string newPhone)
		{
			string userName = User.Identity.Name;

			try
			{
				PhoneNumber newNumber = new PhoneNumber(newPhone);
				Username username = new Username(userName);
				UserR userR = await _userManagementService.ChangePhoneNumber(newNumber, username);
				return Results.Ok(userR);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
		}

		[HttpPost("UserData")]
		[Authorize]
		public async Task<IResult> GetUserData()
		{
			string userName = User.Identity.Name;

			try
			{
				Username username = new Username(userName);
				UserR userR = await _userManagementService.GetUserData(username);
				return Results.Ok(userR);
			}
			catch (ArgumentException ex)
			{
				return Results.BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return Results.Problem(ex.Message);
			}
		}

		[HttpPost("Unautorize")]
		[Authorize]
		public async Task<IResult> Unautorize()
		{
			string username = User.Identity.Name;
			var result = await _userManagementService.Unautorize(new Username(username));

			if (!result) return Results.Problem("Error during deauthentication");
			return Results.Ok(result);

			
		}
	}
}
