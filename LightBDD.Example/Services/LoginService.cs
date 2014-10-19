using System.Collections.Generic;

namespace LightBDD.Example.Services
{
    public class LoginService
	{
		readonly IDictionary<string, string> _users = new Dictionary<string, string>();
		public void AddUser(string userName, string password)
		{
			_users.Add(userName, password);
		}

		public LoginResult Login(LoginRequest loginRequest)
		{
			string password;
			return (_users.TryGetValue(loginRequest.UserName, out password) && password == loginRequest.Password)
				? new LoginResult(true, string.Format("Welcome {0}!", loginRequest.UserName))
				: new LoginResult(false, "Invalid user name or password.");
		}
	}
}