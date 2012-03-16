/*
 * Author: Nick Bitounis
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

namespace ComLib
{
    /// <summary>
    /// This class is used to store validation error messages.
    /// </summary>
    public class ValidationExtensionMessages
    {
        /// <summary>
        /// Message for text less than min length.
        /// Default is "Text supplied is less than min length ({0}).".
        /// </summary>
        public string TextLessThanMinLength { get; set; }


        /// <summary>
        /// Message for text more than max length.
        /// Default is "Text supplied is more than max length ({0}).".
        /// </summary>
        public string TextMoreThanMaxLength { get; set; }


        /// <summary>
        /// Message for text not matching pattern.
        /// Default is "Text supplied does not match expected pattern.".
        /// </summary>
        public string TextNotMatchPattern { get; set; }


        /// <summary>
        /// Message for text must be in values.
        /// Default is "Text must be in : {0}".
        /// </summary>
        public string TextMustBeIn { get; set; } 


        /// <summary>
        /// Message for invalid number.
        /// Default is "Text supplied is not a valid number.".
        /// </summary>
        public string TextNotValidNumber { get; set; }


        /// <summary>
        /// Message for not numeric text.
        /// Default is "Text supplied is not numeric.".
        /// </summary>
        public string TextNotNumeric { get; set; }


        /// <summary>
        /// Message for number less than.
        /// Default is "Number supplied is less than {0}.".
        /// </summary>
        public string NumberLessThan { get; set; }


        /// <summary>
        /// Message for number more than.
        /// Default is "Number supplied is more than {0}.".
        /// </summary>
        public string NumberMoreThan { get; set; }


        /// <summary>
        /// Message for text must contain only chars.
        /// Default is "Text supplied must only contain chars {0}".
        /// </summary>
        public string TextMustContainOnlyChars { get; set; }


        /// <summary>
        /// Message for text must contain only chars and numbers.
        /// Default is "Text supplied must only contain chars and numbers {0}".
        /// </summary>
        public string TextMustContainOnlyCharsAndNumbers { get; set; } 


        /// <summary>
        /// Message for invalid date.
        /// Default is "Text supplied is not a valid date".
        /// </summary>
        public string TextInvalidDate { get; set; }


        /// <summary>
        /// Message for date less than min date.
        /// Default is "Date supplied is less than minimum date {0}".
        /// </summary>
        public string DateLessThanMinDate { get; set; }


        /// <summary>
        /// Message for date more than max date.
        /// Default is "Date supplied is more than maximum date {0}".
        /// </summary>
        public string DateMoreThanMaxDate { get; set; }


        /// <summary>
        /// Message for invalid time.
        /// Default is "Text supplied is not a valid time.".
        /// </summary>
        public string TextInvalidTime { get; set; }


        /// <summary>
        /// Message for invalid us phone.
        /// Default is "Text supplied is not a valid US phone number.".
        /// </summary>
        public string TextInvalidUSPhone { get; set; }


        /// <summary>
        /// Message for invalid ssn.
        /// Default is "Text supplied is not a valid social security number.".
        /// </summary>
        public string TextInvalidSSN { get; set; }


        /// <summary>
        /// Message for invalid email.
        /// Default is "Text supplied is not a valid email.".
        /// </summary>
        public string TextInvalidEmail { get; set; }


        /// <summary>
        /// Message for invalid url.
        /// Default is "Text supplied is not a valid url.".
        /// </summary>
        public string TextInvalidUrl { get; set; }


        /// <summary>
        /// Message for invalid us zip code.
        /// Default is "Text supplied is not a valid US zipcode.".
        /// </summary>
        public string TextInvalidUSZip { get; set; } 


        /// <summary>
        /// Message for not equal objects.
        /// Default is "Objects supplied are not equal".
        /// </summary>
        public string ObjectsAreNotEqual { get; set; }


        /// <summary>
        /// Message for equal objects.
        /// Default is "Objects supplied are equal".
        /// </summary>
        public string ObjectsAreEqual { get; set; }


        /// <summary>
        /// Message for not supplied.
        /// Default is " is not supplied.".
        /// </summary>
        public string IsNotSupplied { get; set; }


        /// <summary>
        /// Default class constructor.
        /// </summary>
        /// <param name="TextLessThanMinLength">Message for text less than min length.</param>
        /// <param name="TextMoreThanMaxLength">Message for text more than max length..</param>
        /// <param name="TextNotMatchPattern">Message for text not matching pattern.</param>
        /// <param name="TextMustBeIn">Message for text must be in values.</param>
        /// <param name="TextNotValidNumber">Message for invalid number.</param>
        /// <param name="TextNotNumeric">Message for not numeric text.</param>
        /// <param name="NumberLessThan">Message for number less than.</param>
        /// <param name="NumberMoreThan">Message for number more than.</param>
        /// <param name="TextMustContainOnlyChars">Message for text must contain only chars.</param>
        /// <param name="TextMustContainOnlyCharsAndNumbers">Message for text must contain only chars and numbers.</param>
        /// <param name="TextInvalidDate">Message for invalid date.</param>
        /// <param name="DateLessThanMinDate">Message for date less than min date.</param>
        /// <param name="DateMoreThanMaxDate">Message for date more than max date.</param>
        /// <param name="TextInvalidTime">Message for invalid time.</param>
        /// <param name="TextInvalidUSPhone">Message for invalid us phone.</param>
        /// <param name="TextInvalidSSN">Message for invalid ssn.</param>
        /// <param name="TextInvalidEmail">Message for invalid email.</param>
        /// <param name="TextInvalidUrl">Message for invalid url.</param>
        /// <param name="TextInvalidUSZip">Message for invalid us zip code.</param>
        /// <param name="ObjectsAreNotEqual">Message for not equal objects.</param>
        /// <param name="ObjectsAreEqual">Message for equal objects.</param>
        /// <param name="IsNotSupplied">Message for not supplied.</param>
        public ValidationExtensionMessages(string TextLessThanMinLength = DefaultValues.DEFAULT_TEXT_LESS_THAN_MIN_LENGTH,
                                           string TextMoreThanMaxLength = DefaultValues.DEFAULT_TEXT_MORE_THAN_MAX_LENGTH,
                                           string TextNotMatchPattern = DefaultValues.DEFAULT_TEXT_NOT_MATCH_PATTERN,
                                           string TextMustBeIn = DefaultValues.DEFAULT_TEXT_MUST_BE_IN,
                                           string TextNotValidNumber = DefaultValues.DEFAULT_TEXT_NOT_VALID_NUMBER,
                                           string TextNotNumeric = DefaultValues.DEFAULT_TEXT_NOT_NUMERIC,
                                           string NumberLessThan = DefaultValues.DEFAULT_NUMBER_LESS_THAN,
                                           string NumberMoreThan = DefaultValues.DEFAULT_NUMBER_MORE_THAN,
                                           string TextMustContainOnlyChars = DefaultValues.DEFAULT_TEXT_MUST_CONTAIN_ONLY_CHARS,
                                           string TextMustContainOnlyCharsAndNumbers = DefaultValues.DEFAULT_TEXT_MUST_CONTAIN_ONLY_CHARS_AND_NUMBERS,
                                           string TextInvalidDate = DefaultValues.DEFAULT_TEXT_INVALID_DATE,
                                           string DateLessThanMinDate = DefaultValues.DEFAULT_DATE_LESS_THAN_MIN_DATE,
                                           string DateMoreThanMaxDate = DefaultValues.DEFAULT_DATE_MORE_THAN_MAX_DATE,
                                           string TextInvalidTime = DefaultValues.DEFAULT_TEXT_INVALID_TIME,
                                           string TextInvalidUSPhone = DefaultValues.DEFAULT_TEXT_INVALID_US_PHONE,
                                           string TextInvalidSSN = DefaultValues.DEFAULT_TEXT_INVALID_SSN,
                                           string TextInvalidEmail = DefaultValues.DEFAULT_TEXT_INVALID_EMAIL,
                                           string TextInvalidUrl = DefaultValues.DEFAULT_TEXT_INVALID_URL,
                                           string TextInvalidUSZip = DefaultValues.DEFAULT_TEXT_INVALID_US_ZIP,
                                           string ObjectsAreNotEqual = DefaultValues.DEFAULT_OBJECTS_ARE_NOT_EQUAL,
                                           string ObjectsAreEqual = DefaultValues.DEFAULT_OBJECTS_ARE_EQUAL,
                                           string IsNotSupplied = DefaultValues.DEFAULT_IS_NOT_SUPPLIED)
        {
            // Ensure that messages that need a parameter have one.
            Guard.IsTrue(TextLessThanMinLength.Contains("{0}"),"Text-less-than message requires a parameter.");
            Guard.IsTrue(TextMoreThanMaxLength.Contains("{0}"), "Text-more-than message requires a parameter.");
            Guard.IsTrue(TextMustBeIn.Contains("{0}"), "Text-must-be-in message requires a parameter.");
            Guard.IsTrue(NumberLessThan.Contains("{0}"), "Number-less-than message requires a parameter.");
            Guard.IsTrue(NumberMoreThan.Contains("{0}"), "Number-more-than message requires a parameter.");
            Guard.IsTrue(TextMustContainOnlyChars.Contains("{0}"), "Text-must-contain-only-chars message requires a parameter.");
            Guard.IsTrue(TextMustContainOnlyCharsAndNumbers.Contains("{0}"), "Text-must-contain-chars-and-numbers message requires a parameter.");
            Guard.IsTrue(DateLessThanMinDate.Contains("{0}"), "Date-less-than-min-date message requires a parameter.");
            Guard.IsTrue(DateMoreThanMaxDate.Contains("{0}"), "Date-more-than-max-date message requires a parameter.");

            this.TextLessThanMinLength = TextLessThanMinLength;
            this.TextMoreThanMaxLength = TextMoreThanMaxLength;
            this.TextNotMatchPattern = TextNotMatchPattern;
            this.TextMustBeIn = TextMustBeIn;
            this.TextNotValidNumber = TextNotValidNumber;
            this.TextNotNumeric = TextNotNumeric;
            this.NumberLessThan = NumberLessThan;
            this.NumberMoreThan = NumberMoreThan;
            this.TextMustContainOnlyChars = TextMustContainOnlyChars;
            this.TextMustContainOnlyCharsAndNumbers = TextMustContainOnlyCharsAndNumbers;
            this.TextInvalidDate = TextInvalidDate;
            this.DateLessThanMinDate = DateLessThanMinDate;
            this.DateMoreThanMaxDate = DateMoreThanMaxDate;
            this.TextInvalidTime = TextInvalidTime;
            this.TextInvalidUSPhone = TextInvalidUSPhone;
            this.TextInvalidSSN = TextInvalidSSN;
            this.TextInvalidEmail = TextInvalidEmail;
            this.TextInvalidUrl = TextInvalidUrl;
            this.TextInvalidUSZip = TextInvalidUSZip;
            this.ObjectsAreNotEqual = ObjectsAreNotEqual;
            this.ObjectsAreEqual = ObjectsAreEqual;
            this.IsNotSupplied = IsNotSupplied;
        }


        /// <summary>
        /// Class to hold default message values.
        /// </summary>
        internal class DefaultValues
        {
            
            public const string DEFAULT_TEXT_LESS_THAN_MIN_LENGTH = "Text supplied is less than min length ({0}).";
            public const string DEFAULT_TEXT_MORE_THAN_MAX_LENGTH = "Text supplied is more than max length ({0}).";
            public const string DEFAULT_TEXT_NOT_MATCH_PATTERN = "Text supplied does not match expected pattern.";
            public const string DEFAULT_TEXT_MUST_BE_IN = "Text must be in : {0}";
            public const string DEFAULT_TEXT_NOT_VALID_NUMBER = "Text supplied is not a valid number.";
            public const string DEFAULT_TEXT_NOT_NUMERIC = "Text supplied is not numeric.";
            public const string DEFAULT_NUMBER_LESS_THAN = "Number supplied is less than {0}.";
            public const string DEFAULT_NUMBER_MORE_THAN = "Number supplied is more than {0}.";
            public const string DEFAULT_TEXT_MUST_CONTAIN_ONLY_CHARS = "Text supplied must only contain chars {0}";
            public const string DEFAULT_TEXT_MUST_CONTAIN_ONLY_CHARS_AND_NUMBERS = "Text supplied must only contain chars and numbers {0}";
            public const string DEFAULT_TEXT_INVALID_DATE = "Text supplied is not a valid date";
            public const string DEFAULT_DATE_LESS_THAN_MIN_DATE = "Date supplied is less than minimum date {0}";
            public const string DEFAULT_DATE_MORE_THAN_MAX_DATE = "Date supplied is more than maximum date {0}";
            public const string DEFAULT_TEXT_INVALID_TIME = "Text supplied is not a valid time.";
            public const string DEFAULT_TEXT_INVALID_US_PHONE = "Text supplied is not a valid US phone number.";
            public const string DEFAULT_TEXT_INVALID_SSN = "Text supplied is not a valid social security number.";
            public const string DEFAULT_TEXT_INVALID_EMAIL = "Text supplied is not a valid email.";
            public const string DEFAULT_TEXT_INVALID_URL = "Text supplied is not a valid url.";
            public const string DEFAULT_TEXT_INVALID_US_ZIP = "Text supplied is not a valid US zipcode.";
            public const string DEFAULT_OBJECTS_ARE_NOT_EQUAL = "Objects supplied are not equal";
            public const string DEFAULT_OBJECTS_ARE_EQUAL = "Objects supplied are equal";
            public const string DEFAULT_IS_NOT_SUPPLIED = " is not supplied.";
        }
    }
}
