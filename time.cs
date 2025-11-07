using System;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;

public class CPHInline
{
    public class GeoNamesSearchResult
    {
        public GeoName[] geonames { get; set; }
    }

    public class GeoName
    {
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class GeoNamesTimezoneResult
    {
        public string timezoneId { get; set; }
        public string time { get; set; }
    }

    public bool Execute()
    {
        const string username = ""; // 👈 Replace with your GeoNames username

        if (!args.TryGetValue("rawInput", out object rawInputObj) || rawInputObj == null)
        {
            CPH.SendMessage("Please provide a city. Example: !time Tokyo");
            return false;
        }

        string[] parts = rawInputObj.ToString().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            CPH.SendMessage("Please provide a city. Example: !time Tokyo");
            return false;
        }

        string city = parts[1].Trim();

        try
        {
            using (WebClient client = new WebClient())
            {
                // Step 1: Get lat/lng from GeoNames city search
                string searchUrl = $"http://api.geonames.org/searchJSON?q={Uri.EscapeDataString(city)}&maxRows=1&username={username}";
                string searchJson = client.DownloadString(searchUrl);
                GeoNamesSearchResult searchResult = JsonConvert.DeserializeObject<GeoNamesSearchResult>(searchJson);

                if (searchResult.geonames.Length == 0)
                {
                    CPH.SendMessage($"City '{city}' not found.");
                    return false;
                }

                var geo = searchResult.geonames[0];
                double lat = geo.lat;
                double lng = geo.lng;

                // Step 2: Get timezone from lat/lng
                string tzUrl = $"http://api.geonames.org/timezoneJSON?lat={lat}&lng={lng}&username={username}";
                string tzJson = client.DownloadString(tzUrl);
                GeoNamesTimezoneResult tzResult = JsonConvert.DeserializeObject<GeoNamesTimezoneResult>(tzJson);

                if (string.IsNullOrEmpty(tzResult.timezoneId))
                {
                    CPH.SendMessage("Failed to determine timezone.");
                    return false;
                }

                // Format response
                string formattedTime = DateTime.Parse(tzResult.time).ToString("dddd, MMMM d, yyyy - h:mm tt");
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                string prettyCity = textInfo.ToTitleCase(city.ToLower());

                CPH.SendMessage($"🕒 The current time in {prettyCity} is: {formattedTime} ({tzResult.timezoneId})");
            }
        }
        catch (Exception ex)
        {
            CPH.SendMessage($"❌ Error getting time for '{city}': {ex.Message}");
        }

        return true;
    }
}
