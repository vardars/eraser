/*
 * Author: Justin Dial
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
using System.Data.Linq.Mapping;

namespace ComLib.Logging
{
    [Table(Name = "event_log")]
    public class LogEventEntity
    {
        [Column(Name = "application", CanBeNull = true, DbType = "VarChar(255)")] 
        public string Application;

        [Column(Name = "computer", CanBeNull = true, DbType = "NVarChar(255)")] 
        public string Computer;

        [Column(Name = "exception", CanBeNull = true, DbType = "NVarChar(255)")] 
        public string Exception;

        [Column(Name = "id", DbType = "Int", CanBeNull = false, IsPrimaryKey = true, IsDbGenerated = true)] 
        public int Id;

        [Column(Name = "log_level", DbType = "Int")] 
        public LogLevel LogLevel;

        [Column(Name = "message", CanBeNull = true, DbType = "VarChar(255)")] 
        public string Message;

        [Column(Name = "user_name", CanBeNull = true, DbType = "NVarChar(255)")] 
        public string UserName;
    }
}