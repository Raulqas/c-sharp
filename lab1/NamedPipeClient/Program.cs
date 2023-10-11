﻿using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Client
{
    class Program
    {
        public struct Ad
        {
            public int X;
            public int Y;
            public bool DA;
        }

        static async Task Main()
        {
            Console.WriteLine("Соединяю с сервером...\n");

            using (var clientPipe = new NamedPipeClientStream(".", "tonel", PipeDirection.InOut, PipeOptions.Asynchronous))
            {
                await clientPipe.ConnectAsync();

                Console.WriteLine("Соединение установлено, отправляем данные...\n");

                Ad dataToSend = new Ad
                {
                    X = 100,
                    Y = 150,
                    DA = false
                };

                SendData(clientPipe, dataToSend);

                Console.WriteLine($"Отправлены данные: X={dataToSend.X}, Y={dataToSend.Y}, DA={dataToSend.DA}\n");

                byte[] buffer = new byte[Marshal.SizeOf<Ad>()];
                await clientPipe.ReadAsync(buffer, 0, buffer.Length);

                Ad receivedData = MemoryMarshal.Read<Ad>(buffer);

                Console.WriteLine($"Получены данные от сервера: X={receivedData.X}, Y={receivedData.Y}, DA={receivedData.DA}\n");
            }

            Console.WriteLine("Клиент завершил работу.");
        }

        static void SendData(NamedPipeClientStream pipe, Ad data)
        {
            byte[] buffer = new byte[Marshal.SizeOf<Ad>()];
            MemoryMarshal.Write(buffer, ref data);
            pipe.Write(buffer, 0, buffer.Length);
        }
    }
}