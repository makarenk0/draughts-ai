using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace draughts_ai
{
    class RequestsExecutor
    {

        private HttpClient client;
        private String _getGameInfoRequest = "";
        private String _joinGameRequest = "";
        private String _makeMoveRequest = "";

        private JsonElement _root;
        private JsonElement _gameInfoRequestPattern;
        private JsonElement _gameJoinRequestPattern;
        private JsonElement _makeMoveRequestPattern;

        private DraughtColor _myColor;
        private String _token;


        public enum DraughtColor
        {
           RED,
           BLACK
        };

        public RequestsExecutor(String requestsPatternsFilename)
        {
            client = new HttpClient();
            LoadRequestsPatterns(requestsPatternsFilename);


            JoinTheGame();
            MakeMove("[9, 13]");
            //Task.Delay(2000).ContinueWith(t => );
         
           

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

        private async Task<String> PostAsync(string path, HttpContent httpContent)
        {
            HttpResponseMessage response = await client.PostAsync(path, httpContent);

            String responseJSON = "";
            if (response.IsSuccessStatusCode)
            {
                responseJSON = await response.Content.ReadAsStringAsync();
                return responseJSON;
               
            }
            return response.ReasonPhrase;
            
        }
        
        private void JoinTheGame()
        {
            HttpContent httpContent = new StringContent(_gameJoinRequestPattern.GetRawText());
            String result = PostAsync(_joinGameRequest, httpContent).Result;

            using JsonDocument parsedResult = JsonDocument.Parse(result);
            _token = parsedResult.RootElement.GetProperty("data").GetProperty("token").GetRawText().Trim('\"');
            _myColor = parsedResult.RootElement.GetProperty("data").GetProperty("color").GetRawText().Trim('\"') == "RED" ? DraughtColor.RED : DraughtColor.BLACK;


            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        }

        private void MakeMove(string move)
        {
            HttpContent httpContent = new StringContent(String.Concat("\"name\": \"Make move\",", "\"move\": ", move));
            String result = PostAsync(_makeMoveRequest, httpContent).Result;
        }


        private void LoadRequestsPatterns(String filename)
        {
            if (File.Exists(filename))
            {
                string text = File.ReadAllText(filename);
                using JsonDocument doc = JsonDocument.Parse(text);
                _root = doc.RootElement.Clone();

             

                _gameInfoRequestPattern = _root.GetProperty("item")[0];
                _getGameInfoRequest = _gameInfoRequestPattern.GetProperty("request").GetProperty("url").GetProperty("raw").GetRawText().Trim('\"');

                _gameJoinRequestPattern = _root.GetProperty("item")[1];
                _joinGameRequest = _gameJoinRequestPattern.GetProperty("request").GetProperty("url").GetProperty("raw").GetRawText().Trim('\"');

                _makeMoveRequestPattern = _root.GetProperty("item")[2];
                _makeMoveRequest = _gameJoinRequestPattern.GetProperty("request").GetProperty("url").GetProperty("raw").GetRawText().Trim('\"');
            }
            else
            {
                throw new FileNotFoundException(String.Concat(filename, " doesn't exist!"));
            }
        }
    }
}
