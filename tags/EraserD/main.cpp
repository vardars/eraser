// main.cpp
//
// Eraser. Secure data removal. For DOS.
// Copyright © 1997-2001  Sami Tolvanen (sami@tolvanen.com).
// Copyright © 2002  Garrett Trant (gtrant@heidi.ie).
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
#include <signal.h>
#include <assert.h>

#include "EraserD.h"
#include "Wipe.h"
#include "Random.h"
#include "..\version.h"

// context
static EraserContext context;

static void
printUsage(FILE *fOut)
{
    fprintf(fOut, szCopyright, VERSION_NUMBER_STRING);
    fprintf(fOut, "\n");
    fprintf(fOut, szUsage);
}

static void
signalHandler(int signal)
{
    if (signal == SIGINT || signal == SIGABRT) {
        context.m_bStop = true;
    }
}

int
main(int argc, char **argv)
{
    // variables
    bool bCompleted = false;
    bool bIncorrectParameter = false;

    // data
    E_UINT32 uPasses = 1;
    EraserAction eaAction = NoAction;

    // initialize context
    memset(&context, 0, sizeof(EraserContext));

    // parse command line
    if (argc > 1) {
        for (int i = 1; i < argc; i++) {
            if (!_stricmp(argv[i], szHelp1) ||
                !_stricmp(argv[i], szHelp2) ||
                !_stricmp(argv[i], szHelp3)) {
                // show usage and copyright
                printUsage(stdout);
                return EXIT_SUCCESS;
            } else if (_stricmp(argv[i], szFile) == 0) {
                // files (wildcard search)
                if (i > (argc - 2)) {
                    bIncorrectParameter = true;
                } else {
                    i++;
                    context.m_szFileName = argv[i];
                    eaAction = File;
                }
            } else if (_stricmp(argv[i], szFolder) == 0) {
                // directory
                if (i > (argc - 2)) {
                    bIncorrectParameter = true;
                } else {
                    i++;
                    context.m_szFileName = argv[i];
                    if (strlen(context.m_szFileName) <= _MAX_DRIVE) {
                        bIncorrectParameter = true;
                    } else {
                        eaAction = Folder;
                    }
                }
            } else if (_stricmp(argv[i], szDisk) == 0) {
                // unused disk space & cluster tips
                if (i > (argc - 2)) {
                    bIncorrectParameter = true;
                } else {
                    i++;
                    context.m_szFileName = argv[i];
                    eaAction = Disk;
                }
            } else if (_stricmp(argv[i], szAllFiles) == 0) {
                // all files on drive
                if (i > (argc - 2)) {
                    bIncorrectParameter = true;
                } else {
                    i++;
                    context.m_szFileName = argv[i];
                    if (strlen(context.m_szFileName) > _MAX_DRIVE) {
                        bIncorrectParameter = true;
                    } else {
                        eaAction = AllFiles;
                    }
                }
            } else if (_stricmp(argv[i], szPasses) == 0) {
                // amount of passes
                if (i > (argc - 2)) {
                    bIncorrectParameter = true;
                } else {
                    i++;
                    char *sztmp = 0;
                    uPasses = (E_UINT32)strtoul(argv[i], &sztmp, 10);

                    if (*sztmp != '\0') {
                        bIncorrectParameter = true;
                    }
                }
            } else if (_stricmp(argv[i], szSilent) == 0) {
                context.m_bSilent = true;
            } else if (_stricmp(argv[i], szNoDelete) == 0) {
                context.m_bNoDelete = true;
            } else if (_stricmp(argv[i], szSubFolders) == 0) {
                context.m_bSubFolders = true;
            } else if (_stricmp(argv[i], szKeepFolder) == 0) {
                context.m_bKeepFolder = true;
            } else if (_stricmp(argv[i], szNoTips) == 0) {
                context.m_bNoClusterTips = true;
            } else {
                bIncorrectParameter = true;
                break;
            }
        }

        // check for wacky combinations
        if (eaAction == NoAction ||
            (context.m_bNoDelete && eaAction != File) ||
            ((context.m_bSubFolders || context.m_bKeepFolder) && eaAction != Folder) ||
            (context.m_bNoClusterTips && eaAction != Disk)) {
            bIncorrectParameter = true;
        }
    }

    // invalid command line parameters
    if (argc <= 1 || bIncorrectParameter || context.m_szFileName == 0) {
        printUsage(stderr);
        return EXIT_FAILURE;
    }

    // set signal handlers
    if (signal(SIGINT, signalHandler) == SIG_ERR ||
        signal(SIGABRT, signalHandler) == SIG_ERR) {
        if (!context.m_bSilent) {
            fprintf(stderr, szSignalFailed);
        }
    }

    // sanity check: 1-65535 passes
    if (uPasses < 1) {
        uPasses = 1;
    } else if (uPasses > (E_UINT16)-1) {
        // although I wouldn't call this sane...
        uPasses = (E_UINT16)-1;
    }

    printf(szCopyright, VERSION_NUMBER_STRING);
    printf("\n");

    // seed the prng
    ACSeed();

    // allocate the buffer
    context.m_uPasses = (E_UINT16)uPasses;
    context.m_uBufferSize = ERASER_BUFFER_SIZE;
    context.m_puBuffer = (E_PUINT32)malloc(context.m_uBufferSize);

    // ouch!
    if (context.m_puBuffer == NULL) {
        if (!context.m_bSilent) {
            fprintf(stderr, szOutOfMemory);
        }
        return EXIT_FAILURE;
    }

    // perform overwriting
    if (eaAction == File) {
        bCompleted = wipeMatchingFiles(&context);
    } else if (eaAction == AllFiles) {
        // confirmation
        if (!context.m_bSilent) {
            printf(szEraserAllConfirmation, context.m_szFileName[0]);
            int iResponse = fgetc(stdin);
            if (iResponse != EOF && toupper((char)iResponse) == 'Y') {
                bCompleted = wipeAllFiles(&context);
            } else {
                printf(szOperationCanceled);
            }
        } else {
            // I hope the user knows what she is doing...
            bCompleted = wipeAllFiles(&context);
        }
    } else if (eaAction == Folder) {
        bCompleted = wipeFolder(&context);
    } else if (eaAction == Disk) {
        bCompleted = wipeUnusedSpace(&context);
    } else {
        bCompleted = false;
    }

    // free the memory
    if (context.m_puBuffer != NULL) {
        free(context.m_puBuffer);
        context.m_puBuffer = NULL;
    }

    if (context.m_bStop && !context.m_bSilent) {
        fprintf(stderr, szTerminated);
    }

    if (!bCompleted) {
        return EXIT_FAILURE;
    } else {
        return EXIT_SUCCESS;
    }
}
