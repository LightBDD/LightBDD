namespace SimpleBDD.Example.Services
{
	internal class LoginResult
	{
		public LoginResult(bool isSuccessful, string resultMessage)
		{
			IsSuccessful = isSuccessful;
			ResultMessage = resultMessage;
		}

		public bool IsSuccessful { get; private set; }

		public string ResultMessage { get; private set; }
	}
}