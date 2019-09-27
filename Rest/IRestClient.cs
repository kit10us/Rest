using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest
{
	/// <summary>
	/// The supported REST communication methods.
	/// </summary>
	public enum Method
	{
		GET,
		PUT,
		POST,
		DELETE
	}

	/// <summary>
	/// Interface for rest client. Using interfaces in this way would enable us to modify the way rest communicates, including
	/// the ability to swap out our rest client for a "dummy" client that can simulate rest, thus allowing unit testing.
	/// </summary>
	public interface IRestClient
	{
	}
}
