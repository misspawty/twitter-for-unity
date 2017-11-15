using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using LaserSystemLibrary;


namespace Twitter
{

    public delegate void TwitterStreamCallback(string response, StreamMessageType messageType);

    public class Stream
    {

        private string REQUEST_URL;
        private UnityWebRequest request;

        public Stream(StreamType type)
        {
            string[] endpoints = { "statuses/filter", "statuses/sample", "user", "site" };

            if (type == StreamType.PublicFilter || type == StreamType.PublicSample)
            {
                REQUEST_URL = "https://stream.twitter.com/1.1/" + endpoints[(int)type] + ".json";
                Debug.Log("publicfilter");
            } else if (type == StreamType.User)
            {
                REQUEST_URL = "https://userstream.twitter.com/1.1/user.json";
            } else if (type == StreamType.Site)
            {
                REQUEST_URL = "https://sitestream.twitter.com/1.1/site.json";
            }

        }

        public IEnumerator On(Dictionary<string, string> APIParams, TwitterStreamCallback callback)
        {
            SortedDictionary<string, string> parameters = Helper.ConvertToSortedDictionary(APIParams);

            WWWForm form = new WWWForm();
            foreach (KeyValuePair<string, string> parameter in APIParams)
            {
                form.AddField(parameter.Key, parameter.Value);
            }

            request = UnityWebRequest.Post(REQUEST_URL, form);
            request.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
            request.SetRequestHeader("Authorization", Oauth.GenerateHeaderWithAccessToken(parameters, "POST", REQUEST_URL));
            request.downloadHandler = new StreamingDownloadHandler(callback);
            yield return request.Send();

        }

        public void Off()
        {
            Debug.Log("Connection Aborted");
            request.Abort();
        }

    }

    #region DownloadHandler

    public class StreamingDownloadHandler : DownloadHandlerScript
    {

        TwitterStreamCallback callback;
        StreamMessageType messageType;

        public StreamingDownloadHandler(TwitterStreamCallback callback)
        {
            this.callback = callback;
        }


        //
        public static string getBetween(string strSource, string strStart, int End)
        {
            int Start;
            if (strSource.Contains(strStart))
            {
                Debug.Log("Found String");
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;

                Debug.Log(strSource.Length + ">" + Start + "+" + End);
                if (strSource.Length > Start + End)
                    return strSource.Substring(Start, End);
                else
                {
                    Debug.Log("Longer than expected:");
                    return strSource.Substring(Start, strSource.Length - Start);
                }
            }
            else
            {
                return "";
            }
        }
        //

        public static System.Collections.Generic.List<string> StringsinString(string strSource, string strstrToFind, int NumberOfCharacters)
        {
            int start=0;
            int at=0;
            int end = strSource.Length;
            int count=0;
            var list = new List<string>();
            while ((start <= end) && (at > -1))
            {
                count = end - start;
                at = strSource.IndexOf(strstrToFind, start, count);
                if (at == -1) break;
                //Console.Write("{0} ", at);
                //Debug.Log(at);
                if (strSource.Length > at + strstrToFind.Length)
                {
                    list.Add(strSource.Substring(at + strstrToFind.Length, NumberOfCharacters));
                }
                start = at + 1;
            }
            return list;
        }

        public static GameObject tweecube;
        public Vector3 WGScoordinates;
        //public Vector3 OffsettoUnityspace;
        //GeoUTMConverter GeoUTM = new GeoUTMConverter();
       

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length < 1)
            {
                Debug.Log("LoggingDownloadHandler :: ReceiveData - received a null/empty buffer");
                return false;
            }

            string response = Encoding.UTF8.GetString(data);
           
           // Debug.Log("1.:" + response);
            //Debug.Log("RESPONSE: " + getBetween(response, "coordinates\"", 10));

            //if (getBetween(response, "coordinates\"",5) != ":null"|| getBetween(response, "coordinates\"",1)==null)
            //System.Collections.Generic.List<string> coordinates = StringsinString(response, "coordinates\":", 100);
         //   System.Collections.Generic.List<string> timet = StringsinString(response, "{\"created_at\":", 20);
            //System.Collections.Generic.List<string> textt = StringsinString(response, "{\"created_at\":", 100);
            System.Collections.Generic.List<string> coordinates = StringsinString(response, "\"coordinates\":{\"type\":\"Point\",\"coordinates\":", 100);
            
           

            //parse date
            /*
            for (int t = 0; t < timet.Count; t++)
            {
                if (!timet[t].Substring(0, 4).Equals("null") )
                {
                    tweecube.GetComponent<storetweet>().date = timet[t].Substring(0, 20);
                    //Debug.Log(timet[t].Substring(0, 20));
                }
            }
            */
            
            //parse coordinates
            for (int c=0; c<coordinates.Count;c++)
            {
                //Debug.Log(coordinates[c]);
                //Debug.Log(coordinates[c].Substring(0, 4));
                //Debug.Log(coordinates[c].Substring(0, 2));
                if (!coordinates[c].Substring(0, 4).Equals("null") && !coordinates[c].Substring(0, 2).Equals("[["))
                {
                    //Debug.Log("COORDINATES: " + coordinates[c]);
                    //Debug.Log(coordinates[c].IndexOf("]"));
                    //Debug.Log(coordinates[c].Substring(1,coordinates[c].IndexOf("]")-1));
                    string c1 = coordinates[c].Substring(1, coordinates[c].IndexOf("]") - 1);
                    string[] c2 = c1.Split(',');

                    //Debug.Log("RAW COordinates: x"+ c2[0] +" y"+ c2[1]); 
                    //translate to coordinates form GeoUtMConverter

                //    Debug.Log("HELOOOO:" + GeoUTM.ConvertToUnityCoords(float.Parse(c2[1]), float.Parse(c2[0])));

                    /*
                                        //translate to coordinates (from Vilo) not to use if GEOUTM Converter is in use. 
                                        WGScoordinates = Utils.CoordHelper.WGS84toWebMercator(new Vector3(float.Parse(c2[0]), 0, float.Parse(c2[1])));
                                        Debug.Log("WGS: " + WGScoordinates);

                                        //OFFSET to unity space non working yet
                                        OffsettoUnityspace = new Vector3(WGScoordinates.x + 20000f, 0, WGScoordinates.z - 6700000f);

                                        //Debug.Log("Offset: x:"+ OffsettoUnityspace.x + " y:"+ OffsettoUnityspace.z);

                        */
                    //OffsettoUnityspace = GeoUTM.ConvertToUnityCoords(float.Parse(c2[1]), float.Parse(c2[0]));
                    //Debug.Log (Utils.CoordHelper.WebMercatorToWGS84(new Vector3(float.Parse(c2[1]), 0, float.Parse(c2[0]))));
                    //Debug.Log("x: " + c2[0] + "y: " + c2[1]);  /// Original coordinates 

                    //Instantiate(tweecube, new Vector3(i * 2.0F, 0, 0), Quaternion.identity);
                    //MonoBehaviour.Instantiate(tweecube, new Vector3(float.Parse(c2[0]), 0, float.Parse(c2[1])), Quaternion.identity);

                    //INstantiate HERE-->
                    //tweecube.GetComponent<storetweet>().coordinates_x = c2[1];
                    //tweecube.GetComponent<storetweet>().coordinates_y = c2[0];
              //      tweecube.GetComponent<storetweet>().date = timet[c].Substring(0, 20);
                    
                   // MonoBehaviour.Instantiate(tweecube, new Vector3(OffsettoUnityspace.x, 0, OffsettoUnityspace.z), Quaternion.identity);
                    
                    
                    ////////MonoBehaviour.Instantiate(tweecube, new Vector3(-WGScoordinates.x, 0, -WGScoordinates.z), Quaternion.identity);
                    //MonoBehaviour.Instantiate(tweecube, new Vector3(OffsettoUnityspace.x, 0, OffsettoUnityspace.z), Quaternion.identity);


                }
            }

            //Debug.Log(getBetween(response, "coordinates\"", 5));
            //Debug.Log(getBetween(response, "coordinates\"", 1));
            //Debug.Log(getBetween(response, "coordinates\"", 100));

            //if (!getBetween(response, "coordinates\"", 5).Equals(":null") && !getBetween(response, "coordinates\"", 1).Equals(""))
            //{
            //    Debug.Log("COORDINATES: " + getBetween(response, "coordinates", 100));//+response);

            //}

            //else { Debug.Log("notgeolocated"); }

            response = response.Replace("\"event\":", "\"event_name\":");
            messageType = StreamMessageType.None;
            CheckMessageType(response);
            try
            {
                callback(JsonHelper.ArrayToObject(response), messageType);
                return true;
            } catch (System.Exception e)
            {
                Debug.Log("ReceiveData Error : " + e.ToString());
                return true;
            }
        }

        private void CheckMessageType(string data)
        {
            try
            {
                Tweet tweet = JsonUtility.FromJson<Tweet>(data);
                //Debug.Log("DATA"+data);
                Coordinates coordinate = JsonUtility.FromJson<Coordinates>(data);
                Debug.Log("Coordinates?? " + coordinate.coordinates.coordinates[0]);

                //Debug.Log("1:"+tweet.text);
                //Debug.Log("2:" + tweet.coordinates);
                //Debug.Log("2-1:" + tweet.coordinates == null);
                //Debug.Log("3:"+tweet.coordinates.coordinates[0]);

                

                if (tweet.text != null && tweet.id_str != null )
                {
                    if (coordinate != null)
                    {
                        messageType = StreamMessageType.GeoTweet;
                    }

                    else
                    {
                        messageType = StreamMessageType.Tweet;
                    }
                    
            
                    return;
                }
                
               

               

                StreamEvent streamEvent = JsonUtility.FromJson<StreamEvent>(data);
                if (streamEvent.event_name != null)
                {
                    messageType = StreamMessageType.StreamEvent;
                    return;
                }

                FriendsList friendsList = JsonUtility.FromJson<FriendsList>(data);
                if (friendsList.friends != null)
                {
                    messageType = StreamMessageType.FriendsList;
                    return;
                }

                

                DirectMessage directMessage = JsonUtility.FromJson<DirectMessage>(data);
                if (directMessage.recipient_screen_name != null)
                {
                    messageType = StreamMessageType.DirectMessage;
                    return;
                }

                StatusDeletionNotice statusDeletionNotice = JsonUtility.FromJson<StatusDeletionNotice>(data);
                if (statusDeletionNotice.delete != null)
                {
                    messageType = StreamMessageType.StatusDeletionNotice;
                    return;
                }

                LocationDeletionNotice locationDeletionNotice = JsonUtility.FromJson<LocationDeletionNotice>(data);
                if (locationDeletionNotice.scrub_geo != null)
                {
                    messageType = StreamMessageType.LocationDeletionNotice;
                    Debug.Log("LOCATION NOTICE");
                    return;
                }

                LimitNotice limitNotice = JsonUtility.FromJson<LimitNotice>(data);
                if (limitNotice.limit != null)
                {
                    messageType = StreamMessageType.LimitNotice;
                    return;
                }

                WithheldContentNotice withheldContentNotice = JsonUtility.FromJson<WithheldContentNotice>(data);
                if (withheldContentNotice.status_withheld != null || withheldContentNotice.user_withheld != null)
                {
                    messageType = StreamMessageType.WithheldContentNotice;
                    return;
                }

                DisconnectMessage disconnectMessage = JsonUtility.FromJson<DisconnectMessage>(data);
                if (disconnectMessage.disconnect != null)
                {
                    messageType = StreamMessageType.DisconnectMessage;
                    return;
                }

                StallWarning stallWarning = JsonUtility.FromJson<StallWarning>(data);
                if (stallWarning.warning != null)
                {
                    messageType = StreamMessageType.StallWarning;
                    Debug.Log("StallWarning");
                    return;
                }

                messageType = StreamMessageType.None;
                return;

            } catch (System.Exception e)
            {
                //Toactivatelater-->//Debug.Log("CheckMessageType Error : " + e.ToString());
                messageType = StreamMessageType.None;
                return;
            }
        }
    }

    #endregion


    #region Parameters for statuses/filter
    public class FilterTrack
    {
        private List<string> tracks;

        public FilterTrack(string track)
        {
            tracks = new List<string>();
            tracks.Add(track);
        }
        public FilterTrack(List<string> tracks)
        {
            this.tracks = tracks;
        }
        public void AddTrack(string track)
        {
            tracks.Add(track);
        }
        public void AddTracks(List<string> tracks)
        {
            foreach (string track in tracks)
            {
                this.tracks.Add(track);
            }
        }
        public string GetKey()
        {
            return "track";
        }
        public string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string track in tracks)
            {
                sb.Append(track + ",");
            }
            sb.Length -= 1;
            return sb.ToString();
        }
    }


    public class FilterLocations
    {
        private List<Coordinate> locations;

        public FilterLocations()
        {
            locations = new List<Coordinate>();
            locations.Add(new Coordinate(-0.135701f, 51.430735f));// extended olympic park area
            locations.Add(new Coordinate(0.104096f, 51.608062f)); // extended olympic park area
            //locations.Add(new Coordinate(-0.055701f, 51.530735f)); // (benthal green) Olympic Park (southwest) 51.530735, -0.035701
            //locations.Add(new Coordinate(-0.000091f, 51.558062f)); //( mid st. patricks park) olympic Park (NorthEast)  51.558062, -0.000691
            //locations.Add(new Coordinate(-0.486767f, 51.343943f));//<--Whole London //(-180f, -90f));//<--whole globe coordinates (southwest)
           // locations.Add(new Coordinate(0.217927f,51.635277f));//<--whole London //( 180f,  90f));// <-- whole globe coordinates (NorthEast)
            Debug.Log("LOCATION:" + locations);
        }
        public FilterLocations(Coordinate southwest, Coordinate northeast)
        {
            locations = new List<Coordinate>();
            locations.Add(southwest);
            locations.Add(northeast);
        }
        public void AddLocation(Coordinate southwest, Coordinate northeast)
        {
            locations.Add(southwest);
            locations.Add(northeast);
        }
        public string GetKey()
        {
            return "locations";
        }
        public string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            foreach(Coordinate location in locations)
            {
                sb.Append(location.lng.ToString("F1") + "," + location.lat.ToString("F1") + ",");

            }
            sb.Length -= 1;
            return sb.ToString();
        }
    }

    public class Coordinate
    {
        public float lng { get; set; }
        public float lat { get; set; }

        public Coordinate(float lng, float lat)
        {
            this.lng = lng;
            this.lat = lat;
        }
    }

    public class FilterFollow
    {
        private List<string> screen_names;
        private List<long> ids;

        public FilterFollow(List<string> screen_names)
        {
            this.screen_names = screen_names;
        }
        public FilterFollow(List<long> ids)
        {
            this.ids = ids;
        }
        public FilterFollow(long id)
        {
            ids = new List<long>();
            ids.Add(id);
        }
        public void AddId(long id)
        {
            ids.Add(id);
        }
        public void AddIds(List<long> ids)
        {
            foreach(long id in ids)
            {
                this.ids.Add(id);
            }
        }
        public string GetKey()
        {
            return "follow";
        }
        public string GetValue()
        {
            StringBuilder sb = new StringBuilder();
            if (ids.Count > 0)
            {
                foreach (long id in ids)
                {
                    sb.Append(id.ToString() + ",");
                }
            } else
            {
                foreach (string screen_name in screen_names)
                {
                    sb.Append(screen_name + ",");
                }
            }
            sb.Length -= 1;
            return sb.ToString();
        }
    }
    #endregion
}

