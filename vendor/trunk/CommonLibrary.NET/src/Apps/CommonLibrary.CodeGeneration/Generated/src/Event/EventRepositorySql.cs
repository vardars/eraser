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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using ComLib.Entities;
using ComLib.Database;
using ComLib.LocationSupport;


namespace CommonLibrary.WebModules.Events
{
    /// <summary>
    /// Generic repository for persisting Event.
    /// </summary>
    public partial class EventRepository : RepositorySql<Event>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedQueryRepository"/> class.
        /// </summary>
        public EventRepository() { }


        /// <summary>
        /// Initializes a new instance of the <see cref="Repository&lt;TId, T&gt;"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="helper">The helper.</param>
        public EventRepository(ConnectionInfo connectionInfo) : base(connectionInfo)
        {            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Repository&lt;TId, T&gt;"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="helper">The helper.</param>
        public EventRepository(ConnectionInfo connectionInfo, IDBHelper helper)
            : base(connectionInfo, helper)
        {
        }


        /// <summary>
        /// Initialize the rowmapper
        /// </summary>
        public override void Init(ConnectionInfo connectionInfo, IDBHelper helper)
        {
            base.Init(connectionInfo, helper);
            this.RowMapper = new EventRowMapper();
        }


        /// <summary>
        /// Create the entity using sql.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Event Create(Event entity)
        {
            string sql = "insert into Events ( CreateDate, UpdateDate, CreateUser, UpdateUser, UpdateComment, Version" + 
			", IsActive, Title, Summary, Description, StartDate" + 
			", EndDate, StartTime, EndTime, Email, Phone" + 
			", Url, Keywords, Street, City, State" + 
			", Country, Zip, CityId, StateId, CountryId" + 
			", IsOnline, AverageRating, TotalLiked, TotalDisLiked, TotalBookMarked" + 
			", TotalAbuseReports) " + 
 "VALUES (" + "'" + entity.CreateDate.ToString("yyyy-MM-dd") + "'" + "," + "'" + entity.UpdateDate.ToString("yyyy-MM-dd") + "'" + "," + "'" + DataUtils.Encode(entity.CreateUser) + "'" + "," + "'" + DataUtils.Encode(entity.UpdateUser) + "'" + "," + "'" + DataUtils.Encode(entity.UpdateComment) + "'"
			 + "," +  entity.Version  + "," +  Convert.ToSByte(entity.IsActive)  + "," + "'" + DataUtils.Encode(entity.Title) + "'" + "," + "'" + DataUtils.Encode(entity.Summary) + "'"
			 + "," + "'" + DataUtils.Encode(entity.Description) + "'" + "," + "'" + entity.StartDate.ToString("yyyy-MM-dd") + "'" + "," + "'" + entity.EndDate.ToString("yyyy-MM-dd") + "'" + "," +  entity.StartTime 
			 + "," +  entity.EndTime  + "," + "'" + DataUtils.Encode(entity.Email) + "'" + "," + "'" + DataUtils.Encode(entity.Phone) + "'" + "," + "'" + DataUtils.Encode(entity.Url) + "'"
			 + "," + "'" + DataUtils.Encode(entity.Keywords) + "'" + "," + "'" + DataUtils.Encode(entity.Address.Street) + "'" + "," + "'" + DataUtils.Encode(entity.Address.City) + "'" + "," + "'" + DataUtils.Encode(entity.Address.State) + "'"
			 + "," + "'" + DataUtils.Encode(entity.Address.Country) + "'" + "," + "'" + DataUtils.Encode(entity.Address.Zip) + "'" + "," +  entity.Address.CityId  + "," +  entity.Address.StateId 
			 + "," +  entity.Address.CountryId  + "," +  Convert.ToSByte(entity.Address.IsOnline)  + "," +  entity.AverageRating  + "," +  entity.TotalLiked 
			 + "," +  entity.TotalDisLiked  + "," +  entity.TotalBookMarked  + "," +  entity.TotalAbuseReports + ");select scope_identity();";
            object result = _db.ExecuteScalarText(sql, null);
            entity.Id = Convert.ToInt32(result);
            return entity;
        }


        /// <summary>
        /// Update the entity using sql.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override Event Update(Event entity)
        {
            string sql = "update Events set CreateDate = " + "'" + entity.CreateDate.ToString("yyyy-MM-dd") + "'" + ", UpdateDate = " + "'" + entity.UpdateDate.ToString("yyyy-MM-dd") + "'" + ", CreateUser = " + "'" + DataUtils.Encode(entity.CreateUser) + "'" + ", UpdateUser = " + "'" + DataUtils.Encode(entity.UpdateUser) + "'" + ", UpdateComment = " + "'" + DataUtils.Encode(entity.UpdateComment) + "'" + ", Version = " +  entity.Version  + 
			", IsActive = " +  Convert.ToSByte(entity.IsActive)  + ", Title = " + "'" + DataUtils.Encode(entity.Title) + "'" + ", Summary = " + "'" + DataUtils.Encode(entity.Summary) + "'" + ", Description = " + "'" + DataUtils.Encode(entity.Description) + "'" + ", StartDate = " + "'" + entity.StartDate.ToString("yyyy-MM-dd") + "'" + 
			", EndDate = " + "'" + entity.EndDate.ToString("yyyy-MM-dd") + "'" + ", StartTime = " +  entity.StartTime  + ", EndTime = " +  entity.EndTime  + ", Email = " + "'" + DataUtils.Encode(entity.Email) + "'" + ", Phone = " + "'" + DataUtils.Encode(entity.Phone) + "'" + 
			", Url = " + "'" + DataUtils.Encode(entity.Url) + "'" + ", Keywords = " + "'" + DataUtils.Encode(entity.Keywords) + "'" + ", Street = " + "'" + DataUtils.Encode(entity.Address.Street) + "'" + ", City = " + "'" + DataUtils.Encode(entity.Address.City) + "'" + ", State = " + "'" + DataUtils.Encode(entity.Address.State) + "'" + 
			", Country = " + "'" + DataUtils.Encode(entity.Address.Country) + "'" + ", Zip = " + "'" + DataUtils.Encode(entity.Address.Zip) + "'" + ", CityId = " +  entity.Address.CityId  + ", StateId = " +  entity.Address.StateId  + ", CountryId = " +  entity.Address.CountryId  + 
			", IsOnline = " +  Convert.ToSByte(entity.Address.IsOnline)  + ", AverageRating = " +  entity.AverageRating  + ", TotalLiked = " +  entity.TotalLiked  + ", TotalDisLiked = " +  entity.TotalDisLiked  + ", TotalBookMarked = " +  entity.TotalBookMarked  + 
			", TotalAbuseReports = " +  entity.TotalAbuseReports  +  " where Id = " + entity.Id;;
            _db.ExecuteNonQueryText(sql, null);
            return entity;
        }
    }


    
    /// <summary>
    /// RowMapper for Event.
    /// </summary>
    /// <typeparam name="?"></typeparam>
    public partial class EventRowMapper : EntityRowMapper<Event>, IEntityRowMapper<Event>
    {
        public override Event MapRow(IDataReader reader, int rowNumber)
        {
            Event entity = Events.New();
            			entity.Id = reader["Id"] == DBNull.Value ? 0 : (int)reader["Id"];
			entity.CreateDate = reader["CreateDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["CreateDate"];
			entity.UpdateDate = reader["UpdateDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["UpdateDate"];
			entity.CreateUser = reader["CreateUser"] == DBNull.Value ? string.Empty : reader["CreateUser"].ToString();
			entity.UpdateUser = reader["UpdateUser"] == DBNull.Value ? string.Empty : reader["UpdateUser"].ToString();
			entity.UpdateComment = reader["UpdateComment"] == DBNull.Value ? string.Empty : reader["UpdateComment"].ToString();
			entity.Version = reader["Version"] == DBNull.Value ? 0 : (int)reader["Version"];
			entity.IsActive = reader["IsActive"] == DBNull.Value ? false : (bool)reader["IsActive"];
			entity.Title = reader["Title"] == DBNull.Value ? string.Empty : reader["Title"].ToString();
			entity.Summary = reader["Summary"] == DBNull.Value ? string.Empty : reader["Summary"].ToString();
			entity.Description = reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString();
			entity.StartDate = reader["StartDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["StartDate"];
			entity.EndDate = reader["EndDate"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["EndDate"];
			entity.StartTime = reader["StartTime"] == DBNull.Value ? 0 : (int)reader["StartTime"];
			entity.EndTime = reader["EndTime"] == DBNull.Value ? 0 : (int)reader["EndTime"];
			entity.Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString();
			entity.Phone = reader["Phone"] == DBNull.Value ? string.Empty : reader["Phone"].ToString();
			entity.Url = reader["Url"] == DBNull.Value ? string.Empty : reader["Url"].ToString();
			entity.Keywords = reader["Keywords"] == DBNull.Value ? string.Empty : reader["Keywords"].ToString();
			entity.AverageRating = reader["AverageRating"] == DBNull.Value ? 0 : Convert.ToDouble(reader["AverageRating"]);
			entity.TotalLiked = reader["TotalLiked"] == DBNull.Value ? 0 : (int)reader["TotalLiked"];
			entity.TotalDisLiked = reader["TotalDisLiked"] == DBNull.Value ? 0 : (int)reader["TotalDisLiked"];
			entity.TotalBookMarked = reader["TotalBookMarked"] == DBNull.Value ? 0 : (int)reader["TotalBookMarked"];
			entity.TotalAbuseReports = reader["TotalAbuseReports"] == DBNull.Value ? 0 : (int)reader["TotalAbuseReports"];
entity.Address = new Address();
			entity.Address.Street = reader["Street"] == DBNull.Value ? string.Empty : reader["Street"].ToString();
			entity.Address.City = reader["City"] == DBNull.Value ? string.Empty : reader["City"].ToString();
			entity.Address.State = reader["State"] == DBNull.Value ? string.Empty : reader["State"].ToString();
			entity.Address.Country = reader["Country"] == DBNull.Value ? string.Empty : reader["Country"].ToString();
			entity.Address.Zip = reader["Zip"] == DBNull.Value ? string.Empty : reader["Zip"].ToString();
			entity.Address.CityId = reader["CityId"] == DBNull.Value ? 0 : (int)reader["CityId"];
			entity.Address.StateId = reader["StateId"] == DBNull.Value ? 0 : (int)reader["StateId"];
			entity.Address.CountryId = reader["CountryId"] == DBNull.Value ? 0 : (int)reader["CountryId"];
			entity.Address.IsOnline = reader["IsOnline"] == DBNull.Value ? false : (bool)reader["IsOnline"];

            return entity;
        }
    }
}