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

#include <stdafx.h>
#include <windows.h>
#include <atlstr.h>

#include "FatApi.h"

using namespace System::IO;
using namespace System::Runtime::InteropServices;

namespace Eraser {
namespace Util {
	Fat32Api::Fat32Api(VolumeInfo^ info) : FatApi(info)
	{
		//Sanity checks: check that this volume is FAT32!
		if (info->VolumeFormat != L"FAT32")
			throw gcnew ArgumentException(L"The volume provided is not a FAT32 volume.");
	}

	Fat32Api::Fat32Api(VolumeInfo^ info, Microsoft::Win32::SafeHandles::SafeFileHandle^ handle,
		IO::FileAccess access) : FatApi(info, handle, access)
	{
		//Sanity checks: check that this volume is FAT32!
		if (info->VolumeFormat != L"FAT32")
			throw gcnew ArgumentException(L"The volume provided is not a FAT32 volume.");
	}

	void Fat32Api::LoadFat()
	{
		Fat = new char[SectorSizeToSize(BootSector->Fat32ParameterBlock.SectorsPerFat)];

		//Seek to the FAT
		VolumeStream->Seek(SectorSizeToSize(BootSector->ReservedSectorCount), SeekOrigin::Begin);

		//Read the FAT
		array<Byte>^ buffer = gcnew array<Byte>(SectorSizeToSize(BootSector->Fat32ParameterBlock.SectorsPerFat));
		VolumeStream->Read(buffer, 0, SectorSizeToSize(BootSector->Fat32ParameterBlock.SectorsPerFat));
		Marshal::Copy(buffer, 0, static_cast<IntPtr>(Fat), buffer->Length);
	}

	FatDirectory^ Fat32Api::LoadDirectory(unsigned cluster, String^ name)
	{
		return gcnew Directory(name, cluster, this);
	}

	long long Fat32Api::ClusterToOffset(unsigned cluster)
	{
		unsigned long long sector = BootSector->ReservedSectorCount +				//Reserved area
			BootSector->FatCount * BootSector->Fat32ParameterBlock.SectorsPerFat +	//FAT area
			(static_cast<unsigned long long>(cluster) - 2) * (ClusterSize / SectorSize);
		return SectorToOffset(sector);
	}

	bool Fat32Api::IsClusterAllocated(unsigned cluster)
	{
		unsigned* fatPtr = reinterpret_cast<unsigned*>(Fat);
		if (
			fatPtr[cluster] <= 0x00000001 ||
			(fatPtr[cluster] >= 0x0FFFFFF0 && fatPtr[cluster] <= 0x0FFFFFF6) ||
			fatPtr[cluster] == 0x0FFFFFF7
		)
			return false;

		return true;
	}

	unsigned Fat32Api::GetNextCluster(unsigned cluster)
	{
		unsigned* fatPtr = reinterpret_cast<unsigned*>(Fat);
		if (fatPtr[cluster] <= 0x00000001 || (fatPtr[cluster] >= 0x0FFFFFF0 && fatPtr[cluster] <= 0x0FFFFFF6))
			throw gcnew ArgumentException(L"Invalid FAT cluster: cluster is marked free.");
		else if (fatPtr[cluster] == 0x0FFFFFF7)
			throw gcnew ArgumentException(L"Invalid FAT cluster: cluster is marked bad.");
		else if (fatPtr[cluster] >= 0x0FFFFFF8)
			return 0xFFFFFFFF;
		else
			return fatPtr[cluster];
	}

	unsigned Fat32Api::FileSize(unsigned cluster)
	{
		unsigned* fatPtr = reinterpret_cast<unsigned*>(Fat);
		for (unsigned result = 1; ; ++result)
		{
			if (fatPtr[cluster] <= 0x00000001 || (fatPtr[cluster] >= 0x0FFFFFF0 && fatPtr[cluster] <= 0x0FFFFFF6))
				throw gcnew ArgumentException(L"Invalid FAT cluster: cluster is marked free.");
			else if (fatPtr[cluster] == 0x0FFFFFF7)
				throw gcnew ArgumentException(L"Invalid FAT cluster: cluster is marked bad.");
			else if (fatPtr[cluster] >= 0x0FFFFFF8)
				return result * ClusterSize;
			else
				cluster = fatPtr[cluster];
		}
	}

	unsigned Fat32Api::DirectoryToCluster(String^ path)
	{
		//The path must start with a backslash as it must be volume-relative.
		if (path[0] != L'\\')
			throw gcnew ArgumentException(L"The path provided is not volume relative. " +
				gcnew String(L"Volume relative paths must begin with a backslash."));

		//Chop the path into it's constituent directory components
		array<String^>^ components = path->Substring(1)->Split(Path::DirectorySeparatorChar,
			Path::AltDirectorySeparatorChar);

		//Traverse the directories until we get the cluster we want.
		unsigned cluster = BootSector->Fat32ParameterBlock.RootDirectoryCluster;
		String^ parentDir = nullptr;
		for each (String^ component in components)
		{
			if (component == String::Empty)
				break;

			FatDirectory^ currentDirectory = LoadDirectory(cluster, parentDir);
			cluster = currentDirectory->Items[component]->Cluster;
			parentDir = component;
		}

		return cluster;
	}

	Fat32Api::Directory::Directory(String^ name, unsigned cluster, Fat32Api^ api)
		: FatDirectory(name, cluster, api)
	{
	}

	unsigned Fat32Api::Directory::GetStartCluster(::FatDirectory& directory)
	{
		if (directory.Short.Attributes == 0x0F)
			throw gcnew ArgumentException(L"The provided directory is a long file name.");
		return directory.Short.StartClusterLow | (unsigned(directory.Short.StartClusterHigh) << 16);
	}
}
}
