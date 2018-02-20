namespace Example.Domain.Services
{
    public class LoginResult
	{
		public bool IsSuccessful { get; }

		public string ResultMessage { get; }

		public LoginResult(bool isSuccessful, string resultMessage)
		{
			IsSuccessful = isSuccessful;
			ResultMessage = resultMessage;
		}
	}
}