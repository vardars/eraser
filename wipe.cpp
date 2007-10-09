// wipe.cpp
//
// Eraser. Secure data removal. For DOS.
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

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <dos.h>
#include <direct.h>
#include <io.h>
#include <fcntl.h>
#include <errno.h>
#include <time.h>
#include <process.h>
#include <sys\utime.h>
#include <sys\types.h>
#include <assert.h>

#include "EraserD.h"
#include "Random.h"
#include "List.h"
#include "Wipe.h"

#define min __min

#define ISNT_SUBFOLDER(lpsz) \
    ((lpsz)[0] == '.' && \
     ((lpsz)[1] == '\0' || \
      ((lpsz)[1] == '.' && \
       (lpsz)[2] == '\0')))

static inline bool
resetDate(const char *szFile)
{
    assert(szFile != NULL);
    _utimbuf ut;
    ut.actime = 0;
    ut.modtime = 0;

    return (_utime(szFile, &ut) == 0);
 }

static inline bool
overwriteFileName(const char *szFile, char *szLastFileName)
{
    if (szFile == 0 || szLastFileName == 0) {
        return false;
    }

    char szNewName[_MAX_PATH];
    char *pszLastSlash;
    E_UINT32 i, j, index;

    szNewName[0] = '\0';

    strcpy(szLastFileName, szFile);
    pszLastSlash = strrchr(szLastFileName, '\\');

    if (pszLastSlash == NULL) {
        index = 0;
    } else {
        index = (pszLastSlash - szLastFileName) + 1;
    }

    strcpy(szNewName, szLastFileName);

    for (i = 0; i < ERASER_FILENAME_PASSES; i++) {
        // replace each non-'.' character with a random letter
        for (j = index; j < strlen(szFile); j++) {
            if (szFile[j] != '.') {
                szNewName[j] = ERASER_SAFEARRAY[(E_UINT16)(ACRandom() * ERASER_SAFEARRAY_SIZE)];
            }
        }
        if (rename(szLastFileName, szNewName) == 0) {
            strcpy(szLastFileName, szNewName);
        }
    }

    return true;
}

static bool
wipeFileWithPseudoRandom(EraserContext *context)
{
    if (context == 0) {
        return false;
    }

    bool bCompleted = false;
    E_UINT32 uLength = 0;
    E_UINT16 uDataSize = 0;
    E_UINT16 uWritten = 0;

    // progress
    E_UINT32 uTotal = context->m_uSize * context->m_uPasses;
    E_UINT32 uWiped = 0;
    float fProgress = 0.0f;
    int iProgress = 0;
    char szBar[iBarLength + 1];

    if (context->m_bShowProgress) {
        memset(szBar, ' ', iBarLength);
        szBar[iBarLength] = '\0';
    }

    // seed prng
    ACSeed(context->m_puBuffer);

    for (E_UINT16 uPass = 1; uPass <= context->m_uPasses; uPass++) {
        // start from the beginning
        _dos_seek(context->m_iFile, context->m_uStart, 0);
        // restore size
        uLength = context->m_uSize;

        while (uLength > 0) {
            if (context->m_bStop) {
                bCompleted = false;
                break;
            }

            // progress
            if (context->m_bShowProgress) {
                if (iProgress > 0 && iProgress <= iBarLength) {
                    memset(szBar, '=', iProgress);
                }
                printf(szProgress, szBar, context->m_szShowName, fProgress);
            }

            // refill random data
            ACFill(context->m_puBuffer, context->m_uBufferSize);

            // how much to write
            uDataSize = (E_UINT16)min(uLength, context->m_uBufferSize);

            // write
            bCompleted = (_dos_write(context->m_iFile, context->m_puBuffer,
                                     uDataSize, &uWritten) == 0);
            // flush
            _dos_commit(context->m_iFile);

            if (!bCompleted) {
                break;
            }

            // remaining
            uLength -= (E_UINT16)uDataSize;
            // progress
            if (context->m_bShowProgress) {
                uWiped += (E_UINT32)uDataSize;
                fProgress = (float)(uWiped * 100.0f / uTotal);
                iProgress = (int)((fProgress * iBarLength / 100.0f) + 0.5f);
            }
        }

        if (context->m_bShowProgress) {
            if (iProgress > 0 && iProgress <= iBarLength) {
                memset(szBar, '=', iProgress);
            }
            printf(szProgress, szBar, context->m_szShowName, fProgress);
        }
    }

    // save random seed
    ACSave(context->m_puBuffer);

    // clear progress information
    if (context->m_bShowProgress) {
        printf(szClearProgress);
    }

    return bCompleted;
}

static bool
wipeFile(EraserContext *context)
{
    if (context == 0) {
        return false;
    }

    // maximum file size we can handle is 2GB

    if (_dos_open(context->m_szFileName, _O_RDWR, &context->m_iFile) != 0) {
        return false;
    } else {
        // file opened, now get its size (2 == from the end of file)
        context->m_uSize = _dos_seek(context->m_iFile, 0, 2);
        if (context->m_uSize == (E_UINT32)-1L && (errno == EBADF || errno == EINVAL)) {
            _dos_close(context->m_iFile);
            context->m_iFile = -1;
            return false;
        }
        // size is multiple of max. cluster size so the slack space will be cleared
        context->m_uSize = ((context->m_uSize / ERASER_MAX_CLUSTER) + 1) * ERASER_MAX_CLUSTER;
    }

    bool bCompleted = false;
    if (context->m_uSize > 0) {
        // start from the beginning
        context->m_uStart = 0;
        // show progress if not in silent mode
        context->m_bShowProgress = !context->m_bSilent;
        // overwrite
        bCompleted = wipeFileWithPseudoRandom(context);
        // length to zero
        if (bCompleted && !context->m_bNoDelete) {
            _chsize(context->m_iFile, 0);
        }
    } else {
        // nothing to wipe
        bCompleted = true;
    }

    // and we're done
    _dos_close(context->m_iFile);
    context->m_iFile = -1;

    return bCompleted;
}

static bool
wipeClusterTip(EraserContext *context)
{
    if (context == 0) {
        return false;
    }

    // maximum file size we can handle is 2GB - ERASER_MAX_CLUSTER

    E_UINT16 uFileTime = 0, uFileDate = 0;

    if (_dos_open(context->m_szFileName, _O_RDWR, &context->m_iFile) != 0) {
        return false;
    } else {
        bool bCompleted;
        // file opened, save last modified date
        _dos_getftime(context->m_iFile, &uFileDate, &uFileTime);
        // get file size (2 == from the end of file)
        context->m_uStart = _dos_seek(context->m_iFile, 0, 2);
        // failed?
        if (context->m_uStart == (E_UINT32)-1L && (errno == EBADF || errno == EINVAL)) {
            bCompleted = false;
        } else {
            // we write just enough to overwrite the cluster tip area
            context->m_uSize = ERASER_MAX_CLUSTER;
            // no progress
            context->m_bShowProgress = false;

            // overwrite cluster tip
            bCompleted = wipeFileWithPseudoRandom(context);
            // restore length
            _chsize(context->m_iFile, (E_INT32)context->m_uStart);
            // and date
            _dos_setftime(context->m_iFile, uFileDate, uFileTime);
        }

        // and we're done
        _dos_close(context->m_iFile);
        context->m_iFile = -1;

        return bCompleted;
    }
}

static bool
wipeFreeSpace(EraserContext *context)
{
    char cDrive = toupper(context->m_szFileName[0]);
    E_UINT16 uDrive = cDrive - 'A' + 1;
    _diskfree_t dtFree;

    memset(&dtFree, 0, sizeof(_diskfree_t));

    if (_dos_getdiskfree(uDrive, &dtFree) != 0) {
        if (!context->m_bSilent) {
            printf(szGetFreeSpaceFailed, cDrive);
        }
    } else {
        char szTempFile[_MAX_PATH];
        char szScrambledName[_MAX_PATH];
        const char *szFileName = 0;
        E_UINT32 uSize = (E_UINT32)dtFree.avail_clusters *
                         (E_UINT32)dtFree.sectors_per_cluster *
                         (E_UINT32)dtFree.bytes_per_sector;

        memset(szTempFile, 0, _MAX_PATH);
        memset(szScrambledName, 0, _MAX_PATH);

        _snprintf(szTempFile, _MAX_PATH, "%c:\\erad%04X.tmp", cDrive, (E_UINT16)_getpid());

        if (_dos_creat((const char*)szTempFile, _A_NORMAL, &context->m_iFile) == 0) {
            bool bCompleted = false;

            // set filename
            szFileName = context->m_szFileName;
            context->m_szFileName = (const char*)szTempFile;
            // start from zero and overwrite everything
            context->m_uStart = 0;
            context->m_uSize = uSize;
            // show progress if not in silent mode
            context->m_bShowProgress = !context->m_bSilent;

            // start erasing
            bCompleted = wipeFileWithPseudoRandom(context);

            // length to zero
            _chsize(context->m_iFile, 0);

            if (bCompleted && !context->m_bSilent) {
                printf(szFreeSpaceWiped, cDrive, context->m_uPasses);
            }

            // close the file
            _dos_close(context->m_iFile);
            context->m_iFile = -1;

            // remove it
            resetDate(context->m_szFileName);
            overwriteFileName(context->m_szFileName, szScrambledName);

            if (_unlink(szScrambledName) != 0) {
                if (!context->m_bSilent) {
                    printf(szTempRemoveFailed);
                }
                rename(szScrambledName, context->m_szFileName);
            }

            context->m_szFileName = szFileName;
            return bCompleted;
        } else {
            if (!context->m_bSilent) {
                printf(szTempCreateFailed);
            }
        }
    }

    return false;
}

static inline bool
parseMatchingDirectories(const char *szBase, const char *szSearch, List **root)
{
    char szPath[_MAX_PATH];
    _find_t ft;

    memset(szPath, 0, _MAX_PATH);
    memset(&ft, 0, sizeof(_find_t));

    strncpy(szPath, szBase, _MAX_PATH);
    if (szPath[strlen(szPath) - 1] != '\\') {
        strcat(szPath, "\\");
    }
    strcat(szPath, szSearch);

    // search files matching the description and erase them
    if (_dos_findfirst(szPath, _A_SUBDIR | _A_ARCH | _A_HIDDEN | _A_NORMAL |
                       _A_SYSTEM | _A_RDONLY, &ft) == 0) {
        szPath[0] = 0;
        do {
            if (ft.attrib & _A_SUBDIR) {
                if (ISNT_SUBFOLDER(ft.name)) {
                    continue;
                }
                // the full path name
                strncpy(szPath, szBase, _MAX_PATH);
                strcat(szPath, ft.name);
                strcat(szPath, "\\");
                // add to list
                listAdd(root, (const char*)szPath);
                // recursive
                parseMatchingDirectories((const char*)szPath, szSearch, root);
            }
        } while (_dos_findnext(&ft) == 0);
    }

    return (*root != 0);
}

static bool
parseDirectories(const char *szBase, List **root)
{
    if (root != 0 && *root != 0) {
        // search all directories
        parseMatchingDirectories(szBase, "*.*", root);
        return (*root != 0);
    } else {
        return false;
    }
}

static bool
wipeMatchingClusterTips(EraserContext *context)
{
    static E_UINT32 uCounter = 0;

    if (context == 0) {
        return false;
    }

    bool bCompleted = true;
    bool bOriginalShowProgress;
    char szCurrentFile[_MAX_PATH];
    char szPath[_MAX_PATH];
    char szDrive[_MAX_DRIVE];
    char szDir[_MAX_DIR];
    E_UINT16 uAttrib = 0;
    _find_t ft;
    E_UINT16 uProgress = 0;
    char szBar[iBarLength + 1];

    memset(szCurrentFile, 0, _MAX_PATH);
    memset(szPath, 0, _MAX_PATH);
    memset(szDrive, 0, _MAX_DRIVE);
    memset(szDir, 0, _MAX_DIR);
    memset(&ft, 0, sizeof(_find_t));

    // initialize progress bar
    if (!context->m_bSilent) {
        memset(szBar, ' ', iBarLength);
        szBar[iBarLength] = '\0';
    }

    // remember progress settings
    bOriginalShowProgress = context->m_bShowProgress;

    // find out the base directory (if any)
    _splitpath(context->m_szFileName, szDrive, szDir, NULL, NULL);
    strcpy(szPath, szDrive);
    strcat(szPath, szDir);

    // search files in the folder and erase cluster tips
    if (_dos_findfirst(context->m_szFileName,
            _A_ARCH | _A_HIDDEN | _A_NORMAL | _A_SYSTEM | _A_RDONLY, &ft) == 0) {
        do {
            if (context->m_bStop) {
                bCompleted = false;
                break;
            }

            // show "entertaining" progress bar
            if (!context->m_bSilent) {
                // clear bar
                memset(szBar, ' ', iBarLength);

                // first bar character
                uProgress = (E_UINT16)(uCounter % iBarLength);
                szBar[uProgress] = '=';

                // the rest two characters
                if (uProgress > 0) {
                    if (uProgress < iBarLength - 1) {
                        szBar[uProgress + 1] = '=';
                        if (uProgress < iBarLength - 2) {
                            szBar[uProgress + 2] = '=';
                        } else {
                            // start from the beginning
                            szBar[0] = '=';
                        }
                    } else {
                        // start from the beginning
                        szBar[0] = szBar[1] = '=';
                    }
                } else {
                    szBar[1] = szBar[2] = '=';
                }
                printf(szClusterTipProgress, szBar, context->m_szShowName[0]);
            }

            strncpy(szCurrentFile, szPath, _MAX_PATH);
            strcat(szCurrentFile, ft.name);
            // set filename
            context->m_szFileName = (const char*)szCurrentFile;

            // save file attributes
            _dos_getfileattr(context->m_szFileName, &uAttrib);
            // set file attributes to normal
            _dos_setfileattr(context->m_szFileName, _A_NORMAL);

            if (!wipeClusterTip(context)) {
                if (!context->m_bSilent) {
                    printf(szClearProgress);
                    printf(szClusterTipFailed, context->m_szFileName);
                }
                bCompleted = false;
            }

            // restore file attributes
            _dos_setfileattr(context->m_szFileName, uAttrib);

            // increase counter
            uCounter++;
        } while (_dos_findnext(&ft) == 0);
    }

    // restore progress settings;
    context->m_bShowProgress = bOriginalShowProgress;

    return bCompleted;
}

static bool
wipeClusterTipsOnDrive(EraserContext *context)
{
    bool bCompleted = false;
    List *plCurrent = 0;
    List *plDirectories = 0;

    char szPath[_MAX_PATH];
    memset(szPath, 0, _MAX_PATH);

    strncpy(szPath, context->m_szFileName, _MAX_PATH);
    if (szPath[strlen(szPath) - 1] != '\\') {
        strcat(szPath, "\\");
    }

    // add the root directory
    listAdd(&plDirectories, (const char*)szPath);

    // add subdirectories
    parseDirectories((const char*)szPath, &plDirectories);

    // start from the beginning
    plCurrent = plDirectories;

    do {
        if (context->m_bStop) {
            bCompleted = false;
            break;
        }

        // wipe cluster tips from all files
        strncpy(szPath, plCurrent->szData, _MAX_PATH);
        strcat(szPath, "*.*");

        context->m_szFileName = (const char*)szPath;
        bCompleted = wipeMatchingClusterTips(context);

        plCurrent = plCurrent->plNext;
    } while (plCurrent != 0);

    // clear progress information and let the user
    // know we are done
    if (!context->m_bSilent) {
        printf(szClearProgress);
        printf(szClusterTipsWiped, context->m_szShowName[0], context->m_uPasses);
    }

    listDestroy(&plDirectories);
    return bCompleted;
}

bool
wipeUnusedSpace(EraserContext *context)
{
    char szShowName[iShowNameLength + 1];

    // set show name
    memset(szShowName, 0, iShowNameLength + 1);
    _snprintf(szShowName, iShowNameLength + 1, szDriveShowName, context->m_szFileName[0]);

    context->m_szShowName = (const char*)szShowName;

    // wipe cluster tips (and handle progress bar)
    if (!context->m_bNoClusterTips) {
        wipeClusterTipsOnDrive(context);
    }

    // wipe free space
    return wipeFreeSpace(context);
}

static inline bool
removeDirectories(List **root, bool bShowErrors)
{
    if (root != 0 && *root != 0) {
        bool bCompleted = true;
        List *plCurrent = *root;

        int nLength = 0;
        char szScrambledName[_MAX_PATH];
        memset(szScrambledName, 0, _MAX_PATH);

        do {
            nLength = strlen(plCurrent->szData);
            if (nLength > 0 && nLength <= _MAX_PATH) {
                if (plCurrent->szData[nLength - 1] == '\\') {
                    plCurrent->szData[nLength - 1] = 0;
                    nLength--;
                }
            }

            if (nLength > _MAX_DRIVE) {
                overwriteFileName(plCurrent->szData, szScrambledName);

                // funny, this returns -1 under Windows even though it works...
                if (_rmdir((const char*)szScrambledName) == -1) {
                    if (bShowErrors) {
                        printf(szRmdirFailed, plCurrent->szData);
                    }
                    rename(szScrambledName, plCurrent->szData);
                    bCompleted = false;
                }
            }
            plCurrent = plCurrent->plNext;
        } while (plCurrent != 0);

        return bCompleted;
    } else {
        return false;
    }
}

bool
wipeFolder(EraserContext *context)
{
    bool bCompleted = false;
    List *plCurrent = 0;
    List *plDirectories = 0;

    char szPath[_MAX_PATH];
    memset(szPath, 0, _MAX_PATH);

    strncpy(szPath, context->m_szFileName, _MAX_PATH);
    if (szPath[strlen(szPath) - 1] != '\\') {
        strcat(szPath, "\\");
    }

    // add the root directory
    listAdd(&plDirectories, (const char*)szPath);

    // add subdirectories
    if (context->m_bSubFolders) {
        parseDirectories((const char*)szPath, &plDirectories);
    }

    // don't print an error if no files are found
    context->m_bNoMatchError = true;

    // start from the beginning
    plCurrent = plDirectories;

    do {
        // wipe all files
        strncpy(szPath, plCurrent->szData, _MAX_PATH);
        strcat(szPath, "*.*");

        context->m_szFileName = (const char*)szPath;
        bCompleted = wipeMatchingFiles(context);

        if (!bCompleted) {
            break;
        }

        plCurrent = plCurrent->plNext;
    } while (plCurrent != 0);

    if (bCompleted) {
        if (context->m_bKeepFolder) {
            listRemoveLast(&plDirectories);
        }
        // remove directories
        bCompleted =
            removeDirectories(&plDirectories, !context->m_bSilent);
    }

    listDestroy(&plDirectories);
    return bCompleted;
}

bool
wipeAllFiles(EraserContext *context)
{
    context->m_bSubFolders = true;
    context->m_bKeepFolder = true;
    return wipeFolder(context);
}

bool
wipeMatchingFiles(EraserContext *context)
{
    if (context == 0) {
        return false;
    }

    bool bCompleted = true;
    char szCurrentFile[_MAX_PATH];
    char szScrambledName[_MAX_PATH];
    char szPath[_MAX_PATH];
    char szDrive[_MAX_DRIVE];
    char szDir[_MAX_DIR];
    E_UINT16 uAttrib = 0;
    _find_t ft;

    memset(szCurrentFile, 0, _MAX_PATH);
    memset(szScrambledName, 0, _MAX_PATH);
    memset(szPath, 0, _MAX_PATH);
    memset(szDrive, 0, _MAX_DRIVE);
    memset(szDir, 0, _MAX_DIR);
    memset(&ft, 0, sizeof(_find_t));

    // find out the base directory (if any)
    _splitpath(context->m_szFileName, szDrive, szDir, NULL, NULL);
    strcpy(szPath, szDrive);
    strcat(szPath, szDir);

    // search files matching the description and erase them
    if (_dos_findfirst(context->m_szFileName,
            _A_ARCH | _A_HIDDEN | _A_NORMAL | _A_SYSTEM | _A_RDONLY, &ft) == 0) {
        do {
            if (context->m_bStop) {
                bCompleted = false;
                break;
            }

            strncpy(szCurrentFile, szPath, _MAX_PATH);
            strcat(szCurrentFile, ft.name);
            // set filenames
            context->m_szFileName = (const char*)szCurrentFile;
            context->m_szShowName = (const char*)ft.name;
            // save file attributes in case we need to restore them
            _dos_getfileattr(context->m_szFileName, &uAttrib);
            // set file attributes to normal
            _dos_setfileattr(context->m_szFileName, _A_NORMAL);

            if (!wipeFile(context)) {
                if (!context->m_bSilent) {
                    printf(szFailed, context->m_szFileName);
                }
                bCompleted = false;
            } else if (!context->m_bNoDelete) {
                // reset date and clear the filename
                resetDate(context->m_szFileName);
                overwriteFileName(context->m_szFileName, szScrambledName);
                // finally, delete the file
                if (_unlink(szScrambledName) == 0) {
                    if (!context->m_bSilent) {
                        printf(szSucceeded, context->m_szFileName, context->m_uPasses);
                    }
                } else {
                    if (!context->m_bSilent) {
                        printf(szWiped, context->m_uPasses, context->m_szFileName);
                    }
                    rename(szScrambledName, context->m_szFileName);
                    bCompleted = false;
                }
            } else {
                // restore file attributes
                _dos_setfileattr(context->m_szFileName, uAttrib);
                if (!context->m_bSilent) {
                    printf(szSucceeded, context->m_szFileName, context->m_uPasses);
                }
            }
        } while (_dos_findnext(&ft) == 0);
    } else {
        // no files matched the search string
        if (!context->m_bNoMatchError) {
            if (!context->m_bSilent) {
                printf(szNoSuchFile, context->m_szFileName);
            }
            bCompleted = false;
        }
    }

    return bCompleted;
}
