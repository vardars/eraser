/* 
 * $Id$
 * Copyright 2009 The Eraser Project
 * Original Author: Joel Low <lowjoel@users.sourceforge.net>
 * Modified By: 
 * 
 * This file is part of Eraser.
 * 
 * Eraser is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option) any later
 * version.
 * 
 * Eraser is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR
 * A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * A copy of the GNU General Public License can be found at
 * <http://www.gnu.org/licenses/>.
 */

#pragma once

#include <vector>
#include "Fat.h"

using namespace System;

namespace Eraser {
namespace Util {
	ref class FatDirectoryBase;

	/// Represents an abstract API to interface with FAT file systems.
	public ref class FatApi abstract
	{
	public:
		/// Constructor.
		/// 
		/// \param[in] info   The volume to create the FAT API for. The volume handle
		///                   created has read access only.
		FatApi(VolumeInfo^ info);

		/// Constructor.
		/// 
		/// \param[in] info   The volume to create the FAT API for.
		/// \param[in] stream The stream to use to read/write to the disk.
		FatApi(VolumeInfo^ info, IO::Stream^ stream);

	public:
		/// Loads the File Allocation Table from disk.
		virtual void LoadFat() = 0;
		
		/// Helper function to loads the directory structure representing the
		/// directory with the given volume-relative path.
		///
		/// \overload LoadDirectory
		FatDirectoryBase^ LoadDirectory(String^ directory);

		/// Loads the directory structure at the given cluster.
		virtual FatDirectoryBase^ LoadDirectory(unsigned cluster, String^ name,
			FatDirectoryBase^ parent) = 0;

	internal:
		/// Converts a sector-based address to a byte offset relative to the start
		/// of the volume.
		virtual unsigned long long SectorToOffset(unsigned long long sector);

		/// Converts a cluster-based address to a byte offset relative to the start
		/// of the volume.
		virtual long long ClusterToOffset(unsigned cluster) = 0;

		/// Converts a sector-based file size fo the actual size of the file in bytes.
		unsigned SectorSizeToSize(unsigned size);

		/// Converts a cluster-based file size fo the actual size of the file in bytes.
		unsigned ClusterSizeToSize(unsigned size);

		/// Verifies that the given cluster is allocated and in use.
		/// 
		/// \param[in] cluster The cluster to verify.
		virtual bool IsClusterAllocated(unsigned cluster) = 0;

		/// Gets the next cluster in the file.
		///
		/// \param[in] cluster The current cluster to check.
		/// \return            0xFFFFFFFF if the cluster given is the last one,
		///                    otherwise the next cluster in the file.
		virtual unsigned GetNextCluster(unsigned cluster) = 0;

		/// Gets the size of the file in bytes starting at the given cluster.
		/// Make sure that the given cluster is the first one, there is no way
		/// to verify it is indeed the first one and if later clusters are given
		/// the calculated size will be wrong.
		virtual unsigned FileSize(unsigned cluster) = 0;

		/// Gets the contents of the file starting at the given cluster.
		std::vector<char> GetFileContents(unsigned cluster);

		/// Set the contents of the file starting at the given cluster. The length
		/// of the contents must exactly match the length of the file.
		/// 
		/// \param[in] buffer  The data to write.
		/// \param[in] length  The amount of data to write.
		/// \param[in] cluster The cluster to begin writing to.
		void SetFileContents(const void* buffer, size_t length, unsigned cluster);

		/// \see SetFileContents
		void SetFileContents(const std::vector<char>& contents, unsigned cluster);

		/// Resolves a directory to the position on-disk
		///
		/// \param[in] path A volume-relative path to the directory.
		virtual unsigned DirectoryToCluster(String^ path) = 0;

	protected:
		IO::Stream^ VolumeStream;

		unsigned SectorSize;                 // Size of one sector, in bytes
		unsigned ClusterSize;                // Size of one cluster, in bytes
		FatBootSector* BootSector;
		char* Fat;
	};

	/// Represents the types of FAT directory entries.
	public enum class FatDirectoryEntryTypes
	{
		File,
		Directory
	};

	/// Represents a FAT directory entry.
	public ref class FatDirectoryEntry
	{
	public:
		/// Gets the name of the file or directory.
		property String^ Name
		{
			String^ get() { return name; }
		private:
			void set(String^ value) { name = value; }
		}

		/// Gets the full path to the file or directory.
		property String^ FullName
		{
			String^ get();
		}

		/// Gets the parent directory of this entry.
		property FatDirectoryBase^ Parent
		{
			FatDirectoryBase^ get() { return parent; }
		private:
			void set(FatDirectoryBase^ value) { parent = value; }
		}

		/// Gets the type of this entry.
		property FatDirectoryEntryTypes Type
		{
			FatDirectoryEntryTypes get() { return type; }
		private:
			void set(FatDirectoryEntryTypes value) { type = value; }
		}

		/// Gets the first cluster of this entry.
		property unsigned Cluster
		{
			unsigned get() { return cluster; }
		private:
			void set(unsigned value) { cluster = value; }
		}

	internal:
		/// Constructor.
		/// 
		/// \param[in] name    The name of the entry.
		/// \param[in] parent  The parent directory containing this file.
		/// \param[in] type    The type of this entry.
		/// \param[in] cluster The first cluster of the file.
		FatDirectoryEntry(String^ name, FatDirectoryBase^ parent, FatDirectoryEntryTypes type,
			unsigned cluster);

	private:
		String^ name;
		FatDirectoryBase^ parent;
		FatDirectoryEntryTypes type;
		unsigned cluster;
	};

	/// Represents an abstract FAT directory (can also represent the root directory of
	/// FAT12 and FAT16 volumes.)
	public ref class FatDirectoryBase abstract : FatDirectoryEntry
	{
	public:
		/// Constructor.
		/// 
		/// \param[in] name    The name of the current directory.
		/// \param[in] parent  The parent directory containing this directory.
		/// \param[in] cluster The cluster at which the directory list starts.
		FatDirectoryBase(String^ name, FatDirectoryBase^ parent, unsigned cluster);

		/// Compacts the directory structure, updating the structure on-disk as well.
		void ClearDeletedEntries();

		/// The list of files and subfolders in this directory.
		property Collections::Generic::Dictionary<String^, FatDirectoryEntry^>^ Items
		{
			Collections::Generic::Dictionary<String^, FatDirectoryEntry^>^ get()
			{
				return Entries;
			}
		}

	protected:
		/// Reads the directory structures from disk.
		/// 
		/// \remarks This function must set the \see Directory instance as well as the
		///          \see DirectorySize fields. Furthermore, call the \see ParseDirectory
		///          function to initialise the directory entries on-disk.
		virtual void ReadDirectory() = 0;

		/// Writes the directory to disk.
		virtual void WriteDirectory() = 0;

		/// This function reads the raw directory structures in \see Directory and
		/// sets the \see Entries field for easier access to the directory entries.
		void ParseDirectory();

		/// Gets the start cluster from the given directory entry.
		virtual unsigned GetStartCluster(::FatDirectoryEntry& directory) = 0;

	protected:
		/// A pointer to the directory structure.
		::FatDirectory Directory;

		/// The number of entries in the directory
		size_t DirectorySize;

	private:
		/// The list of parsed entries in the folder.
		Collections::Generic::Dictionary<String^, FatDirectoryEntry^>^ Entries;
	};

	/// Represents a FAT directory file.
	public ref class FatDirectory abstract : FatDirectoryBase
	{
	public:
		/// Constructor.
		/// 
		/// \param[in] name    The name of the current directory.
		/// \param[in] parent  The parent directory containing this directory.
		/// \param[in] cluster The cluster at which the directory list starts.
		/// \param[in] api     The FAT API object which is creating this object.
		FatDirectory(String^ name, FatDirectoryBase^ parent, unsigned cluster, FatApi^ api);

	protected:
		virtual void ReadDirectory() override;
		virtual void WriteDirectory() override;

	private:
		FatApi^ Api;
	};

	public ref class Fat12Or16Api abstract : FatApi
	{
	public:
		Fat12Or16Api(VolumeInfo^ info);
		Fat12Or16Api(VolumeInfo^ info, IO::Stream^ stream);

		virtual void LoadFat() override;
		virtual FatDirectoryBase^ LoadDirectory(unsigned cluster, String^ name,
			FatDirectoryBase^ parent) override;

	internal:
		virtual long long ClusterToOffset(unsigned cluster) override;
		virtual unsigned DirectoryToCluster(String^ path) override;

	protected:
		ref class Directory : FatDirectory
		{
		public:
			Directory(String^ name, FatDirectoryBase^ parent, unsigned cluster, Fat12Or16Api^ api);

		protected:
			virtual unsigned GetStartCluster(::FatDirectoryEntry& directory) override;
		};

	protected:
		bool IsFat12();
	};

	public ref class Fat16Api : Fat12Or16Api
	{
	public:
		Fat16Api(VolumeInfo^ info);
		Fat16Api(VolumeInfo^ info, IO::Stream^ stream);

	internal:
		virtual bool IsClusterAllocated(unsigned cluster) override;
		virtual unsigned GetNextCluster(unsigned cluster) override;
		virtual unsigned FileSize(unsigned cluster) override;
	};

	public ref class Fat32Api : FatApi
	{
	public:
		Fat32Api(VolumeInfo^ info);
		Fat32Api(VolumeInfo^ info, IO::Stream^ stream);

	public:
		virtual void LoadFat() override;
		virtual FatDirectoryBase^ LoadDirectory(unsigned cluster, String^ name,
			FatDirectoryBase^ parent) override;

	internal:
		virtual long long ClusterToOffset(unsigned cluster) override;
		virtual bool IsClusterAllocated(unsigned cluster) override;
		virtual unsigned GetNextCluster(unsigned cluster) override;
		virtual unsigned FileSize(unsigned cluster) override;
		virtual unsigned DirectoryToCluster(String^ path) override;

	private:
		ref class Directory : FatDirectory
		{
		public:
			Directory(String^ name, FatDirectoryBase^ parent, unsigned cluster, Fat32Api^ api);

		protected:
			virtual unsigned GetStartCluster(::FatDirectoryEntry& directory) override;
		};
	};
}
}
