
Twitter API Client for Unity C#. (beta)

Inspired by [Let's Tweet In Unity](https://www.assetstore.unity3d.com/jp/#!/content/536).

# Environment

- Unity 5.6.0f3

# Available API Methods

## REST API

- GET  : Available
- POST : Available
-- Chunked POST "media/upload" (INIT/APPEND/FINALIZE) is not available yet (now in progress)

## Streaming API

- POST statuses/filter : Available(beta)
- GET  statuses/sample : Available(beta)
- UserStreams : Available(beta)

# Usage

## Initialize

```C#
using Twitter;

public class EventHandler : MonoBehaviour {
  void Start () {
    Twitter.Oauth.consumerKey       = "...";
    Twitter.Oauth.consumerSecret    = "...";
    Twitter.Oauth.accessToken       = "...";
    Twitter.Oauth.accessTokenSecret = "...";
  }  
}
```
## REST API

### GET search/tweets

```C#
using Twitter;

void Start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["q"] = "search word";
  parameters ["count"] = 30.ToString();;
  StartCoroutine (Client.Get ("search/tweets", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse> (response);
  } else {
    Debug.Log (response);
  }
}
```

### GET statuses/home_timeline

```C#
using Twitter;

void Start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["count"] = 30.ToString();;
  StartCoroutine (Client.Get ("statuses/home_timeline", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    StatusesHomeTimelineResponse Response = JsonUtility.FromJson<StatusesHomeTimelineResponse> (response);
  } else {
    Debug.Log (response);
  }
}
```

### POST statuses/update

```C#
using Twitter;

void Start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["status"] = "Tweet from Unity";
  StartCoroutine (Client.Post ("statuses/update", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    Tweet tweet = JsonUtility.FromJson<Tweet> (response);
  } else {
    Debug.Log (response);
  }
}
```

### POST statuses/retweet/:id
ex. search tweets with the word "Unity", and retweet 5 tweets.
```C#
using Twitter;

void start() {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["q"] = "Unity";       // Search keywords
  parameters ["count"] = 5.ToString();   // Number of Tweets
  StartCoroutine (Client.Get ("search/tweets", parameters, this.Callback));
}

void Callback(bool success, string response) {
  if (success) {
    SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse> (response);
    foreach (Tweet tweet in Response.statuses) { Retweet (tweet); }
  } else {
    Debug.Log (response);
  }
}

void Retweet(Tweet tweet) {
  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters ["id"] = tweet.id_str;
  StartCoroutine (twitter.Client.Post ("statuses/retweet/" + tweet.id_str, parameters, this.RetweetCallback));
}

void RetweetCallback(bool success, string response) {
  if (success) {
    Debug.Log ("Retweet Done");
  } else {
    Debug.Log (response);
  }
}
```

### POST media/upload
```C#
using Twitter;

void start() {
  byte[] imgBinary = File.ReadAllBytes(path/to/the/file);
  string imgbase64 = System.Convert.ToBase64String(imgBinary);

  Dictionary<string, string> parameters = new Dictionary<string, string>();
  parameters["media_data"] = imgbase64;
  parameters["additional_owners"] = "additional owner if you have";
  StartCoroutine (Client.Post ("media/upload", parameters, MediaUploadCallback));
}

void MediaUploadCallback(bool success, string response) {
  if (success) {
    UploadMedia media = JsonUtility.FromJson<UploadMedia>(response);

    Dictionary<string, string> parameters = new Dictionary<string, string>();
    parameters["media_ids"] = media.media_id.ToString();
    parameters["status"] = "Tweet text with image";
    StartCoroutine (Client.Post ("statuses/update", parameters, StatusesUpdateCallback));
  } else {
    Debug.Log (response);
  }
}

void StatusesUpdateCallback(bool success, string response) {
  if (success) {
    Tweet tweet = JsonUtility.FromJson<Tweet> (response);
  } else {
    Debug.Log (response);
  }
}
```

See https://dev.twitter.com/rest/reference for more Methods.


## Streaming API

### POST statuses/filter
```C#
using Twitter;

Stream stream;

void Start() {
  stream = new Stream(StreamType.PublicFilter);
  Dictionary<string, string> streamParameters = new Dictionary<string, string>();

  List<string> tracks = new List<string>();
  tracks.Add("Unity");
  tracks.Add("Twitter");
  Twitter.FilterTrack filterTrack = new Twitter.FilterTrack(tracks);
  streamParameters.Add(filterTrack.GetKey(), filterTrack.GetValue());
  StartCoroutine(stream.On(streamParameters, this.OnStream));
}

void OnStream(string response, StreamMessageType messageType) {
  try
  {
    if(messageType == StreamMessageType.Tweet)
    {
      Tweet tweet = JsonUtility.FromJson<Tweet>(response);
    }
  }
  catch (System.Exception e)
  {
    Debug.Log(e);
  }
}
```

### User Stream
```C#
using Twitter;

Stream stream;

void Start() {
  stream = new Stream(StreamType.User);
  Dictionary<string, string> streamParameters = new Dictionary<string, string>();

  StartCoroutine(stream.On(streamParameters, OnStream));
}

void OnStream(string response, StreamMessageType messageType) {
  try
  {
    if(messageType == StreamMessageType.Tweet)
    {
      Tweet tweet = JsonUtility.FromJson<Tweet>(response);
    }
    else if(messageType == StreamMessageType.StreamEvent)
    {
      StreamEvent streamEvent = JsonUtility.FromJson<StreamEvent>(response);
      Debug.Log(streamEvent.event_name); // Response Key 'event' is replaced 'event_name' in this library.
    }
    else if(messageType == StreamMessageType.FriendsList)
    {
      FriendsList friendsList = JsonUtility.FromJson<FriendsList>(response);
    }
  }
  catch (System.Exception e)
  {
    Debug.Log(e);
  }
}
```
See `StreamType` and `StreamMessageType` at `TwitterStreamType.cs`. and https://dev.twitter.com/streaming/overview/messages-types#withheld_content_notices .

See https://dev.twitter.com/streaming/reference for more Methods.

## Response class
See `TwitterJson.cs`, and https://dev.twitter.com/overview/api/tweets , https://dev.twitter.com/overview/api/users , https://dev.twitter.com/overview/api/entities , https://dev.twitter.com/overview/api/entities-in-twitter-objects .

You can modify `TwitterJson.cs` to get a response item.


# License
- There are some modified code from other library. Check "TwitterOauth.cs" and "TwitterHelper.cs".
- For other parts, MIT.
