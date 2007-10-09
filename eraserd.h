// EraserD.h
//
// Eraser. Secure data removal. For DOS.
// Copyright © 2002  Garrett Trant (gtrant@heidi.ie).
// Copyright © 1997-2001  Sami Tolvanen (sami@tolvanen.com).
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
// 02111-1307, USA.

#ifndef _ERASERD_H_
#define _ERASERD_H_

// basic types
typedef signed char     E_INT8,   *E_PINT8;
typedef signed int      E_INT16,  *E_PINT16;
typedef signed long     E_INT32,  *E_PINT32;
typedef unsigned char   E_UINT8,  *E_PUINT8;
typedef unsigned int    E_UINT16, *E_PUINT16;
typedef unsigned long   E_UINT32, *E_PUINT32;

//#ifndef HAS_BOOL
//typedef unsigned char bool;
//#define false 0
//#define true (!false)
//#endif

// command line parameters
const char * const szHelp1      = "/?";
const char * const szHelp2      = "-help";
const char * const szHelp3      = "--help";

const char * const szFile       = "-file";
const char * const szAllFiles   = "-allfiles";
const char * const szFolder     = "-folder";
const char * const szSubFolders = "-subfolders";
const char * const szKeepFolder = "-keepfolder";
const char * const szDisk       = "-disk";
const char * const szNoTips     = "-notips";

const char * const szPasses     = "-passes";
const char * const szSilent     = "-silent";
const char * const szNoDelete   = "-nodel";

// messages
const char * const szUsage =
    "Usage:\n"
    "  eraserd [Data] [-passes passes] [-silent]\n\n"
    "  Data:\n\n"
    "    -file\tdata [-nodel]\n"
    "    -folder\tdata [-subfolders] [-keepfolder]\n"
    "    -disk\tdrive: [-notips]\n"
    "    -allfiles\tdrive:\n\n"
    "  Parameters:\n\n"
    "    -file\t\tErase file(s) (wildcards allowed)\n"
    "     -nodel\t\t Do not delete file(s) after erasing\n"
    "    -folder\t\tErase all files in the folder\n"
    "     -subfolders\t Include subfolders\n"
    "     -keepfolder\t Do not delete the folder\n"
    "    -disk\t\tErase unused space on the drive\n"
    "     -notips\t\t Do not erase cluster tip area\n"
    "    -allfiles\t\tErase all files on a drive\n"
    "    -passes\t\tNumber of overwriting passes (default 1)\n"
    "    -silent\t\tNothing to standard output\n";

const char * const szCopyright = "Eraser %s for DOS. Free Software.\nCopyright 2007 Garrett Trant. (http://www.heidi.ie/eraser/)\n";

const int iBarLength = 54;
const int iShowNameLength = 8 + 1 + 3;
const char * const szDriveShowName = "%c: (Drive)";

// |===[iBarLength]===| AAAAAAAA.BBB | 100.00%
const char * const szProgress = "\r|%s| %s | %.02f%%";
const char * const szClusterTipProgress = "\r|%s| %c: (Cluster tips)";
const char * const szClearProgress = "\r                                                                               \r";

const char * const szSignalFailed = "EraserD: failed to set signal handler\n";
const char * const szTerminated = "EraserD: erasing terminated by user\n";
const char * const szFailed = "EraserD: failed to erase file %s\n";
const char * const szClusterTipFailed = "EraserD: failed to erase cluster tip area of file %s\n";
const char * const szSucceeded = "EraserD: erased file %s (%d passes)\n";
const char * const szWiped = "EraserD: managed to wipe (%d passes) but not delete file %s\n";
const char * const szClusterTipsWiped = "EraserD: wiped cluster tips on drive %c: (%d passes)\n";
const char * const szFreeSpaceWiped = "EraserD: wiped free space on drive %c: (%d passes)\n";

const char * const szRmdirFailed = "EraserD: failed to remove %s\n";
const char * const szGetFreeSpaceFailed = "EraserD: failed to get the amount of free space on drive %c:\n";
const char * const szTempCreateFailed = "EraserD: failed to create a temporary file\n";
const char * const szTempRemoveFailed = "EraserD: failed to remove a temporary file\n";

const char * const szOutOfMemory = "EraserD: out of memory\n";
const char * const szNoSuchFile = "EraserD: no such file (%s)\n";

const char * const szEraserAllConfirmation = "Are you sure you want to erase all files on drive %c: [y/N]? ";
const char * const szOperationCanceled = "Operation canceled.\n";

// constants
const E_UINT16 ERASER_MAX_CLUSTER = 0x8000; // 32K
const E_UINT16 ERASER_BUFFER_SIZE = ERASER_MAX_CLUSTER; // we write data in max. cluster size blocks

// for overwriting file name
const E_UINT16 ERASER_FILENAME_PASSES = 7;
const E_UINT16 ERASER_SAFEARRAY_SIZE = 36;
const char * const ERASER_SAFEARRAY = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

// the eraser context
struct EraserContext {
    const char *m_szFileName;   // the file to erase

    E_INT16   m_iFile;          // handle to the file
    E_UINT16  m_uPasses;        // how many passes
    E_UINT32  m_uStart;         // position where to start overwriting
    E_UINT32  m_uSize;          // the amount of data to overwrite

    E_PUINT32 m_puBuffer;       // write buffer
    E_UINT16  m_uBufferSize;    // buffer size

    const char *m_szShowName;   // just the 8+3 filename
    bool      m_bShowProgress;  // whether to show progress bar
    bool      m_bNoMatchError;  // no error if matching files were not found

    bool      m_bSilent;        // nothing to stdout
    bool      m_bNoDelete;      // don't delete file(s)
    bool      m_bKeepFolder;    // keep the root folder
    bool      m_bSubFolders;    // wipe subfolders as well
    bool      m_bNoClusterTips; // include cluster tips?

    bool      m_bStop;          // stop signal
};

// action
enum EraserAction {
    NoAction,
    File,
    AllFiles,
    Folder,
    Disk
};

#endif
