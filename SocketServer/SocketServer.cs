﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace BotJobAndCommands.SocketServer;

public class WebSocketServer
{
    private Dictionary<string, ICommand> _availableCommands;

    public WebSocketServer(Dictionary<string, ICommand> availableCommands)
    {
        _availableCommands = availableCommands;
    }

    public async Task Start()
    {
        string ip = "127.0.0.1";
        int port = 80;
        var server = new TcpListener(IPAddress.Parse(ip), port);

        server.Start();
        Console.WriteLine("WebSocketServer has started on {0}:{1}, Waiting for a connection…", ip, port);

        TcpClient client = server.AcceptTcpClient();

        NetworkStream clientStream = client.GetStream();

        // enter to an infinite cycle to be able to handle every change in stream
        while (true)
        {
            while (!clientStream.DataAvailable) ;
            while (client.Available < 3) ; // match against "get"

            byte[] bytes = new byte[client.Available];
            clientStream.Read(bytes, 0, client.Available);
            string s = Encoding.UTF8.GetString(bytes);

            if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase))
            {
                //Console.WriteLine("=====Handshaking from client=====\n{0}", s);

                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                // 3. Compute SHA-1 and Base64 hash of the new value
                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                byte[] response = Encoding.UTF8.GetBytes(
                    "HTTP/1.1 101 Switching Protocols\r\n" +
                    "Connection: Upgrade\r\n" +
                    "Upgrade: websocket\r\n" +
                    "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                clientStream.Write(response, 0, response.Length);
            }
            else
            {
                bool fin = (bytes[0] & 0b10000000) != 0, 
                    mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
                int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                    offset = 2;
                ulong msglen = (ulong)(bytes[1] & 0b01111111);

                if (msglen == 126)
                {
                    // bytes are reversed because websocket will print them in Big-Endian, whereas
                    // BitConverter will want them arranged in little-endian on windows
                    msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                    offset = 4;
                }
                else if (msglen == 127)
                {
                    // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering
                    // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                    // websocket frame available through client.Available).
                    msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] }, 0);
                    offset = 10;
                }

                if (msglen == 0)
                    Console.WriteLine("msglen == 0");
                else if (mask)
                {
                    byte[] decoded = new byte[msglen];
                    byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                    offset += 4;

                    for (ulong i = 0; i < msglen; ++i)
                        decoded[i] = (byte)(bytes[(ulong)offset + i] ^ masks[i % 4]);

                    string text = Encoding.UTF8.GetString(decoded);
                    Console.WriteLine("{0}", text);
                    WsClientData data = JsonSerializer.Deserialize<WsClientData>(text)!;

                    //string response = await RunClientCommand(data);
                    Socket socket = client.GetStream().Socket;
                    string response = "AHHHHHHH";
                    byte[] bytesResponse = Encoding.UTF8.GetBytes(response);

                    client.Client.Send(bytesResponse);
                    SendMessage(socket, response);
                    socket.Send(bytesResponse);
                    clientStream.Write(bytesResponse, 0, bytesResponse.Length);
                    
                }
                else
                    Console.WriteLine("mask bit not set");

                Console.WriteLine();
            }
        }
    }

    private void SendMessage(Socket socket, string message)
    {
        byte[] asd = Encoding.UTF8.GetBytes(message);
        socket.Send(asd);
    }

    private async Task<string> RunClientCommand(WsClientData data)
    {
        try
        {
            switch (data.Command)
            {
                case WebSocketClientCommand.Command:
                    await RunCommand(data.Data["command"].ToString()!);
                    break;
            }
        }
        catch(Exception e)
        {
            return $"Bad Stuff duud, {e.Message}";
        }

        return "Success";
    }

    private async Task RunCommand(string command)
    {
        if (_availableCommands.ContainsKey(command.ToLower()))
        {
            ICommand theCommand = _availableCommands[command.ToLower()];
            if (!theCommand.IsBotCommand)
                return;

            CommandResponse response = await theCommand.RunCommand(Array.Empty<string>());
            Console.WriteLine(response.Message);
        }
    }

    private record WsClientData(WebSocketClientCommand Command, Dictionary<string, object> Data);
}

public enum WebSocketClientCommand
{
    Command
}