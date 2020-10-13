using System;
using System.Collections.Generic;

namespace Dashboard.API.DTOs
{
    public class VpnDTO
    {
        public DateTime updatedTime;
        public List<VpnClientDTO> clientList;
        public float sumTrafficMbytes;
    }
}