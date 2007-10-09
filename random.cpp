// Random.cpp
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

#include <time.h>
#include <process.h>
#include <memory.h>

#include "EraserD.h"
#include "Random.h"

E_UINT32 uRandomArray[ADDITIVE_KK];

void ACSeed()
{
    time_t ttTime = time(NULL);
    tm * tmTime = gmtime(&ttTime);

    ACSeed(ttTime ^ tmTime->tm_wday ^ tmTime->tm_sec ^ tmTime->tm_min ^
           tmTime->tm_hour ^ getpid());
}

void ACSeed(E_UINT32 uSeed)
{
    E_UINT16  uCount;
    E_PUINT32 p1;

    // initialize first ADDITIVE_KK words of buf with pseudo-random stuff
    p1 = (E_PUINT32) uRandomArray + ADDITIVE_KK;

    do {
        for (uCount = 0; uCount < 32; uCount++) {
            uSeed = (uSeed & 0x80000000) ? (uSeed << 1 ^ POLY) : (uSeed << 1);
        }

        *--p1 = uSeed;
    } while (p1 > (E_PUINT32) uRandomArray);
}

void ACSeed(E_PUINT32 puBuffer)
{
    memcpy(puBuffer, uRandomArray, ADDITIVE_KK * sizeof(E_UINT32));
}

void ACSave(E_PUINT32 puBuffer)
{
    memcpy(uRandomArray, puBuffer, ADDITIVE_KK * sizeof(E_UINT32));
}
