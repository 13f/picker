using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json.Linq;

namespace CognitiveApp {
  public static class Emoji {

    static void save( JObject jo, string path ) {
      string json = jo.ToString( Newtonsoft.Json.Formatting.Indented );
      File.WriteAllText( path, json );
    }

    public static async Task Run( string pathSource, string pathResult, int millisecondsDelay = 3000 ) {
      string key = "4571486e47444cb9988cb2cb78647106";
      EmotionServiceClient emotionServiceClient = new EmotionServiceClient( key );

      Console.WriteLine( "load data file..." );
      // load
      string json = File.ReadAllText( pathSource );
      JObject root = JObject.Parse( json );

      JArray people = null;
      JArray items = (JArray)root["data"];
      foreach ( var item in items ) {
        if ( (string)item["name"] == "people" ) {
          people = (JArray)item["items"];
          break;
        }
      }
      Console.WriteLine( "got [people] node..." );

      foreach ( var item in people ) {
        if ( item["updatedAt"] != null ) // skip
          continue;

        await pickEmotion( item, emotionServiceClient );
        // update tag
        item["updatedAt"] = DateTime.UtcNow;
        // save
        save( root, pathResult );
        // wait
        Console.WriteLine( "  ... wait... and then continue..." );
        await Task.Delay( millisecondsDelay );
      }
      Console.WriteLine( "saved...over..." );
    }

    static async Task pickEmotion( JToken token, EmotionServiceClient emotionServiceClient ) {
      if ( token == null )
        return;
      string image = (string)token["image"];
      Console.WriteLine( "  > " + image );

      try {
        Emotion[] emotionsResult = await emotionServiceClient.RecognizeAsync( image );
        JArray emotions = new JArray();
        if ( emotionsResult != null ) {
          foreach ( var er in emotionsResult ) {
            JObject rect = new JObject();
            rect["left"] = er.FaceRectangle.Left;
            rect["top"] = er.FaceRectangle.Top;
            rect["width"] = er.FaceRectangle.Width;
            rect["height"] = er.FaceRectangle.Height;

            JObject scores = new JObject();
            scores["anger"] = er.Scores.Anger;
            scores["contempt"] = er.Scores.Contempt;
            scores["disgust"] = er.Scores.Disgust;
            scores["fear"] = er.Scores.Fear;
            scores["happiness"] = er.Scores.Happiness;
            scores["neutral"] = er.Scores.Neutral;
            scores["sadness"] = er.Scores.Sadness;
            scores["surprise"] = er.Scores.Surprise;

            JObject emotion = new JObject();
            emotion["faceRectangle"] = rect;
            emotion["scores"] = scores;

            emotions.Add( emotion );
          }
        }
        token["emotions"] = emotions;
      }
      catch(Exception ex ) {
        Console.WriteLine( " " + ex.Message );
      }
      
    }

  }

}
