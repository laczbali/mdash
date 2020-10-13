using System.Collections.Generic;

namespace Dashboard.API.DTOs
{
    public class ServerStatusDTO
    {
        public VpnDTO vpn;
        public List<DriveDTO> drives;
        public List<ServerProcessDTO> processes;
        public ServerResourceDTO resources;
        public string errors;
    }
}