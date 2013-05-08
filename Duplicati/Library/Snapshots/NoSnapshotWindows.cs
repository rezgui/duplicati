//  Copyright (C) 2011, Kenneth Skovhede

//  http://www.hexad.dk, opensource@hexad.dk
//
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
using System;
using System.Text;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;

namespace Duplicati.Library.Snapshots
{
    /// <summary>
    /// Handler for providing a snapshot like access to files and folders
    /// </summary>
    public class NoSnapshotWindows : NoSnapshot
    {
        private SystemIOWindows m_sysIO = new SystemIOWindows();

        public NoSnapshotWindows(string[] sourcefolders)
            : base(sourcefolders)
        {
        }

        public NoSnapshotWindows(string[] sourcefolders, Dictionary<string, string> options)
            : base(sourcefolders, options)
        {
        }

        /// <summary>
        /// Returns the symlink target if the entry is a symlink, and null otherwise
        /// </summary>
        /// <param name="file">The file or folder to examine</param>
        /// <returns>The symlink target</returns>
        public override string GetSymlinkTarget(string file)
        {
            return File.GetLinkTargetInfo(SystemIOWindows.PrefixWithUNC(file)).PrintName;
        }

        /// <summary>
        /// Gets the attributes for the given file or folder
        /// </summary>
        /// <returns>The file attributes</returns>
        /// <param name="file">The file or folder to examine</param>
        public override System.IO.FileAttributes GetAttributes (string file)
        {
            return m_sysIO.GetFileAttributes(file);
        }

        /// <summary>
        /// Returns the size of a file
        /// </summary>
        /// <param name="file">The full path to the file in non-snapshot format</param>
        /// <returns>The lenth of the file</returns>
        public override long GetFileSize (string file)
        {
            return m_sysIO.FileLength(file);
        }

        /// <summary>
        /// Gets the last write time of a given file
        /// </summary>
        /// <param name="file">The full path to the file in non-snapshot format</param>
        /// <returns>The last write time of the file</returns>
        public override DateTime GetLastWriteTime (string file)
        {
            if (!SystemIOWindows.IsPathTooLong(file))
                try { return base.GetLastWriteTime(file); }
                catch (System.IO.PathTooLongException) { }

            return File.GetLastWriteTime(SystemIOWindows.PrefixWithUNC(file));
        }

        /// <summary>
        /// Opens a file for reading
        /// </summary>
        /// <param name="file">The full path to the file in non-snapshot format</param>
        /// <returns>An open filestream that can be read</returns>
        public override System.IO.Stream OpenRead (string file)
        {
            if (!SystemIOWindows.IsPathTooLong(file))
                try { return base.OpenRead(file); }
                catch (System.IO.PathTooLongException) { }

            return File.OpenRead(SystemIOWindows.PrefixWithUNC(file));
        }

        /// <summary>
        /// Lists all files in the given folder
        /// </summary>
        /// <returns>All folders found in the folder</returns>
        /// <param name='folder'>The folder to examinate</param>
        protected override string[] ListFiles (string folder)
        {
            if (!SystemIOWindows.IsPathTooLong(folder))
                try { return base.ListFiles(folder); }
                catch (System.IO.PathTooLongException) { }

            string[] tmp = Directory.GetFiles(SystemIOWindows.PrefixWithUNC(folder));
            string[] res = new string[tmp.Length];
            for(int i = 0; i < tmp.Length; i++)
                res[i] = SystemIOWindows.StripUNCPrefix(tmp[i]);

            return res;
        }

        /// <summary>
        /// Lists all folders in the given folder
        /// </summary>
        /// <returns>All folders found in the folder</returns>
        /// <param name='folder'>The folder to examinate</param>
        protected override string[] ListFolders (string folder)
        {
            if (!SystemIOWindows.IsPathTooLong(folder))
                try { return base.ListFolders(folder); }
                catch (System.IO.PathTooLongException) { }

            string[] tmp = Directory.GetDirectories(SystemIOWindows.PrefixWithUNC(folder));
            string[] res = new string[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
                res[i] = SystemIOWindows.StripUNCPrefix(tmp[i]);

            return res;
        }
    }
}

