using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RMMBY_Installer_RM
{
    public static class GameData
    {
        // Stores All Supported Games
        public static GetGameList.Game currentGame;
        public static string currentCategory;
        public static int modCount;

        public static bool exclusiveMode;
        public static string exclusiveSchema;

        public static TcpClient clientSocket = new TcpClient();
        public static NetworkStream stream;
    }
}
