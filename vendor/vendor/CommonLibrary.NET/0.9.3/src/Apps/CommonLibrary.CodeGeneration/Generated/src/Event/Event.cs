/*
 * Author: Kishore Reddy
 * Url: http://commonlibrarynet.codeplex.com/
 * Title: CommonLibrary.NET
 * Copyright: � 2009 Kishore Reddy
 * License: LGPL License
 * LicenseUrl: http://commonlibrarynet.codeplex.com/license
 * Description: A C# based .NET 3.5 Open-Source collection of reusable components.
 * Usage: Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ComLib.Entities;
using ComLib.LocationSupport;


namespace CommonLibrary.WebModules.Events
{
    /// <summary>
    /// Event entity.
    /// </summary>
    public partial class Event : DomainObject<Event>
    {
		/// <summary>
		/// Get/Set Title
		/// </summary>
		public string Title { get; set; }


		/// <summary>
		/// Get/Set Summary
		/// </summary>
		public string Summary { get; set; }


		/// <summary>
		/// Get/Set Description
		/// </summary>
		public string Description { get; set; }


		/// <summary>
		/// Get/Set StartDate
		/// </summary>
		public DateTime StartDate { get; set; }


		/// <summary>
		/// Get/Set EndDate
		/// </summary>
		public DateTime EndDate { get; set; }


		/// <summary>
		/// Get/Set StartTime
		/// </summary>
		public int StartTime { get; set; }


		/// <summary>
		/// Get/Set EndTime
		/// </summary>
		public int EndTime { get; set; }


		/// <summary>
		/// Get/Set Email
		/// </summary>
		public string Email { get; set; }


		/// <summary>
		/// Get/Set Phone
		/// </summary>
		public string Phone { get; set; }


		/// <summary>
		/// Get/Set Url
		/// </summary>
		public string Url { get; set; }


		/// <summary>
		/// Get/Set Keywords
		/// </summary>
		public string Keywords { get; set; }


		private Address _Address = new Address();
		/// <summary>
		/// Get/Set Address
		/// </summary>
		public Address Address
		 { 
		 get { return _Address;  }
		 set { _Address = value; }
		 }


		/// <summary>
		/// Get/Set AverageRating
		/// </summary>
		public double AverageRating { get; set; }


		/// <summary>
		/// Get/Set TotalLiked
		/// </summary>
		public int TotalLiked { get; set; }


		/// <summary>
		/// Get/Set TotalDisLiked
		/// </summary>
		public int TotalDisLiked { get; set; }


		/// <summary>
		/// Get/Set TotalBookMarked
		/// </summary>
		public int TotalBookMarked { get; set; }


		/// <summary>
		/// Get/Set TotalAbuseReports
		/// </summary>
		public int TotalAbuseReports { get; set; }



    }
}
