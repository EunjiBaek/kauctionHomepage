using Newtonsoft.Json;
using System.Collections.Generic;

namespace KA.Entities.Models.Content
{
    public class Board
    {
        [JsonProperty("board_info")]
        private readonly BoardInfo _boardInfo;

        [JsonProperty("board_docs")]
        private readonly List<BoardDoc> _boardDocs;

        [JsonProperty("total_count")]
        private readonly int _totalCount;

        public Board(BoardInfo boardInfo, List<BoardDoc> boardDocs, int totalCount)
        {
            _boardInfo = boardInfo;
            _boardDocs = boardDocs;
            _totalCount = totalCount;
        }
    }
}
