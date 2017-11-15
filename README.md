This is a modified version of the toofu37/twitter-for-unity for Unity 2017, to enable the collection of Geolocated Tweets usin gthe Stream API.


1.1 Twitter API: 

TwitterCore provides a TwitterApiClient for making authenticated Twitter API requests. Twitter has two different  API’s. REST and STREAM are commonly used for getting geolocated tweets. 
The Rest API, is sending twitter a request and receives a list of tweets in one event within a radius of a specified location. These may be current tweets or a few days old. To receive new tweets the application must create a new request and filter out the overlapping tweets. 
The Stream API, opens a firehose and tweets are delivered per event until this firehose is stopped. Each tweet is sent as a GeoJson and needs to be parsed using a custom made parser. It’s the only API which produces real-time outputs. 

I am using the twitter for Unity from toofu37, for authentication and the basic function for receiving real time tweets. Tweeter stream API requires 4 authentication keys:
-	Aoath
-	Secter
-	Token
-	Token Secret
Which are generated once signed up as a developer in Twitter and generating new tokens. 
The library consists of several C# scripts, 5 in total, which are used to 1. Authorize the app to download tweets, 2. Make a request, 3. Separate tweets by type, 4. Serialize 5. Develop.
On top of that we developed several scripts to deal with the current projects. 
1.	A script for initiating the process,  starting the firehose 
2.	A script for the serialization. The script reads the GeoJson and creates a class to store the values.
3.	A script to do the instantiation in unity and store attributes in a Game object. 
 

The application stream live tweets and visualizes them as twitter objects within the current scene. The testLaserSystemLibrary brings tweets in the same coordinate system in London QUEP and visualizes them in the unity coordinate system and scale. 
