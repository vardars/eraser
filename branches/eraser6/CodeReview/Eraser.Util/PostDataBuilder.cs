/* 
 * $Id$
 * Copyright 2008-2009 The Eraser Project
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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Eraser.Util
{
	class MultipartFormDataBuilder
	{
		public MultipartFormDataBuilder()
		{
			FileName = Path.GetTempFileName();
		}

		public void AddPart(FormField field)
		{
			//Generate a random part boundary
			if (Boundary == null)
			{
				Random rand = new Random();
				for (int i = 0, j = 20 + rand.Next(40); i < j; ++i)
					Boundary += ValidBoundaryChars[rand.Next(ValidBoundaryChars.Length)];
			}

			using (FileStream stream = new FileStream(FileName, FileMode.Open, FileAccess.Write,
				FileShare.Read))
			{
				//Append data!
				stream.Seek(0, SeekOrigin.End);

				StringBuilder currentBoundary = new StringBuilder();
				currentBoundary.AppendFormat("--{0}\r\n", Boundary);
				if (field is FormFileField)
				{
					currentBoundary.AppendFormat(
						"Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"\r\n",
						field.FieldName, ((FormFileField)field).FileName);
					currentBoundary.AppendLine("Content-Type: application/octet-stream");
				}
				else
				{
					currentBoundary.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n",
						field.FieldName);
				}

				currentBoundary.AppendLine();
				byte[] boundary = Encoding.UTF8.GetBytes(currentBoundary.ToString());
				stream.Write(boundary, 0, boundary.Length);

				int lastRead = 0;
				byte[] buffer = new byte[524288];
				while ((lastRead = field.Stream.Read(buffer, 0, buffer.Length)) != 0)
					stream.Write(buffer, 0, lastRead);

				currentBoundary = new StringBuilder();
				currentBoundary.AppendFormat("\r\n--{0}--\r\n", Boundary);
				boundary = Encoding.UTF8.GetBytes(currentBoundary.ToString());
				stream.Write(boundary, 0, boundary.Length);
			}
		}

		/// <summary>
		/// Gets a stream with which to read the data from.
		/// </summary>
		public Stream Stream
		{
			get
			{
				return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
		}

		/// <summary>
		/// The Multipart/Form-Data boundary in use. If this is NULL, WritePostData will generate one
		/// and store it here.
		/// </summary>
		public string Boundary
		{
			get;
			set;
		}

		private string FileName;

		/// <summary>
		/// Characters valid for use in the multipart boundary.
		/// </summary>
		private static readonly string ValidBoundaryChars =
			"0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
	}

	class FormField
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="stream">The stream containing the field data.</param>
		public FormField(string fieldName, Stream stream)
		{
			FieldName = fieldName;
			Stream = stream;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="stream">The content of the field.</param>
		public FormField(string fieldName, string content)
			: this(fieldName, new MemoryStream(Encoding.UTF8.GetBytes(content)))
		{
		}

		/// <summary>
		/// The name of the field.
		/// </summary>
		public string FieldName;

		/// <summary>
		/// The stream containing the data for this field.
		/// </summary>
		public Stream Stream;
	}

	class FormFileField : FormField
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fieldName">The name of the form field.</param>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="stream">The stream containing the field data.</param>
		public FormFileField(string fieldName, string fileName, Stream stream)
			: base(fieldName, stream)
		{
			FileName = fileName;
		}

		/// <summary>
		/// The name of the file.
		/// </summary>
		public string FileName;
	}
}
