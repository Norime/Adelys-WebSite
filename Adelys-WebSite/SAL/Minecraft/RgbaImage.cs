using System.Buffers.Binary;
using System.IO.Compression;

namespace Adelys_WebSite.SAL.Minecraft
{
    /// <summary>
    /// Image RGBA 8 bits non prémultipliée, avec juste ce qu'il faut de PNG pour lire les textures du jeu et écrire les icônes
    /// </summary>
    public class RgbaImage
    {
        private static readonly byte[] PngSignature = { 137, 80, 78, 71, 13, 10, 26, 10 };

        public int Width { get; }
        public int Height { get; }
        public byte[] Pixels { get; }

        public RgbaImage(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new byte[width * height * 4];
        }

        public static RgbaImage ReadPng(Stream stream)
        {
            using BinaryReader reader = new BinaryReader(stream);
            if (!reader.ReadBytes(8).SequenceEqual(PngSignature))
            {
                throw new InvalidDataException("Ce fichier n'est pas un PNG");
            }

            int width = 0, height = 0, bitDepth = 0, colorType = 0;
            byte[] palette = Array.Empty<byte>();
            byte[] transparency = Array.Empty<byte>();
            using MemoryStream compressed = new MemoryStream();
            while (true)
            {
                int length = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                string type = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));
                byte[] data = reader.ReadBytes(length);
                reader.ReadBytes(4);
                if (type == "IHDR")
                {
                    width = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(0));
                    height = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(4));
                    bitDepth = data[8];
                    colorType = data[9];
                    if (data[12] != 0 || bitDepth == 16)
                    {
                        throw new InvalidDataException("PNG entrelacé ou 16 bits non pris en charge");
                    }
                }
                else if (type == "PLTE")
                {
                    palette = data;
                }
                else if (type == "tRNS")
                {
                    transparency = data;
                }
                else if (type == "IDAT")
                {
                    compressed.Write(data);
                }
                else if (type == "IEND")
                {
                    break;
                }
            }

            int channels = colorType switch { 0 => 1, 2 => 3, 3 => 1, 4 => 2, 6 => 4, _ => throw new InvalidDataException("Type de couleur PNG inconnu") };
            int bitsPerPixel = channels * bitDepth;
            int stride = (width * bitsPerPixel + 7) / 8;
            int bytesPerPixel = Math.Max(1, bitsPerPixel / 8);

            compressed.Position = 0;
            using ZLibStream inflater = new ZLibStream(compressed, CompressionMode.Decompress);
            byte[] raw = new byte[(stride + 1) * height];
            inflater.ReadExactly(raw);

            RgbaImage image = new RgbaImage(width, height);
            byte[] previous = new byte[stride];
            byte[] current = new byte[stride];
            for (int y = 0; y < height; y++)
            {
                int filter = raw[y * (stride + 1)];
                Array.Copy(raw, y * (stride + 1) + 1, current, 0, stride);
                Unfilter(filter, current, previous, bytesPerPixel);

                for (int x = 0; x < width; x++)
                {
                    int o = (y * width + x) * 4;
                    switch (colorType)
                    {
                        case 0:
                            int gray = ReadSample(current, x, bitDepth);
                            byte grayByte = ScaleSample(gray, bitDepth);
                            image.Pixels[o] = image.Pixels[o + 1] = image.Pixels[o + 2] = grayByte;
                            image.Pixels[o + 3] = transparency.Length >= 2 && BinaryPrimitives.ReadUInt16BigEndian(transparency) == gray ? (byte)0 : (byte)255;
                            break;
                        case 2:
                            image.Pixels[o] = current[x * 3];
                            image.Pixels[o + 1] = current[x * 3 + 1];
                            image.Pixels[o + 2] = current[x * 3 + 2];
                            image.Pixels[o + 3] = 255;
                            break;
                        case 3:
                            int index = ReadSample(current, x, bitDepth);
                            if (index * 3 + 2 < palette.Length)
                            {
                                image.Pixels[o] = palette[index * 3];
                                image.Pixels[o + 1] = palette[index * 3 + 1];
                                image.Pixels[o + 2] = palette[index * 3 + 2];
                            }
                            image.Pixels[o + 3] = index < transparency.Length ? transparency[index] : (byte)255;
                            break;
                        case 4:
                            image.Pixels[o] = image.Pixels[o + 1] = image.Pixels[o + 2] = current[x * 2];
                            image.Pixels[o + 3] = current[x * 2 + 1];
                            break;
                        case 6:
                            Array.Copy(current, x * 4, image.Pixels, o, 4);
                            break;
                    }
                }
                (previous, current) = (current, previous);
            }
            return image;
        }

        public void WritePng(Stream stream)
        {
            using MemoryStream compressed = new MemoryStream();
            using (ZLibStream deflater = new ZLibStream(compressed, CompressionLevel.SmallestSize, leaveOpen: true))
            {
                for (int y = 0; y < Height; y++)
                {
                    deflater.WriteByte(0);
                    deflater.Write(Pixels, y * Width * 4, Width * 4);
                }
            }

            byte[] header = new byte[13];
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(0), Width);
            BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), Height);
            header[8] = 8;
            header[9] = 6;

            stream.Write(PngSignature);
            WriteChunk(stream, "IHDR", header);
            WriteChunk(stream, "IDAT", compressed.ToArray());
            WriteChunk(stream, "IEND", Array.Empty<byte>());
        }

        private static void WriteChunk(Stream stream, string type, byte[] data)
        {
            byte[] typeBytes = System.Text.Encoding.ASCII.GetBytes(type);
            Span<byte> number = stackalloc byte[4];
            BinaryPrimitives.WriteInt32BigEndian(number, data.Length);
            stream.Write(number);
            stream.Write(typeBytes);
            stream.Write(data);

            uint crc = UpdateCrc(UpdateCrc(0xFFFFFFFF, typeBytes), data) ^ 0xFFFFFFFF;
            BinaryPrimitives.WriteUInt32BigEndian(number, crc);
            stream.Write(number);
        }

        private static readonly uint[] CrcTable = Enumerable.Range(0, 256).Select(n =>
        {
            uint c = (uint)n;
            for (int k = 0; k < 8; k++)
            {
                c = (c & 1) != 0 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            }
            return c;
        }).ToArray();

        private static uint UpdateCrc(uint crc, byte[] data)
        {
            foreach (byte b in data)
            {
                crc = CrcTable[(crc ^ b) & 0xFF] ^ (crc >> 8);
            }
            return crc;
        }

        private static void Unfilter(int filter, byte[] current, byte[] previous, int bpp)
        {
            for (int i = 0; i < current.Length; i++)
            {
                int left = i >= bpp ? current[i - bpp] : 0;
                int up = previous[i];
                int upLeft = i >= bpp ? previous[i - bpp] : 0;
                current[i] = filter switch
                {
                    1 => (byte)(current[i] + left),
                    2 => (byte)(current[i] + up),
                    3 => (byte)(current[i] + (left + up) / 2),
                    4 => (byte)(current[i] + Paeth(left, up, upLeft)),
                    _ => current[i]
                };
            }
        }

        private static int Paeth(int a, int b, int c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a), pb = Math.Abs(p - b), pc = Math.Abs(p - c);
            return pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
        }

        private static int ReadSample(byte[] row, int x, int bitDepth)
        {
            if (bitDepth == 8)
            {
                return row[x];
            }
            int bit = x * bitDepth;
            return (row[bit / 8] >> (8 - bitDepth - bit % 8)) & ((1 << bitDepth) - 1);
        }

        private static byte ScaleSample(int sample, int bitDepth)
        {
            return (byte)(sample * 255 / ((1 << bitDepth) - 1));
        }
    }
}
