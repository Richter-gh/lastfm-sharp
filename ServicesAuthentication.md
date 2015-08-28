# Services Authentication #

During the course of using the last.fm webservices (the namespace Lastfm.Services), you're going to be needing an API account "Key" and "Secret" values. You can obtain those from [there](http://www.last.fm/api/account).

In order to perform _write_ operations to a last.fm user profile, you're going to need a "Session Key" which represents the user's permission to perform operations on his behalf.

A session in Lastfm.Services is represented by an instance of the class Lastfm.Services.Session which can be constructed as follows:

```
using System;
using Lastfm.Services;

namespace MyApplication
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			string API_KEY =	"b25b959554ed76058ac220b7b2e0a026";
			string API_SECRET = 	"361505f8eeaf61426ef95a4317482251";
      
			// An unauthenticated session will perform read-only operations.
			Session session = new Session(API_KEY, API_SECRET);
			
			// In order to perform *write* operations, a session must
			// be authenticated by the user.
			session.Authenticate("TheUserName", Lastfm.Utilities.md5("profilepassword"));
		}
	}
}
```

The lifetime of a Session Key is infinite, unless the user provokes your rights to access his profile. You should store it somewhere for later use.