# Examples #

Here's a couple of examples of how you could be using lastfm-sharp.

## Authentication ##
```
using System;
using Lastfm.Services;

namespace MyApp
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      // Get your own API_KEY and API_SECRET from http://www.last.fm/api/account
      string API_KEY =  "b25b959554ed76058ac220b7b2e0a026";
      string API_SECRET =   "361505f8eeaf61426ef95a4317482251";

      // Creating an unauthenticated session that could only allow me
      // to perform read operations.
      Session session = new Session(API_KEY, API_SECRET);
      
      Console.WriteLine("Please enter your username: ");
      string username = Console.ReadLine();
      Console.WriteLine("Please enter your password: ");
      string md5password = Lastfm.Utilities.md5(Console.ReadLine());
      
      // Authenticate it with a username and password to be able
      // to perform write operations and access this user's profile
      // private data.
      session.Authenticate(username, md5password);
      
      // You can now use the "session" object with everything in your project.
    }
  }
}
```

## Alternative authentication ##
This is the recommended authentication method by Last.fm's developers, even though it's a little pain to implement it right.

```
using System;
using Lastfm.Services;

namespace MyApp
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      // Get your own API_KEY and API_SECRET from http://www.last.fm/api/account
      string API_KEY =  "b25b959554ed76058ac220b7b2e0a026";
      string API_SECRET =   "361505f8eeaf61426ef95a4317482251";
      
      // Creating an unauthenticated session that could only allow me
      // to perform read operations.
      Session session = new Session(API_KEY, API_SECRET);
      
      // Generate a web authentication url
      string url = session.GetWebAuthenticationURL();
      
      // Ask the user to open it and allow you to access his/her profile.
      Console.WriteLine("Please open the following url in your web browser and follow the procedure, then press Enter...");
      Console.WriteLine(url);
      
      // Wait for it...
      Console.ReadLine();
      
      // Now that he's pressed Enter
      session.AuthenticateViaWeb();
      
      // You can now use the authenticated "session" object with everything in your project.
    }
  }
}
```

## Tag Manipulation ##
```
using System;
using Lastfm.Services;
using Lastfm;
using System.Collections.Generic;

namespace MyApp
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      Session session = new Session(API_KEY, API_SECRET);
      
      // ...
      // Do whatever you want here to authenticate your session.
      // ...
      
      // Create an Artist object.
      Artist artist = new Artist("system of a down", session);
      
      // Tag it.
      artist.AddTags("awesome", "rock", "hard rock", "classical", "smooth");
      
      // Display your current tags for system of a down.
      foreach(Tag tag in artist.GetTags())
        Console.WriteLine(tag);
      
      // Remove tags from it
      artist.RemoveTags("classical", "smooth");
      
      // .. and so on.
    }
  }
}
```

## Using a proxy server ##
```
using System;
using Lastfm.Services;
using Lastfm;
using System.Collections.Generic;

namespace sandbox
{
  class MainClass
  {
    public static void Main(string[] args)
    {
      // Get your own API_KEY and API_SECRET from http://www.last.fm/api/account
      string API_KEY =  "b25b959554ed76058ac220b7b2e0a026";
      string API_SECRET =   "361505f8eeaf61426ef95a4317482251";
      
      // Create your session
      Session session = new Session(API_KEY, API_SECRET);
      
      // Set this static property to a System.Net.IWebProxy object
      Lastfm.ProxySupport.Proxy = new System.Net.WebProxy("221.2.216.38", 8080);
      
      // Test it out...
      Track track = new Track("david arnold", "the hot fuzz suite", session);
      Console.WriteLine(track.GetAlbum());
    }
  }
}
```