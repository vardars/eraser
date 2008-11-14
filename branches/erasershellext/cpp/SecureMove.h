#pragma once

#pragma unmanaged
#pragma managed

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace System::IO

using namespace Eraser::Manager;
using namespace Eraser::Util;

#define ERASER_SECUREMOVE_CLUSTERS		32

// open the source a file
// create the target file
// read from ($source) => ($target)

ref class SecureMove
{
private: 
	FileStream ^m_src;
	FileStream ^m_dst;
public:
	SecureMove(String ^source, String ^destination)
	{
		m_src = gcnew FileStream(source, FileAccess::Read, true);
		m_dst = gcnew FileStream(source, FileAccess::Write, true);
	}

	int DoJob()
	{
		array<Byte> ^buffer = gcnew array<char>(ERASER_SECUREMOVE_BUFFER_SIZE);
		Long read;

		do
		{
			read = m_src->Read(buffer, 0, ERASER_SECUREMOVE_BUFFER_SIZE);
			m_dst->Write(buffer, 0, read);
		} while(read);
	}

};
