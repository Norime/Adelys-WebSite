using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;

namespace Adelys_WebSite.SAL.Minecraft
{
    /// <summary>
    /// Interroge le serveur comme la liste des serveurs du jeu, pour connaître sa version sans plugin
    /// </summary>
    public static class MinecraftServerPing
    {
        public static async Task<string?> GetVersionName(string address, CancellationToken cancellationToken)
        {
            string host = address;
            int port = 25565;
            int separator = address.LastIndexOf(':');
            if (separator > 0 && int.TryParse(address[(separator + 1)..], out int parsedPort))
            {
                host = address[..separator];
                port = parsedPort;
            }

            using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            using TcpClient client = new TcpClient();
            await client.ConnectAsync(host, port, timeout.Token);
            NetworkStream stream = client.GetStream();

            // Handshake (état 1 = statut), puis demande de statut
            using MemoryStream handshake = new MemoryStream();
            WriteVarInt(handshake, 0);
            WriteVarInt(handshake, -1);
            byte[] hostBytes = Encoding.UTF8.GetBytes(host);
            WriteVarInt(handshake, hostBytes.Length);
            handshake.Write(hostBytes);
            handshake.WriteByte((byte)(port >> 8));
            handshake.WriteByte((byte)port);
            WriteVarInt(handshake, 1);
            await WritePacket(stream, handshake.ToArray(), timeout.Token);
            await WritePacket(stream, new byte[] { 0 }, timeout.Token);

            await ReadVarInt(stream, timeout.Token);
            await ReadVarInt(stream, timeout.Token);
            int length = await ReadVarInt(stream, timeout.Token);
            byte[] json = new byte[length];
            await stream.ReadExactlyAsync(json, timeout.Token);
            return JsonNode.Parse(json)?["version"]?["name"]?.GetValue<string>();
        }

        private static async Task WritePacket(NetworkStream stream, byte[] payload, CancellationToken cancellationToken)
        {
            using MemoryStream packet = new MemoryStream();
            WriteVarInt(packet, payload.Length);
            packet.Write(payload);
            await stream.WriteAsync(packet.ToArray(), cancellationToken);
        }

        private static void WriteVarInt(Stream stream, int value)
        {
            uint remaining = (uint)value;
            do
            {
                byte current = (byte)(remaining & 0x7F);
                remaining >>= 7;
                stream.WriteByte(remaining != 0 ? (byte)(current | 0x80) : current);
            } while (remaining != 0);
        }

        private static async Task<int> ReadVarInt(NetworkStream stream, CancellationToken cancellationToken)
        {
            int value = 0;
            byte[] buffer = new byte[1];
            for (int shift = 0; shift < 35; shift += 7)
            {
                await stream.ReadExactlyAsync(buffer, cancellationToken);
                value |= (buffer[0] & 0x7F) << shift;
                if ((buffer[0] & 0x80) == 0)
                {
                    return value;
                }
            }
            throw new InvalidDataException("Réponse du serveur Minecraft invalide");
        }
    }
}
