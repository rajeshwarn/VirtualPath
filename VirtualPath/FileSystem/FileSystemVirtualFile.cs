﻿using System;
using System.IO;
using VirtualPath;
using VirtualPath.Common;

namespace VirtualPath.FileSystem
{
    public class FileSystemVirtualFile : AbstractVirtualFileBase
    {
        protected FileInfo BackingFile;
        
        public override string Name
        {
            get { return BackingFile.Name; }
        }

        public override string RealPath
        {
            get { return BackingFile.FullName; }
        }

        public override DateTime LastModified
        {
            get { return BackingFile.LastWriteTime; }
        }

        public FileSystemVirtualFile(IVirtualPathProvider owningProvider, IVirtualDirectory directory, FileInfo fInfo) 
            : base(owningProvider, directory)
        {
            this.BackingFile = fInfo;
        }

        public override Stream OpenRead()
        {
            return BackingFile.OpenRead();
        }

        public override void Delete()
        {
            BackingFile.Delete();
        }

        public override Stream OpenWrite(WriteMode mode)
        {
            switch (mode)
            {
                case WriteMode.Overwrite: return BackingFile.OpenWrite();
                case WriteMode.Append: return BackingFile.Open(FileMode.Append, FileAccess.Write);
                case WriteMode.Truncate: return BackingFile.Open(FileMode.Truncate, FileAccess.Write);
                default: throw new NotImplementedException();
            }
        }

        protected override IVirtualFile CopyBackingFileToDirectory(IVirtualDirectory directory, string name)
        {
            if (directory is FileSystemVirtualDirectory)
            {
                var copyFInfo = BackingFile.CopyTo(Path.Combine(directory.RealPath, name), true);
                return new FileSystemVirtualFile(VirtualPathProvider, directory, copyFInfo);
            }
            else
            {
                return directory.CopyFile(this, name);
            }
        }

        protected override IVirtualFile MoveBackingFileToDirectory(IVirtualDirectory directory, string name)
        {
            if (directory is FileSystemVirtualDirectory)
            {
                BackingFile.MoveTo(Path.Combine(directory.RealPath, name));
                return new FileSystemVirtualFile(VirtualPathProvider, directory, BackingFile);
            }
            else
            {
                var newFile = directory.CopyFile(this, name);
                this.Delete();
                return newFile;
            }
        }
    }
}
