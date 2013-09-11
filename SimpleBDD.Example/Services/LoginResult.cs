namespace SimpleBDD.Example.Services
{
	internal class LoginResult
	{
		public bool IsSuccessful { get; private set; }

		public string ResultMessage { get; private set; }

		public LoginResult(bool isSuccessful, string resultMessage)
		{
			IsSuccessful = isSuccessful;
			ResultMessage = resultMessage;
		}
	}
}