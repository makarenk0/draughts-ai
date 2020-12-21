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
        public double AvailableTime { get; set; }

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


        public KeyValuePair<int, byte[,]> GetBoardState()
        {
            String board = GetAsync(_getGameInfoRequest).Result;

            JsonDocument parsedResult = JsonDocument.Parse(board);

            //in case game is over
            if (parsedResult.RootElement.GetProperty("data").GetProperty("status").GetRawText().Trim('\"') == "Game is over")
            {
                return new KeyValuePair<int, byte[,]>(2, null);
            }
            //in case not my turn
            if (parsedResult.RootElement.GetProperty("data").GetProperty("whose_turn").GetRawText().Trim('\"') != MyColor)
            {
                return new KeyValuePair<int, byte[,]>(0, null);
            }

            JsonElement boardRaw = parsedResult.RootElement.GetProperty("data").GetProperty("board");
            byte[,] result = new byte[8, 8];
            for (int i = 0; i < boardRaw.GetArrayLength(); i++)
            {
                String color = boardRaw[i].GetProperty("color").GetRawText().Trim('\"');
                bool king = boardRaw[i].GetProperty("king").GetRawText() == "true" ? true : false;
                int x = (int)UInt32.Parse(boardRaw[i].GetProperty("row").GetRawText());
                int y = (int)UInt32.Parse(boardRaw[i].GetProperty("column").GetRawText());
                result[(y * 2) + (1 * ((x + 1) % 2)), x] = "RED" == color ? (king ? (byte)3 : (byte)1) : (king ? (byte)4 : (byte)2);
            }
            AvailableTime = parsedResult.RootElement.GetProperty("data").GetProperty("available_time").GetDouble();

            return new KeyValuePair<int, byte[,]>(1, result);
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
            try
            {
                String result = PostAsync(_joinGameRequest, httpContent).Result;

                JsonDocument parsedResult = JsonDocument.Parse(result);
                _token = parsedResult.RootElement.GetProperty("data").GetProperty("token").GetRawText().Trim('\"');
                MyColor = parsedResult.RootElement.GetProperty("data").GetProperty("color").GetRawText().Trim('\"');

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
            }
            catch (System.AggregateException)
            {
                Console.WriteLine("Specified host doesn't respond");
                Console.WriteLine(String.Concat("Attempt to do: ", _joinGameRequest));
                throw new Exception("Cant start the game");
            }
            
        }

        public void MakeMove(string move)
        {
            
            HttpContent httpContent = new StringContent(String.Concat("{\"name\": \"Make move\",", "\"move\": ", move, "}"));
            String result = PostAsync(_makeMoveRequest, httpContent).Result;
        }


        private void LoadRequestsPatterns(String filename)
        {
            if (File.Exists(filename))
            {
                string text = File.ReadAllText(filename);
                JsonDocument doc = JsonDocument.Parse(text);
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
