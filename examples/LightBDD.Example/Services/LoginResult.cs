namespace LightBDD.Example.Services
{
    public class LoginResult
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