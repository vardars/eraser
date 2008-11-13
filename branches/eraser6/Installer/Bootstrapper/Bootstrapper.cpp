/* 
 * $Id$
 * Copyright 2008 The Eraser Project
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

#include "stdafx.h"
#include "Bootstrapper.h"

/// ISzInStream interface for extracting the archives.
struct LZFileStream
{
public:
	/// Constructor.
	/// 
	/// \param[in] fileHandle A HANDLE to the file stream, returned by CreateFile.
	LZFileStream(HANDLE fileHandle)
	{
		InStream.Read = LZFileStreamRead;
		InStream.Seek = LzFileStreamSeek;
		FileHandle = fileHandle;

		FileRead = FileRead = 0;
		LARGE_INTEGER largeInt;
		largeInt.QuadPart = 0;
		if (!SetFilePointerEx(FileHandle, largeInt, &largeInt, FILE_CURRENT))
			throw GetErrorMessage(GetLastError());

		long long currPos = largeInt.QuadPart;
		largeInt.QuadPart = 0;
		if (!SetFilePointerEx(FileHandle, largeInt, &largeInt, FILE_END))
			throw GetErrorMessage(GetLastError());
		FileSize = largeInt.QuadPart - currPos;

		largeInt.QuadPart = currPos;
		if (!SetFilePointerEx(FileHandle, largeInt, NULL, FILE_BEGIN))
			throw GetErrorMessage(GetLastError());
	}

	~LZFileStream()
	{
		CloseHandle(FileHandle);
	}

	ISzInStream InStream;

private:
	HANDLE FileHandle;
	long long FileRead;
	long long FileSize;

	static SZ_RESULT LZFileStreamRead(void* object, void** bufferPtr, size_t size,
		size_t* processedSize)
	{
		LZFileStream* s = static_cast<LZFileStream*>(object);

		//Since we can allocate as much as we want to allocate, take a decent amount
		//of memory and stop.
		size = std::min(1048576u * 4, size);
		char* buffer = new char[size];

		DWORD readSize = 0;
		if (ReadFile(s->FileHandle, buffer, size, &readSize, NULL) && readSize != 0)
		{
			*bufferPtr = buffer;
			*processedSize = readSize;
			s->FileRead += readSize;

			SetProgress((float)((double)s->FileRead / s->FileSize));
		}

		return SZ_OK;
	}

	static SZ_RESULT LzFileStreamSeek(void *object, CFileSize pos)
	{
		LZFileStream* s = static_cast<LZFileStream*>(object);

		LARGE_INTEGER value;
		value.LowPart = (DWORD)pos;
		value.HighPart = (LONG)((UInt64)pos >> 32);
		if (!SetFilePointerEx(s->FileHandle, value, NULL, FILE_BEGIN) &&
			GetLastError() != NO_ERROR)
			return SZE_FAIL;
		return SZ_OK;
	}
};

void ExtractTempFiles(std::wstring pathToExtract)
{
	if (std::wstring(L"\\/").find(pathToExtract[pathToExtract.length() - 1]) == std::wstring::npos)
		pathToExtract += L"\\";

	//Open the file
#if _DEBUG
	HANDLE srcFile = CreateFileW((GetApplicationPath() + L".7z").c_str(), GENERIC_READ, FILE_SHARE_READ,
		NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL , NULL);
	if (srcFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());

	//Seek to the 128th kb.
	LARGE_INTEGER fPos;
	fPos.QuadPart = 0;
#else
	HANDLE srcFile = CreateFileW(GetApplicationPath().c_str(), GENERIC_READ, FILE_SHARE_READ,
		NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL , NULL);
	if (srcFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());

	//Seek to the 128th kb.
	LARGE_INTEGER fPos;
	fPos.QuadPart = 128 * 1024;
#endif

	if (!SetFilePointerEx(srcFile, fPos, &fPos, FILE_BEGIN))
		throw GetErrorMessage(GetLastError());

	//7z archive database structure
	CArchiveDatabaseEx db;

	//memory functions
	ISzAlloc allocImp;
	ISzAlloc allocTempImp;
	allocTempImp.Alloc = allocImp.Alloc = SzAlloc;
	allocTempImp.Free = allocImp.Free = SzFree;

	//Initialize the CRC and database structures
	LZFileStream stream(srcFile);
	CrcGenerateTable();
	SzArDbExInit(&db);
	if (SzArchiveOpen(&stream.InStream, &db, &allocImp, &allocTempImp) != SZ_OK)
		throw std::wstring(L"Could not open archive for reading.");

	//Read the database for files
	unsigned blockIndex = 0;
	Byte* outBuffer = NULL;
    size_t outBufferSize = 524288;
	for (unsigned i = 0; i < db.Database.NumFiles; ++i)
	{
		size_t offset = 0;
		size_t processedSize = 0;
		CFileItem *f = db.Database.Files + i;
		SZ_RESULT result = SZ_OK;

		//Create the output file
		size_t convertedChars = 0;
		wchar_t fileName[MAX_PATH];
		mbstowcs_s(&convertedChars, fileName, f->Name, sizeof(fileName) / sizeof(fileName[0]));
		HANDLE destFile = CreateFileW((pathToExtract + fileName).c_str(), GENERIC_WRITE, 0,
			NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		if (destFile == INVALID_HANDLE_VALUE)
			throw GetErrorMessage(GetLastError());
		unsigned long long destFileSize = f->Size;

		//Extract the file
		while (result == SZ_OK && destFileSize)
		{
			result = SzExtract(&stream.InStream, &db, i, &blockIndex,
				&outBuffer, &outBufferSize, &offset, &processedSize, &allocImp,
				&allocTempImp);
			if (result != SZ_OK)
				_asm int 3;

			DWORD bytesWritten = 0;
			if (!WriteFile(destFile, outBuffer + offset, processedSize, &bytesWritten, NULL) ||
				bytesWritten != processedSize)
				throw GetErrorMessage(GetLastError());
			destFileSize -= bytesWritten;
		}

		CloseHandle(destFile);
	}
	allocImp.Free(outBuffer);
}

bool HasNetFramework()
{
	HKEY key;
	std::wstring highestVer;
	std::wstring keys[] = { L"v2.0.50727", L"v3.0", L"v3.5" };

	for (int i = 0, j = sizeof(keys) / sizeof(keys[0]); i != j; ++i)
	{
		//Open the key for reading
		DWORD result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
			(L"SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\" + keys[i]).c_str(),
			0, KEY_READ, &key);

		//Retry for 64-bit WoW
		if (result == ERROR_FILE_NOT_FOUND)
		{
			result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
				(L"SOFTWARE\\Wow6432Node\\Microsoft\\NET Framework Setup\\NDP\\" + keys[i]).c_str(),
				0, KEY_READ, &key);
		}

		if (result != ERROR_SUCCESS)
			throw GetErrorMessage(result);

		//Query the Installed string
		DWORD installedVal = 0;
		DWORD bufferSize = sizeof(installedVal);
		result = RegQueryValueExW(key, L"Install", NULL, NULL, (BYTE*)&installedVal,
			&bufferSize);
		if (result != ERROR_SUCCESS && result != ERROR_MORE_DATA)
			throw GetErrorMessage(result);
		if (installedVal == 1)
			highestVer = keys[i].substr(1);
		RegCloseKey(key);
	}

	return !highestVer.empty();
}

int CreateProcessAndWait(const std::wstring& commandLine)
{
	//Get a mutable version of the command line
	wchar_t* cmdLine = new wchar_t[commandLine.length() + 1];
	wcscpy_s(cmdLine, commandLine.length() + 1, commandLine.c_str());

	//Launch the process
	STARTUPINFOW startupInfo;
	PROCESS_INFORMATION pInfo;
	::ZeroMemory(&startupInfo, sizeof(startupInfo));
	::ZeroMemory(&pInfo, sizeof(pInfo));
	if (!CreateProcessW(NULL, cmdLine, NULL, NULL, false, 0, NULL,  NULL, &startupInfo,
		&pInfo))
	{
		delete[] cmdLine;
		throw GetErrorMessage(GetLastError());
	}
	delete[] cmdLine;

	//Ok the process was created, wait for it to terminate.
	DWORD lastWait = 0;
	while ((lastWait = WaitForSingleObject(pInfo.hProcess, 50)) == WAIT_TIMEOUT)
		Yield();
	if (lastWait == WAIT_ABANDONED)
		throw std::wstring(L"The condition waiting on the termination of the .NET installer was abandoned.");

	//Get the exit code
	DWORD exitCode = 0;
	if (!GetExitCodeProcess(pInfo.hProcess, &exitCode))
		throw GetErrorMessage(GetLastError());

	//Clean up
	CloseHandle(pInfo.hProcess);
	CloseHandle(pInfo.hThread);

	//Return the exit code.
	return exitCode;
}

bool InstallNetFramework(std::wstring tempDir)
{
	//Update the UI
	SetProgressIndeterminate();
	SetMessage(L"Installing .NET Framework...");

	//Get the path to the installer
	if (std::wstring(L"\\/").find(tempDir[tempDir.length() - 1]) == std::wstring::npos)
		tempDir += L"\\";
	std::wstring commandLine(L'"' + tempDir);
	commandLine += L"dotnetfx.exe\"";

	//And the return code is true if the process exited with 0.
	return CreateProcessAndWait(commandLine) == 0;
}

bool InstallEraser(std::wstring tempDir)
{
	SetProgressIndeterminate();
	SetMessage(L"Installing Eraser...");

	//Determine the system architecture.
	SYSTEM_INFO sysInfo;
	ZeroMemory(&sysInfo, sizeof(sysInfo));
	GetSystemInfo(&sysInfo);

	if (std::wstring(L"\\/").find(tempDir[tempDir.length() - 1]) == std::wstring::npos)
		tempDir += L"\\";
	switch (sysInfo.wProcessorArchitecture)
	{
	case PROCESSOR_ARCHITECTURE_AMD64:
		tempDir += L"Eraser (x64).msi";
		break;

	default:
		tempDir += L"Eraser (x86).msi";
		break;
	}

	std::wstring commandLine(L"msiexec.exe /i ");
	commandLine += L'"' + tempDir + L'"';
	
	//And the return code is true if the process exited with 0.
	return CreateProcessAndWait(commandLine) == 0;
}
