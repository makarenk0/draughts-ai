using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace draughts_ai
{
    class RequestsExecutor
    {

        private HttpClient client;
        private const String _getGameInfoRequest = "http://192.168.1.19:8081/game";
        private const String _joinGameRequest = "http://192.168.1.19:8081/game?team_name=Loom";
        //private const String _getGameInfoRequest = "http://192.168.1.19:8081/game";

        private JsonElement _root;
        //private JsonElement _gameInfoRequestPattern;
        private JsonElement _gameJoinRequestPattern;
        private JsonElement _makeMoveRequestPattern;

        public RequestsExecutor(String requestsPatternsFilename)
        {
            client = new HttpClient();
            LoadRequestsPatterns(requestsPatternsFilename);
            

            String res = GetAsync(_getGameInfoRequest).Result;
            String res1 = PostAsync(_joinGameRequest, _gameJoinRequestPattern.GetRawText()).Result;

            Console.WriteLine(res);
            Console.WriteLine(res1);
        }


        private async Task<String> GetAsync(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            String responseJSON = "";
            if (response.IsSuccessStatusCode)
            {
                responseJSON = await response.Content.ReadAsStringAsync();
            }
            return responseJSON;
        }

        private async Task<String> PostAsync(string path, string content)
        {
            HttpContent httpContent = new StringContent(content);
            HttpResponseMessage response = await client.PostAsync(path, httpContent);

            String responseJSON = "";
            String errorMsg = "";
            if (response.IsSuccessStatusCode)
            {
                responseJSON = await response.Content.ReadAsStringAsync();
                return responseJSON;
               
            }
            return response.ReasonPhrase;
            
        }


        private void LoadRequestsPatterns(String filename)
        {
            if (File.Exists(filename))
            {
                string text = File.ReadAllText(filename);
                using JsonDocument doc = JsonDocument.Parse(text);
                _root = doc.RootElement.Clone();

                //_gameInfoRequestPattern = _root.GetProperty("item")[0];
                _gameJoinRequestPattern = _root.GetProperty("item")[1];
            }
            else
            {
                throw new FileNotFoundException(String.Concat(filename, " doesn't exist!"));
            }
        }
    }
}
