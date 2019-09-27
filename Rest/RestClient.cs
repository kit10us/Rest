using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rest
{
	public class RestClient : IRestClient
	{
		#region Properties...
		/// <summary>
		/// Stores the last command executed, for troubleshooting.
		/// </summary>
		public string LastCommand { get; private set; }

		/// <summary>
		/// Logs all commands in and out of the API, for testing. These could be used with a dummy
		/// class to simulated commands (map input to output and unit test results to verify API).
		/// </summary>
		public bool LogTranscations { get; set; }

		/// <summary>
		/// Location to log API transactions.
		/// </summary>
		public string TranscationLogFile { get; set; }
		#endregion

		/// <summary>
		/// Constructor for the JIRA REST API. Requires the URL to JIRA, as well as the sub URL to the rest API with version.
		/// </summary>
		/// <param name="url">Complete URL to JIRA.</param>
		/// <param name="restSubUrl">Sub URL, relative to the above URL, for the REST API and version.</param>
		public RestClient()
		{
			LogTranscations = false;
			TranscationLogFile = "trans.log";
		}

		public void ReportTranscation( string input, string output )
		{
			if ( LogTranscations == true && string.IsNullOrEmpty( TranscationLogFile ) == false )
			{
				string text = "[" + DateTime.Now.ToString() + "]\r\n";
				text += "input:\r\n" + input + "\r\n";
				text += "output:\r\n" + output + "\r\n\r\n";
				System.IO.File.AppendAllText( TranscationLogFile, text );
			}
		}

		/// <summary>
		/// Method to execute JIRA REST command. Omit a leading slash (/).
		/// </summary>
		public string Command( Config config, string command, Method method, string data = null )
		{
			if ( command[ 0 ] != '/' && command[ 0 ] != '\\' )
			{
				command = '/' + command;
			}
			LastCommand = config.SiteURL + command;

			HttpWebRequest webRequest = WebRequest.Create( LastCommand ) as HttpWebRequest;
			webRequest.ContentType = "application/json";
			webRequest.Method = method.ToString();

			if ( !String.IsNullOrEmpty( config.Username ) && !String.IsNullOrEmpty( config.Password ) )
			{
				string unencodedText = config.Username + ":" + config.Password;
				byte[] unencodedBytes = Encoding.ASCII.GetBytes( unencodedText );
				string encodedUsernameAndPassword = Convert.ToBase64String( unencodedBytes );
				webRequest.Timeout = config.Timeout;
				webRequest.Headers.Add( "Authorization: Basic " + encodedUsernameAndPassword );
			}

			if ( !String.IsNullOrEmpty( data ) )
			{
				using ( System.IO.BinaryWriter binaryWriter = new BinaryWriter( webRequest.GetRequestStream() ) )
				{
					byte[] bytes = Encoding.ASCII.GetBytes( data );

					binaryWriter.Write( bytes, 0, data.Length );
				}
			}

			HttpWebResponse response = null;

			response = webRequest.GetResponse() as HttpWebResponse;
			using ( Stream stream = response.GetResponseStream() )
			{
				StreamReader streamReader = new StreamReader( stream, Encoding.UTF8 );
				string asString = streamReader.ReadToEnd();
				streamReader = null;
				string text = "   command: " + LastCommand + "\r\n   method:" + method.ToString() + "\r\n   data:" + data;
				ReportTranscation( text, asString );
				return asString;
			}
		}

		/// <summary>
		/// Method to execute JIRA REST command. Omit a leading slash (/). This version will retry a finite number of times.
		/// </summary>
		public string CommandWithRetry( Config config, string command, Method method, int retryCount = 10, int retrySleepInMs = 1000, string data = null )
		{
			string result = null;
			while( result == null &&  (retryCount-- ) > 0 )
			{
				try
				{
					result = Command( config, command, method, data );
				}
				catch( System.Exception ex )
				{
					System.Diagnostics.Debug.WriteLine( "Exception from CommandWithRetry: " + ex.ToString() );
				}

				if ( result == null )
				{
					System.Threading.Thread.Sleep( retrySleepInMs );
				}
			}
			return result;
		}
	}
}
