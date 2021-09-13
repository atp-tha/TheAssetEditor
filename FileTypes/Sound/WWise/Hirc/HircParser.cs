﻿using Filetypes.ByteParsing;
using System;
using System.Linq;
using System.Text;

namespace FileTypes.Sound.WWise.Hirc
{
    public abstract class HricItem
    {
        public string DisplayName { get; set; }
        public string OwnerFile { get; set; }
        public int IndexInFile { get; set; }

        public HircType Type { get; set; }
        public uint Size { get; set; }
        public uint Id { get; set; }


        protected void LoadCommon(ByteChunk chunk)
        {
            Type = (HircType)chunk.ReadByte();
            Size = chunk.ReadUInt32();
            Id = chunk.ReadUInt32();
        }

        protected void SkipToEnd(ByteChunk chunk, int startIndex)
        {
            chunk.Index = (int)(startIndex + Size);
        }
    }

    public class HircParser : IParser
    {
        public void Parse(string fileName, ByteChunk chunk, SoundDataBase soundDb)
        {
            var chunkSize = chunk.ReadUInt32();
            var numItems = chunk.ReadUInt32();

            for (int i = 0; i < numItems; i++)
            {
                var hircType = (HircType)chunk.PeakByte();
                switch (hircType)
                {
                    case HircType.Sound:
                        soundDb.Hircs.Add(CAkSound.Create(chunk));
                        break;
                    case HircType.Event:
                        soundDb.Hircs.Add(CAkEvent.Create(chunk));
                        break;
                    case HircType.Action:
                        soundDb.Hircs.Add(CAkAction.Create(chunk));
                        break;
                    case HircType.SwitchContainer:
                        soundDb.Hircs.Add(CAkSwitchCntr.Create(chunk));
                        break;
                    case HircType.SequenceContainer:
                        soundDb.Hircs.Add(CAkRanSeqCnt.Create(chunk));
                        break;
                    case HircType.LayerContainer:
                        soundDb.Hircs.Add(CAkLayerCntr.Create(chunk));
                        break;
                    default:
                        soundDb.Hircs.Add(CAkUnknown.Create(chunk));
                        break;
                }

                soundDb.Hircs.Last().IndexInFile = i;
                soundDb.Hircs.Last().OwnerFile = fileName;
            }
        }
    }
}
