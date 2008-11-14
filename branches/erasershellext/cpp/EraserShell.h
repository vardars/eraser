// EraserShell.h

#pragma once
#pragma managed

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace System::IO;

//using namespace Eraser;
//using namespace Eraser::Manager;
//using namespace Eraser::Util;

#define ERASER_SECUREMOVE_CLUSTERS		32

// open the source a file
// create the target file
// read from ($source) => ($target)
namespace EraserShell
{
	ref class SecureMove
	{
	private: 
		FileStream ^m_src;
		FileStream ^m_dst;
	public:
		SecureMove(String ^source, String ^destination)
		{
			m_src = gcnew FileStream(source, FileMode::Open, FileAccess::Read);
			m_dst = gcnew FileStream(source, FileMode::CreateNew, FileAccess::Write);
		}

		int DoWork()
		{
			array<Byte> ^buffer = gcnew array<Byte>(32);
			int read;

			do
			{
				read = m_src->Read(buffer, 0, 32);
				m_dst->Write(buffer, 0, read);
			} while(read);

			return 0;
		}

	};

}