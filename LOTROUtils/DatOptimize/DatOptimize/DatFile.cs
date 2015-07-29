using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace DatOptimize
{

    class DatFile2 : IDisposable
    {
        public DatFile2(string path)
        {
            stream = File.OpenRead(path);
            stream.Seek(256, SeekOrigin.Begin);
            header = new byte[168];
            stream.Read(header, 0, 168);
            fileName = path;
            reader = new BinaryReader(stream);
        }

        public bool FragmentsFound { get; private set; }

        BinaryReader reader;
        FileStream stream;
        string fileName;
        byte[] header;
        // 00504c0000 (5 byte initial code...)
        //?File system table record - points to a jump table and the storage size is what is important, not the size.
        //?Last table(current working table?) offset 0x25, seems to be a jump location, but then it isn't...
        //header tag offset 0x40, 42540000 
        //Block size, offset 0x44, length 4
        //File size, offset 0x48, length 4
        //DatFileId offset 0x4C, length 8, encoded as high low part 32bit ints?
        //StartOfFreeSpaceChain offset 0x54, length 4
        //EndOfFreeSpaceChain offset 0x58, length 4 
        //LengthOfFreeSpaceChain offset 0x5C, length 4
        //First jump table offset 0x60, length 4? 
        //DataIdMasterMap offset 0x70, length 4
        //VersionNumDatFile 0x74, length 4
        //VersionNumGameData  0x78, length 4
        //DatIdStamp offset 0x7C, length 18, some odd guid followed by a short (possibly length 20 with guid followed by int.
        //? offset 0x90 (always 0x60)
        //? offset 0x94 (always 0x20)
        //? offset 0x98 (always 0x08)
        //? offset 0x9C (Total free space (minus 8 * free space chain length))
        //? offset 0xA0 (first free space chain entry size)
        //? offset 0xA4 (I think this is the directory entry size.) 
        //(last 3 seems to be something like sizes...)

#if DEBUG
        SortedDictionary2<uint, Loc> starts;
#endif

        private class Loc
        {
            public Loc(uint start, uint end, uint type)
            {
                Start = start;
                End = end;
                Type = type;
            }
            public uint Start;
            public uint End;
            public uint Type;
        }

        public void CheckJumpTables()
        {
            CheckJumpTables(true);
        }
        public void CheckJumpTables(bool failIfUnmapped)
        {
#if DEBUG
            uint version = BitConverter.ToUInt32(header, 0x74);
            starts = new SortedDictionary2<uint, Loc>();
            UpdateMap(0, 256, 0);
            UpdateMap(256, 168, 1);
            UpdateMap(256 + 168, 1024-256-168, 0);
            if (version < 0x70)
            {
                PopFreeSpace(BitConverter.ToUInt32(header, 0x54), BitConverter.ToUInt32(header, 0xA0));
            }
            else
            {
                PopFreeSpaceNew(BitConverter.ToUInt32(header, 0x54), BitConverter.ToUInt32(header, 0xA0), BitConverter.ToUInt32(header, 0x5c));
            }
#endif
            //            firstInvalid = BitConverter.ToUInt32(header, 0x54);
            uint firstTable = BitConverter.ToUInt32(header, 0x60);
            //uint lastTable = BitConverter.ToUInt32(header, 13);
            uint length = BitConverter.ToUInt32(header, 0xA4);
            RecurseCheck(firstTable, 0x60, length);
            //RecurseCheck(lastTable, 13, false);
#if DEBUG
            // Apparently the 'first' entry in the free list isn't counted... so we have to add it on.
            List<uint> gapStart = new List<uint>();
            List<uint> gapLength = new List<uint>();
            uint foundGaps = 0;
            uint prev = uint.MaxValue;
            foreach (KeyValuePair<uint, Loc> kvp in starts)
            {
                uint cur = kvp.Key;
                // Rely on overflow... for first spot.
                if (prev + 1 < cur)
                {
                    gapStart.Add(prev + 1);
                    gapLength.Add(cur - prev - 1);
                    foundGaps += cur - prev - 1;
                }
                prev = kvp.Value.End;
            }
            uint curEnd = BitConverter.ToUInt32(header, 0x48);
            if (prev + 1 < curEnd)
            {
                gapStart.Add(prev + 1);
                gapLength.Add(curEnd - prev - 1);
                foundGaps += curEnd - prev - 1;
            }

            if (failIfUnmapped && foundGaps != 0)
                throw new Exception("Can't account for all the file!?!");
#endif
        }

        private void PopFreeSpaceNew(uint loc, uint length, uint count)
        {
            UpdateMap(loc, length, 2);
            stream.Position = loc;
            int counter = 0;
            while (stream.Position < loc + length && counter < count)
            {
                uint nextLength = reader.ReadUInt32();
                uint nextLoc = reader.ReadUInt32();
                if (nextLength != 0)
                {
                    UpdateMap(nextLoc, nextLength, 2);
                    counter++;
                }
            }
        }

        private void PopFreeSpace(uint loc, uint length)
        {
            UpdateMap(loc, length, 2);
            stream.Position = loc;
            uint nextLength = reader.ReadUInt32();
            uint nextLoc = reader.ReadUInt32();
            uint realLoc = nextLoc - 1;
            if (nextLength != 0)
                PopFreeSpace(realLoc, nextLength);
        }

        private void UpdateMap(uint loc, uint length, uint type)
        {
#if DEBUG
            Loc hit;
            if (starts.TryGetValue(loc, out hit))
            {
                throw new Exception(string.Format("Invalid collision: {0}, {1}, {2} hit {3}, {4}, {5}", loc, length, type, hit.Start, hit.End - hit.Start + 1, hit.Type));
            }
            IEnumerator<KeyValuePair<uint, Loc>> it = starts.GetEnumeratorForKeyOrPrev(loc);
            if (it.Current.Value != null)
            {
                if (loc <= it.Current.Value.End)
                {
                    hit = it.Current.Value;
                    throw new Exception(string.Format("Invalid overlap: {0}, {1}, {2} hit {3}, {4}, {5}", loc, length, type, hit.Start, hit.End - hit.Start + 1, hit.Type));
                }
            }
            uint freeSpaceEnd = loc + length - 1;
            if (it.MoveNext())
            {
                if (freeSpaceEnd >= it.Current.Value.Start)
                {
                    hit = it.Current.Value;
                    throw new Exception(string.Format("Invalid overlap: {0}, {1}, {2} hit {3}, {4}, {5}", loc, length, type, hit.Start, hit.End - hit.Start + 1, hit.Type));
                }
            }
            starts.Add(loc, new Loc(loc, loc + length - 1, type));
#endif
        }
        //      private uint firstInvalid;

        public uint Size
        {
            get
            {
                return BitConverter.ToUInt32(header, 0x48);
            }
        }

        public uint DirectoryEntrySize
        {
            get
            {
                // Remove 8 to get the actual length of the directory entry data not the full thing when non-fragmented.
                return BitConverter.ToUInt32(header, 0xA4) - 8;
            }
        }

        private enum DirectoryEntryStatus
        {
            OK,
            AlreadySeen,
            Bad,
        }

        private DirectoryEntryStatus RecurseCheck(uint offset, uint parent, uint length)
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Found = offset;
            de.Parent = parent;
            DirectoryEntry pde;
            if (directories.TryGetValue(parent, out pde))
            {
                de.Name = pde.Name + pde.Children.IndexOf(offset).ToString("X2");
            }
            else
            {
                de.Name = "00";
            }
            if (!directories.ContainsKey(de.Found))
                directories.Add(de.Found, de);
            else
            {
                File.AppendAllText("logcheck.txt", "Directory Dupe found." + Environment.NewLine);
                return DirectoryEntryStatus.AlreadySeen;
            }
            uint nextRead = offset;
            uint nextLength = length;
            List<byte[]> frags = new List<byte[]>();
            int count = 0;
            uint totalLength = DirectoryEntrySize;
            uint totalSoFar = 0;
            UpdateMap(offset, length, 3);
            do
            {
                count++;
                if (nextLength < 8)
                {
                    directories.Remove(de.Found);
                    File.AppendAllText("logcheck.txt", "Less than minimum length reference found... Fragment: " + count.ToString() + Environment.NewLine);
                    return DirectoryEntryStatus.Bad;
                }
                if (nextRead >= stream.Length)
                {
                    directories.Remove(de.Found);
                    File.AppendAllText("logcheck.txt", "Directory entry fragment beyond end of file. Fragment: " + count.ToString() + Environment.NewLine);
                    return DirectoryEntryStatus.Bad;
                }
                if (nextRead + nextLength > stream.Length)
                {
                    directories.Remove(de.Found);
                    File.AppendAllText("logcheck.txt", "Directory entry fragment crosses end of file. Fragment: " + count.ToString() + Environment.NewLine);
                    return DirectoryEntryStatus.Bad;
                }
                stream.Seek(nextRead, SeekOrigin.Begin);
                uint fragLength = reader.ReadUInt32();
                uint fragSource = reader.ReadUInt32();
                if (fragLength != 0)
                    UpdateMap(fragSource, fragLength, 19);
                if ((fragSource & 1) != 0)
                {
                    directories.Remove(de.Found);
                    File.AppendAllText("logcheck.txt", "Reference in to free space chain found." + Environment.NewLine);
                    return DirectoryEntryStatus.Bad;
                }
                uint nextFragLength = nextLength - 8;
                if (fragLength == 0 && count > 1)
                {
                    nextFragLength = Math.Min(nextFragLength, totalLength - totalSoFar);
                }
                byte[] frag = new byte[nextFragLength];
                stream.Read(frag, 0, (int)(nextFragLength));
                frags.Add(frag);
                totalSoFar += nextFragLength;
                if (fragLength == 0)
                {
                    break;
                }
                else
                {
                    fragSpots[fragSource] = string.Format("Fragment for directory {0} num {1}", de.Name, count);
                }
                nextRead = fragSource;
                nextLength = fragLength;
            } while (true);
            frags.Reverse();
            byte[] entryData = Concat(frags);
            de.Length = (uint)entryData.Length + 8;
            if (de.Length > length)
            {
                de.Fragmented = true;
                de.FragLength = length;
            }
            using (MemoryStream memStream = new MemoryStream(entryData))
            using (BinaryReader memReader = new BinaryReader(memStream))
            {
                List<uint> childLengths = new List<uint>();
                bool leaf = true;
                for (int i = 0; i < 62; i++)
                {
                    uint childLength = memReader.ReadUInt32();
                    uint destination = memReader.ReadUInt32();
                    if (destination != 0 || childLength != 0)
                        leaf = false;
                    de.Children.Add(destination);
                    childLengths.Add(childLength);
                }
                int fileCount = memReader.ReadInt32();
                if (fileCount < 0 || fileCount > 62)
                {
                    directories.Remove(de.Found);
                    File.AppendAllText("logcheck.txt", "Invalid file count found" + Environment.NewLine);
                    return DirectoryEntryStatus.Bad;
                }
                if (leaf)
                {
                    de.Children.Clear();
                    childLengths.Clear();
                }
                else
                {
                    // If we aren't a leaf there is one file between every directory entry.  This allows log(n) search of the b-tree.
                    if (fileCount < 61)
                    {
                        de.Children.RemoveRange(fileCount + 1, 62 - fileCount - 1);
                        childLengths.RemoveRange(fileCount + 1, 62 - fileCount - 1);
                    }
                }


                for (int i = 0; i < fileCount; i++)
                {
                    int compressed = memReader.ReadInt16();
                    if (compressed < 0 || compressed > 3)
                    {
                        File.AppendAllText("logcheck.txt", "Unexpected Compress mode." + Environment.NewLine);
                    }
                    FileEntry entry = new FileEntry();
                    entry.CompressionMode = (short)compressed;
                    entry.Found = stream.Position;
                    entry.Version = memReader.ReadInt16();
                    entry.Id = memReader.ReadInt32();
                    entry.Parent = offset;
                    entry.FileLoc = memReader.ReadUInt32();
                    de.Files.Add(entry);
                    ids.Add(entry.Id);
                    if (!files.ContainsKey(entry.Id))
                    {
                        files.Add(entry.Id, entry);
                    }
                    else
                    {
                        File.AppendAllText("logcheck.txt", "Dupe found." + Environment.NewLine);
                    }
                    if (!fileSpots.ContainsKey(entry.FileLoc))
                    {
                        fileSpots.Add(entry.FileLoc, entry);
                    }
                    else
                    {
                        File.AppendAllText("logcheck.txt", "Dupe fileLoc found." + Environment.NewLine);
                    }
                    entry.Length = memReader.ReadUInt32();
                    entry.BitField = memReader.ReadInt32();
                    entry.Iteration = memReader.ReadInt32();
                    entry.StorageLength = memReader.ReadUInt32();
                    entry.Reserved = memReader.ReadInt32();
                    if (entry.Reserved != 0)
                    {
                        File.AppendAllText("logcheck.txt", string.Format("File record padding non-empty. {0}", offset) + Environment.NewLine);
                    }
                    CheckFileStart(entry);
                }

                List<int> toRemove = new List<int>();
                for (int i = 0; i < de.Children.Count; i++)
                {
                    uint destination = de.Children[i];
                    DirectoryEntryStatus result = RecurseCheck(destination, offset, childLengths[i]);
                    // Hopefully this doesn't happen, but hey, we'll try and cope...
                    if (result == DirectoryEntryStatus.Bad)
                        toRemove.Add(i);
                }
                for (int i = toRemove.Count - 1; i >= 0; i--)
                {
                    de.Children.RemoveAt(toRemove[i]);
                }
            }

            return DirectoryEntryStatus.OK;
        }

        private static byte[] Concat(List<byte[]> frags)
        {
            if (frags.Count == 1)
                return frags[0];
            int length = 0;
            foreach (byte[] frag in frags)
            {
                length += frag.Length;
            }
            byte[] result = new byte[length];
            int offset = 0;
            foreach (byte[] frag in frags)
            {
                Array.Copy(frag, 0, result, offset, frag.Length);
                offset += frag.Length;
            }
            return result;
        }

        private void CheckFileStart(FileEntry entry)
        {
#if DEBUG
            long savePos = stream.Position;
            UpdateMap(entry.FileLoc, entry.StorageLength, 4);
            // Compression mode bit 2 files fragment differently.
            if ((entry.CompressionMode & 2) != 0)
            {
                stream.Seek(entry.FileLoc, SeekOrigin.Begin);
                uint fragCount = reader.ReadUInt32();
                if (fragCount > 0)
                {
                    stream.Seek(entry.FileLoc + entry.StorageLength - 8 * fragCount, SeekOrigin.Begin);
                    for (int i = 0; i < fragCount; i++)
                    {
                        uint fragLength = reader.ReadUInt32();
                        uint fragSource = reader.ReadUInt32();
                        UpdateMap(fragSource, fragLength, 20);
                    }
                }
            }
            else
            {
                uint nextRead = entry.FileLoc;
                do
                {
                    stream.Seek(nextRead, SeekOrigin.Begin);
                    uint fragLength = reader.ReadUInt32();
                    uint fragSource = reader.ReadUInt32();
                    if (fragLength == 0)
                        break;
                    // TODO: wth?
                    if (fragSource == 0)
                        break;
                    UpdateMap(fragSource, fragLength, 20);
                    nextRead = fragSource;
                } while (true);
            }

            stream.Seek(entry.FileLoc, SeekOrigin.Begin);
            long expectBlank = reader.ReadInt64();
            stream.Seek(savePos, SeekOrigin.Begin);
            if (expectBlank != 0)
            {
                FragmentsFound = true;
            }
#endif
        }



        List<int> ids = new List<int>();
        Dictionary<int, FileEntry> files = new Dictionary<int, FileEntry>();
        Dictionary<uint, FileEntry> fileSpots = new Dictionary<uint, FileEntry>();
        Dictionary<long, DirectoryEntry> directories = new Dictionary<long, DirectoryEntry>();
        Dictionary<uint, string> fragSpots = new Dictionary<uint, string>();

        private class FileEntry
        {
            public long Found;
            public int Version;
            public int Id;
            public uint FileLoc;
            public uint Length;
            public int BitField;
            public int Iteration;
            public uint StorageLength;
            public int Reserved;
            public uint Parent;
            public short CompressionMode;
        }

        private class DirectoryEntry
        {
            public long Found;
            public List<uint> Children = new List<uint>();
            public List<FileEntry> Files = new List<FileEntry>();
            public uint Parent;
            public string Name;
            public uint Length;
            public bool Fragmented;
            public uint FragLength;
        }

        public class DatFileRewriter : IDisposable
        {
            public DatFileRewriter(string output, DatFile2 input)
            {
                stream = File.Create(output);
                writer = new BinaryWriter(stream);
                parent = input;
                try
                {
                    CopyHeader();
                }
                catch
                {
                    stream.Close();
                    throw;
                }
                version = BitConverter.ToUInt32(parent.header, 0x74);
                blockSize = BitConverter.ToUInt32(parent.header, 0x44);
            }

            private void CopyHeader()
            {
                stream.Seek(256, SeekOrigin.Begin);
                stream.Write(parent.header, 0, parent.header.Length);
                // Give the header some buffer.
                stream.Seek(1024, SeekOrigin.Begin);
            }
            BinaryWriter writer;
            FileStream stream;
            DatFile2 parent;
            Dictionary<uint, uint> done = new Dictionary<uint, uint>();
            List<uint> writtenDirectories = new List<uint>();
            uint version;
            uint blockSize;

            public bool ForceById(int id)
            {
                FileEntry fe;
                if (parent.files.TryGetValue(id, out fe))
                {
                    if (done.ContainsKey(fe.FileLoc))
                        return true;
                    uint dirSpot = fe.Parent;
                    List<uint> directories = new List<uint>();
                    while (!done.ContainsKey(dirSpot))
                    {
                        directories.Add(dirSpot);
                        DirectoryEntry parentEntry = parent.directories[(uint)dirSpot];
                        dirSpot = parentEntry.Parent;
                        if (dirSpot == 0x60)
                            break;
                    }
                    for (int i = directories.Count - 1; i >= 0; i--)
                    {
                        Force(directories[i]);
                    }
                    done.Add(fe.FileLoc, (uint)stream.Position);
                    WriteFile(fe);
                    return true;
                }
                return false;
            }

            public void Force(uint location)
            {
                if (location < 424)
                    return;
                if (done.ContainsKey(location))
                    return;
                FileEntry fe;
                if (parent.fileSpots.TryGetValue((uint)location, out fe))
                {
                    done.Add(location, (uint)stream.Position);
                    WriteFile(fe);
                    return;
                }
                DirectoryEntry de;
                if (parent.directories.TryGetValue(location, out de))
                {
                    done.Add(location, (uint)stream.Position);
                    WriteDirectory(de);
                    return;
                }
            }

            private void WriteDirectory(DirectoryEntry de)
            {
                uint startPos = (uint)stream.Position;
                writtenDirectories.Add((uint)de.Found);

                stream.Position = stream.Position + 8;
                foreach (uint child in de.Children)
                {
                    DirectoryEntry childDE = parent.directories[child];
                    writer.Write(childDE.Length);
                    writer.Write(child);
                }
                stream.Position = stream.Position + 8 * (62 - de.Children.Count);

                writer.Write((int)de.Files.Count);
                foreach (FileEntry file in de.Files)
                {
                    writer.Write((short)(file.CompressionMode));
                    writer.Write((short)file.Version);
                    writer.Write(file.Id);
                    writer.Write(file.FileLoc);
                    writer.Write(file.Length);
                    writer.Write(file.BitField);
                    writer.Write(file.Iteration);
                    // Since we are defragmenting, if the storage length is currently insufficient, we need to increase it to what it will be after defragmenting.
                    writer.Write(Math.Max((file.Length + 11) / 4 * 4, file.StorageLength));
                    writer.Write(file.Reserved);
                }

                stream.Position = startPos + de.Length;
            }

            private void WriteFile(FileEntry fe)
            {
                long savePos = parent.stream.Position;

                List<byte[]> frags = new List<byte[]>();
                if ((fe.CompressionMode & 2) != 0)
                {
                    parent.stream.Seek(fe.FileLoc, SeekOrigin.Begin);
                    uint fragCount = parent.reader.ReadUInt32();
                    byte[] frag = new byte[fe.StorageLength - 8 - 8 * fragCount];
                    uint total=(uint)frag.Length;
                    // Blank;
                    parent.reader.ReadUInt32();
                    parent.stream.Read(frag, 0, frag.Length);
                    frags.Add(frag);
                    if (fragCount > 0)
                    {
                        uint[] lengths = new uint[fragCount];
                        uint[] sources = new uint[fragCount];
                        for (int i = 0; i < fragCount; i++)
                        {
                            lengths[i] = parent.reader.ReadUInt32();
                            sources[i] = parent.reader.ReadUInt32();
                        }
                        for (int i = 0; i < fragCount; i++)
                        {
                            parent.stream.Seek(sources[i], SeekOrigin.Begin);
                            uint length = lengths[i];
                            if (i == fragCount - 1)
                                length = fe.Length - total;
                            byte[] frag2 = new byte[length];
                            parent.stream.Read(frag2, 0, frag2.Length);
                            frags.Add(frag2);
                            total += (uint)frag2.Length;
                        }
                    }
                    //frags.Reverse();
                }
                else
                {
                    uint nextRead = fe.FileLoc;
                    uint nextLength = fe.StorageLength;
                    int count = 0;
                    uint total = 0;
                    do
                    {
                        count++;
                        parent.stream.Seek(nextRead, SeekOrigin.Begin);
                        uint fragLength = parent.reader.ReadUInt32();
                        uint fragSource = parent.reader.ReadUInt32();
                        // If last fragment then length is left overs, but if it is also the first fragment, it is the entire storage length.
                        uint dataLength = (fragLength == 0 && total != 0) ? (fe.Length - total) : (nextLength - 8);
                        byte[] frag = new byte[dataLength];
                        parent.stream.Read(frag, 0, (int)dataLength);
                        total += dataLength;
                        frags.Add(frag);
                        if (fragLength == 0)
                            break;
                        nextRead = fragSource;
                        nextLength = fragLength;
                    } while (true);
                    frags.Reverse();
                }
                byte[] entryData = DatFile2.Concat(frags);
                uint check = Math.Max((fe.Length + 11) / 4 * 4, fe.StorageLength);
                if (check-8 != (entryData.Length+3)/4*4)
                    throw new Exception("File length missmatch.");
                long start = stream.Position;
                stream.Position = stream.Position + 8;
                stream.Write(entryData, 0, entryData.Length);
                // Align our position since it may not be at a 4byte boundry.
                stream.Position = (stream.Position + 3) / 4 * 4;

                parent.stream.Position = savePos;
            }

            public void Finish()
            {
                Queue<uint> todoDirs = new Queue<uint>();
                List<uint> todoFiles = new List<uint>();
                todoDirs.Enqueue(BitConverter.ToUInt32(parent.header, 0x60));
                while (todoDirs.Count > 0)
                {
                    uint cur = todoDirs.Dequeue();
                    Force(cur);
                    DirectoryEntry de = parent.directories[cur];
                    foreach (uint child in de.Children)
                        todoDirs.Enqueue(child);
                    foreach (FileEntry fe in de.Files)
                        todoFiles.Add(fe.FileLoc);
                }
                foreach (uint fileLoc in todoFiles)
                {
                    Force(fileLoc);
                }

                if (version < 0x70)
                {
                    long endOfStorage = stream.Position;
                    writer.Write((uint)0);
                    writer.Write((uint)1);
                    long fileLength = stream.Position;
                    long meg = 1024 * 1024;
                    // Round file length to megabyte, seems that is the allocation size used.
                    fileLength = (fileLength + (meg - 1)) / meg * meg;
                    // if we were close to a meg boundry, add an extra meg just in case.
                    if (endOfStorage + 1024 > fileLength)
                        fileLength += meg;
                    stream.SetLength(fileLength);

                    // write fileLength
                    stream.Position = 256 + 0x48;
                    writer.Write((uint)fileLength);
                    // write free space chain to header.
                    stream.Position = 256 + 0x54;
                    writer.Write((uint)endOfStorage);
                    stream.Position = 256 + 0x58;
                    writer.Write((uint)endOfStorage);
                    stream.Position = 256 + 0x5c;
                    writer.Write((uint)1);
                    //? offset 0x9C (Total free space (minus 8 * free space chain length))
                    stream.Position = 256 + 0x9c;
                    writer.Write((uint)(fileLength - endOfStorage - 8));
                    //? offset 0xA0 (first free space chain entry size)
                    stream.Position = 256 + 0xA0;
                    writer.Write((uint)(fileLength - endOfStorage));
                }
                else
                {
                    long endOfStorage = stream.Position;
                    writer.Write((uint)0);
                    writer.Write((uint)0);

                    // Make the free space table at least 12k, larger if block size is larger...
                    uint freeSpaceTableLength = Math.Max(12 * 1024, blockSize);

                    long fileLength = endOfStorage + freeSpaceTableLength;
                    long meg = 1024 * 1024;
                    // Round file length to megabyte, seems that is the allocation size used.
                    fileLength = (fileLength + (meg - 1)) / meg * meg;
                    // if we were close to a meg boundry, add an extra meg just in case.
                    if (endOfStorage + freeSpaceTableLength + 1024 > fileLength)
                        fileLength += meg;
                    stream.SetLength(fileLength);
                    stream.Position = endOfStorage + 8;
                    writer.Write((uint)(fileLength-endOfStorage-freeSpaceTableLength));
                    writer.Write((uint)(endOfStorage + freeSpaceTableLength));

                    // write fileLength
                    stream.Position = 256 + 0x48;
                    writer.Write((uint)fileLength);
                    // write free space table to header.
                    stream.Position = 256 + 0x54;
                    writer.Write((uint)endOfStorage);
                    // free space table is a table, so no 'end' required. TODO: make sure this doesn't break anything.
                    stream.Position = 256 + 0x58;
                    writer.Write((uint)0);
                    // 1 free space block in the free space table.
                    stream.Position = 256 + 0x5c;
                    writer.Write((uint)1);
                    //? offset 0x9C (Total free space (minus 8 * free space chain length))  Free space table doesn't count as free space.
                    stream.Position = 256 + 0x9c;
                    writer.Write((uint)(fileLength - endOfStorage - 8 - freeSpaceTableLength));
                    //? offset 0xA0 (free space table size)
                    stream.Position = 256 + 0xA0;
                    writer.Write((uint)(freeSpaceTableLength));
                }
                Remap();
                finished = true;
            }

            private void Remap()
            {
                MapHeaderBit(0x0D);
                MapHeaderBit(0x25);
                MapHeaderBit(0x39);
                MapHeaderBit(0x60);
                foreach (uint loc in writtenDirectories)
                {
                    stream.Position = done[loc] + 8;
                    DirectoryEntry de = parent.directories[loc];
                    foreach (uint child in de.Children)
                    {
                        stream.Position = stream.Position + 4;

                        uint mapped;
                        if (done.TryGetValue(child, out mapped))
                            writer.Write(mapped);
                        else
                        {
                            File.AppendAllText("loggenerate.txt", string.Format("Directory Entry reference fails to map. {0}", stream.Position) + Environment.NewLine);
                            stream.Position = stream.Position + 4;
                        }
                    }
                    stream.Position = done[loc] + 508; // 8 + 62*8 + 4
                    foreach (FileEntry fe in de.Files)
                    {
                        stream.Position = stream.Position + 8;
                        uint mapped;
                        if (done.TryGetValue(fe.FileLoc, out mapped))
                            writer.Write(mapped);
                        else
                        {
                            File.AppendAllText("loggenerate.txt", string.Format("File Entry reference fails to map. {0}", stream.Position) + Environment.NewLine);
                            stream.Position = stream.Position + 4;
                        }
                        stream.Position = stream.Position + 20;
                    }
                }
            }

            private void MapHeaderBit(int loc)
            {
                stream.Position = 256 + loc;
                uint potential = BitConverter.ToUInt32(parent.header, loc);
                if (potential == 0)
                    return;
                uint mapped;
                if (done.TryGetValue(potential, out mapped))
                    writer.Write(mapped);
                else
                {
                    File.AppendAllText("loggenerate.txt", string.Format("Header bit fails to map. {0}", loc) + Environment.NewLine);
                }
            }
            private bool finished = false;

            public void Cancel()
            {
                string name = Filename;
                stream.Close();
                if (!string.IsNullOrEmpty(name))
                {
                    File.Delete(name);
                }
                finished = true;
            }

            #region IDisposable Members

            public void Dispose()
            {

                if (!finished)
                {
                    try
                    {
                        Finish();
                    }
                    catch
                    {
                        Cancel();
                    }
                }
                stream.Close();
            }

            #endregion

            public string Filename
            {
                get
                {
                    try
                    {
                        return stream == null ? string.Empty : stream.Name;
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }

            internal void CancelIfNotFinished()
            {
                if (finished)
                    stream.Close();
                else
                {
                    Cancel();
                }
            }
        }

        internal DatFileRewriter GetRewriter(string path)
        {
            return new DatFileRewriter(path, this);
        }

        internal bool TryMapToId(long offset, out int id)
        {
            id = 0;
            FileEntry fe;
            if (fileSpots.TryGetValue((uint)offset, out fe))
            {
                id = fe.Id;
                return true;
            }
            return false;
        }

        internal string Map(long offset, long length)
        {
            if (offset < 424)
                return "Header";
            FileEntry fe;
            if (fileSpots.TryGetValue((uint)offset, out fe))
            {
                return string.Format("Reading File {0}", fe.Id);
            }
            DirectoryEntry de;
            if (directories.TryGetValue(offset, out de))
            {
                return string.Format("Reading directory table {0}", de.Name);
            }
            string fragDescription;
            if (fragSpots.TryGetValue((uint)offset, out fragDescription))
            {
                return string.Format("Reading {0}", fragDescription);
            }
            return "Unexpected read offset.";
        }

        #region IDisposable Members

        public void Dispose()
        {
            stream.Close();
        }

        #endregion

        internal byte[] CreateMap()
        {
            byte[] map = new byte[Size / 4];
            for (int i = 0; i < 128; i++ )
                map[i] = 1;
            foreach (DirectoryEntry de in directories.Values)
            {

                if (!de.Fragmented)
                {
                    int start = (int)(de.Found / 4);
                    int end = start + (int)(de.Length / 4);
                    for (int i = start; i < end; i++)
                    {
                        if (map[i] != 0)
                            throw new InvalidOperationException();
                        map[i] = 2;
                    }
                }
                else
                {
                    uint nextRead = (uint)de.Found;
                    uint nextLength = de.FragLength;
                    do
                    {
                        stream.Seek(nextRead, SeekOrigin.Begin);
                        int start2 = (int)(stream.Position / 4);
                        uint fragLength = reader.ReadUInt32();
                        uint fragSource = reader.ReadUInt32();
                        // If last fragment then length is left overs, but if it is also the first fragment, it is the entire storage length.
                        uint dataLength = (nextLength - 8);
                        int end2 = start2 + (int)(dataLength + 8 + 3) / 4;
                        for (int i = start2; i < end2; i++)
                        {
                            if (map[i] != 0)
                                throw new InvalidOperationException();
                            map[i] = 3;
                        }
                        if (fragLength == 0)
                            break;
                        nextRead = fragSource;
                        nextLength = fragLength;
                    } while (true);
                }
            }
            foreach (FileEntry fe in files.Values)
            {
                int start = (int)(fe.FileLoc / 4);
                int end = start + (int)((fe.StorageLength+3)/4);
                // TODO: new fragment format.
                if (fe.CompressionMode > 2)
                    throw new NotImplementedException();
                if (fe.StorageLength - 8 < fe.Length)
                {
                    uint nextRead = fe.FileLoc;
                    uint nextLength = fe.StorageLength;
                    uint total = 0;
                    do
                    {
                        stream.Seek(nextRead, SeekOrigin.Begin);
                        int start2 = (int)(stream.Position / 4);
                        uint fragLength = reader.ReadUInt32();
                        uint fragSource = reader.ReadUInt32();
                        // If last fragment then length is left overs, but if it is also the first fragment, it is the entire storage length.
                        uint dataLength = (fragLength == 0 && total != 0) ? (fe.Length - total) : (nextLength - 8);
                        int end2 = start2 + (int)(dataLength + 8 + 3) / 4;
                        for (int i = start2; i < end2; i++)
                        {
                            if (map[i] != 0)
                                throw new InvalidOperationException();
                            map[i] = 6;
                        }
                        total += dataLength;
                        if (fragLength == 0)
                            break;
                        nextRead = fragSource;
                        nextLength = fragLength;
                    } while (true);
                }
                else
                {
                    for (int i = start; i < end; i++)
                    {
                        if (map[i] != 0)
                            throw new InvalidOperationException();
                        map[i] = 4;
                    }
                }
            }
            uint next = BitConverter.ToUInt32(header, 0x54);
            uint length = BitConverter.ToUInt32(header, 0xa0);
            
            while (next != 0)
            {
                stream.Position = next;
                uint nextLength = reader.ReadUInt32();
                next = reader.ReadUInt32() & (~1u);
                int start = (int)((stream.Position-8) / 4);
                int end = start + (int)(length / 4);
                for (int i = start; i < end; i++)
                {
                    if (map[i] != 0)
                        throw new InvalidOperationException();
                    map[i] = 7;
                }
                length = nextLength;
            }
            return map;
        }
#if DEBUG
        internal void CompareDetails(DatFile2 file2)
        {
            if (this.files.Count != file2.files.Count)
                throw new Exception("Failed comparison, different file counts.");
            foreach (var entry in this.files)
            {
                FileEntry otherFE;
                if (!file2.files.TryGetValue(entry.Key, out otherFE))
                {
                    throw new Exception("Failed comparison, different file list.");
                }
                else
                {
                    FileEntry fe = entry.Value;
                    if (fe.Iteration != otherFE.Iteration)
                        throw new Exception("Failed comparison, different file iteration.");
                    if (fe.CompressionMode != otherFE.CompressionMode)
                        throw new Exception("Failed comparison, different compression status.");
                    //if (fe.BitField != otherFE.BitField)
                    //    throw new Exception("Failed comparison, different bit field status.");
                    if (fe.Length != otherFE.Length)
                        throw new Exception("Failed comparison, different length.");
                    if (fe.Reserved != otherFE.Reserved)
                        throw new Exception("Failed comparison, different reserved dword.");
                    if (fe.Version != otherFE.Version)
                        throw new Exception("Failed comparison, different version.");
                }
            }
        }
#endif

        internal void DebugFileContent()
        {
            string fileName = stream.Name;
            stream.Close();
            stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite);
            reader = new BinaryReader(stream);

            foreach (FileEntry fe in this.files.Values)
            {
                // for each file replace file content with deadbeef...
                uint nextRead = fe.FileLoc;
                uint nextLength = fe.StorageLength;
                // TODO:
                if (fe.CompressionMode > 2)
                    throw new NotImplementedException();
                List<byte[]> frags = new List<byte[]>();
                int count = 0;
                uint total = 0;
                do
                {
                    count++;
                    if (nextRead + 8 >= stream.Length)
                        break;
                    stream.Seek(nextRead, SeekOrigin.Begin);
                    uint fragLength = reader.ReadUInt32();
                    uint fragSource = reader.ReadUInt32();
                    uint dataLength =  (nextLength - 8);
                    dataLength = Math.Min(dataLength, (uint)(stream.Length - stream.Position));
                    byte[] frag = new byte[dataLength];
                    for (int i = 0; i < frag.Length; i++)
                    {
                        switch (i % 4)
                        {
                            case 0:
                                frag[i] = 0xde;
                                break;
                            case 1:
                                frag[i] = 0xad;
                                break;
                            case 2:
                                frag[i] = 0xbe;
                                break;
                            case 3:
                                frag[i] = 0xef;
                                break;
                        }
                    }
                    stream.Write(frag, 0, (int)dataLength);
                    total += dataLength;
                    if (fragLength == 0)
                        break;
                    nextRead = fragSource;
                    nextLength = fragLength;
                } while (true);


            }
        }

        internal IEnumerable<int> GetFileIdEnumeration()
        {
            return files.Keys.Where(a => a >= 0 || a < -65535);
        }

        internal byte[] GetDataById(int id)
        {
            FileEntry fe = files[id];
            List<byte[]> frags = new List<byte[]>();
            if ((fe.CompressionMode & 2) != 0)
            {
                stream.Seek(fe.FileLoc, SeekOrigin.Begin);
                uint fragCount = reader.ReadUInt32();
                uint toRead = fe.StorageLength - 8 - 8 * fragCount;
                if (fragCount == 0)
                    toRead = fe.Length;
                byte[] frag = new byte[toRead];
                uint total = (uint)frag.Length;
                // Blank;
                reader.ReadUInt32();
                stream.Read(frag, 0, frag.Length);
                frags.Add(frag);
                if (fragCount > 0)
                {
                    uint[] lengths = new uint[fragCount];
                    uint[] sources = new uint[fragCount];
                    for (int i = 0; i < fragCount; i++)
                    {
                        lengths[i] = reader.ReadUInt32();
                        sources[i] = reader.ReadUInt32();
                    }
                    for (int i = 0; i < fragCount; i++)
                    {
                        stream.Seek(sources[i], SeekOrigin.Begin);
                        uint length = lengths[i];
                        if (i == fragCount - 1)
                            length = fe.Length - total;
                        byte[] frag2 = new byte[length];
                        stream.Read(frag2, 0, frag2.Length);
                        frags.Add(frag2);
                        total += (uint)frag2.Length;
                    }
                }
            }
            else
            {
                uint nextRead = fe.FileLoc;
                uint nextLength = fe.StorageLength;
                uint total = 0;
                do
                {
                    stream.Seek(nextRead, SeekOrigin.Begin);
                    uint fragLength = reader.ReadUInt32();
                    uint fragSource = reader.ReadUInt32();
                    uint dataLength = (fragLength == 0) ? (fe.Length - total) : (nextLength - 8);
                    byte[] frag = new byte[dataLength];
                    stream.Read(frag, 0, (int)dataLength);
                    frags.Add(frag);
                    total += dataLength;
                    if (fragLength == 0)
                        break;
                    nextRead = fragSource;
                    nextLength = fragLength;
                } while (true);
                frags.Reverse();
            }
            return Concat(frags);
        }
    }
}
