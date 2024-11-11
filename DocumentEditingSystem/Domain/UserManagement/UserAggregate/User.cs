using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UserManagement.UserAggregate
{
    public class User
	{
		public int Id { get; }
		public Name Name { get; private set; }
		public Username Username { get; private set; }
		public Email Email { get; private set; }
		public PhoneNumber PhoneNumber { get; private set; }
		public Password Password { get; private set;  }
		private JwtSecurityToken Token { get; set; }
		public Role Role { get; private set;  }

		public User(Name name, Username username, Email email, PhoneNumber phoneNumber, Password password, Role role)
		{
			if (name == null) throw new ArgumentNullException("Name cannot be null");
			if (username == null) throw new ArgumentNullException("Username cannot be null");
			if (email == null) throw new ArgumentNullException("Email cannot be null");
			if (phoneNumber == null) throw new ArgumentNullException("Phone number cannot be null");
			if (password == null) throw new ArgumentNullException("Password cannot be null");
			

			Name = name;
			Username = username;
			Email = email;
			PhoneNumber = phoneNumber;
			Password = password;
			Role = role;
		}

		public void ChangeName(Name name)
		{
			if(name == null) throw new ArgumentNullException("Name cannot be null");
			Name = name;
		}

		public void ChangeEmail(Email email)
		{
			if(email == null) throw new ArgumentNullException("Email cannot be null");
			Email = email;
		}

		public void ChangePhoneNumber(PhoneNumber phoneNumber)
		{
			if (phoneNumber == null) throw new ArgumentNullException("Phone number cannot be null");
			PhoneNumber = phoneNumber;
		}

		public void ChangePassword(Password password)
		{
			if (password == null) throw new ArgumentNullException("Password cannot be null");
			if (Password.Hash.SequenceEqual(password.Hash)) throw new ArgumentException("Cannot set the same password");
			Password = password;
		}

		public void ChangeRole(Role role)
		{
			if (Role == role) throw new ArgumentException("User already has this role");
			Role = role;
		}

		public void SetToken(JwtSecurityToken token)
		{
			if (token == null) throw new ArgumentNullException("Token cannot be null");
			Token = token;
		}

		public bool ValidateToken(JwtSecurityToken token)
		{
			return Token == token;
		}
		private User() { }
	}
}
