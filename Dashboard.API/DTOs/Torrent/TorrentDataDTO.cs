using System;
using System.Collections.Generic;

namespace Dashboard.API.DTOs
{
    public class TorrentDataDTO
    {
        public List<TorrentSingleDTO> downloads = new List<TorrentSingleDTO>();
        public int numOfUpload = 0;
        public int totalDlSpeed = 0;
        public int totalUpSpeed = 0;
        public int totalTorrents = 0;
        public DateTime querytime = DateTime.Now;
        public int maxEta = 0;
        public string uiUrl = "";
    }
}