// Artist.cs
//
//  Copyright (C) 2008 Amr Hassan
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;
using System.Xml;

namespace Lastfm.Services
{
	public class Artist : Base, ITaggable, IEquatable<Artist>, IShareable, IHasImage
	{
		public string Name {get; private set;}
		
		public ArtistBio Bio
		{ get { return new ArtistBio(this, Session); } }
		
		public Artist(string name, Session session)
			:base(session)
		{
			Name = name;
		}
    
		public override string ToString ()
		{
			return this.Name;
		}
		
		protected override RequestParameters getParams ()
		{
			RequestParameters p = base.getParams();
			p["artist"] = this.Name;
			
			return p;
		}
		
		public Artist[] GetSimilar(int limit)
		{
			RequestParameters p = getParams();
			if (limit > -1)
				p["limit"] = limit.ToString();
			
			XmlDocument doc = request("artist.getSimilar", p);
      
			string[] names = extractAll(doc, "name");
      
			List<Artist> list = new List<Artist>();
      
			foreach(string name in names)
				list.Add(new Artist(name, Session));
			
			return list.ToArray();
		}

		public Artist[] GetSimilar()
		{
			return GetSimilar(-1);
		}
		
		public int GetListenerCount()
		{
			XmlDocument doc = request("artist.getInfo");
			
			return Convert.ToInt32(extract(doc, "listeners"));
		}
		
		public int GetPlaycount()
		{
			XmlDocument doc = request("artist.getInfo");
			
			return Convert.ToInt32(extract(doc, "playcount"));
		}
		
		public string GetImageURL(ImageSize size)
		{
			XmlDocument doc = request("artist.getInfo");
			
			string[] sizes = extractAll(doc, "image", 3);
			
			return sizes[(int)size];
		}
		
		public string GetImageURL()
		{
			return GetImageURL(ImageSize.Large);
		}
		
		public TopTrack[] GetTopTracks()
		{
			XmlDocument doc = request("artist.getTopTracks");
			
			List<TopTrack> list = new List<TopTrack>();
			
			foreach(XmlNode n in doc.GetElementsByTagName("track"))
			{
				Track track = new Track(extract(n, "name", 1), extract(n, "name"), Session);
				int weight = int.Parse(extract(n, "playcount"));
				
				list.Add(new TopTrack(track, weight));
			}
			
			return list.ToArray();
		}
		
		public Event[] GetEvents()
		{
			XmlDocument doc = request("artist.getEvents");
			
			List<Event> list = new List<Event>();
			foreach(string id in extractAll(doc, "id"))
				list.Add(new Event(Int32.Parse(id), Session));
			
			return list.ToArray();
		}
		
		public TopAlbum[] GetTopAlbums()
		{
			XmlDocument doc = request("artist.getTopAlbums");
			
			List<TopAlbum> list = new List<TopAlbum>();
			foreach(XmlNode n in doc.GetElementsByTagName("album"))
			{
				Album album = new Album(extract(n, "name", 1), extract(n, "name"), Session);
				int weight = int.Parse(extract(n, "playcount"));
				
				list.Add(new TopAlbum(album, weight));
			}
			
			return list.ToArray();
		}
		
		public TopFan[] GetTopFans()
		{
			XmlDocument doc = request("artist.getTopFans");
			
			List<TopFan> list = new List<TopFan>();
			foreach(XmlNode node in doc.GetElementsByTagName("user"))
			{
				User user = new User(extract(node, "name"), Session);
				int weight = int.Parse(extract(node, "weight"));
				
				list.Add(new TopFan(user, weight));
			}
			
			return list.ToArray();
		}
		
		public bool Equals(Artist artist)
		{
			return (artist.Name == this.Name);
		}
		
		public void Share(Recipients recipients, string message)
		{
			if (recipients.Count > 1)
			{
				foreach(string recipient in recipients)
				{
					Recipients r = new Recipients();
					r.Add(recipient);
					Share(r, message);
				}
				
				return;
			}
			
			requireAuthentication();
			
			RequestParameters p = getParams();
			p["recipient"] = recipients[0];
			p["message"] = message;
			
			request("artist.Share", p);
		}
		
		public void Share(Recipients recipients)
		{
			if (recipients.Count > 1)
			{
				foreach(string recipient in recipients)
				{
					Recipients r = new Recipients();
					r.Add(recipient);
					Share(r);
				}
				
				return;
			}
			
			requireAuthentication();
			
			RequestParameters p = getParams();
			p["recipient"] = recipients[0];
			
			request("artist.Share", p);
		}
		
		public static ArtistSearch Search(string artistName, Session session, int itemsPerPage)
		{
			Dictionary<string, string> terms = new Dictionary<string,string>();
			terms["artist"] = artistName;
			
			return new ArtistSearch(terms, session, itemsPerPage);
		}
		
		public static ArtistSearch Search(string artistName, Session session)
		{
			return Artist.Search(artistName, session, 30);
		}
		
		public void AddTags(params Tag[] tags)
		{
			//This method requires authentication
			requireAuthentication();
			
			foreach(Tag tag in tags)
			{
				RequestParameters p = getParams();
				p["tags"] = tag.Name;
				
				request("artist.addTags", p);
			}
		}
		
		public void AddTags(params string[] tags)
		{
			foreach(string tag in tags)
				AddTags(new Tag(tag, Session));
		}
		
		public void AddTags(TagCollection tags)
		{
			foreach(Tag tag in tags)
				AddTags(tag);
		}
		
		public Tag[] GetTags()
		{
			//This method requires authentication
			requireAuthentication();
			
			XmlDocument doc = request("artist.getTags");
			
			TagCollection collection = new TagCollection(Session);
			
			foreach(string name in this.extractAll(doc, "name"))
				collection.Add(name);
			
			return collection.ToArray();
		}
		
		public TopTag[] GetTopTags()
		{
			XmlDocument doc = request("artist.getTopTags");
			
			List<TopTag> list = new List<TopTag>();
			foreach(XmlNode n in doc.GetElementsByTagName("tag"))
				list.Add(new TopTag(new Tag(extract(n, "name"), Session), Int32.Parse(extract(n, "count"))));
			
			return list.ToArray();
		}
		
		public TopTag[] GetTopTags(int limit)
		{
			return this.sublist<TopTag>(GetTopTags(), limit);
		}
		
		public void RemoveTags(params Tag[] tags)
		{
			//This method requires authentication
			requireAuthentication();
			
			foreach(Tag tag in tags)
			{
				RequestParameters p = getParams();
				p["tag"] = tag.Name;
				
				request("artist.removeTag", p);
			}
		}
		
		public void RemoveTags(params string[] tags)
		{
			//This method requires authentication
			requireAuthentication();
			
			foreach(string tag in tags)
				RemoveTags(new Tag(tag, Session));
		}
		
		public void RemoveTags(TagCollection tags)
		{
			foreach(Tag tag in tags)
				RemoveTags(tag);
		}
		
		public void SetTags(string[] tags)
		{
			List<Tag> list = new List<Tag>();
			foreach(string name in tags)
				list.Add(new Tag(name, Session));
			
			SetTags(list.ToArray());
		}
		
		public void SetTags(Tag[] tags)
		{
			List<Tag> newSet = new List<Tag>(tags);
			List<Tag> current = new List<Tag>(GetTags());
			List<Tag> toAdd = new List<Tag>();
			List<Tag> toRemove = new List<Tag>();
			
			foreach(Tag tag in newSet)
				if(!current.Contains(tag))
					toAdd.Add(tag);
			
			foreach(Tag tag in current)
				if(!newSet.Contains(tag))
					toRemove.Add(tag);
			
			if (toAdd.Count > 0)
				AddTags(toAdd.ToArray());
			if (toRemove.Count > 0)
				RemoveTags(toRemove.ToArray());
		}
		
		public void SetTags(TagCollection tags)
		{
			SetTags(tags.ToArray());
		}
		
		public void ClearTags()
		{
			foreach(Tag tag in GetTags())
				RemoveTags(tag);
		}
	}
}