// list.cpp
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
#include <string.h>
#include "EraserD.h"
#include "List.h"

bool
listCreate(List **root)
{
    if (root != 0) {
        *root = (List*)malloc(sizeof(List));

        if (*root != NULL) {
            memset(*root, 0, sizeof(List));
            return true;
        }
        *root = 0;
    }
    return false;
}

bool
listAdd(List **root, const char* data)
{
    if (root != 0) {
        List *plNew = 0;
        if (listCreate(&plNew)) {
            strncpy(plNew->szData, data, _MAX_PATH);
            plNew->plNext = *root;
            *root = plNew;
            return true;
        }
    }
    return false;
}

bool
listDestroy(List **root)
{
    if (root != 0 && *root != 0) {
        List *plNext = *root;
        List *plTmp = 0;

        do {
            plTmp = plNext->plNext;
            free(plNext);
            plNext = plTmp;
        } while (plNext != 0);

        return true;
    }
    return false;
}

bool
listRemoveLast(List **root)
{
    if (root != 0 && *root != 0) {
        List *plLastValid = *root;
        List *plLastParent = 0;

        while (plLastValid->plNext) {
            plLastParent = plLastValid;
            plLastValid = plLastParent->plNext;
        }

        free(plLastValid);

        if (plLastParent) {
            plLastParent->plNext = 0;
        } else {
            *root = 0;
        }

        return true;
    }
    return false;

}