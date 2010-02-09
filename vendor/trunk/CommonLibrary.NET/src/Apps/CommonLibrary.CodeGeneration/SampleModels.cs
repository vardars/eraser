using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ComLib;
using ComLib.Models;
using ComLib.Database;

namespace CommonLibrary.CodeGeneration
{
    public class SampleModels
    {
        public static ModelContainer GetModelContainer()
        {
            // Settings for the Code model builders.
            ModelBuilderSettings settings = new ModelBuilderSettings()
            {
                ModelCodeLocation = @"..\..\Generated\src",
                ModelInstallLocation = @"..\..\Generated\install",
                ModelCodeLocationTemplate = @"..\..\..\..\lib\CommonLibrary.NET\CodeGen\Templates\Default",
                ModelDbStoredProcTemplates = @"..\..\..\..\lib\CommonLibrary.NET\CodeGen\Templates\DefaultSql",
                DbAction_Create = DbCreateType.Create,
                Connection = new ConnectionInfo("Server=mypc1;Database=testdata1;User=user1;Password=password;", "System.Data.SqlClient"),
                AssemblyName = "CommonLibrary.Extensions"
            };

            ModelContainer models = new ModelContainer()
            {
                Settings = settings,
                ExtendedSettings = new Dictionary<string, object>() { },
                
                // Model definition.
                AllModels = new List<Model>()
                {
                    new Model("ModelBase")
                    {
                        NameSpace = "CommonLibrary.WebModules",
                        GenerateCode = false,
                        GenerateTable = false,
                        GenerateOrMap = false,
                        PropertiesSortOrder = 1,
                        Properties = new List<PropertyInfo>()
                        {
                            new PropertyInfo( "Id",            typeof(int)      ) { IsRequired = true, ColumnName = "Id", IsKey = true },
                            new PropertyInfo( "CreateDate",    typeof(DateTime) ) { IsRequired = true },
                            new PropertyInfo( "UpdateDate",    typeof(DateTime) ) { IsRequired = true },
                            new PropertyInfo( "CreateUser",    typeof(string)   ) { IsRequired = true, MaxLength = "20" },
                            new PropertyInfo( "UpdateUser",    typeof(string)   ) { IsRequired = true, MaxLength = "20" },
                            new PropertyInfo( "UpdateComment", typeof(string)   ) { IsRequired = false, MaxLength = "150" },
                            new PropertyInfo( "Version",       typeof(int)      ) { IsRequired = true, DefaultValue = 1 },
                            new PropertyInfo( "IsActive",      typeof(bool)     ) { IsRequired = true, DefaultValue = 0 }
                        }
                    }, 
                    new Model("RatingPostBase")
                    {
                        NameSpace = "CommonLibrary.WebModules",
                        GenerateCode = false,
                        GenerateTable = false,
                        GenerateOrMap = false,
                        PropertiesSortOrder = 100,
                        Properties = new List<PropertyInfo>()
                        {
                            new PropertyInfo( "AverageRating",     typeof(double) ) { IsRequired = false },
                            new PropertyInfo( "TotalLiked",        typeof(int)    ),
                            new PropertyInfo( "TotalDisLiked",     typeof(int)    ),
                            new PropertyInfo( "TotalBookMarked",   typeof(int)    ),
                            new PropertyInfo( "TotalAbuseReports", typeof(int)    )
                        }
                    },
                    new Model("Address")
                    {
                        Properties = new List<PropertyInfo>()
                        {
                            new PropertyInfo( "Street",    typeof(string)) { MaxLength = "40" },
                            new PropertyInfo( "City",      typeof(string)) { MaxLength = "40" },
                            new PropertyInfo( "State",     typeof(string)) { MaxLength = "20" },
                            new PropertyInfo( "Country",   typeof(string)) { MaxLength = "20", DefaultValue = "U.S." },
                            new PropertyInfo( "Zip",       typeof(string)) { MaxLength = "10" },
                            new PropertyInfo( "CityId",    typeof(int) )   {  },
                            new PropertyInfo( "StateId",   typeof(int) )   {  },
                            new PropertyInfo( "CountryId", typeof(int) )   {  },
                            new PropertyInfo( "IsOnline",  typeof(bool))   { DefaultValue = false }
                        }
                    },
                    new Model("Event")
                    {
                        TableName = "Events",
                        NameSpace = "CommonLibrary.WebModules.Events",
                        GenerateCode = true, GenerateTests = false, 
                        GenerateUI = false, GenerateRestApi = false, GenerateFeeds = false, GenerateOrMap = false, GenerateTable = true,
                        PropertiesSortOrder = 50,
                        Inherits = "ModelBase",
                        Includes     = new List<Include>()         { new Include("RatingPostBase") },
                        ComposedOf   = new List<Composition>()     { new Composition("Address") },
                        RepositoryType = "RepositorySql",
                        ExcludeFiles = "Repository.cs,Feeds.cs,ImportExport.cs,Serializer.cs",
                        Properties = new List<PropertyInfo>()
                        {
                            new PropertyInfo("Title",       typeof(string))     { ColumnName = "Title", MinLength = "10", MaxLength = "150", IsRequired = true },
                            new PropertyInfo("Summary",     typeof(string))     { MaxLength = "200", IsRequired = true },
                            new PropertyInfo("Description", typeof(StringClob)) { MinLength = "10", MaxLength = "-1", IsRequired = true },
                            new PropertyInfo("StartDate",   typeof(DateTime))   { IsRequired = true },
                            new PropertyInfo("EndDate",     typeof(DateTime))   { IsRequired = true},
                            new PropertyInfo("StartTime",   typeof(int)),
                            new PropertyInfo("EndTime",     typeof(int)),
                            new PropertyInfo("Email",       typeof(string))     { IsRequired = false, MaxLength = "30",  RegEx = "RegexPatterns.Email", IsRegExConst = true },
                            new PropertyInfo("Phone",       typeof(string))     { IsRequired = false, MaxLength = "20",  RegEx = "RegexPatterns.PhoneUS", IsRegExConst = true },
                            new PropertyInfo("Url",         typeof(string))     { IsRequired = false, MaxLength = "150", RegEx = "RegexPatterns.Url", IsRegExConst = true },
                            new PropertyInfo("Keywords",    typeof(string))     { MaxLength = "100"}
                        },                        
                    }
                }            
            };
            return models;
        }        
    }
}
