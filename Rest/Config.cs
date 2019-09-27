using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest
{
	/// <summary>
	/// Grouping of common configuration information for REST commands, to reduce parameter passing and tracking and simplify interfaces.
	/// </summary>
	public class Config
	{
		public string SiteURL { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Timeout { get; set; }

		/// <summary>
		/// Construct a Request. Timeout will default to 20000 (ms).
		/// </summary>
		public Config( int? timeout = null )
		{
			Timeout = timeout.HasValue ? timeout.Value : 20000;
		}

		/// <summary>
		/// Construct a Request. Timeout will default to 20000 (ms).
		/// </summary>
		public Config( string siteUrl, string username, string password, int? timeout = null )
			: this( timeout )
		{
			SiteURL = siteUrl;
			Username = username;
			Password = password;
		}

		public Config( Config request )
		{
			SiteURL = request.SiteURL;
			Username = request.Username;
			Password = request.Password;
			Timeout = request.Timeout;
		}
	}
}
