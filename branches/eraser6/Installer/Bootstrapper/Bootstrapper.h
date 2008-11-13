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

#pragma once

#include <string>

#include "resource.h"

/// Formats the system error code using FormatMessage, returning the message as
/// a std::wstring.
std::wstring GetErrorMessage(DWORD lastError);

/// Processes existing messages, to prevent the application from being "unresponsive"
#undef Yield
void Yield();

/// Retrieves the application's top level window.
HWND GetTopWindow();

void SetProgress(float progress);
void SetProgressIndeterminate();
void SetMessage(std::wstring message);

/// Retrieves the path to the executable file.
std::wstring GetApplicationPath();

/// Extracts the setup files to the users' temporary folder.
/// 
/// \param[in] pathToExtract The path to extract the temporary files to.
void ExtractTempFiles(std::wstring pathToExtract);

/// Checks for the presence of the .NET Framework on the client computer.
bool HasNetFramework();

/// Extracts the included .NET framework installer and runs it.
void InstallNetFramework();
