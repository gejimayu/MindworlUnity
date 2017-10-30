using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class CanvasController : MonoBehaviour {

	public Button bt;
	const string clientID = "581786658708-elflankerquo1a6vsckabbhn25hclla0.apps.googleusercontent.com";
	const string clientSecret = "3f6NggMbPtrmIBpgx-MK2xXK";
	const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
	const string tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";
	const string userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

	// Use this for initialization
	void Start () {
		PlayerPrefs.DeleteAll();

		Button temp = bt.GetComponent<Button> ();
		temp.onClick.AddListener (loginstart);
	}

	void loginstart() {
		// Generates state and PKCE values.
		string state = randomDataBase64url(32);
		string code_verifier = randomDataBase64url(32);
		string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
		const string code_challenge_method = "S256";

		// Creates a redirect URI using an available port on the loopback address.
		string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
		Debug.Log("redirect URI: " + redirectURI);

		// Creates an HttpListener to listen for requests on that redirect URI.
		var http = new HttpListener();
		http.Prefixes.Add(redirectURI);
		Debug.Log("Listening..");
		http.Start();

		// Creates the OAuth 2.0 authorization request.
		string authorizationRequest = string.Format("{0}?response_type=code&scope=email&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
			authorizationEndpoint,
			System.Uri.EscapeDataString(redirectURI),
			clientID,
			state,
			code_challenge,
			code_challenge_method);

		// Opens request in the browser.
		System.Diagnostics.Process.Start(authorizationRequest);

		// Waits for the OAuth authorization response.
		var context = http.GetContext();

		// Sends an HTTP response to the browser.
		var response = context.Response;
		string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
		var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
		response.ContentLength64 = buffer.Length;
		var responseOutput = response.OutputStream;
		responseOutput.Write (buffer, 0, buffer.Length);
		responseOutput.Close();
		http.Stop();
		Debug.Log("HTTP server stopped.");

		// Checks for errors.
		if (context.Request.QueryString.Get("error") != null)
		{
			Debug.Log(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
			return;
		}
		if (context.Request.QueryString.Get("code") == null
			|| context.Request.QueryString.Get("state") == null)
		{
			Debug.Log("Malformed authorization response. " + context.Request.QueryString);
			return;
		}

		// extracts the code
		var code = context.Request.QueryString.Get("code");
		var incoming_state = context.Request.QueryString.Get("state");

		// Compares the receieved state to the expected value, to ensure that
		// this app made the request which resulted in authorization.
		if (incoming_state != state)
		{
			Debug.Log(String.Format("Received request with invalid state ({0})", incoming_state));
			return;
		}
		Debug.Log("Authorization code: " + code);

		// Starts the code exchange at the Token Endpoint.
		performCodeExchange(code, code_verifier, redirectURI);
	}

	void performCodeExchange(string code, string code_verifier, string redirectURI)
	{
		Debug.Log("Exchanging code for tokens...");

		// builds the  request
		string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
		string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&scope=&grant_type=authorization_code",
			code,
			System.Uri.EscapeDataString(redirectURI),
			clientID,
			code_verifier,
			clientSecret
		);
		ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
		// sends the request
		HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
		tokenRequest.Method = "POST";
		tokenRequest.ContentType = "application/x-www-form-urlencoded";
		tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
		byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
		tokenRequest.ContentLength = _byteVersion.Length;
		Stream stream = tokenRequest.GetRequestStream();
		stream.Write(_byteVersion, 0, _byteVersion.Length);
		stream.Close();

		try
		{
			// gets the response
			WebResponse tokenResponse = tokenRequest.GetResponse();
			using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
			{
				// reads response body
				string responseText = reader.ReadToEnd();
				Debug.Log(responseText);

				//converts to dictionary
				Page tokenEndpointDecoded = JsonUtility.FromJson<Page>(responseText);

				string access_token = tokenEndpointDecoded.access_token;
				userinfoCall(access_token);
			}
		}
		catch (WebException ex)
		{
			Debug.Log ("asu gagal");
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				var response = ex.Response as HttpWebResponse;
				if (response != null)
				{
					Debug.Log("HTTP: " + response.StatusCode);
					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						// reads response body
						string responseText = reader.ReadToEnd();
						Debug.Log(responseText);
					}
				}

			}
		}
	}

	void userinfoCall(string access_token)
	{
		Debug.Log("Making API Call to Userinfo...");

		// builds the  request
		string userinfoRequestURI = "https://www.googleapis.com/oauth2/v3/userinfo";

		// sends the request
		HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
		userinfoRequest.Method = "GET";
		userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
		userinfoRequest.ContentType = "application/x-www-form-urlencoded";
		userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

		// gets the response
		WebResponse userinfoResponse = userinfoRequest.GetResponse();
		using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
		{
			// reads response body
			string userinfoResponseText = userinfoResponseReader.ReadToEnd();
			Profile deserializedResponse = JsonUtility.FromJson<Profile>(userinfoResponseText);
			string userEmail = deserializedResponse.email;
			//Debug.Log (userEmail);
			PlayerPrefs.SetString ("Email User", userEmail);
		}
		SceneManager.LoadScene ("MainScene", LoadSceneMode.Single);

	}

	// ref http://stackoverflow.com/a/3978040
	public static int GetRandomUnusedPort()
	{
		var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		var port = ((IPEndPoint)listener.LocalEndpoint).Port;
		listener.Stop();
		return port;
	}

	public static string randomDataBase64url(uint length)
	{
		RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
		byte[] bytes = new byte[length];
		rng.GetBytes(bytes);
		return base64urlencodeNoPadding(bytes);
	}

	public static byte[] sha256(string inputStirng)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
		SHA256Managed sha256 = new SHA256Managed();
		return sha256.ComputeHash(bytes);
	}

	public static string base64urlencodeNoPadding(byte[] buffer)
	{
		string base64 = Convert.ToBase64String(buffer);

		// Converts base64 to base64url.
		base64 = base64.Replace("+", "-");
		base64 = base64.Replace("/", "_");
		// Strips padding.
		base64 = base64.Replace("=", "");

		return base64;
	}

	public bool MyRemoteCertificateValidationCallback(System.Object sender,
		X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
	{
		bool isOk = true;
		// If there are errors in the certificate chain,
		// look at each error to determine the cause.
		if (sslPolicyErrors != SslPolicyErrors.None) {
			for (int i=0; i<chain.ChainStatus.Length; i++) {
				if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
					continue;
				}
				chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
				chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
				chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
				chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
				bool chainIsValid = chain.Build ((X509Certificate2)certificate);
				if (!chainIsValid) {
					isOk = false;
					break;
				}
			}
		}
		return isOk;
	}

}

[Serializable]
public class Page
{
	public string access_token;
	public int expires_in;
	public string token_type;
	public string refresh_token;
}

[Serializable]
public class Profile
{
	public string sub;
	public string name;
	public string given_name;
	public string family_name;
	public string picture;
	public string email;
	public bool email_verified;
}