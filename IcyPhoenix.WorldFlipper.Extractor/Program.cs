using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IcyPhoenix.WorldFlipper.Extractor
{
    class Program
    {
        private const string RootFolder = "..\\Local Store";
        private const string Boot_ffc6 = "..\\Scripts\\boot_ffc6.as";
        private const string Scripts = "..\\Scripts";
        private const string SupplmentFileList = "..\\filelist.txt";
        private const string RegexPathWithQuoteString = "\"[a-zA-Z_0-9\\/]+?\\/[a-zA-Z_0-9\\/]+?\"";
        private const string RegexPathString = "[a-zA-Z_0-9]+?\\/[a-zA-Z_0-9\\/]+";
        private const string FileNameSalt = "K6R9T9Hz22OpeIGEWB0ui6c6PYFQnJGy";
        private static readonly string[] DynamicCharacterPaths = new[]
        {
            @"character/{0}/pixelart/sprite_sheet",
            @"character/{0}/pixelart/special_sprite_sheet",
            @"character/{0}/pixelart/special",
            @"character/{0}/ui/skill_cutin_{1}",
            @"character/{0}/ui/cutin_skill_chain_{1}",
            @"character/{0}/pixelart/pixelart",
            @"character/{0}/ui/thumb_party_unison_{1}",
            @"character/{0}/ui/thumb_party_main_{1}",
            @"character/{0}/ui/battle_member_status_{1}",
            @"character/{0}/ui/thumb_level_up_{1}",
            @"character/{0}/ui/square_{1}",
            @"character/{0}/ui/square_132_132_{1}",
            @"character/{0}/ui/square_round_136_136_{1}",
            @"character/{0}/ui/square_round_95_95_{1}",
            @"character/{0}/ui/battle_member_status_{1}",
            @"character/{0}/ui/episode_banner_0",
            @"character/{0}/ui/battle_control_board_{1}",
            @"character/{0}/battle/character_detail_skill_preview",
            @"character/{0}/ui/illustration_setting_sprite_sheet",
            @"character/{0}/battle/character_info_skill_preview",
            @"character/{0}/ui/full_shot_1440_1920_{1}",
            @"character/{0}/ui/full_shot_illustration_setting_{1}"
        };
        private static readonly Regex potentialPathRegexWithQuotes = new(RegexPathWithQuoteString, RegexOptions.Compiled);
        private static readonly Regex potentialPathRegex = new(RegexPathString, RegexOptions.Compiled);
        private static readonly Regex CSVParser = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);
        private static readonly Dictionary<string, string> HashFileNames = new();

        static void Main(string[] args)
        {
            Extract();
            // ExportDropRateData();

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static void Extract()
        {
            var fileList = ParseFileList();
            AddScriptsFileListToHashFileNames();
            AddCharacterFileListToHashFileNames();
            var files = Directory.GetFiles(RootFolder, "*.", SearchOption.AllDirectories);
            var fileNames = files
                .GroupBy(x => $"{Path.GetFileName(Path.GetDirectoryName(x))}\\{Path.GetFileName(x)}")
                .ToDictionary(x => x.Key, x => x.OrderByDescending(x => CreateOrder(x)).ToList())
                .OrderByDescending(x => x.Value.Count)
                .ToDictionary(x => x.Key, x => x.Value);

            ExtractMasterTableFiles(fileList, files);
            AddSupplmentFileListToHashFileNames();
            AddSpriteSheetSheetToFileNames();

            File.WriteAllText(".\\HashFileNamesMapping.json", JsonConvert.SerializeObject(HashFileNames, Formatting.Indented));

            foreach (var hashFileName in HashFileNames)
            {
                if (!fileNames.TryGetValue(hashFileName.Key, out var filePaths))
                {
                    continue;
                }

                fileNames.Remove(hashFileName.Key);
                var filePath = filePaths.First();
                using var fileStream = new FileStream(filePath, FileMode.Open);
                using var reader = new BinaryReader(fileStream);
                using var memstream = new MemoryStream();
                var bytes = default(byte[]);
                reader.BaseStream.CopyTo(memstream);
                bytes = memstream.ToArray();

                if (bytes.Length >= 4 && bytes[1] == 112 && bytes[2] == 110 && bytes[3] == 103)
                {
                    ParsePng(bytes);
                    Directory.CreateDirectory(".\\images\\" + Path.GetDirectoryName(hashFileName.Value));
                    File.WriteAllBytes(".\\images\\" + hashFileName.Value, bytes);
                    Console.WriteLine($"Writing: {hashFileName.Value}");
                    continue;
                }
            }

            var j = 0;
            foreach (var file in fileNames)
            {
                j++;
                var filePath = file.Value.First();
                using var fileStream = new FileStream(filePath, FileMode.Open);
                using var reader = new BinaryReader(fileStream);
                using var memstream = new MemoryStream();
                var bytes = default(byte[]);
                reader.BaseStream.CopyTo(memstream);
                bytes = memstream.ToArray();

                if (bytes.Length >= 4 && bytes[1] == 112 && bytes[2] == 110 && bytes[3] == 103)
                {
                    ParsePng(bytes);
                    Directory.CreateDirectory(".\\images\\unknown\\");
                    var hashFileName = $"{Path.GetFileName(Path.GetDirectoryName(filePath))}{Path.GetFileName(filePath)}";
                    File.WriteAllBytes($".\\images\\unknown\\{hashFileName}.png", bytes);
                    Console.WriteLine($"[{j}/{fileNames.Count}] Writing: {hashFileName}.png");
                    continue;
                }
            }
        }

        private static string CreateOrder(string fileName)
        {
            var parts = fileName.Split('-')[1..^1].ToList();
            if (parts.Count == 2)
            {
                parts.Add(parts.Last());
                parts[1] = "0";
            }
            for (int i = 0; i < parts.Count; i++)
            {
                var part = parts[i];
                part = string.Join(string.Empty, part.Split('.').Select(x => x.PadLeft(3, '0')));
                parts[i] = part;
            }
            return string.Join(string.Empty, parts);
        }

        private static void ExtractMasterTableFiles(Dictionary<string, string> fileList, string[] files)
        {
            const int CharBatch = 100;
            int j = 0;
            Parallel.ForEach(Partitioner.Create(0, files.Length, CharBatch), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    j++;
                    var filePath = files[i];
                    var filename = $"{Path.GetFileName(Path.GetDirectoryName(filePath))}\\{Path.GetFileName(filePath)}";
                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    using var reader = new BinaryReader(fileStream);

                    if (!fileList.TryGetValue(filename, out var actualFileName))
                    {
                        actualFileName = $"master\\unknown\\{Path.GetFileName(Path.GetDirectoryName(filePath))}{Path.GetFileName(filePath)}.json";
                    }

                    try
                    {
                        var data = ParseMasterTable(reader);
                        Directory.CreateDirectory(".\\" + Path.GetDirectoryName(actualFileName));
                        File.WriteAllText(".\\" + actualFileName, JsonConvert.SerializeObject(data, Formatting.Indented), Encoding.UTF8);
                        Console.WriteLine($"[{j}/{files.Length}] Writing: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        if (ex is not ApplicationException && ex is not EndOfStreamException)
                        {
                            Console.WriteLine($"{filePath} failed to parse " + ex);
                        }
                    }
                }
            });

            var unknownFiles = Directory.GetFiles(".\\master\\unknown\\");
            Console.WriteLine($"Renaming Unknown Master Files - {unknownFiles.Length}");
            for (int i = 0; i < unknownFiles.Length; i++)
            {
                string filePath = unknownFiles[i];
                var fileContents = File.ReadAllLines(filePath);
                if (fileContents[1].Contains("\"story"))
                {
                    var match = potentialPathRegexWithQuotes.Match(fileContents[1]);
                    if (match.Success)
                    {
                        var newPath = ".\\master\\" + match.Value.Replace("\"", string.Empty) + ".json";
                        Directory.CreateDirectory(".\\" + Path.GetDirectoryName(newPath));
                        File.Move(filePath, newPath, true);
                        Console.WriteLine($"[{i}/{unknownFiles.Length}] Renaming: {newPath}");
                    }
                }
            }
        }

        private static void AddSpriteSheetSheetToFileNames()
        {
            const string spriteSheet = "sprite_sheet";
            foreach (var hashFileName in HashFileNames.Values.ToArray())
            {
                if (hashFileName.Length <= spriteSheet.Length) continue;

                var index = hashFileName.IndexOf(spriteSheet);
                if (index > 0)
                {
                    var newPath = hashFileName.Substring(0, index + spriteSheet.Length) + ".png";
                    HashFileNames[SHA1Hash(newPath)] = newPath;
                }
            }
        }

        private static void AddCharacterFileListToHashFileNames()
        {
            foreach (var line in File.ReadAllLines(SupplmentFileList))
            {
                if (line.EndsWith(".png"))
                {
                    HashFileNames[SHA1Hash(line)] = line;
                }
            }
        }

        private static void AddScriptsFileListToHashFileNames()
        {
            var files = Directory.GetFiles(Scripts, "*.as", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                if (Path.GetFileName(file) != "boot_ffc6.as")
                {
                    continue;
                }
                var fileData = File.ReadAllText(file);
                var matches = potentialPathRegexWithQuotes.Matches(fileData);
                foreach (var match in matches.Cast<Match>().Select(x => x.Value.Replace("\"", string.Empty)).Distinct())
                {
                    HashFileNames[SHA1Hash(match)] = $"{match}.png";
                }
            }
        }

        private static void AddSupplmentFileListToHashFileNames()
        {
            var characterList = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(@".\master\character\character.json"))
                .Values
                .Select(x => x[0])
                .ToList();

            var trimmedImage = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(@".\master\generated\trimmed_image.json"))
                .Keys
                .Select(x => x.Split('/')[1])
                .Distinct()
                .ToList();

            characterList.AddRange(trimmedImage);

            foreach (var character in characterList)
            {
                foreach (var dynamicCharacterPath in DynamicCharacterPaths)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var filePath = string.Format(dynamicCharacterPath, character, i.ToString()) + ".png";
                        HashFileNames[SHA1Hash(filePath)] = filePath;
                    }
                }
            }
        }

        private static object ParseMasterTable(BinaryReader reader)
        {
            var outputData = new Dictionary<string, object>();
            var header_size = reader.ReadInt32();
            if (header_size > reader.BaseStream.Length || header_size <= 0)
            {
                throw new ApplicationException();
            }
            var header_data_compressed = reader.ReadBytes(header_size);
            var header_data = Decompress(header_data_compressed);
            var header_data_offsets = new List<Tuple<int, int>>();

            using (var headerMemoryStream = new MemoryStream(header_data))
            using (var headerbinaryReader = new BinaryReader(headerMemoryStream))
            {
                var nb_entries = headerbinaryReader.ReadInt32();
                if (nb_entries * 8 > headerbinaryReader.BaseStream.Length)
                {
                    throw new Exception();
                }

                for (var i = 0; i < nb_entries; i++)
                {
                    var header_end_offset = headerbinaryReader.ReadInt32();
                    var data_end_offset = headerbinaryReader.ReadInt32();
                    header_data_offsets.Add(new Tuple<int, int>(header_end_offset, data_end_offset));
                }

                var totalHeaderRead = 0;
                var totalDataRead = 0;
                foreach (var header_data_offset in header_data_offsets)
                {
                    var header = headerbinaryReader.ReadBytes(header_data_offset.Item1 - totalHeaderRead);
                    var data = reader.ReadBytes(header_data_offset.Item2 - totalDataRead);

                    totalHeaderRead = header_data_offset.Item1;
                    totalDataRead = header_data_offset.Item2;
                    while (data.Length > 2 && data[0] == 120 && data[1] == 218)
                    {
                        data = Decompress(data);
                    }

                    var headerString = Encoding.UTF8.GetString(header);
                    FindHashFileNames(headerString);

                    try
                    {
                        using var memoryStream = new MemoryStream(data);
                        using var innerReader = new BinaryReader(memoryStream);
                        var innerObj = ParseMasterTable(innerReader);
                        outputData.Add(headerString, innerObj);
                    }
                    catch
                    {
                        var dataString = Encoding.UTF8.GetString(data);
                        FindHashFileNames(dataString);
                        using var stringReader = new StringReader(dataString);
                        using var csvReader = new CsvReader(stringReader, CultureInfo.InvariantCulture);

                        if (csvReader.Read())
                        {
                            outputData[headerString] = csvReader.Context.Parser.Record;
                        }
                    }
                }
            }
            return outputData;
        }

        private static void FindHashFileNames(string dataString)
        {
            foreach (var dataRow in dataString.Split(','))
            {
                var matches = potentialPathRegex.Matches(dataRow.Replace("\"", string.Empty).Replace(",", string.Empty));
                foreach (var match in matches.Cast<Match>()
                    .Select(x => x.Value.Replace("\"", string.Empty))
                    .Distinct()
                    .ToDictionary(x => SHA1Hash(x + ".png"), x => x + ".png"))
                {
                    HashFileNames[match.Key] = match.Value;
                }
            }
        }

        private static byte[] Decompress(byte[] data)
        {
            using var inputStream = new MemoryStream(data.Skip(2).ToArray());
            using var decompress = new DeflateStream(inputStream, CompressionMode.Decompress);
            using var resultStream = new MemoryStream();
            decompress.CopyTo(resultStream);
            return resultStream.ToArray();
        }

        private static Dictionary<string, string> ParseFileList()
        {
            var boot_ffc6 = File.ReadAllText(Boot_ffc6);
            var matches = potentialPathRegexWithQuotes.Matches(boot_ffc6);
            return matches.Cast<Match>()
                .Select(x => x.Value.Replace("\"", string.Empty))
                .Distinct()
                .ToDictionary(x => SHA1Hash("master" + x + ".orderedmap"), x => "master" + x + ".json");
        }

        private static void ParsePng(byte[] bytes)
        {
            bytes[1] = 80;
            bytes[2] = 78;
            bytes[3] = 71;
        }

        public static string SHA1Hash(string input)
        {
            using SHA1Managed sha1 = new SHA1Managed();
            var saltedInput = input + FileNameSalt;
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(saltedInput));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            var ret = sb.ToString();
            return ret.Substring(0, 2) + "\\" + ret.Substring(2);
        }
    }
}