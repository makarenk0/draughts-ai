using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
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

        private String _myColor;
        private String _token;

        public string MyColor { get => _myColor; set => _myColor = value; }

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
           
            //MakeMove("[9, 13]");
        }


        public int[,] GetBoardState()
        {
            String board = GetAsync(_getGameInfoRequest).Result;

            using JsonDocument parsedResult = JsonDocument.Parse(board);
            JsonElement boardRaw = parsedResult.RootElement.GetProperty("data").GetProperty("board");

            int[,] result = new int[8, 8];
            for (int i = 0; i < boardRaw.GetArrayLength(); i++)
            {
                String color = boardRaw[i].GetProperty("color").GetRawText().Trim('\"');
                bool king = boardRaw[i].GetProperty("king").GetRawText() == "true" ? true : false;
                int x = (int)UInt32.Parse(boardRaw[i].GetProperty("row").GetRawText());
                int y = (int)UInt32.Parse(boardRaw[i].GetProperty("column").GetRawText());
                result[(y * 2) + (1 * ((x + 1) % 2)), x] = "RED" == color ? (king ? 3 : 1) : (king ? 4 : 2);
            }

            return result;
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
            MyColor = parsedResult.RootElement.GetProperty("data").GetProperty("color").GetRawText().Trim('\"');

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
        }

        private void MakeMove(string move)
        {
            
            HttpContent httpContent = new StringContent(String.Concat("{\"name\": \"Make move\",", "\"move\": ", move, "}"));
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
                _getGameInfoRequest = _gameInfoRequestPattern.GetProperty("url").GetRawText().Trim('\"');

                _gameJoinRequestPattern = _root.GetProperty("item")[1];
                _joinGameRequest = _gameJoinRequestPattern.GetProperty("url").GetRawText().Trim('\"');

                _makeMoveRequestPattern = _root.GetProperty("item")[2];
                _makeMoveRequest = _makeMoveRequestPattern.GetProperty("url").GetRawText().Trim('\"');
            }
            else
            {
                throw new FileNotFoundException(String.Concat(filename, " doesn't exist!"));
            }
        }
    }
}
