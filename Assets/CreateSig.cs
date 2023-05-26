using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;

public class CreateSig : MonoBehaviour {

	public string token;
	// Use this for initialization
	IEnumerator Start () {

        /*   byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                          + "\"app_id\": \"57a5c097-f698-404f-8bd5-3933e8947a38\","
                                          + "\"contents\": {\"en\": \"English Message\"},"
                                          + "\"included_segments\": [\"All\"]}");*/
        /*     byte[] byteArray = Encoding.UTF8.GetBytes("{"
              + "\"app_id\": \"c4adbdc6-6a63-4c38-93b2-66eff1d57970\","
              + "\"contents\": {\"en\": \"English Message\"},"
               + "\"included_segments\": [\"All\"]}");
             //  + "\"include_player_ids\": [\"6180b042-f4c7-44f8-88f8-f5a87067da47\"]}");

             Dictionary<string, string> headers = new Dictionary<string, string>();
             headers.Add("Content-Type", "application/json");

             headers.Add("authorization", "Basic ODVkMGZkNzEtNDUyZS00MTRiLWEzMDMtMzFhYWQ3MWExNGIx");
             WWW dem = new WWW("https://onesignal.com/api/v1/notifications",byteArray, headers);
             yield return dem;

           //  "include_player_ids": ["6180b042-f4c7-44f8-88f8-f5a87067da47"];//,"8e0f21fa-9a5a-4ae7-a9a6-ca1f24294b86"],

             token = dem.text.Split (':')[1].Trim('"');
             token = token.Split (',') [0].Trim ('"');
             Debug.Log(dem.text);*/


        // Enable line below to enable logging if you are having issues setting up OneSignal. (logLevel, visualLogLevel)
        // OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.INFO, OneSignal.LOG_LEVEL.INFO);
        yield return new WaitForSeconds(0f);
        OneSignal.StartInit("c4adbdc6-6a63-4c38-93b2-66eff1d57970")
          .HandleNotificationOpened(HandleNotificationOpened)
          .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

    }

    // Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        Debug.Log("In Handle Notification Opened : ");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
