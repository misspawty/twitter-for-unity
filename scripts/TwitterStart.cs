using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserSystemLibrary;
using Twitter;





public class atest : MonoBehaviour
{
    public string Aoath;
    public string secret;
    public string token;
    public string tokensec;
    public GameObject tweetcube;
    public Vector3 OffsettoUnityspace;

    Stream stream;





    void Start()
    {
        print(Aoath);

        Twitter.Oauth.consumerKey = Aoath;
        Twitter.Oauth.consumerSecret = secret;
        Twitter.Oauth.accessToken = token;
        Twitter.Oauth.accessTokenSecret = tokensec;
        Twitter.StreamingDownloadHandler.tweecube = tweetcube;

        ////RestAPI
        //Dictionary<string, string> parameters = new Dictionary<string, string>();
        //parameters["q"] = "london";
        //parameters["count"] = 30.ToString(); ;
        //StartCoroutine(Client.Get("search/tweets", parameters, Callback));

        ////streamAPI_User
        //stream = new Stream(StreamType.User);
        //Dictionary<string, string> streamParameters = new Dictionary<string, string>();
        ////StartCoroutine(stream.On(streamParameters, OnStream));

        //streamAPI_Post
        stream = new Stream(StreamType.PublicFilter);
        Dictionary<string, string> streamParameters = new Dictionary<string, string>();

        List<string> tracks = new List<string>();
        //tracks.Add("Unity");
        //tracks.Add("Twitter");
        tracks.Add("London");

        //tracks.Add("Uni");

        //-----Adding a string filter
        //Twitter.FilterTrack filterTrack = new Twitter.FilterTrack(tracks);
        //streamParameters.Add(filterTrack.GetKey(), filterTrack.GetValue());
        //    print(filterTrack.GetKey() + ", " + filterTrack.GetValue());

        //-----Adding a locationfilter
        Twitter.FilterLocations filterLocation = new Twitter.FilterLocations();
        streamParameters.Add(filterLocation.GetKey(), filterLocation.GetValue());
            print(filterLocation.GetKey() + ", " + filterLocation.GetValue());

        StartCoroutine(stream.On(streamParameters, OnStream));
        
    }
    


    //RestAPIwith search word
    void Callback(bool success, string response)
    {
        if (success)
        {
            SearchTweetsResponse Response = JsonUtility.FromJson<SearchTweetsResponse>(response);
            Debug.Log(response);
        }
        else {
            Debug.Log(response);
        }
    }

    
    //streaming API
    void OnStream(string response, StreamMessageType messageType)
    {
        try
        {
            if (messageType == StreamMessageType.GeoTweet)
            {

                //Tweet tweet = JsonUtility.FromJson<Tweet>(response);
                Coordinates coordinates = JsonUtility.FromJson<Coordinates>(response);
                //Debug.Log("problima");

                if (coordinates.coordinates.type.Equals("Point"))
                {

                    Debug.Log("2. the date" + coordinates.created_at + " THE TEXT:" + coordinates.text);
                    Debug.Log("Coordinates: x" + coordinates.coordinates.coordinates[0] + " y: " + coordinates.coordinates.coordinates[1]);

                    tweetcube.GetComponent<storetweet>().coordinates_x = coordinates.coordinates.coordinates[1];
                    tweetcube.GetComponent<storetweet>().coordinates_y = coordinates.coordinates.coordinates[0];
                    tweetcube.GetComponent<storetweet>().text = coordinates.text;
                    tweetcube.GetComponent<storetweet>().date = coordinates.created_at;

                    //offset to unity. 
                    GeoUTMConverter GeoUTM = new GeoUTMConverter();
                    OffsettoUnityspace = GeoUTM.ConvertToUnityCoords(float.Parse(coordinates.coordinates.coordinates[1]), float.Parse(coordinates.coordinates.coordinates[0]));


                    MonoBehaviour.Instantiate(tweetcube, new Vector3(OffsettoUnityspace.x, 0, OffsettoUnityspace.z), Quaternion.identity);
                }
                else
                {
                    Debug.Log("Something with coordinates");
                }



            }

            else if (messageType == StreamMessageType.Tweet) // normal tweet response
            {
                Tweet tweet = JsonUtility.FromJson<Tweet>(response);
                Debug.Log(response);
            }

            else if (messageType == StreamMessageType.StreamEvent)
            {
                StreamEvent streamEvent = JsonUtility.FromJson<StreamEvent>(response);
                Debug.Log("2 " + streamEvent.event_name); // Response Key 'event' is replaced 'event_name' in this library.
            }
            else if (messageType == StreamMessageType.FriendsList)
            {
                FriendsList friendsList = JsonUtility.FromJson<FriendsList>(response);
                Debug.Log("3 " + friendsList);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
    ////StreamAPI
    /*void OnStream(string response, StreamMessageType messageType)
    {
        try
        {
            if (messageType == StreamMessageType.Tweet)
            {
                Tweet tweet = JsonUtility.FromJson<Tweet>(response);
                Debug.Log("text: "+tweet.text + " " ); 
            }
            else if (messageType == StreamMessageType.StreamEvent)
            {
                StreamEvent streamEvent = JsonUtility.FromJson<StreamEvent>(response);
                Debug.Log("2 " +streamEvent.event_name); // Response Key 'event' is replaced 'event_name' in this library.
            }
           else if (messageType == StreamMessageType.FriendsList)
            {
                FriendsList friendsList = JsonUtility.FromJson<FriendsList>(response);
                Debug.Log("3 "+ friendsList);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("4 "+e);
        }
    }
    */


    // Update is called once per frame
    void Update () {
		
	}
}
