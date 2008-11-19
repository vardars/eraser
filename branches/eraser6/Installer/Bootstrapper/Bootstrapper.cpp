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

class Handle
{
public:
	Handle(HANDLE handle)
	{
		thisHandle = handle;
	}

	~Handle()
	{
		CloseHandle(thisHandle);
	}

	operator HANDLE()
	{
		return thisHandle;
	}

private:
	HANDLE thisHandle;
};

int Integrate(const std::wstring& destItem, const std::wstring& package)
{
	//Open a handle to ourselves
	Handle srcFile(CreateFileW(Application::Get().GetPath().c_str(), GENERIC_READ,
		FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL));
	if (srcFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());

	//Copy ourselves, in essence.
	Handle destFile(CreateFileW(destItem.c_str(), GENERIC_WRITE, 0, NULL,
		CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL));
	if (destFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());

	DWORD lastOperation = 0;
	char buffer[262144];
	while (ReadFile(srcFile, buffer, sizeof(buffer), &lastOperation, NULL) && lastOperation)
		WriteFile(destFile, buffer, lastOperation, &lastOperation, NULL);

	//Fill to the predetermined file position
	int amountToWrite = DataOffset - GetFileSize(srcFile, NULL);
	if (amountToWrite < 0)
		throw std::wstring(L"The file size of the binary is larger than the data "
			L"offset position; recompile the package, increasing DataOffset.");
	ZeroMemory(buffer, sizeof(buffer));
	while (amountToWrite > 0)
	{
		WriteFile(destFile, buffer, std::min<unsigned>(amountToWrite, sizeof(buffer)),
			&lastOperation, NULL);
		amountToWrite -= lastOperation;
	}

	//Then copy the package
	Handle packageFile(CreateFileW(package.c_str(), GENERIC_READ, FILE_SHARE_READ, NULL,
		OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL));
	if (packageFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());
	int error;
	SetLastError(0);
	while (ReadFile(packageFile, buffer, sizeof(buffer), &lastOperation, NULL) && lastOperation)
	{
		WriteFile(destFile, buffer, lastOperation, &lastOperation, NULL);
		error = GetLastError();
	}
	return 0;
}

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

		FileRead = 0;
		LARGE_INTEGER largeInt;
		largeInt.QuadPart = 0;
		if (!SetFilePointerEx(FileHandle, largeInt, &largeInt, FILE_CURRENT))
			throw GetErrorMessage(GetLastError());
		DataOffset = largeInt.QuadPart;

		if (!GetFileSizeEx(fileHandle, &largeInt))
			throw GetErrorMessage(GetLastError());
		FileSize = largeInt.QuadPart - DataOffset;
	}

	~LZFileStream()
	{
		CloseHandle(FileHandle);
	}

	ISzInStream InStream;

private:
	HANDLE FileHandle;
	long long DataOffset;
	long long FileRead;
	long long FileSize;

	static SZ_RESULT LZFileStreamRead(void* object, void** bufferPtr, size_t size,
		size_t* processedSize)
	{
		LZFileStream* s = static_cast<LZFileStream*>(object);

		//Since we can allocate as much as we want to allocate, take a decent amount
		//of memory and stop.
		size = std::min(1048576u * 4, size);
		static char* buffer = NULL;
		if (buffer)
			delete[] buffer;
		buffer = new char[size];

		DWORD readSize = 0;
		if (ReadFile(s->FileHandle, buffer, size, &readSize, NULL) && readSize != 0)
		{
			*bufferPtr = buffer;
			*processedSize = readSize;
			s->FileRead += readSize;

			MainWindow& mainWin = Application::Get().GetTopWindow();
			mainWin.SetProgress((float)((double)s->FileRead / s->FileSize));
		}

		return SZ_OK;
	}

	static SZ_RESULT LzFileStreamSeek(void *object, CFileSize pos)
	{
		LZFileStream* s = static_cast<LZFileStream*>(object);

		LARGE_INTEGER value;
		value.QuadPart = pos + s->DataOffset;
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
	HANDLE srcFile = CreateFileW((Application::Get().GetPath() + L".7z").c_str(),
		GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (srcFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());
#else
	HANDLE srcFile = CreateFileW(Application::Get().GetPath().c_str(), GENERIC_READ,
		FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (srcFile == INVALID_HANDLE_VALUE)
		throw GetErrorMessage(GetLastError());

	//Seek to the 196th kb.
	LARGE_INTEGER fPos;
	fPos.QuadPart = DataOffset;

	if (!SetFilePointerEx(srcFile, fPos, &fPos, FILE_BEGIN))
		throw GetErrorMessage(GetLastError());
#endif

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
    size_t outBufferSize = 0;
	for (unsigned i = 0; i < db.Database.NumFiles; ++i)
	{
		size_t offset = 0;
		size_t processedSize = 0;
		CFileItem* file = db.Database.Files + i;
		SZ_RESULT result = SZ_OK;

		//Create the output file
		size_t convertedChars = 0;
		wchar_t fileName[MAX_PATH];
		mbstowcs_s(&convertedChars, fileName, file->Name, sizeof(fileName) / sizeof(fileName[0]));
		Handle destFile(CreateFileW((pathToExtract + fileName).c_str(), GENERIC_WRITE, 0,
			NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL));
		if (destFile == INVALID_HANDLE_VALUE)
			throw GetErrorMessage(GetLastError());
		unsigned long long destFileSize = file->Size;

		//Extract the file
		while (result == SZ_OK && destFileSize)
		{
			result = SzExtract(&stream.InStream, &db, i, &blockIndex,
				&outBuffer, &outBufferSize, &offset, &processedSize, &allocImp,
				&allocTempImp);
			if (result != SZ_OK)
				throw std::wstring(L"Could not decompress data as it is corrupt.");

			DWORD bytesWritten = 0;
			if (!WriteFile(destFile, outBuffer + offset, processedSize, &bytesWritten, NULL) ||
				bytesWritten != processedSize)
				throw GetErrorMessage(GetLastError());
			destFileSize -= bytesWritten;
			Application::Get().Yield();
		}
	}

	allocImp.Free(outBuffer);
}

bool HasNetFramework()
{
	HKEY key;
	std::wstring highestVer;
	std::wstring keys[] = { L"v3.5" };

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

			if (result == ERROR_FILE_NOT_FOUND)
				continue;
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
		Application::Get().Yield();
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
	MainWindow& mainWin = Application::Get().GetTopWindow();
	mainWin.SetProgressIndeterminate();
	mainWin.SetMessage(L"Installing .NET Framework...");

	//Get the path to the installer
	if (std::wstring(L"\\/").find(tempDir[tempDir.length() - 1]) == std::wstring::npos)
		tempDir += L"\\";
	std::wstring commandLine(L'"' + tempDir);
	commandLine += L"dotnetfx35.exe\"";

	//And the return code is true if the process exited with 0.
	return CreateProcessAndWait(commandLine) == 0;
}

bool InstallEraser(std::wstring tempDir)
{
	MainWindow& mainWin = Application::Get().GetTopWindow();
	mainWin.SetProgressIndeterminate();
	mainWin.SetMessage(L"Installing Eraser...");

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
