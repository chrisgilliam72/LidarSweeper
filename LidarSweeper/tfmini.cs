
using System.IO.Ports;

const string PortName = "/dev/serial0";
const int BaudRate = 115200;

Console.WriteLine($"Opening {PortName}...");

using var port = new SerialPort(PortName, BaudRate)
{
    ReadTimeout = 1000
};

port.Open();

Console.WriteLine("Connected.");
Console.WriteLine();

while (true)
{
    try
    {
        // Find first header byte
        int b;
        do
        {
            b = port.ReadByte();
        }
        while (b != 0x59);

        // Find second header byte
        if (port.ReadByte() != 0x59)
            continue;

        // Read remaining 7 bytes
        byte[] frame = new byte[7];

        int offset = 0;
        while (offset < 7)
        {
            offset += port.Read(frame, offset, 7 - offset);
        }

        int distance = frame[0] | (frame[1] << 8);
        int strength = frame[2] | (frame[3] << 8);
        int temperatureRaw = frame[4] | (frame[5] << 8);

        // Convert according to Benewake documentation
        double temperature = temperatureRaw / 8.0 - 256.0;

        // Verify checksum
        int checksum =
            0x59 +
            0x59 +
            frame[0] +
            frame[1] +
            frame[2] +
            frame[3] +
            frame[4] +
            frame[5];

        checksum &= 0xFF;

        bool valid = checksum == frame[6];

        Console.WriteLine(
            $"{DateTime.Now:HH:mm:ss.fff}  " +
            $"Distance={distance,4} cm  " +
            $"Strength={strength,5}  " +
            $"Temp={temperature,6:F1}°C  " +
            $"Checksum={(valid ? "OK" : "BAD")}"
        );
    }
    catch (TimeoutException)
    {
        Console.WriteLine("Timeout waiting for data...");
    }
}
