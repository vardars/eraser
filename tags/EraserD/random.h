// Random.h
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

#ifndef RANDOM_H
#define RANDOM_H

#define TWOEXPMINUS32  2.3283064365386963e-10
// CRC polynomial used to initialize additional congruential prng
#define POLY            0x04c11db7          // remember PGP 2.x?
// lags used for additive congruential prng - see Knuth, vol 2
#define ADDITIVE_KK     55                  // the long lag
#define ADDITIVE_LL     24                  // the short lag
#define PRNG_STATE_SIZE ((ADDITIVE_KK + 1) * sizeof(E_UINT32))

extern E_UINT32 uRandomArray[ADDITIVE_KK];

void ACSeed();
void ACSeed(E_UINT32);
void ACSeed(E_PUINT32);
void ACSave(E_PUINT32);

inline void ACFill(E_PUINT32 puBuffer, E_UINT32 uSize)
{
    E_UINT32  uUSize = uSize / sizeof(E_UINT32);
    E_PUINT32 p3     = (E_PUINT32)(puBuffer + ADDITIVE_KK);
    E_PUINT32 p2     = (E_PUINT32)(puBuffer + ADDITIVE_LL);
    E_PUINT32 p1     = (E_PUINT32)(puBuffer + uUSize);

    do {
        *--p1 = *--p2 + *--p3;
    } while (p2 > puBuffer);

    p2 = (E_PUINT32)(puBuffer + uUSize);

    do {
        *--p1 = *--p2 + *--p3;
    } while (p3 > puBuffer);

    p3 = (E_PUINT32)(puBuffer + uUSize);

    do {
        *--p1 = *--p2 + *--p3;
    } while (p1 > puBuffer);
}

inline double ACRandom()
{
    static E_UINT16 uIndex = 0;

    if (uIndex >= ADDITIVE_KK) {
        ACFill(uRandomArray, ADDITIVE_KK * sizeof(E_UINT32));
        uIndex = 0;
    }

    return (double)(uRandomArray[uIndex++] * TWOEXPMINUS32);
}

#endif
