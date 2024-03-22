using GBFRDataTools.Entities;
using FlatSharp;
using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using Vortice.Direct3D11;

namespace Relink_Mod_Manager
{
    public class DataManager
    {
        // Base functionality of handling data.i archives from https://github.com/WistfulHopes/gbfrelink.utility.manager

        private IndexFile _index;
        private string _gameDirectory;

        public const string INDEX_ORIGINAL_CODENAME = "relink";
        public const string INDEX_MODDED_CODENAME = "relink-mod-manager";

        public DataManager(string gameDirectory)
        {
            _gameDirectory = gameDirectory;
        }

        public bool IsIndexOriginal(string indexFilePath)
        {
            byte[] gameIndexFile = File.ReadAllBytes(indexFilePath);
            IndexFile gameIndex;

            gameIndex = IndexFile.Serializer.Parse(gameIndexFile);

            if (gameIndex.Codename != INDEX_ORIGINAL_CODENAME)
            {
                // index file is not original, and we don't have a backup of the original
                // was modified by another modding tool though
                return false;
            }
            else if (BinaryPrimitives.ReadInt32LittleEndian(gameIndexFile.AsSpan(4)) != 0)
            {
                // index file is not original, though we don't know what changed it
                return false;
            }

            return true;
        }

        public bool BackupDataIndex(string ModArchivesDirectory, string GameDirectory)
        {
            Directory.CreateDirectory(ModArchivesDirectory);

            string GameDataIndexFile = Path.Combine(GameDirectory, "data.i");
            string BackupDataIndexFile = Path.Combine(ModArchivesDirectory, "orig_data.i");

            if (!File.Exists(GameDataIndexFile))
            {
                // data.i is missing from game directory somehow, restore from backup automatically if possible
                if (File.Exists(BackupDataIndexFile))
                {
                    File.Copy(BackupDataIndexFile, GameDataIndexFile, true);
                }
                else
                {
                    return false;
                }
            }

            byte[] gameIndexFile = File.ReadAllBytes(GameDataIndexFile);
            IndexFile gameIndex;

            gameIndex = IndexFile.Serializer.Parse(gameIndexFile);

            if (!File.Exists(BackupDataIndexFile))
            {
                if (gameIndex.Codename != INDEX_ORIGINAL_CODENAME)
                {
                    // Check if the current game data.i has an original codename value

                    // TODO: issue - index file is not original anymore
                    return false;
                }
                else if (BinaryPrimitives.ReadInt32LittleEndian(gameIndexFile.AsSpan(4)) != 0)
                {
                    // Check if the current game data.i has potentially been modified already

                    // TODO: issue - index file is not original anymore
                    return false;
                }
            }
            else
            {
                if (gameIndex.Codename != INDEX_ORIGINAL_CODENAME || BinaryPrimitives.ReadInt32LittleEndian(gameIndexFile.AsSpan(4)) != 0)
                {
                    // TODO: data.i is already modified, we already have a backed up file, don't update it with this one
                    return true;
                }
            }

            // Game has a different data.i than we have in storage, and it's an original file, update our stored one

            // Perform a comparison check to see if stored file is actually different than game file to save on a file copy
            if (File.Exists(BackupDataIndexFile))
            {
                byte[] backupIndexFile = File.ReadAllBytes(BackupDataIndexFile);
                var gameHash = Crc32.Hash(gameIndexFile);
                var backupHash = Crc32.Hash(backupIndexFile);
                if (!gameHash.SequenceEqual(backupHash))
                {
                    File.Copy(GameDataIndexFile, BackupDataIndexFile, true);
                }
            }
            else
            {
                File.Copy(GameDataIndexFile, BackupDataIndexFile, true);
            }

            return true;
        }

        bool LoadDataIndexForEditing()
        {
            try
            {
                var OriginalIndexFile = File.ReadAllBytes(Path.Combine(_gameDirectory, "data.i"));
                _index = IndexFile.Serializer.Parse(OriginalIndexFile);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data index for editing: {ex.Message}");
                return false;
            }
        }

        public bool UpdateDataIndexFromList(List<string> FilesToAddList)
        {
            if (!LoadDataIndexForEditing())
            {
                return false;
            }

            foreach (var item in FilesToAddList)
            {
                // Trims full path down to inside GBFR/data and changes backslash to forwardslash
                string str = item.Remove(0, 5).Replace('\\', '/');
                byte[] hashBytes = XxHash64.Hash(Encoding.ASCII.GetBytes(str), 0);
                ulong hash = BinaryPrimitives.ReadUInt64BigEndian(hashBytes);

                long fileSize = new FileInfo(Path.Combine(_gameDirectory, item)).Length;
                if (AddOrUpdateExternalFile(hash, (ulong)fileSize))
                {
                    Console.WriteLine($"Added {str} as new external file");
                }
                else
                {
                    Console.WriteLine($"Updated {str} external file");
                }
                RemoveArchiveFile(hash);
            }

            return WriteUpdatedDataIndexFile();
        }

        public void ProcessFile(string FilePath)
        {
            switch (Path.GetExtension(FilePath))
            {
                case ".minfo":
                    {
                        UpgradeMInfoData(FilePath);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        void UpgradeMInfoData(string FilePath)
        {
            try
            {
                var minfoBytes = File.ReadAllBytes(FilePath);
                ModelInfo modelInfo = ModelInfo.Serializer.Parse(minfoBytes);

                if (modelInfo.Magic < 20240213)
                {
                    modelInfo.Magic = 10000_01_01;

                    int size = ModelInfo.Serializer.GetMaxSize(modelInfo);
                    byte[] buf = new byte[size];
                    ModelInfo.Serializer.Write(buf, modelInfo);
                    File.WriteAllBytes(FilePath, buf);
                }
            }
            catch (Exception ex)
            {
                // Error encountered, leaving file as-is
                Console.WriteLine($"Error trying to upgrade MInfo data: {ex.Message}");
            }
        }

        bool AddOrUpdateExternalFile(ulong hash, ulong fileSize)
        {
            bool added = false;

            int idx = _index.ExternalFileHashes.BinarySearch(hash);
            if (idx < 0)
            {
                idx = _index.ExternalFileHashes.AddSorted(hash);
                added = true;

                _index.ExternalFileSizes.Insert(idx, fileSize);
            }
            else
            {
                _index.ExternalFileSizes[idx] = fileSize;
            }

            return added;
        }

        void RemoveArchiveFile(ulong hash)
        {
            int idx = _index.ArchiveFileHashes.BinarySearch(hash);
            if (idx > -1)
            {
                _index.ArchiveFileHashes.RemoveAt(idx);
                _index.FileToChunkIndexers.RemoveAt(idx);
            }
        }

        bool WriteUpdatedDataIndexFile()
        {
            try
            {
                byte[] outBuffer = new byte[IndexFile.Serializer.GetMaxSize(_index)];
                // Track if this is original or modded index file by setting our own codename
                _index.Codename = INDEX_MODDED_CODENAME;
                IndexFile.Serializer.Write(outBuffer, _index);
                File.WriteAllBytes(Path.Combine(_gameDirectory, "data.i"), outBuffer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing updates to data index file: {ex.Message}");
                return false;
            }
            
        }
    }
}
